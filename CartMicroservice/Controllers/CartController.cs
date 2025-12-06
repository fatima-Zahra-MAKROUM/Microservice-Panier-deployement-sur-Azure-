using CartMicroservice.Models;
using CartMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // → /api/cart
    public class CartController : ControllerBase
    {
        private readonly ICartService _shoppingCartService;

        public CartController(ICartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // GET /api/cart/{userId} → Récupère le panier d'un utilisateur
        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> RetrieveUserCart(string userId)
        {
            var cart = await _shoppingCartService.FetchCartByUserAsync(userId);
            return Ok(cart);
        }

        // POST /api/cart/{userId}/items → Ajoute un produit (ou incrémente)
        [HttpPost("{userId}/items")]
        public async Task<ActionResult<Cart>> AddProductToCart(string userId, [FromBody] CartRequest productRequest)
        {
            if (productRequest.Quantity <= 0)
                return BadRequest(new { error = "La quantité doit être > 0" });

            if (productRequest.Price < 0)
                return BadRequest(new { error = "Le prix doit être >= 0" });

            var updatedCart = await _shoppingCartService.InsertProductAsync(userId, productRequest);
            return CreatedAtAction(nameof(RetrieveUserCart), new { userId }, updatedCart);
        }

        // PUT /api/cart/{userId}/items/{productId} → Modifie la quantité (0 = suppression)
        [HttpPut("{userId}/items/{productId}")]
        public async Task<ActionResult<Cart>> UpdateProductQuantity(
            string userId, int productId, [FromBody] CartRequest productRequest)
        {
            if (productRequest.Quantity < 0)
                return BadRequest(new { error = "La quantité doit être >= 0" });

            try
            {
                var updatedCart = await _shoppingCartService.ModifyProductQuantityAsync(userId, productId, productRequest.Quantity);
                return Ok(updatedCart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // DELETE /api/cart/{userId}/items/{productId} → Supprime un article
        [HttpDelete("{userId}/items/{productId}")]
        public async Task<ActionResult<Cart>> RemoveProductFromCart(string userId, int productId)
        {
            var updatedCart = await _shoppingCartService.DeleteProductAsync(userId, productId);
            return Ok(updatedCart);
        }

        // DELETE /api/cart/{userId} → Vide complètement le panier
        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearUserCart(string userId)
        {
            var deleted = await _shoppingCartService.EmptyCartAsync(userId);
            return deleted ? NoContent() : Ok(new { message = "Panier déjà vide" });
        }

        // GET /api/cart/{userId}/exists → Utile pour le frontend (éviter appels inutiles)
        [HttpGet("{userId}/exists")]
        public async Task<ActionResult> CheckCartExistence(string userId)
        {
            var exists = await _shoppingCartService.VerifyCartExistenceAsync(userId);
            return Ok(new { userId, exists });
        }
    }
}