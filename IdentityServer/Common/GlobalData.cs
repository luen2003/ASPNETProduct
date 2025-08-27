
namespace Mobile.MasterDataService
{
    public static class GlobalData
    {
        public static Audience audience { get { return _audience; } }

        private static Audience _audience { get; set; }

        public static void InitData()
        {
            _audience = new Audience();
            IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
            var audienceConfig = configuration.GetSection("Audience");

            if (audienceConfig != null)
            {
                _audience.Secret = audienceConfig["Secret"];
                _audience.Iss = audienceConfig["Iss"];
                _audience.Aud = audienceConfig["Aud"];
            }

        }
    }

    
    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}
