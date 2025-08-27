using ServiceAPI.Models.MD;

namespace ServiceAPI.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<List<Customer>> GetAllData();

        public Task<List<Customer>> GetDatabyCustomerCode(string customer);

    }
}
