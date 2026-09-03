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
    public string TransactionHash { get; set; } = string.Empty;
    public string TreasuryAddress { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;

    // True when the failure was caused by the client's request (400);
    // false for upstream/configuration failures (502).
    public bool IsClientError { get; set; } = true;
}

public sealed class RewardIssuerService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public RewardIssuerService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<RewardSubmissionResponse> SubmitRewardAsync(RewardSubmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WalletAddress))
        {
            return ClientFailure("Wallet address is required.");
        }

        if (!IsValidAddress(request.WalletAddress))
        {
            return ClientFailure("Wallet address is invalid.");
        }

        if (request.Score < 0)
        {
            return ClientFailure("Score cannot be negative.");
        }

        var maxScore = _configuration.GetValue<int>("Crypto:MaxScore", 7500);
        if (request.Score > maxScore)
        {
            return ClientFailure($"Score exceeds the maximum allowed ({maxScore}).");
        }

        // Always compute the reward server-side from the validated score;
        // never trust a client-supplied amount.
        var rewardPerPoint = _configuration.GetValue<decimal>("Crypto:RewardPerPoint", 0.01m);
        var rewardAmount = request.Score * rewardPerPoint;
        if (rewardAmount <= 0m)
        {
            return ClientFailure("No reward earned for the submitted score.");
        }

        var treasuryAddress = _configuration["Crypto:RewardVaultAddress"] ?? string.Empty;
        if (!IsValidAddress(treasuryAddress))
        {
            return ServerFailure("Treasury wallet is not configured.");
        }

        var issuerUrl = _configuration["Issuer:BaseUrl"] ?? string.Empty;
        var issuerName = _configuration["Issuer:IssuerName"] ?? "Crypto Trivia Issuer";
        if (string.IsNullOrWhiteSpace(issuerUrl))
        {
            return ServerFailure("Reward issuer is not configured.");
        }

        var payload = new
        {
            walletAddress = request.WalletAddress,
            score = request.Score,
            rewardAmount,
            treasuryAddress,
            issuer = issuerName
        };

        try
        {
            var client = _httpClientFactory.CreateClient("RewardIssuer");
            client.BaseAddress = new Uri(issuerUrl, UriKind.Absolute);
            using var response = await client.PostAsJsonAsync("/api/issuer/submit-score", payload);
            var body = await response.Content.ReadAsStringAsync();
            string? message = null;
            string? transactionHash = null;
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var m)) message = m.GetString();
                if (doc.RootElement.TryGetProperty("transactionHash", out var t)) transactionHash = t.GetString();
                if (doc.RootElement.TryGetProperty("txHash", out var tx)) transactionHash ??= tx.GetString();
            }
            catch (JsonException)
            {
                // Non-JSON response; treat as issuer message.
                message = body;
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServerFailure($"Reward issuer rejected the submission ({(int)response.StatusCode}).");
            }

            return new RewardSubmissionResponse
            {
                Success = true,
                Message = message ?? "Reward submitted to the issuer.",
                RewardAmount = rewardAmount,
                TransactionHash = transactionHash ?? string.Empty,
                TreasuryAddress = treasuryAddress,
                IssuerName = issuerName
            };
        }
        catch (Exception)
        {
            return ServerFailure("Could not reach the reward issuer.");
        }
    }

    private static RewardSubmissionResponse ClientFailure(string message) =>
        new() { Success = false, Message = message, IsClientError = true };

    private static RewardSubmissionResponse ServerFailure(string message) =>
        new() { Success = false, Message = message, IsClientError = false };

    private static bool IsValidAddress(string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
        value.Length == 42 &&
        value[2..].All(Uri.IsHexDigit);
}
