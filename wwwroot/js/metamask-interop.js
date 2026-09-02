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

    // Transfer ERC-20 tokens (requires Web3.js)
    transferToken: async function (tokenAddress, toAddress, amount, decimals, fromAddress) {
        try {
            if (!this.isMetaMaskInstalled()) {
                return { success: false, error: 'MetaMask is not installed' };
            }

            // ERC-20 Transfer ABI
            const erc20Abi = [
                {
                    "constant": false,
                    "inputs": [
                        { "name": "_to", "type": "address" },
                        { "name": "_value", "type": "uint256" }
                    ],
                    "name": "transfer",
                    "outputs": [{ "name": "", "type": "bool" }],
                    "type": "function"
                }
            ];

            // Initialize Web3
            const web3 = new Web3(window.ethereum);
            const contract = new web3.eth.Contract(erc20Abi, tokenAddress);

            // Convert amount to token units (multiply by 10^decimals)
            const tokenAmount = web3.utils.toWei(amount.toString(), 'ether');

            const data = contract.methods.transfer(toAddress, tokenAmount).encodeABI();

            const tx = {
                from: fromAddress,
                to: tokenAddress,
                data: data,
                gas: '100000',
                gasPrice: await web3.eth.getGasPrice()
            };

            const txHash = await window.ethereum.request({
                method: 'eth_sendTransaction',
                params: [tx]
            });

            return { success: true, error: null, txHash: txHash };
        } catch (error) {
            return { success: false, error: error.message, txHash: null };
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

// Load Web3.js library
function loadWeb3() {
    return new Promise((resolve, reject) => {
        if (typeof Web3 !== 'undefined') {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/web3@1.10.0/dist/web3.min.js';
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Initialize on page load
window.addEventListener('DOMContentLoaded', () => {
    loadWeb3().catch(err => console.error('Failed to load Web3:', err));
});
