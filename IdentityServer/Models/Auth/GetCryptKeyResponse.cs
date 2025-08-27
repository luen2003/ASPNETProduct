using IdentityServer.Domains.Dto;
using IdentityServer.Models.Settings;

namespace IdentityServer.Models.Auth
{
    public class GetCryptKeyResponse
    { 
        public GetCryptKeyResponse() { }
        public GetCryptKeyResponse(RSACrypt defaultCryptKeys)
        {
            CryptKey = defaultCryptKeys.PublicKey;
            KeySize = defaultCryptKeys.KeySize;
        }
        public string CryptKey { get; set; }
        public int KeySize { get; set; }
    }
        
   
}
