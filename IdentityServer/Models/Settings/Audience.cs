namespace IdentityServer.Models.Settings
{
    public class Audience
    {
        public Audience(IConfiguration configuration)
        {
            var audienceConfig = configuration.GetSection("Audience");

            Secret = audienceConfig["Secret"];
            Iss = audienceConfig["Iss"];
            Aud = audienceConfig["Aud"];
            Expires = Convert.ToInt16(audienceConfig["Expires"]);

        }

        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; } 
        public int Expires { get; set; }
        
    }
}
