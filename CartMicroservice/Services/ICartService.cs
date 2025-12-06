using CartMicroservice.Models;

namespace CartMicroservice.Services
{
    // Contrat du service panier (facilite les tests unitaires et le remplacement futur)
    public interface ICartService
    {
        // Récupère le panier (crée un vide si inexistant)
        Task<Cart> FetchCartByUserAsync(string customerId);

        // Ajoute ou incrémente un produit
        Task<Cart> InsertProductAsync(string customerId, CartRequest productData);

        // Met à jour la quantité (0 = suppression)
        Task<Cart> ModifyProductQuantityAsync(string customerId, int productIdentifier, int newQuantity);

        // Supprime un produit du panier
        Task<Cart> DeleteProductAsync(string customerId, int productIdentifier);

        // Vide complètement le panier
        Task<bool> EmptyCartAsync(string customerId);

        // Vérifie si un panier existe déjà dans Redis
        Task<bool> VerifyCartExistenceAsync(string customerId);
    }
}