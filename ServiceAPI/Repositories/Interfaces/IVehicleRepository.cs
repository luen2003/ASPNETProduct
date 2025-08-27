using ServiceAPI.Models.MD;
using ServiceAPI.Models.TransactionResponse;

namespace ServiceAPI.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        public Task<List<Vehicle>> GetAllData();

        public Task<List<Vehicle>> GetDatabyVehicleCode(string vehicle);

        public Task<ResponseSaveVehicle> SaveVehicleNewRepo(VehicleSaveRequest request);

    }
}
