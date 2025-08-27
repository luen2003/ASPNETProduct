namespace IdentityServer.Domains.Entities
{
    public class ServiceUrl
    {
        public long Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string ShortName { get; set; }
        public string ServicePath { get; set; }
        public int ServiceType { get; set; }
        public string Var1 { get; set; }
        public string Var2 { get; set; }
        public string Var3 { get; set; }
        public string Var4 { get; set; }
        public string Var5 { get; set; }
        public int Status { get; set; }
        public long SysU { get; set; }
        public long SysD { get; set; }
        public long SysV { get; set; }
        public long SysS { get; set; }
        public long PosId { get; set; }
        public long UserId { get; set; }

    }
}
