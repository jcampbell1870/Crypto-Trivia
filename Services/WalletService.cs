using System.Threading.Tasks;

namespace Crypto_Trivia.Services
{
    public class WalletService
    {
        private const string TokenContractAddress = "0x8eddD4edea39c5B5f77662453600F53A202EE47C";
        private const string TokenSymbol = "Arcade1870";
        private const int TokenDecimals = 18;

        public async Task<string> ConnectWalletAsync()
        {
            // This will be called from JavaScript interop
            return await Task.FromResult("");
        }

        public string GetTokenContractAddress()
        {
            return TokenContractAddress;
        }

        public string GetTokenSymbol()
        {
            return TokenSymbol;
        }

        public int GetTokenDecimals()
        {
            return TokenDecimals;
        }
    }

    public class TokenRewardService
    {
        private readonly WalletService _walletService;
        private const decimal RewardPerPoint = 0.01m; // 0.01 tokens per point

        public TokenRewardService(WalletService walletService)
        {
            _walletService = walletService;
        }

        public decimal CalculateRewardAmount(int score)
        {
            return score * RewardPerPoint;
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
