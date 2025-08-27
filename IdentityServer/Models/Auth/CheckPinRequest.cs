namespace IdentityServer.Models.Auth
{
    public class CheckPinRequest
    {
        public string RequestId { get; set; }
        public string RequestDate { get; set; }
        public string PlxId { get; set; }
        public string? VehicleNo { get; set; }
        public string PointType { get; set; }
        public long pumpTranType { get; set; }
        public string POS { get; set; }
        public string Company { get; set; }
        public string TID { get; set; }
        public string TXNID { get; set; }
        public string PIN { get; set; }
        public string CheckSum { get; set; }
    }
}
