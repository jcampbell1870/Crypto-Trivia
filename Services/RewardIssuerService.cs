using System.Collections.Concurrent;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Crypto_Trivia.Services;

public sealed record RewardClaimRequest(
    string GameId,
    string WalletAddress,
    int Score,
    string ChainId,
    string TokenAddress);

public sealed record RewardClaimResponse(string Status, string? TransactionHash = null);

public sealed class RewardIssuerService
{
    private const int MaximumScore = 3750; // 25 questions with values from 100 through 500.
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, string> _claims = new();

    public RewardIssuerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<RewardClaimResponse> SubmitAsync(RewardClaimRequest request, CancellationToken cancellationToken)
    {
        var expectedReward = Validate(request);

        if (_claims.TryGetValue(request.GameId, out var existingHash))
            return new("already_submitted", existingHash == "pending" ? null : existingHash);
        if (!_claims.TryAdd(request.GameId, "pending"))
            return new("already_submitted");

        try
        {
            var rpcUrl = RequiredSetting("Crypto:IssuerRpcUrl");
            var privateKey = RequiredSetting("Crypto:IssuerPrivateKey");
            var configuredToken = RequiredSetting("Crypto:TokenContractAddress");
            var configuredChain = RequiredSetting("Crypto:IssuerChainId");
            var configuredTreasury = RequiredSetting("Crypto:RewardVaultAddress");
            var account = new Account(privateKey, new HexBigInteger(Convert.ToInt64(configuredChain[2..], 16)));
            var web3 = new Web3(account, rpcUrl);

            if (!string.Equals(account.Address, configuredTreasury, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Issuer key does not control the configured reward treasury.");
            if (!string.Equals(request.TokenAddress, configuredToken, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(request.ChainId, configuredChain, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Token or network does not match issuer configuration.");

            var decimals = _configuration.GetValue("Crypto:TokenDecimals", 18);
            var rewardUnits = Nethereum.Util.UnitConversion.Convert.ToWei(expectedReward, decimals);
            var rewardHex = Convert.ToHexString(rewardUnits.ToByteArray(isUnsigned: true, isBigEndian: true)).ToLowerInvariant();
            if (rewardHex.Length > 64)
                throw new InvalidOperationException("The reward amount is too large.");
            var transferData = "0xa9059cbb" + request.WalletAddress[2..].PadLeft(64, '0') + rewardHex.PadLeft(64, '0');
            var txHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(
                new Nethereum.RPC.Eth.DTOs.TransactionInput
                {
                    From = account.Address,
                    To = configuredToken,
                    Data = transferData
                });

            _claims[request.GameId] = txHash;
            return new("submitted", txHash);
        }
        catch
        {
            _claims.TryRemove(request.GameId, out _);
            throw;
        }
    }

    private decimal Validate(RewardClaimRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.GameId) || request.GameId.Length > 100)
            throw new ArgumentException("A valid game ID is required.");
        if (string.IsNullOrWhiteSpace(request.WalletAddress) ||
            !Nethereum.Util.AddressUtil.Current.IsValidEthereumAddressHexFormat(request.WalletAddress))
            throw new ArgumentException("A valid wallet address is required.");
        if (request.Score is < 0 or > MaximumScore)
            throw new ArgumentException("The score and reward are invalid.");
        if (string.IsNullOrWhiteSpace(request.ChainId) ||
            !request.ChainId.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A valid chain ID is required.");
        return request.Score * _configuration.GetValue("Crypto:RewardPerPoint", 0.01m);
    }

    private string RequiredSetting(string key) =>
        _configuration[key] ?? throw new InvalidOperationException($"{key} is not configured.");
}
