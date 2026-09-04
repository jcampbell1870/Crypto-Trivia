using System.Globalization;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Crypto_Trivia.Services;

public sealed class RewardSubmissionRequest
{
    public string? WalletAddress { get; set; }
    public int Score { get; set; }
}

public sealed class RewardSubmissionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? TransactionHash { get; set; }
    public decimal RewardAmount { get; set; }
    public string? RecipientAddress { get; set; }
    public string? TokenSymbol { get; set; }
}

public sealed class RewardIssuerService
{
    private readonly IConfiguration _configuration;
    private readonly WalletService _walletService;
    private readonly ILogger<RewardIssuerService> _logger;

    public RewardIssuerService(
        IConfiguration configuration,
        WalletService walletService,
        ILogger<RewardIssuerService> logger)
    {
        _configuration = configuration;
        _walletService = walletService;
        _logger = logger;
    }

    public decimal CalculateRewardAmount(int score)
    {
        var rewardPerPoint = GetRewardPerPoint();
        return score * rewardPerPoint;
    }

    public async Task<RewardSubmissionResult> SubmitScoreAsync(string? walletAddress, int score, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(walletAddress))
        {
            return Failure("A wallet address is required.");
        }

        if (score < 0)
        {
            return Failure("The score cannot be negative.");
        }

        var tokenContractAddress = _configuration["Crypto:TokenContractAddress"] ?? _walletService.GetTokenContractAddress();
        var rewardVaultAddress = _configuration["Crypto:RewardVaultAddress"];
        var issuerPrivateKey = _configuration["Crypto:IssuerPrivateKey"] ?? _configuration["Crypto__IssuerPrivateKey"];
        var rpcUrl = _configuration["Crypto:IssuerRpcUrl"] ?? _configuration["Crypto__IssuerRpcUrl"];

        if (string.IsNullOrWhiteSpace(rewardVaultAddress) ||
            string.IsNullOrWhiteSpace(issuerPrivateKey) ||
            string.IsNullOrWhiteSpace(rpcUrl))
        {
            return Failure("Live payout is not configured. Set Crypto:RewardVaultAddress, Crypto:IssuerPrivateKey, and Crypto:IssuerRpcUrl.");
        }

        var normalizedVaultAddress = rewardVaultAddress.Trim();
        if (!IsValidEthereumAddress(normalizedVaultAddress))
        {
            return Failure("Crypto:RewardVaultAddress is not a valid Ethereum address.");
        }

        try
        {
            var account = new Account(issuerPrivateKey.Trim());
            var signerAddress = account.Address.Trim();

            if (!string.Equals(signerAddress, normalizedVaultAddress, StringComparison.OrdinalIgnoreCase))
            {
                return Failure("Crypto:IssuerPrivateKey does not control the configured Crypto:RewardVaultAddress.");
            }

            var rewardAmount = CalculateRewardAmount(score);
            var transferValue = ToTokenUnits(rewardAmount, _walletService.GetTokenDecimals());
            var transferFunction = new TransferFunction
            {
                To = walletAddress.Trim(),
                Value = transferValue
            };

            var web3 = new Web3(account, rpcUrl);
            var transactionHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var txHash = await transactionHandler.SendRequestAsync(tokenContractAddress, transferFunction);

            _logger.LogInformation("Submitted reward payout for score {Score}.", score);

            return new RewardSubmissionResult
            {
                Success = true,
                Message = "Reward transferred successfully.",
                TransactionHash = txHash,
                RewardAmount = rewardAmount,
                RecipientAddress = walletAddress.Trim(),
                TokenSymbol = _walletService.GetTokenSymbol()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit reward payout for score {Score}.", score);
            return Failure($"Reward payout failed: {ex.Message}");
        }
    }

    private decimal GetRewardPerPoint()
    {
        var raw = _configuration["Crypto:RewardPerPoint"] ?? "0.01";
        return decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0.01m;
    }

    private static BigInteger ToTokenUnits(decimal amount, int tokenDecimals)
    {
        var factor = BigInteger.Pow(10, tokenDecimals);
        var scaled = amount * (decimal)factor;
        return BigInteger.Parse(scaled.ToString(CultureInfo.InvariantCulture).Split('.')[0], CultureInfo.InvariantCulture);
    }

    private static bool IsValidEthereumAddress(string? candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        try
        {
            var address = candidate.Trim();
            if (address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                address = address[2..];
            }

            if (address.Length != 40)
            {
                return false;
            }

            return address.All(char.IsLetterOrDigit) && address.All(ch => Uri.IsHexDigit(ch));
        }
        catch
        {
            return false;
        }
    }

    private static RewardSubmissionResult Failure(string message) => new()
    {
        Success = false,
        Message = message,
        RewardAmount = 0m,
        RecipientAddress = null,
        TokenSymbol = null,
        TransactionHash = null
    };

    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; } = string.Empty;

        [Parameter("uint256", "_value", 2)]
        public BigInteger Value { get; set; }
    }
}
