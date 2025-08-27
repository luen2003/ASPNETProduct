using System.Text.Json.Serialization;

namespace IdentityServer.Models.Auth
{
    public class ResponsePLX
    {
        public string? responseId { get; set; }
        public string? message { get; set; }
        public string? checkSum { get; set; }
        public string? statusCode { get; set; }
        public string? responseDate { get; set; }
        public DataPLX? data { get; set; }
    }

    public class DataPLX
    {
        public string? address { get; set; }
        public string? sex { get; set; }
        public int provinceId { get; set; }
        public int districtId { get; set; }
        public int wardId { get; set; }
        public string? birthday { get; set; }
        public string? otpPhone { get; set; }
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
        public string? accountStatus { get; set; }
        public string? plxId { get; set; }
        public string? fullName { get; set; }
        public string? taxNumber { get; set; }
        public string? mobileNumber { get; set; }
        public string? emailAddress { get; set; }
        public int type { get; set; }
    }
}
