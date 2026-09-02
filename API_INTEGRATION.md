# Crypto Trivia - API & Integration Documentation

## 📡 JavaScript Interop API

The Crypto Trivia application communicates with MetaMask and Web3 through JavaScript interop. Below is the complete API documentation.

---

## MetaMaskInterop Object

### Methods

#### 1. `isMetaMaskInstalled()`
Checks if MetaMask extension is installed in the browser.

**Returns**: `boolean`

**Usage**:
```javascript
const installed = MetaMaskInterop.isMetaMaskInstalled();
if (!installed) {
  console.log("Please install MetaMask");
}
```

---

#### 2. `connectWallet()`
Requests wallet connection from MetaMask and sets up event listeners.

**Returns**: `Promise<{success: boolean, error: string | null, address: string | null}>`

**Usage (C#)**:
```csharp
var result = await _module.InvokeAsync<dynamic>("MetaMaskInterop.connectWallet");
if (result["success"]) {
	string walletAddress = result["address"];
}
```

**Response Example**:
```json
{
  "success": true,
  "error": null,
  "address": "0x742d35Cc6634C0532925a3b844Bc390e17d57F3d"
}
```

---

#### 3. `getCurrentAccount()`
Gets the currently connected account without prompting the user.

**Returns**: `Promise<string | null>`

**Usage**:
```javascript
const account = await MetaMaskInterop.getCurrentAccount();
console.log("Connected as:", account);
```

---

#### 4. `getChainId()`
Retrieves the current blockchain network ID.

**Returns**: `Promise<string | null>`

**Supported Chain IDs**:
- `0x1` - Ethereum Mainnet
- `0xaa36a7` - Sepolia Testnet
- `0x89` - Polygon Mainnet

**Usage**:
```javascript
const chainId = await MetaMaskInterop.getChainId();
console.log("Network:", chainId); // "0x1"
```

---

#### 5. `getBalance(address)`
Fetches ETH balance for a given address.

**Parameters**:
- `address` (string): Ethereum address

**Returns**: `Promise<string | null>` - Balance in ETH

**Usage**:
```javascript
const balance = await MetaMaskInterop.getBalance("0x742d35Cc6634C0532925a3b844Bc390e17d57F3d");
console.log("Balance:", balance); // "1.5" ETH
```

---

#### 6. `transferToken(tokenAddress, toAddress, amount, decimals, fromAddress)`
Initiates an ERC-20 token transfer transaction.

**Parameters**:
- `tokenAddress` (string): Token contract address
- `toAddress` (string): Recipient address
- `amount` (number): Amount to transfer
- `decimals` (number): Token decimals (usually 18)
- `fromAddress` (string): Sender address

**Returns**: `Promise<{success: boolean, error: string | null, txHash: string | null}>`

**Usage (C#)**:
```csharp
var result = await _module.InvokeAsync<dynamic>(
	"MetaMaskInterop.transferToken",
	"0x8eddD4edea39c5B5f77662453600F53A202EE47C",
	userAddress,
	37.5,
	18,
	userAddress
);

if (result["success"]) {
	string txHash = result["txHash"];
	// Transaction submitted
}
```

**Response Example**:
```json
{
  "success": true,
  "error": null,
  "txHash": "0x2c7e8d9f1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p"
}
```

---

#### 7. `isCorrectNetwork()`
Verifies the user is on a supported blockchain network.

**Returns**: `Promise<boolean>`

**Supported Networks**:
- Ethereum Mainnet
- Sepolia Testnet
- Polygon Mainnet

**Usage**:
```javascript
const isSupported = await MetaMaskInterop.isCorrectNetwork();
if (!isSupported) {
  console.log("Please switch to a supported network");
}
```

---

#### 8. `switchNetwork(chainId)`
Requests MetaMask to switch to a different network.

**Parameters**:
- `chainId` (string): Target chain ID (e.g., "0x1")

**Returns**: `Promise<{success: boolean, error: string, needsAdd: boolean}>`

**Usage**:
```javascript
const result = await MetaMaskInterop.switchNetwork("0x1");
if (result.success) {
  console.log("Switched to Ethereum Mainnet");
} else if (result.needsAdd) {
  console.log("Network needs to be added to MetaMask");
}
```

---

## C# Service Integration

### GameService API

#### Properties & Methods

```csharp
public class GameService
{
	// Get all categories
	public List<Category> GetCategories()

	// Get specific category
	public Category GetCategory(string categoryName)

	// Get specific question
	public Question GetQuestion(string categoryName, int value)

	// Mark question as answered and add to score
	public void AnswerQuestion(string categoryName, int value)

	// Get current player score
	public int GetPlayerScore()

	// Add points to score
	public void AddToScore(int amount)

	// Reset game
	public void ResetGame()

	// Check if all questions answered
	public bool IsGameOver()
}
```

#### Usage Example

```csharp
@inject GameService GameService

@code {
	private void PlayGame()
	{
		var categories = GameService.GetCategories();
		var question = GameService.GetQuestion("Bitcoin Basics", 100);

		if (AnsweredCorrectly()) {
			GameService.AnswerQuestion("Bitcoin Basics", 100);
			int score = GameService.GetPlayerScore();
		}
	}
}
```

---

### WalletService API

```csharp
public class WalletService
{
	// Get token contract address
	public string GetTokenContractAddress()
	// Returns: "0x8eddD4edea39c5B5f77662453600F53A202EE47C"

	// Get token symbol
	public string GetTokenSymbol()
	// Returns: "Arcade1870"

	// Get token decimals
	public int GetTokenDecimals()
	// Returns: 18
}
```

---

### TokenRewardService API

```csharp
public class TokenRewardService
{
	// Calculate reward amount from score
	public decimal CalculateRewardAmount(int score)
	// Example: CalculateRewardAmount(1000) returns 10.0m

	// Get token contract address
	public string GetTokenContractAddress()

	// Get token symbol
	public string GetTokenSymbol()

	// Get token decimals
	public int GetTokenDecimals()
}
```

#### Usage Example

```csharp
@inject TokenRewardService TokenRewardService

@code {
	private void DisplayReward(int finalScore)
	{
		decimal reward = TokenRewardService.CalculateRewardAmount(finalScore);
		string symbol = TokenRewardService.GetTokenSymbol();

		Console.WriteLine($"You earned {reward} {symbol} tokens!");
		// Output: "You earned 37.5 Arcade1870 tokens!"
	}
}
```

---

## Event Listeners

MetaMask automatically sets up these event listeners:

### Account Change
```javascript
window.ethereum.on('accountsChanged', (accounts) => {
	if (accounts.length === 0) {
		console.log('MetaMask disconnected');
	} else {
		console.log('Switched account:', accounts[0]);
	}
});
```

### Network Change
```javascript
window.ethereum.on('chainChanged', () => {
	window.location.reload();
});
```

---

## Error Handling

### Common MetaMask Errors

```javascript
try {
	const accounts = await window.ethereum.request({
		method: 'eth_requestAccounts'
	});
} catch (error) {
	if (error.code === 4001) {
		// User rejected the request
		console.log('User rejected wallet connection');
	} else if (error.code === -32603) {
		// MetaMask internal error
		console.log('MetaMask error');
	}
}
```

### Network Errors

```csharp
try 
{
	var result = await _module.InvokeAsync<dynamic>(
		"MetaMaskInterop.transferToken", 
		// parameters...
	);

	if (!result["success"]) 
	{
		string errorMsg = result["error"];
		// Handle error
	}
}
catch (Exception ex)
{
	Console.WriteLine($"Integration error: {ex.Message}");
}
```

---

## Configuration

### Environment Variables

Configure through `appsettings.json`:

```json
{
  "Crypto": {
	"TokenContractAddress": "0x8eddD4edea39c5B5f77662453600F53A202EE47C",
	"TokenSymbol": "Arcade1870",
	"TokenDecimals": 18,
	"RewardPerPoint": 0.01,
	"SupportedChains": [
	  {
		"id": "0x1",
		"name": "Ethereum Mainnet"
	  },
	  {
		"id": "0xaa36a7",
		"name": "Sepolia Testnet"
	  },
	  {
		"id": "0x89",
		"name": "Polygon"
	  }
	]
  }
}
```

---

## Web3.js Integration

### Web3 Instance

The application uses Web3.js v1.10.0 loaded from CDN:

```html
<script src="https://cdn.jsdelivr.net/npm/web3@1.10.0/dist/web3.min.js"></script>
```

### ERC-20 ABI

The following contract methods are used:

```javascript
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
```

### Transaction Building

```javascript
const web3 = new Web3(window.ethereum);
const contract = new web3.eth.Contract(erc20Abi, tokenAddress);

const encodedData = contract.methods.transfer(toAddress, amount).encodeABI();

const tx = {
	from: senderAddress,
	to: tokenAddress,
	data: encodedData,
	gas: '100000',
	gasPrice: await web3.eth.getGasPrice()
};

const txHash = await window.ethereum.request({
	method: 'eth_sendTransaction',
	params: [tx]
});
```

---

## Data Models

### Category
```csharp
public class Category
{
	public string Name { get; set; }
	public List<Question> Questions { get; set; }
}
```

### Question
```csharp
public class Question
{
	public int Id { get; set; }
	public string Text { get; set; }
	public string Answer { get; set; }
	public int Value { get; set; }
	public bool IsAnswered { get; set; }
}
```

---

## Performance Considerations

### Gas Estimation

For token transfers, estimate gas before sending:

```javascript
const gasEstimate = await contract.methods
	.transfer(toAddress, amount)
	.estimateGas({ from: senderAddress });

console.log("Estimated gas:", gasEstimate);
```

### Transaction Confirmation

Monitor transaction status:

```javascript
const receipt = await web3.eth.getTransactionReceipt(txHash);

if (receipt && receipt.status) {
	console.log("Transaction successful");
} else {
	console.log("Transaction failed");
}
```

---

## Testing the API

### Unit Test Example (xUnit)

```csharp
[Fact]
public void AnswerQuestion_IncrementsScore()
{
	var gameService = new GameService();

	gameService.AnswerQuestion("Bitcoin Basics", 100);

	Assert.Equal(100, gameService.GetPlayerScore());
}

[Fact]
public void CalculateRewardAmount_CorrectFormula()
{
	var tokenService = new TokenRewardService(new WalletService());

	decimal reward = tokenService.CalculateRewardAmount(1000);

	Assert.Equal(10.0m, reward);
}
```

---

## Browser Compatibility

| Browser | Support | Notes |
|---------|---------|-------|
| Chrome | ✅ Full | Recommended |
| Firefox | ✅ Full | Requires MetaMask |
| Edge | ✅ Full | Chromium-based |
| Safari | ⚠️ Partial | Requires MetaMask |
| Opera | ✅ Full | MetaMask support |

---

## Rate Limiting & Throttling

When implementing additional features, consider:

```javascript
// Rate limit wallet connection requests
let lastConnectTime = 0;
const CONNECT_COOLDOWN = 5000; // 5 seconds

async function safeConnect() {
	if (Date.now() - lastConnectTime < CONNECT_COOLDOWN) {
		throw new Error("Please wait before trying again");
	}

	lastConnectTime = Date.now();
	return await MetaMaskInterop.connectWallet();
}
```

---

## Security Best Practices

1. **Never log sensitive data**
   ```javascript
   // ❌ BAD
   console.log(privateKey);

   // ✅ GOOD
   console.log("Transaction initiated");
   ```

2. **Validate all user input**
   ```csharp
   if (string.IsNullOrEmpty(address) || !IsValidAddress(address))
   {
	   throw new ArgumentException("Invalid address");
   }
   ```

3. **Use HTTPS only in production**
   ```json
   {
	   "RequireHttpsMetadata": true
   }
   ```

4. **Implement CORS properly**
   ```csharp
   builder.Services.AddCors(options =>
   {
	   options.AddPolicy("AllowMetaMask", policy =>
	   {
		   policy.AllowAnyOrigin()
				 .AllowAnyMethod()
				 .AllowAnyHeader();
	   });
   });
   ```

---

## Troubleshooting API Issues

### MetaMask Connection Fails
- Verify MetaMask is installed
- Check browser console for errors
- Ensure user approved connection
- Test in incognito mode

### Transaction Fails
- Verify sufficient gas
- Check account balance
- Ensure correct network
- Review error message from MetaMask

### Web3 Initialization Issues
- Verify CDN is accessible
- Check `window.ethereum` exists
- Wait for DOM ready before initialization

---

## API Versioning

Current version: **1.0.0**

Planned future versions:
- 2.0.0: GraphQL API
- 2.1.0: Websocket support
- 3.0.0: Multi-chain aggregation

---

## Support Resources

- **MetaMask Documentation**: https://docs.metamask.io/
- **Web3.js Documentation**: https://web3js.readthedocs.io/
- **Ethereum JSON-RPC API**: https://ethereum.org/en/developers/docs/apis/json-rpc/
- **ERC-20 Standard**: https://eips.ethereum.org/EIPS/eip-20

