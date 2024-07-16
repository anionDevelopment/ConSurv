using ContinuousSurveillanceBackend.Core.Database.Contexts.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public class TransientPersistence : IPersistence
    {
        private readonly IDictionary<int, Customer> _Customer;
        public TransientPersistence()
        {
            this._Customer = new Dictionary<int, Customer>();
        }
        public void SetCustomer(Customer customer)
        {
            this._Customer[customer.Id] = customer;
        }

        public ISet<Customer> GetAllCustomer()
        {
            return this._Customer.Values.ToHashSet();
        }

        public Customer GetCustomer(int customerId)
        {
            return this._Customer[customerId];
        }

        public void RemoveCustomer(int customerId)
        {
            if (this._Customer.ContainsKey(customerId))
            {
                this._Customer.Remove(customerId);
            }
        }

        public bool IsAvailable()
        {
            return true;
        }
    }
}
