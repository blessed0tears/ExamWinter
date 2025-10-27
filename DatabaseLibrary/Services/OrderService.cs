using DatabaseLibrary.Data;
using DatabaseLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLibrary.Services
{
    public class OrderService
    {
        private readonly ShopContext _context = new();

        public async Task<List<PickupPoint>> GetPickupPointsAsync()
        {
            return await _context.PickupPoints.ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderProduct> orderProducts)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var orderProduct in orderProducts)
                {
                    orderProduct.OrderId = order.OrderId;
                    _context.OrderProducts.Add(orderProduct);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.PickupPoint)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}