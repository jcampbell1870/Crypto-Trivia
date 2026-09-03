using System.Globalization;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Crypto_Trivia.Services;

public sealed class TreasuryTransferResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionHash { get; set; } = string.Empty;
}

public sealed class TreasuryPayoutService
{
    private readonly IConfiguration _configuration;

    public TreasuryPayoutService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<TreasuryTransferResult> ExecuteTransferAsync(string recipientAddress, decimal rewardAmount, int tokenDecimals)
    {
        var rpcUrl = _configuration["Treasury:RpcUrl"] ?? Environment.GetEnvironmentVariable("CRYPTO_TRIVIA_TREASURY_RPC_URL");
        var privateKey = _configuration["Treasury:PrivateKey"] ?? Environment.GetEnvironmentVariable("CRYPTO_TRIVIA_TREASURY_KEY");
        var tokenAddress = _configuration["Crypto:TokenContractAddress"] ?? "0xcF0A9F89ab34D39C11B5e08e1c6aC33A47e207c8";
        var treasuryAddress = _configuration["Crypto:RewardVaultAddress"] ??
                              _configuration["Treasury:Address"] ??
                              "0x1e4f6e4a382adbdb662733a19ae773d3ab8f497d";

        if (string.IsNullOrWhiteSpace(rpcUrl))
        {
            return new TreasuryTransferResult { Success = false, Message = "Treasury RPC URL is not configured." };
        }

        if (string.IsNullOrWhiteSpace(privateKey))
        {
            return new TreasuryTransferResult { Success = false, Message = "Treasury private key is not configured." };
        }

        if (!IsValidAddress(recipientAddress) || !IsValidAddress(treasuryAddress) || !IsValidAddress(tokenAddress))
        {
            return new TreasuryTransferResult { Success = false, Message = "Invalid treasury, recipient, or token address." };
        }

        if (rewardAmount <= 0m)
        {
            return new TreasuryTransferResult { Success = false, Message = "Reward amount must be greater than zero." };
        }

        try
        {
            var account = new Account(privateKey);
            var web3 = new Web3(account, rpcUrl);
            var amountWei = BigInteger.Parse(((rewardAmount * (decimal)Math.Pow(10, tokenDecimals)).ToString("F0", CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
            var contractHandler = web3.Eth.GetContractHandler(tokenAddress);
            var transferFunction = new TransferFunction
            {
                To = recipientAddress,
                TokenAmount = amountWei
            };

            var receipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferFunction);
            var txHash = receipt.TransactionHash;

            return new TreasuryTransferResult
            {
                Success = true,
                Message = "Treasury transfer executed successfully.",
                TransactionHash = txHash
            };
        }
        catch (Exception ex)
        {
            return new TreasuryTransferResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    private static bool IsValidAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
               value.Length == 42 &&
               value[2..].All(char.IsLetterOrDigit);
    }
}

[Function("transfer", "bool")]
public class TransferFunction : FunctionMessage
{
    [Parameter("address", "_to", 1)]
    public string To { get; set; } = string.Empty;

    [Parameter("uint256", "_value", 2)]
    public BigInteger TokenAmount { get; set; }
}
