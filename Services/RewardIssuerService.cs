using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Crypto_Trivia.Services;

public sealed class RewardSubmissionRequest
{
    public string WalletAddress { get; set; } = string.Empty;
    public int Score { get; set; }
    public decimal RewardAmount { get; set; }
}

public sealed class RewardSubmissionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; }
    public string ClaimId { get; set; } = string.Empty;
    public string ClaimToken { get; set; } = string.Empty;
    public string TreasuryAddress { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
}

public sealed class RewardIssuerService
{
    private readonly IConfiguration _configuration;
    private readonly TreasuryPayoutService _treasuryPayoutService;

    public RewardIssuerService(IConfiguration configuration, TreasuryPayoutService treasuryPayoutService)
    {
        _configuration = configuration;
        _treasuryPayoutService = treasuryPayoutService;
    }

    public async Task<RewardSubmissionResponse> SubmitRewardAsync(RewardSubmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WalletAddress))
        {
            return Failure("Wallet address is required.");
        }

        if (!IsValidAddress(request.WalletAddress))
        {
            return Failure("Wallet address is invalid.");
        }

        if (request.Score < 0)
        {
            return Failure("Score cannot be negative.");
        }

        var rewardPerPoint = _configuration.GetValue<decimal>("Crypto:RewardPerPoint", 0.01m);
        var rewardAmount = request.RewardAmount > 0 ? request.RewardAmount : request.Score * rewardPerPoint;
        var treasuryAddress = _configuration["Crypto:RewardVaultAddress"] ??
                              _configuration["RewardVaultAddress"] ??
                              _configuration["Treasury:Address"] ??
                              "0x1e4f6e4a382adbdb662733a19ae773d3ab8f497d";

        if (string.IsNullOrWhiteSpace(treasuryAddress))
        {
            return Failure("Treasury wallet is not configured.");
        }

        var issuerName = _configuration["Issuer:IssuerName"] ?? "Crypto Trivia Issuer";
        var signingKey = _configuration["Issuer:SigningKey"] ??
                        Environment.GetEnvironmentVariable("CRYPTO_TRIVIA_ISSUER_KEY") ??
                        "dev-issuer-key";

        var claimId = Guid.NewGuid().ToString("N");
        var issuedAt = DateTimeOffset.UtcNow;
        var nonce = RandomNumberGenerator.GetInt32(100000, 999999);

        var claimPayload = new
        {
            claimId,
            walletAddress = request.WalletAddress,
            score = request.Score,
            rewardAmount,
            treasuryAddress,
            issuedAtUtc = issuedAt,
            nonce,
            issuer = issuerName
        };

        var payloadJson = JsonSerializer.Serialize(claimPayload);
        var signature = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(signingKey), Encoding.UTF8.GetBytes(payloadJson))); 
        var claimToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson + "." + signature));

        var payoutResult = await _treasuryPayoutService.ExecuteTransferAsync(request.WalletAddress, rewardAmount, 18);
        if (!payoutResult.Success)
        {
            return new RewardSubmissionResponse
            {
                Success = false,
                Message = "Reward claim was created but payout failed: " + payoutResult.Message,
                RewardAmount = rewardAmount,
                ClaimId = claimId,
                ClaimToken = claimToken,
                TreasuryAddress = treasuryAddress,
                IssuerName = issuerName
            };
        }

        return new RewardSubmissionResponse
        {
            Success = true,
            Message = "Reward claim issued successfully by the backend issuer and paid from the treasury wallet.",
            RewardAmount = rewardAmount,
            ClaimId = claimId,
            ClaimToken = claimToken,
            TreasuryAddress = treasuryAddress,
            IssuerName = issuerName
        };
    }

    private static RewardSubmissionResponse Failure(string message)
    {
        return new RewardSubmissionResponse
        {
            Success = false,
            Message = message,
            RewardAmount = 0m
        };
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
