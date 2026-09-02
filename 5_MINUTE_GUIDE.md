# 🎮 Crypto Trivia - 5 Minute Quick Guide

## ⏱️ START HERE - 5 Minutes to Game Launch

---

## 🚀 Minute 1: Preparation

### Prerequisites (have these ready)
✅ Visual Studio 2026 open  
✅ Terminal/PowerShell ready  
✅ MetaMask installed in browser  
✅ Internet connection  

### Get to Project Directory
```powershell
cd "C:\Users\thund\source\repos\Crypto Trivia"
```

---

## ⚡ Minute 2: Build the Project

```powershell
dotnet run
```

**What happens:**
- Project compiles
- NuGet packages restore
- Development server starts
- URL displays (usually `https://localhost:7000`)

**Watch for:** Green checkmarks in output = SUCCESS

---

## 🌐 Minute 3: Open in Browser

1. **Copy the URL** from terminal output
2. **Paste into browser** (Chrome recommended)
3. **Page loads** with Crypto Trivia home page

### You should see:
- 🎮 "CRYPTO TRIVIA" title
- "Master Blockchain Knowledge & Earn Tokens" subtitle
- Feature cards
- "PLAY NOW" button

---

## 🦊 Minute 4: Connect MetaMask

### Click "Play Game" in navigation
- See welcome screen with blue/purple background
- See "Connect MetaMask" button

### Click "Connect MetaMask"
- MetaMask popup appears
- Click "Connect"
- See wallet address display

### Now you can play!

---

## 🎯 Minute 5: Play & Test

### Start Game
1. Click "🚀 START GAME 🚀" button
2. Game board appears with 5 categories

### Play a Question
1. Click any question ($100, $200, etc.)
2. Modal opens with question
3. Click "Reveal Answer"
4. Answer appears
5. Click "✓ Got it right!" or "✗ Got it wrong!"
6. Score updates

### Repeat & See Results
- Answer all 25 questions (or just a few to test)
- Watch score accumulate
- See reward calculation update
- Click "Game Over" when done (or answer all)

---

## ✅ That's It!

You now have:
✅ Crypto Trivia running locally  
✅ MetaMask connected  
✅ Game fully playable  
✅ All features working  

---

## 📱 What's Next?

### Option A: Continue Testing (Recommended)
- Play through more questions
- Try "Play Again"
- Test different browsers
- Read "About" page

### Option B: Deploy to Cloud
- See `DEPLOYMENT_GUIDE.md`
- Takes 15-30 minutes
- Multiple options (Azure, Docker, etc.)

### Option C: Customize
- See `IMPLEMENTATION_SUMMARY.md`
- Modify questions in `Services/GameService.cs`
- Adjust rewards in `appsettings.json`

---

## 🆘 Quick Troubleshooting

### "Port already in use"
```powershell
dotnet run --urls=https://localhost:7001
```

### "MetaMask not found"
Install from: https://metamask.io/

### "Build fails"
```powershell
dotnet clean
dotnet restore
dotnet build
```

### "Page won't load"
- Check firewall
- Try different port
- Clear browser cache (Ctrl+Shift+Delete)

---

## 📚 Learn More

| Want to... | Read |
|-----------|------|
| Full details | `README.md` |
| Deploy online | `DEPLOYMENT_GUIDE.md` |
| Customize game | `IMPLEMENTATION_SUMMARY.md` |
| API reference | `API_INTEGRATION.md` |
| Check progress | `PROJECT_SUMMARY.md` |

---

## 🎉 Success Indicators

You'll know everything is working when:
- ✅ Application starts without errors
- ✅ Page loads in browser
- ✅ Navigation menu visible
- ✅ MetaMask connects
- ✅ Game board displays all 25 questions
- ✅ Score updates when answering
- ✅ Reward amount calculates
- ✅ No console errors (F12 to check)

---

## 💡 Pro Tips

1. **Use Chrome** - Most compatible
2. **Test on Sepolia** - Free testnet
3. **Try mobile** - Responsive design works great
4. **Read the docs** - Everything is documented
5. **Save your URLs** - For future reference

---

## 🚀 Ready?

```powershell
cd "C:\Users\thund\source\repos\Crypto Trivia"
dotnet run
```

Then visit: **https://localhost:7000**

### That's all you need! 🎮

---

**Questions? Check the detailed guides in the project root directory.**

Happy gaming! 🎊
