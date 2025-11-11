using System.Collections;

namespace ProjectEcomerceFinal.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);
    }
}