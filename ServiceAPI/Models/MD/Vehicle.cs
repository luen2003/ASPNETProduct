
using ServiceAPI.Models.TransactionResponse;

namespace ServiceAPI.Models.MD
{
    public class VehicleSaveRequest
    {
        public List<VehicleRequest> data { get; set; } = new();
    }

    public class VehicleRequest
    {
        public string? vehicleCode { get; set; }

        public string? vehicleGroup { get; set; }
    }
    public class ResponseLstVehicle
    {
        public string? errorCode { get; set; }
        public string? description { get; set; }
        public List<Vehicle> vehicle { get; set; } = new();

    }
    public class Vehicle
    {
        public string? vehicleCode { get; set; }
        public string? vehicleName { get; set; }  
    
    }

    public class ResponseSaveVehicle
    {
        public string? errorCode { get; set; }
        public string? description { get; set; }
        public string? listSuccess { get; set; }
        public string? listFail { get; set; }

    }
}
