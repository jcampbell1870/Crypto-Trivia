# Crypto Trivia 🎮

A web-based trivia game inspired by Jeopardy that calculates 1870Coin ERC-20 rewards. Players connect their MetaMask wallet and test their knowledge of cryptocurrency and blockchain technology.

## Features

✅ **Jeopardy-Style Gameplay**
- 5 categories with 25 questions total
- Questions valued from $100 to $500
- Classic question reveal and answering system

✅ **MetaMask Integration**
- Secure wallet connection via MetaMask browser extension
- No private key exposure
- Support for Ethereum Mainnet, Sepolia Testnet, and Polygon

✅ **Token Rewards**
- Earn 1870Coin (ERC-20) rewards for every point scored
- Reward amounts are calculated for authorized distribution to the connected wallet
- Reward rate: 0.01 tokens per point
- Maximum potential reward: 37.5 tokens (3,750 points ÷ 100)

✅ **Beautiful UI**
- Modern gradient design with glassmorphism effects
- Responsive design for desktop and mobile
- Smooth animations and transitions
- Real-time score and reward tracking

## Technical Stack

- **Frontend**: Blazor Server (ASP.NET Core 10)
- **Static deployment**: Root `index.html`, `static.js`, and `static.css` for GitHub Pages and Cloudflare
- **Backend**: C# with dependency injection
- **Blockchain**: Web3.js, MetaMask
- **Styling**: CSS3 with scoped component styles
- **Token Standard**: ERC-20

## Project Structure

```
Crypto Trivia/
├── Components/
│   ├── Pages/
│   │   ├── Home.razor                 # Landing page
│   │   ├── Home.razor.css             # Home page styles
│   │   ├── CryptoTrivia.razor          # Main game page
│   │   ├── CryptoTrivia.razor.css      # Game styles
│   │   ├── About.razor                 # About/info page
│   │   └── About.razor.css             # About styles
│   ├── Layout/
│   │   ├── MainLayout.razor            # Main layout
│   │   └── NavMenu.razor               # Navigation menu
│   └── WalletConnect.razor             # MetaMask connection component
├── Services/
│   ├── GameService.cs                  # Game logic and questions
│   └── WalletService.cs                # Blockchain interaction services
├── wwwroot/
│   ├── js/
│   │   └── metamask-interop.js         # JavaScript interop for MetaMask
│   └── css/
│       └── app.css                     # Global styles
├── Program.cs                          # Application startup
├── appsettings.json                    # Configuration (token address, rewards)
└── README.md                           # This file
```

## Token Information

- **Token Name**: 1870Coin
- **Standard**: ERC-20
- **Contract Address**: `0xcF0A9F89ab34D39C11B5e08e1c6aC33A47e207c8`
- **Blockchain**: Ethereum (also supports Sepolia Testnet & Polygon)
- **Decimals**: 18

## Game Categories

1. **🪙 Bitcoin Basics** - Learn about Bitcoin's history, supply, and fundamentals
2. **⚡ Ethereum Essentials** - Master Ethereum, smart contracts, and token standards
3. **🔗 Blockchain Technology** - Understand core blockchain concepts
4. **💳 Wallets & Transactions** - Learn about wallet security and blockchain transactions
5. **🏦 DeFi & Web3** - Explore Decentralized Finance and Web3

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2026 (or later) or VS Code
- MetaMask browser extension installed
- A crypto wallet with some ETH for gas fees (if transferring tokens on mainnet)

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd "Crypto Trivia"
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to `https://localhost:7000` (or the URL shown in the console)

### Configuration

Edit `appsettings.json` to modify:
- Token contract address
- Token symbol
- Reward rate per point
- Supported blockchain networks

## How to Play

1. **Open the Game**: Navigate to the "Play Game" page
2. **Connect Wallet**: Click "Connect MetaMask" and approve the connection in MetaMask
3. **Start Game**: Click "START GAME" button
4. **Select Questions**: Click any question button in the grid ($100-$500)
5. **Reveal Answer**: Click "Reveal Answer" button in the modal
6. **Answer**: Select "Got it right!" or "Got it wrong!"
7. **Game Over**: Note the 1870Coin reward calculated for your connected wallet
8. **Claim Rewards**: Click **Claim reward** after the game finishes. The completion is submitted to the configured issuer, which verifies the score and sends tokens from its funded treasury; the game never asks your wallet to transfer tokens.

## Smart Contract Interaction

The application calculates ERC-20 rewards using the 1870Coin contract metadata. A production distributor must securely submit any token transfer from a funded treasury:

1. Calculates total points earned
2. Converts points to token amount (points × 0.01)
3. An authorized treasury initiates an ERC-20 transfer to the player's wallet
4. The distributor provides the transaction hash for verification

## GitHub Pages, Cloudflare, and reward issuer

The root `index.html` is a self-contained static version of the game and is deployed by `.github/workflows/pages.yml`. This makes the game compatible with GitHub Pages and a Cloudflare custom domain without requiring an ASP.NET server at the edge.

Configure the custom domain in **GitHub Pages** first, then create the corresponding DNS record in Cloudflare with proxying disabled during GitHub verification. GitHub writes the custom-domain value to a `CNAME` file; do not commit a placeholder domain.

The static game uses the configured public reward treasury `0x1e4f6e4a382adbdb662733a19ae773d3ab8f497d` and an issuer endpoint to verify a finished game and send a payout. Set `rewardIssuerUrl` in `js/config.js` to the deployed issuer base URL only after it accepts the `/api/issuer/submit-score` payload and enforces server-side replay protection, score limits, and wallet ownership. Never put a treasury or signer private key in this repository, GitHub Pages, or Cloudflare. If the URL is empty, the game will correctly show the calculated reward as pending rather than pretending that tokens were deposited.

### Issuer deployment

Run the ASP.NET application as the issuer API and provide these secrets through the hosting platform's environment settings:

```text
Crypto__IssuerRpcUrl=https://your-ethereum-rpc.example
Crypto__IssuerPrivateKey=the-private-key-for-the-treasury-wallet
Crypto__IssuerAllowedOrigin=https://jcampbell1870.github.io
```

The private key must derive to the configured `Crypto:RewardVaultAddress`; the API refuses to send if it does not. The treasury must hold 1870Coin and enough ETH for gas, and the token contract must authorize transfers from that treasury. Set `rewardIssuerUrl` to the public API origin (for example, `https://issuer.example.com`) and set `Crypto__IssuerAllowedOrigin` to the exact static game origin. Do not put the private key in `appsettings.json`, source control, or GitHub Pages. The sample uses in-memory replay protection; use a durable, atomic claim store before running multiple issuer instances or relying on replay protection across restarts.

### ERC-20 Transfer ABI

The application implements the ERC-20 `transfer` function:

```solidity
function transfer(address _to, uint256 _value) public returns (bool success)
```

## Architecture Notes

### Services Layer

**GameService**: Manages game state, questions, and scoring
- Maintains category/question data
- Tracks player score
- Handles game initialization and reset

**WalletService & TokenRewardService**: Blockchain interaction
- Stores token contract information
- Calculates reward amounts
- Provides token transfer configuration

### Components

**WalletConnect.razor**: Reusable wallet connection component
- Handles MetaMask connection
- Emits events for parent components
- Displays connected address

**CryptoTrivia.razor**: Main game component
- Manages game state machine (Initial → Playing → GameOver)
- Handles question selection and answering
- Integrates wallet and reward services

## Browser Compatibility

- Chrome/Chromium (recommended)
- Firefox
- Edge
- Opera

**Required**: MetaMask extension installed

## Security Considerations

- ✅ Private keys never exposed (MetaMask handles all signing)
- ✅ No server-side wallet storage
- ✅ All blockchain interactions use MetaMask's secure infrastructure
- ✅ The game never asks a player to transfer tokens to claim a reward
- ✅ Contract address is hardcoded to prevent manipulation

## Development

### Adding New Questions

Edit `Services/GameService.cs` and add to the category's question list:

```csharp
new Question 
{ 
	Id = 26, 
	Text = "Your question?", 
	Answer = "Your answer", 
	Value = 100 
}
```

### Customizing Rewards

Update `appsettings.json`:

```json
"Crypto": {
  "RewardPerPoint": 0.01  // Change this value
}
```

## Deployment

### To Azure

1. Set up an Azure App Service
2. Configure the application settings in Azure
3. Deploy using Visual Studio or Azure CLI

### To Docker

1. Create a Dockerfile
2. Build and run: `docker run -p 80:80 crypto-trivia`

## Troubleshooting

**MetaMask Connection Issues**
- Ensure MetaMask is installed and unlocked
- Check browser console for errors
- Try switching networks and back

**Token Transfer Failures**
- Verify sufficient gas funds
- Check network connection
- Ensure token contract is accessible on selected network

**Build Issues**
- Clear bin/obj folders: `dotnet clean`
- Restore packages: `dotnet restore`
- Rebuild: `dotnet build`

## Future Enhancements

- [ ] Leaderboard system
- [ ] Question difficulty levels
- [ ] Multiplayer mode
- [ ] Custom game creation
- [ ] Social sharing features
- [ ] Achievement badges
- [ ] Daily challenges
- [ ] Token burning for power-ups
- [ ] NFT achievements
- [ ] Admin panel for question management

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see LICENSE file for details.

## Support

For issues, questions, or suggestions, please:
- Open an issue on GitHub
- Contact the development team
- Check the About page for more information

## Disclaimer

This is a demonstration project for educational purposes. The token reward system would require proper contract deployment and testing on a live blockchain before production use. Always test thoroughly on testnet before mainnet deployment.

---

**Version**: 1.0.0  
**Last Updated**: 2024  
**Built with**: Blazor Server + Web3.js + MetaMask
