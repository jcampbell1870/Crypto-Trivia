# Crypto Trivia - Deployment Guide

## 🚀 Deployment Options

This guide covers multiple deployment options for the Crypto Trivia application.

---

## 1. Local Development Deployment

### Requirements
- .NET 10 SDK
- Visual Studio 2026 or VS Code
- MetaMask browser extension

### Steps
```powershell
cd "C:\Users\thund\source\repos\Crypto Trivia"
dotnet run
```

Access at: `https://localhost:7000` (or displayed URL)

---

## 2. Azure App Service Deployment

### Prerequisites
- Azure subscription
- Azure CLI installed
- PowerShell

### Step 1: Create Azure Resource Group
```powershell
az group create `
  --name CryptoTriviaRG `
  --location eastus
```

### Step 2: Create App Service Plan
```powershell
az appservice plan create `
  --name CryptoTriviaPlan `
  --resource-group CryptoTriviaRG `
  --sku B1 `
  --is-linux
```

### Step 3: Create Web App
```powershell
az webapp create `
  --resource-group CryptoTriviaRG `
  --plan CryptoTriviaPlan `
  --name cryptotrivia-app `
  --runtime "DOTNETCORE|8.0"
```

### Step 4: Configure Application Settings
```powershell
az webapp config appsettings set `
  --resource-group CryptoTriviaRG `
  --name cryptotrivia-app `
  --settings ASPNETCORE_ENVIRONMENT=Production
```

### Step 5: Deploy from Local Git
```powershell
cd "C:\Users\thund\source\repos\Crypto Trivia"
dotnet publish -c Release -o publish
cd publish
Compress-Archive -Path * -DestinationPath app.zip
az webapp deployment source config-zip `
  --resource-group CryptoTriviaRG `
  --name cryptotrivia-app `
  --src app.zip
```

### Access Deployed App
Visit: `https://cryptotrivia-app.azurewebsites.net`

---

## 3. Docker Containerization

### Create Dockerfile

Create `Dockerfile` in project root:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80 443
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Crypto_Trivia.dll"]
```

### Create .dockerignore

```
.git
.gitignore
bin
obj
.vs
.vscode
*.user
*.db
*.log
node_modules
```

### Build Docker Image
```powershell
docker build -t cryptotrivia:latest .
```

### Run Docker Container
```powershell
docker run -d `
  --name cryptotrivia `
  -p 8080:80 `
  cryptotrivia:latest
```

Access at: `http://localhost:8080`

### Push to Docker Hub
```powershell
# Tag image
docker tag cryptotrivia:latest yourusername/cryptotrivia:latest

# Login
docker login

# Push
docker push yourusername/cryptotrivia:latest
```

---

## 4. Azure Container Registry & Azure Container Instances

### Create Container Registry
```powershell
az acr create `
  --resource-group CryptoTriviaRG `
  --name cryptotriviaregistry `
  --sku Basic
```

### Build and Push Image
```powershell
az acr build `
  --registry cryptotriviaregistry `
  --image cryptotrivia:latest `
  --file Dockerfile .
```

### Deploy to Container Instances
```powershell
az container create `
  --resource-group CryptoTriviaRG `
  --name cryptotrivia-container `
  --image cryptotriviaregistry.azurecr.io/cryptotrivia:latest `
  --registry-login-server cryptotriviaregistry.azurecr.io `
  --registry-username <username> `
  --registry-password <password> `
  --ports 80 443 `
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

---

## 5. GitHub Pages with Static Export (Alternative)

### Export as Static Site
```powershell
dotnet publish -c Release -o ./gh-pages
```

### Setup GitHub Actions
Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to GitHub Pages

on:
  push:
	branches: [ main ]

jobs:
  deploy:
	runs-on: ubuntu-latest
	steps:
	- uses: actions/checkout@v2
	- uses: actions/setup-dotnet@v1
	  with:
		dotnet-version: '10.0.x'
	- run: dotnet publish -c Release -o ./gh-pages
	- uses: peaceiris/actions-gh-pages@v3
	  with:
		github_token: ${{ secrets.GITHUB_TOKEN }}
		publish_dir: ./gh-pages/wwwroot
```

---

## 6. Production Checklist

Before deploying to production:

### Security
- [ ] Remove debug symbols: `PublishTrimmed=true` in .csproj
- [ ] Enable HTTPS enforcement
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure CORS if needed
- [ ] Enable HSTS headers
- [ ] Use environment variables for secrets

### Configuration
- [ ] Update `appsettings.Production.json` with production token contract
- [ ] Configure correct blockchain network (Ethereum Mainnet)
- [ ] Set up error logging (Application Insights)
- [ ] Configure backup strategy

### Performance
- [ ] Enable compression
- [ ] Configure caching headers
- [ ] Use CDN for static assets
- [ ] Monitor application performance

### Monitoring
- [ ] Set up Application Insights
- [ ] Configure alerts
- [ ] Enable logging
- [ ] Monitor blockchain transaction costs

### Testing
- [ ] Test on Sepolia Testnet first
- [ ] Verify token transfer functionality
- [ ] Test all browsers
- [ ] Test MetaMask connection
- [ ] Load testing

---

## 7. Environment Configuration

### Development (appsettings.Development.json)
```json
{
  "Crypto": {
	"SupportedChains": ["0xaa36a7"],
	"TokenContractAddress": "0x8eddD4edea39c5B5f77662453600F53A202EE47C"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Debug"
	}
  }
}
```

### Production (appsettings.Production.json)
```json
{
  "Crypto": {
	"SupportedChains": ["0x1", "0x89"],
	"TokenContractAddress": "0x8eddD4edea39c5B5f77662453600F53A202EE47C"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Warning"
	}
  }
}
```

---

## 8. Cost Estimation (Azure)

### Azure App Service (B1 Plan)
- Monthly cost: ~$55 USD
- 1 vCPU
- 1.75 GB RAM
- 10 GB storage

### Azure Container Registry
- Monthly cost: ~$30 USD
- Basic tier

### Application Insights
- Monthly cost: ~$2-10 USD (based on usage)

**Total Estimated Monthly Cost: $87-95 USD**

---

## 9. Scaling Considerations

### Database (if needed for scores/leaderboard)
```powershell
# Create Azure SQL Database
az sql server create `
  --name cryptotriviaserver `
  --resource-group CryptoTriviaRG `
  --admin-user serveradmin `
  --admin-password <secure-password>

az sql db create `
  --resource-group CryptoTriviaRG `
  --server cryptotriviaserver `
  --name cryptotriviadb `
  --edition Basic
```

### Redis Cache (for session state)
```powershell
az redis create `
  --name cryptotrivia-cache `
  --resource-group CryptoTriviaRG `
  --sku Basic `
  --vm-size c0
```

---

## 10. Troubleshooting Deployment

### Azure Deployment Issues
```powershell
# View logs
az webapp log tail --name cryptotrivia-app --resource-group CryptoTriviaRG

# Check deployment status
az webapp deployment slot list --name cryptotrivia-app --resource-group CryptoTriviaRG

# Restart app
az webapp restart --name cryptotrivia-app --resource-group CryptoTriviaRG
```

### Docker Issues
```powershell
# View logs
docker logs cryptotrivia

# Check if container is running
docker ps

# SSH into container
docker exec -it cryptotrivia /bin/bash
```

### Blockchain Connection Issues
- Verify token contract address
- Check network connectivity
- Ensure MetaMask is on correct network
- Review browser console for errors

---

## 11. CI/CD Pipeline Example

### Azure Pipelines (azure-pipelines.yml)

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'

stages:
- stage: Build
  jobs:
  - job: BuildJob
	steps:
	- task: UseDotNet@2
	  inputs:
		version: $(dotnetVersion)
	- task: DotNetCoreCLI@2
	  inputs:
		command: 'build'
		arguments: '--configuration $(buildConfiguration)'
	- task: DotNetCoreCLI@2
	  inputs:
		command: 'test'
	- task: DotNetCoreCLI@2
	  inputs:
		command: 'publish'
		publishWebProjects: true
		arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
	- task: PublishBuildArtifacts@1

- stage: Deploy
  dependsOn: Build
  condition: succeeded()
  jobs:
  - deployment: DeployWeb
	environment: 'Production'
	strategy:
	  runOnce:
		deploy:
		  steps:
		  - task: AzureWebApp@1
			inputs:
			  azureSubscription: 'Azure Subscription'
			  appType: 'webAppLinux'
			  appName: 'cryptotrivia-app'
			  package: '$(Pipeline.Workspace)/drop/**/*.zip'
```

---

## 12. Post-Deployment Verification

After deployment, verify:

```powershell
# Test health endpoint
Invoke-WebRequest -Uri "https://cryptotrivia-app.azurewebsites.net/health"

# Check MetaMask connectivity
# Navigate to /game and test wallet connection

# Verify token reward functionality
# Complete a game and attempt to claim tokens

# Monitor performance
# Review Application Insights dashboard
```

---

## 13. Rollback Procedures

### Azure App Service Rollback
```powershell
# View deployment history
az webapp deployment list --resource-group CryptoTriviaRG --name cryptotrivia-app

# Redeploy previous version
az webapp deployment slot swap `
  --resource-group CryptoTriviaRG `
  --name cryptotrivia-app `
  --slot staging
```

### Docker Rollback
```powershell
# Pull previous image
docker pull yourusername/cryptotrivia:v1.0.0

# Stop current container
docker stop cryptotrivia

# Run previous version
docker run -d --name cryptotrivia-old -p 8080:80 yourusername/cryptotrivia:v1.0.0
```

---

## Summary

| Method | Complexity | Cost | Scale |
|--------|-----------|------|-------|
| Local Development | Low | $0 | 1 user |
| Azure App Service | Medium | $55-100/mo | 1k-10k users |
| Docker | Medium | $10-50/mo | 10k+ users |
| Kubernetes (AKS) | High | $100-500/mo | 100k+ users |

---

**Recommended for Production**: Azure App Service B2 or Docker on Container Instances

For questions or issues during deployment, refer to:
- Azure documentation: https://docs.microsoft.com/azure/
- Docker documentation: https://docs.docker.com/
- Blazor deployment guide: https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/

