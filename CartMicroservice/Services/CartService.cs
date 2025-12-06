using CartMicroservice.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CartMicroservice.Services
{
    public class CartService : ICartService
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _cacheKeyPrefix;

        public CartService(IConnectionMultiplexer redisConnection, IConfiguration configuration)
        {
            _redisDatabase = redisConnection.GetDatabase();
            // Préfixe configurable 
            _cacheKeyPrefix = configuration["Redis:InstanceName"] ?? "cart:";
        }

        // Construit la clé Redis 
        private string BuildCacheKey(string customerId) => $"{_cacheKeyPrefix}{customerId}";

        public async Task<Cart> FetchCartByUserAsync(string customerId)
        {
            var key = BuildCacheKey(customerId);
            var data = await _redisDatabase.StringGetAsync(key);

            if (data.IsNullOrEmpty)
                return new Cart { UserId = customerId }; // Panier vide

            return JsonConvert.DeserializeObject<Cart>(data!) ?? new Cart { UserId = customerId };
        }

        public async Task<Cart> InsertProductAsync(string customerId, CartRequest productData)
        {
            var cart = await FetchCartByUserAsync(customerId);

            var existing = cart.Items.FirstOrDefault(p => p.ProductId == productData.ProductId);

            if (existing != null)
                existing.Quantity += productData.Quantity;
            else
                cart.Items.Add(new CartItem
                {
                    ProductId = productData.ProductId,
                    ProductName = productData.ProductName,
                    Price = productData.Price,
                    Quantity = productData.Quantity
                });

            return await PersistCartDataAsync(customerId, cart);
        }

        public async Task<Cart> ModifyProductQuantityAsync(string customerId, int productId, int newQuantity)
        {
            var cart = await FetchCartByUserAsync(customerId);
            var item = cart.Items.FirstOrDefault(p => p.ProductId == productId)
                       ?? throw new KeyNotFoundException($"Produit {productId} non trouvé dans le panier");

            if (newQuantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = newQuantity;

            return await PersistCartDataAsync(customerId, cart);
        }

        public async Task<Cart> DeleteProductAsync(string customerId, int productId)
        {
            var cart = await FetchCartByUserAsync(customerId);
            cart.Items.RemoveAll(p => p.ProductId == productId);
            return await PersistCartDataAsync(customerId, cart);
        }

        public async Task<bool> EmptyCartAsync(string customerId)
        {
            return await _redisDatabase.KeyDeleteAsync(BuildCacheKey(customerId));
        }

        public async Task<bool> VerifyCartExistenceAsync(string customerId)
        {
            return await _redisDatabase.KeyExistsAsync(BuildCacheKey(customerId));
        }

        // Sauvegarde le panier dans Redis avec expiration 7 jours
        private async Task<Cart> PersistCartDataAsync(string customerId, Cart cart)
        {
            cart.LastUpdated = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(cart);
            await _redisDatabase.StringSetAsync(
                BuildCacheKey(customerId),
                json,
                TimeSpan.FromDays(7)); // TTL 7 jours

            return cart;
        }
    }
}