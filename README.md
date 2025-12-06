# 🛒 Microservice de Gestion de Panier - Déploiement Azure

## 📌 Description
Microservice ASP.NET Core pour la gestion d'un panier d'achat avec Redis comme système de cache, déployé sur Azure Container Apps.

## 🏗️ Architecture
- **Backend** : ASP.NET Core 8.0
- **Cache** : Azure Cache for Redis
- **Conteneurisation** : Docker
- **Registry** : Azure Container Registry (ACR)
- **Déploiement** : Azure Container Apps
- **Région** : France Central

## 🚀 Déploiement Azure

### Ressources créées
- **Resource Group** : `rg-panier-microservice`
- **Redis Cache** : `redis-panier-cache` (Basic, C0)
- **Container Registry** : `acrpaniermicroservice`
- **Container App** : `panier-api`
- **Environment** : `panier-env`

### URL de l'API déployée
```
https://panier-api.blackcliff-aaac1926.francecentral.azurecontainerapps.io
```

## 📡 Endpoints API

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/cart/{userId}` | Récupérer le panier |
| POST | `/api/cart/{userId}/items` | Ajouter un article |
| PUT | `/api/cart/{userId}/items/{productId}` | Modifier la quantité |
| DELETE | `/api/cart/{userId}/items/{productId}` | Supprimer un article |
| DELETE | `/api/cart/{userId}` | Vider le panier |

## 🧪 Tests de l'API

### Récupérer un panier
```powershell
Invoke-RestMethod -Uri "https://panier-api.blackcliff-aaac1926.francecentral.azurecontainerapps.io/api/cart/user123" -Method Get
```

### Ajouter un article
```powershell
$body = @{
    ProductId = 101
    ProductName = "Laptop HP"
    Price = 899.99
    Quantity = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://panier-api.blackcliff-aaac1926.francecentral.azurecontainerapps.io/api/cart/user123/items" -Method Post -Body $body -ContentType "application/json"
```

## 🐳 Docker

### Build local
```bash
docker build -t acrpaniermicroservice.azurecr.io/panier-microservice:v1 .
```

### Push vers ACR
```bash
az acr login --name acrpaniermicroservice
docker push acrpaniermicroservice.azurecr.io/panier-microservice:v1
```

## 📦 Structure du projet
```
CartMicroservice/
├── Controllers/
│   └── CartController.cs
├── Models/
│   ├── Cart.cs
│   └── CartItem.cs
├── Services/
│   └── CartService.cs
├── Program.cs
├── CartMicroservice.csproj
├── Dockerfile
└── README.md
```

## ⚙️ Configuration

### Variables d'environnement
- `Redis__ConnectionString` : Connexion à Azure Redis Cache
- `Redis__InstanceName` : Préfixe des clés Redis (`panier:`)

## 🔧 Commandes Azure CLI utilisées

### Créer les ressources
```bash
# Resource Group
az group create --name rg-panier-microservice --location francecentral

# Redis Cache
az redis create --resource-group rg-panier-microservice --name redis-panier-cache --location francecentral --sku Basic --vm-size c0

# Container Registry
az acr create --resource-group rg-panier-microservice --name acrpaniermicroservice --sku Basic --location francecentral

# Container App Environment
az containerapp env create --name panier-env --resource-group rg-panier-microservice --location francecentral

# Deploy Container App
az containerapp create --name panier-api --resource-group rg-panier-microservice --environment panier-env --image acrpaniermicroservice.azurecr.io/panier-microservice:v1 --target-port 8080 --ingress external
```

## 📊 Captures d'écran

### Déploiement réussi
![Déploiement](screenshots/deployment-success.png)

### Test de l'API
![Test API](screenshots/api-test.png)

### Ressources Azure
![Resources Azure](screenshots/azure-resources.png)

## 👤 Auteur
- **Nom** : [Votre Nom]
- **Date** : Décembre 2025
- **Cours** : [Nom du cours]

## 📝 Notes
- Le projet utilise Azure for Students
- Le déploiement est effectué via Azure CLI
- Les données du panier sont stockées dans Redis et persistent entre les sessions