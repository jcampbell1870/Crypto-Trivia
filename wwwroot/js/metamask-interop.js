// MetaMask and Web3 Integration for Crypto Trivia
export const MetaMaskInterop = {
    // Check if MetaMask is installed
    isMetaMaskInstalled: function () {
        return typeof window.ethereum !== 'undefined' && window.ethereum.isMetaMask;
    },

    // Connect to MetaMask wallet
    connectWallet: async function () {
        try {
            if (!this.isMetaMaskInstalled()) {
                return { success: false, error: 'MetaMask is not installed', address: null };
            }

            const accounts = await window.ethereum.request({
                method: 'eth_requestAccounts'
            });

            const address = accounts[0];

            // Listen for account changes
            window.ethereum.on('accountsChanged', (accounts) => {
                if (accounts.length === 0) {
                    console.log('MetaMask disconnected');
                    window.metamaskConnected = false;
                } else {
                    window.metamaskAddress = accounts[0];
                }
            });

            // Listen for chain changes
            window.ethereum.on('chainChanged', () => {
                window.location.reload();
            });

            return { success: true, error: null, address: address };
        } catch (error) {
            return { success: false, error: error.message, address: null };
        }
    },

    // Get current account
    getCurrentAccount: async function () {
        try {
            if (!this.isMetaMaskInstalled()) {
                return null;
            }

            const accounts = await window.ethereum.request({
                method: 'eth_accounts'
            });

            return accounts.length > 0 ? accounts[0] : null;
        } catch (error) {
            console.error('Error getting current account:', error);
            return null;
        }
    },

    // Get network/chain ID
    getChainId: async function () {
        try {
            if (!this.isMetaMaskInstalled()) {
                return null;
            }

            const chainId = await window.ethereum.request({
                method: 'eth_chainId'
            });

            return chainId;
        } catch (error) {
            console.error('Error getting chain ID:', error);
            return null;
        }
    },

    // Get balance
    getBalance: async function (address) {
        try {
            if (!this.isMetaMaskInstalled()) {
                return null;
            }

            const balance = await window.ethereum.request({
                method: 'eth_getBalance',
                params: [address, 'latest']
            });

            // Convert from Wei to ETH
            return (parseInt(balance, 16) / Math.pow(10, 18)).toString();
        } catch (error) {
            console.error('Error getting balance:', error);
            return null;
        }
    },

    // Check if connected to Ethereum mainnet or Sepolia testnet
    isCorrectNetwork: async function () {
        try {
            const chainId = await this.getChainId();
            // Mainnet = 0x1, Sepolia = 0xaa36a7
            return chainId === '0x1' || chainId === '0xaa36a7' || chainId === '0x89'; // Also allow Polygon
        } catch (error) {
            console.error('Error checking network:', error);
            return false;
        }
    },

    // Execute an ERC-20 transfer from the specified sender to the recipient.
    // This is used to submit the reward payout after the game completes.
    transferToken: async function (tokenAddress, toAddress, amount, decimals, fromAddress) {
        try {
            if (!this.isMetaMaskInstalled()) {
                return { success: false, error: 'MetaMask is not installed', txHash: null };
            }

            if (!tokenAddress || !toAddress || !fromAddress) {
                return { success: false, error: 'Missing token, recipient, or sender address.', txHash: null };
            }

            const numericAmount = Number(amount);
            if (!Number.isFinite(numericAmount) || numericAmount <= 0) {
                return { success: false, error: 'Reward amount must be greater than zero.', txHash: null };
            }

            const transferMethodId = '0xa9059cbb';
            const normalizedDecimals = Number(decimals) || 18;
            const baseUnits = BigInt(Math.round(numericAmount * (10 ** normalizedDecimals)));
            const paddedAddress = toAddress.replace(/^0x/i, '').padStart(64, '0');
            const paddedAmount = baseUnits.toString(16).padStart(64, '0');
            const data = `${transferMethodId}${paddedAddress}${paddedAmount}`;

            const txHash = await window.ethereum.request({
                method: 'eth_sendTransaction',
                params: [{
                    from: fromAddress,
                    to: tokenAddress,
                    data: data,
                    value: '0x0'
                }]
            });

            return { success: true, error: null, txHash: txHash };
        } catch (error) {
            const message = error && error.message ? error.message : 'Token transfer failed.';
            return { success: false, error: message, txHash: null };
        }
    },

    // Switch network
    switchNetwork: async function (chainId) {
        try {
            await window.ethereum.request({
                method: 'wallet_switchEthereumChain',
                params: [{ chainId: chainId }]
            });
            return { success: true, error: null };
        } catch (error) {
            // If chain doesn't exist, offer to add it
            if (error.code === 4902) {
                return { success: false, error: 'Chain not added to MetaMask', needsAdd: true };
            }
            return { success: false, error: error.message };
        }
    }
};
