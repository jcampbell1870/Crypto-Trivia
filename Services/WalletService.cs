using System.Threading.Tasks;

namespace Crypto_Trivia.Services
{
    public class WalletService
    {
        private const string TokenContractAddress = "0xcF0A9F89ab34D39C11B5e08e1c6aC33A47e207c8";
        private const string TokenSymbol = "1870Coin";
        private const int TokenDecimals = 18;
        private const string RewardVaultAddress = "0x1e4f6e4a382adbdb662733a19ae773d3ab8f497d";

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

        public string GetRewardVaultAddress()
        {
            return RewardVaultAddress;
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

        public string GetRewardVaultAddress()
        {
            return _walletService.GetRewardVaultAddress();
        }
    }
}
