using DatabaseLibrary.Models;

namespace DatabaseLibrary.Services
{
    public class CartService
    {
        private List<CartItem> _cartItems = new List<CartItem>();

        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Cost = product.Cost,
                    Manufacturer = product.Manufacturer,
                    Quantity = quantity
                });
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = _cartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                if (item.Quantity <= 0)
                {
                    RemoveFromCart(productId);
                }
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }

        public List<CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public decimal GetTotalPrice()
        {
            return _cartItems.Sum(item => item.Cost * item.Quantity);
        }

        public int GetTotalItems()
        {
            return _cartItems.Sum(item => item.Quantity);
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice => Cost * Quantity;
    }
}