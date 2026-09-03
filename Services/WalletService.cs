using System.Threading.Tasks;

namespace Crypto_Trivia.Services
{
    public class WalletService
    {
        private readonly IConfiguration _configuration;

        public WalletService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> ConnectWalletAsync()
        {
            // This will be called from JavaScript interop
            return await Task.FromResult("");
        }

        public string GetTokenContractAddress()
        {
            return _configuration["Crypto:TokenContractAddress"] ?? "0xcF0A9F89ab34D39C11B5e08e1c6aC33A47e207c8";
        }

        public string GetTokenSymbol()
        {
            return _configuration["Crypto:TokenSymbol"] ?? "1870Coin";
        }

        public int GetTokenDecimals()
        {
            return _configuration.GetValue<int>("Crypto:TokenDecimals", 18);
        }

        public string GetRewardVaultAddress()
        {
            return _configuration["Crypto:RewardVaultAddress"] ?? string.Empty;
        }

        public string GetRewardIssuerUrl()
        {
            return _configuration["Issuer:BaseUrl"] ?? string.Empty;
        }
    }

    public class TokenRewardService
    {
        private readonly WalletService _walletService;
        private readonly IConfiguration _configuration;

        public TokenRewardService(WalletService walletService, IConfiguration configuration)
        {
            _walletService = walletService;
            _configuration = configuration;
        }

        public decimal CalculateRewardAmount(int score)
        {
            var rewardPerPoint = _configuration.GetValue<decimal>("Crypto:RewardPerPoint", 0.01m);
            return score * rewardPerPoint;
        }

        public string GetTokenContractAddress()
        {
            return _walletService.GetTokenContractAddress();
        }

        public string GetTokenSymbol()
        {
            return _walletService.GetTokenSymbol();
        }

        public int GetTokenDecimals()
        {
            return _walletService.GetTokenDecimals();
        }
    }
}
