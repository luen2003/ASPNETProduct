using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Models.Settings
{
    public class RSACrypt
    {
        public RSACrypt(IConfiguration configuration)
        {
            var audienceConfig = configuration.GetSection("DefaultCryptKeys");

            PublicKey = audienceConfig["PublicKey"];
            PrivateKey = audienceConfig["PrivateKey"];
            KeySize = Convert.ToInt16(audienceConfig["KeySize"]);
            //RSA = new RSACryptoServiceProvider(KeySize);
            //RSA.FromXmlString(PrivateKey);

        }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int KeySize { get; set; }
        public RSACryptoServiceProvider RSA { get; set; }

        public string DecryptAscii(string cryptString)
        {
            var cryptParam = Encoding.ASCII.GetBytes(cryptString);
            var decryptResult = RSA.Decrypt(cryptParam, false);
            var stringResult = Encoding.ASCII.GetString(decryptResult);

            return stringResult;
        }
        public string DecryptAscii2(string cryptString)
        {
            var cryptParam = Encoding.ASCII.GetBytes(cryptString);
            var decryptResult = RSA.Decrypt(cryptParam, false);
            var stringResult = Encoding.ASCII.GetString(decryptResult);
            string cryptStringValue = "";
            if (stringResult != null)
            {
                var cryptTimeStr = stringResult.Substring(0, stringResult.IndexOf("."));
                //todo: validate cryptTime nếu cần thiết

                cryptStringValue = stringResult.Substring(stringResult.IndexOf("."));
            }

            return cryptStringValue;
        }
        public string DecryptUtf8(string cryptString)
        {
            var cryptParam = Encoding.UTF8.GetBytes(cryptString);
            var decryptResult = RSA.Decrypt(cryptParam, false);
            var stringResult = Encoding.UTF8.GetString(decryptResult);
            return stringResult;
        }

        public string DecryptUtf82(string cryptString)
        {
            var cryptParam = Encoding.UTF8.GetBytes(cryptString);
            var decryptResult = RSA.Decrypt(cryptParam, false);
            var stringResult = Encoding.UTF8.GetString(decryptResult);
            string cryptStringValue = "";
            if (stringResult!= null)
            {
                var cryptTimeStr = stringResult.Substring(0, stringResult.IndexOf("."));
                //todo: validate cryptTime nếu cần thiết

                cryptStringValue = stringResult.Substring(stringResult.IndexOf("."));
            }
            
            return cryptStringValue;
        }
    }
}