
namespace ServiceAPI.Models.MD
{
    public class CustomerRequest
    {
        public string? customerCode { get; set; }
    }
    public class ResponseCustomer
    {
        public string? errorCode { get; set; }
        public string? description { get; set; }
        public List<Customer> cust { get; set; } = new();

    }
    public class Customer
    {
        public string? custcode { get; set; }
        public string? custName { get; set; } 
        public string? shortName { get; set; }
        public string? taxCode { get; set; } 
        public string? tel { get; set; } 
        public string? fax { get; set; } 
        public string? address { get; set; } 
        public string? province { get; set; } 
        public string? sysD { get; set; }
        public string? status { get; set; } 
    
    }
}
