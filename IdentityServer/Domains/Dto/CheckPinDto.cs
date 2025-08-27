namespace IdentityServer.Models.Auth
{
    public class CheckPinDto
    {
        public string RequestId { get; set; }
        public string RequestDate { get; set; }
        public string PlxId { get; set; }
        public string VehicleNo { get; set; }
        public string PointType { get; set; }
        public string POS { get; set; }
        public string Company { get; set; }
        public string TID { get; set; }
        public string TXNID { get; set; }
        public string PIN { get; set; }
        public long UserPinId { get; set; }
        public long CountFalse { get; set; }
        public long PINState { get; set; }
        public long LockDate { get; set; }
        public string MsgText { get; set; }
        public string ResDate { get; set; }
        public string UserId { get; set; }

    }
}
