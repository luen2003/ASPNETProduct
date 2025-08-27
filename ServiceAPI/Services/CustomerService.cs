using ServiceAPI.Authentication; 
using ServiceAPI.Repositories.Interfaces;
using ServiceAPI.Models.MD;
using ServiceAPI.Utils;
using ServiceAPI.Models.TransactionRequest;

namespace ServiceAPI.Services
{
    public class CustomerService 
    {
        private readonly ICustomerRepository _CustomerRepo;
        private readonly ApplicationUser _applicationUser;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(
            ICustomerRepository driverPurchaseRepo,
            ApplicationUser applicationUser,
            ILogger<CustomerService> logger)
        {
            _CustomerRepo = driverPurchaseRepo;
            _applicationUser = applicationUser;
            _logger = logger;
        }
        public async Task<ResponseCustomer> GetAllData()
        {
            ResponseCustomer responseCustomer = new();
            try
            {
                var CustomerData = await _CustomerRepo.GetAllData();
                if (CustomerData != null && CustomerData.Count() > 0)
                {
                    responseCustomer.errorCode = "00";
                    responseCustomer.description = "Thành công";
                    responseCustomer.cust = CustomerData;
                }
                else
                {
                    responseCustomer.errorCode = "01";
                    responseCustomer.description = "Không có dữ liệu từ ERP ";
                    responseCustomer.cust = null;
                }
            }
            catch (Exception ex)
            {
                responseCustomer.errorCode = "500";
                responseCustomer.description = "ERR:" + ex.Message + ex.StackTrace;
                responseCustomer.cust = null;
            }
            return responseCustomer;
        }

        public async Task<ResponseCustomer> GetDatabyCustomerCode(CustomerRequest request)
        {
            ResponseCustomer responseCustomer = new();
            try
            {
                var CustomerData = await _CustomerRepo.GetDatabyCustomerCode(request.customerCode);
                if (CustomerData != null && CustomerData.Count() > 0)
                {
                    responseCustomer.errorCode = "00";
                    responseCustomer.description = "Thành công";
                    responseCustomer.cust = CustomerData;
                }
                else
                {
                    responseCustomer.errorCode = "01";
                    responseCustomer.description = "Không có dữ liệu từ ERP ";
                    responseCustomer.cust = null;
                }
            }
            catch (Exception ex)
            {
                responseCustomer.errorCode = "500";
                responseCustomer.description = "ERR:" + ex.Message + ex.StackTrace;
                responseCustomer.cust = null;
            }
            return responseCustomer;
        }

    }
}
