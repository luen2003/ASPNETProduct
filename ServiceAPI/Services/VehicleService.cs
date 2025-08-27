using ServiceAPI.Authentication; 
using ServiceAPI.Repositories.Interfaces;
using ServiceAPI.Models.MD;
using ServiceAPI.Utils;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Models.TransactionResponse;

namespace ServiceAPI.Services
{
    public class VehicleService 
    {
        private readonly IVehicleRepository _VehicleRepo;
        private readonly ApplicationUser _applicationUser;
        private readonly ILogger<VehicleService> _logger;
        public VehicleService(
            IVehicleRepository driverPurchaseRepo,
            ApplicationUser applicationUser,
            ILogger<VehicleService> logger)
        {
            _VehicleRepo = driverPurchaseRepo;
            _applicationUser = applicationUser;
            _logger = logger;
        }
        public async Task<ResponseLstVehicle> GetAllData()
        {
            ResponseLstVehicle responseVehicle = new();
            try
            {
                var VehicleData = await _VehicleRepo.GetAllData();
                if (VehicleData != null && VehicleData.Count() > 0)
                {
                    responseVehicle.errorCode = "00";
                    responseVehicle.description = "Thành công";
                    responseVehicle.vehicle = VehicleData;
                }
                else
                {
                    responseVehicle.errorCode = "01";
                    responseVehicle.description = "Không có dữ liệu từ ERP ";
                    responseVehicle.vehicle = null;
                }
            }
            catch (Exception ex)
            {
                responseVehicle.errorCode = "500";
                responseVehicle.description = "ERR:" + ex.Message + ex.StackTrace;
                responseVehicle.vehicle = null;
            }
            return responseVehicle;
        }

        public async Task<ResponseLstVehicle> GetDatabyVehicleCode(VehicleRequest request)
        {
            ResponseLstVehicle responseVehicle = new();
            try
            {
                request.vehicleCode = request.vehicleCode.Replace(" ", "").Replace("-", "").Replace(".", "");
                var VehicleData = await _VehicleRepo.GetDatabyVehicleCode(request.vehicleCode);
                if (VehicleData != null && VehicleData.Count() > 0)
                {
                    responseVehicle.errorCode = "00";
                    responseVehicle.description = "Thành công";
                    responseVehicle.vehicle = VehicleData;
                }
                else
                {
                    responseVehicle.errorCode = "01";
                    responseVehicle.description = "Không có dữ liệu từ ERP ";
                    responseVehicle.vehicle = null;
                }
            }
            catch (Exception ex)
            {
                responseVehicle.errorCode = "500";
                responseVehicle.description = "ERR:" + ex.Message + ex.StackTrace;
                responseVehicle.vehicle = null;
            }
            return responseVehicle;
        }

        public async Task<ResponseSaveVehicle> SaveVehicleNew(VehicleSaveRequest request)
        {
            ResponseSaveVehicle responseVehicle = new();
            try
            {
                if (request.data.Count() <= 0)
                {
                    responseVehicle.errorCode = "03";
                    responseVehicle.description = "Thông tin gửi đi trống";
                    return responseVehicle;
                }
                else
                {
                    // Thực hiện lưu dữ liệu vào hệ thống ERP
                    var resut = await _VehicleRepo.SaveVehicleNewRepo(request);
                    if (resut.errorCode == "00")
                    {
                        responseVehicle.errorCode = "00";
                        responseVehicle.description = "Thành công";
                        responseVehicle.listSuccess = resut.listSuccess;
                    }
                    else
                    {
                        responseVehicle = resut;
                    }
                }
            }
            catch (Exception ex)
            {
                responseVehicle.errorCode = "500";
                responseVehicle.description = "ERR:" + ex.Message + ex.StackTrace;
            }
            return responseVehicle;
        }

    }
}
