# Crypto Trivia - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- .NET 10 SDK installed
- MetaMask browser extension installed
- Visual Studio 2026 or VS Code with C# extensions

### Step 1: Open the Project
```bash
cd "C:\Users\thund\source\repos\Crypto Trivia"
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Build the Project
```bash
dotnet build
```

### Step 4: Run the Application
```bash
dotnet run
```

The application will start and display the local URL (typically `https://localhost:7000`)

### Step 5: Access in Browser
1. Open your browser
2. Navigate to the URL shown in console (e.g., `https://localhost:7000`)
3. Click "Play Game" in the navigation menu

### Step 6: Play the Game

1. **Connect MetaMask**
   - Click "🦊 Connect MetaMask" button
   - MetaMask popup will appear
   - Click "Connect" to approve
   - Your wallet address will display

2. **Start Game**
   - Click "🚀 START GAME 🚀"
   - Game board appears with 5 categories and 25 questions

3. **Answer Questions**
   - Click any question button ($100-$500)
   - Read the question in the modal
   - Click "Reveal Answer" to see the answer
   - Select "✓ Got it right!" or "✗ Got it wrong"
   - Your score updates automatically

4. **Claim Rewards**
   - After answering all questions, game ends
   - See your final score and reward amount
   - Click "💰 Claim Your Tokens"
   - Approve the token transfer in MetaMask

## 📋 Game Info

**Token Rewards**: Arcade1870 (ERC-20)
- Contract: 0x8eddD4edea39c5B5f77662453600F53A202EE47C
- Reward: 0.01 tokens per point
- Max Reward: 37.5 tokens (for perfect score)

**Categories**:
1. 🪙 Bitcoin Basics
2. ⚡ Ethereum Essentials
3. 🔗 Blockchain Technology
4. 💳 Wallets & Transactions
5. 🏦 DeFi & Web3

## 🔧 Configuration

### To Change Token Settings
Edit `appsettings.json`:

```json
{
  "Crypto": {
	"TokenContractAddress": "0x8eddD4edea39c5B5f77662453600F53A202EE47C",
	"TokenSymbol": "Arcade1870",
	"RewardPerPoint": 0.01
  }
}
```

### To Add Custom Questions
Edit `Services/GameService.cs` in the `InitializeCategories()` method:

```csharp
new Question 
{ 
	Id = 26, 
	Text = "What does DeFi stand for?", 
	Answer = "Decentralized Finance", 
	Value = 100 
}
```

## 🌐 Network Support

- **Ethereum Mainnet** (production tokens)
- **Sepolia Testnet** (testing)
- **Polygon** (low-cost transactions)

### To Test on Testnet
1. Add Sepolia network to MetaMask
2. Get test ETH from a faucet (https://sepolia-faucet.pk910.de/)
3. The app will detect and allow connections to Sepolia

## 🎨 Project Structure

```
Crypto Trivia/
├── Components/
│   ├── Pages/
│   │   ├── Home.razor              # Landing page
│   │   ├── CryptoTrivia.razor      # Game page
│   │   └── About.razor              # Info page
│   └── WalletConnect.razor         # Wallet component
├── Services/
│   ├── GameService.cs              # Game logic
│   └── WalletService.cs            # Blockchain services
├── wwwroot/
│   └── js/
│       └── metamask-interop.js     # Web3 integration
└── Program.cs                      # App startup
```

## ❓ Common Issues & Solutions

### "MetaMask is not installed"
**Solution**: Install MetaMask browser extension from metamask.io

### "User rejected the request"
**Solution**: Click "Connect" when MetaMask popup appears

### "Network not supported"
**Solution**: Switch to Ethereum, Sepolia, or Polygon network in MetaMask

### "Build failed"
**Solution**: 
```bash
dotnet clean
dotnet restore
dotnet build
```

### Port already in use
**Solution**: 
```bash
dotnet run --urls=https://localhost:7001
```

## 📚 Learn More

- **README.md** - Complete documentation
- **IMPLEMENTATION_SUMMARY.md** - Technical details
- **About Page** - In-app information

## 🎯 Next Steps

1. Play the game and test all features
2. Try connecting different wallets
3. Test on different networks
4. Deploy to Azure or other hosting
5. Customize questions and rewards
6. Add more features from the roadmap

## 📞 Need Help?

- Check the About page in the app
- Review README.md for detailed docs
- Check browser console for errors (F12)
- Review MetaMask logs for connection issues

---

**Enjoy Crypto Trivia! 🎮**

Test your blockchain knowledge and earn tokens! 🪙
