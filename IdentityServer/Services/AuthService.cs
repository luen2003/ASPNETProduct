using IdentityServer.Authentication;
using IdentityServer.Extensions;
using IdentityServer.Communication.Sync;
using IdentityServer.Converters;
using IdentityServer.Enums;
using IdentityServer.Models;
namespace IdentityServer.Services
{
public class AuthService
{
    private readonly Audience _audience;
    private readonly RSACrypt _RSACrypt;
    private readonly IServiceUrlRepository _iServiceUrlRepository;
    private readonly IUserRepository _userRepo;
    private readonly ILogger _logger;
    private readonly TokenExtension _tokenExtension;

    public AuthService(
        ILogger<AuthService> logger,
        IServiceUrlRepository serviceRepo,
        Audience audience,
        RSACrypt rsaCrypt,
        TokenExtension tokenExtension,
        IUserRepository userRepo)
    {
        _audience = audience;
        _RSACrypt = rsaCrypt;
        _iServiceUrlRepository = serviceRepo;
        _userRepo = userRepo;
        _logger = logger;
        _tokenExtension = tokenExtension;
    }

    public async Task<User?> RegisterUserAsync(RegisterRequest request)
{
    var existingUser = await _userRepo.GetUserByUserName(request.Username);
    if (existingUser != null)
        return null; // user đã tồn tại

    var encryptedPassword = Utils.Utils.TBTEncrypt(request.Password);

    var newUser = new User
    {
        UserName = request.Username,          // C trong SQL
        FullName = request.Username,          // N trong SQL
        ShortName = request.Username,         // SHORTN trong SQL
        Password = encryptedPassword,         // PWD trong SQL
        UserType = 1,                         // UTYPE, mặc định user thường
    };

    await _userRepo.CreateUser(newUser);
    return newUser;
}



        //public async Task<AuthServiceResponse> AuthSrv(AuthServiceRequest param)
        //{
        //    var serviceUrl = await _iServiceUrlRepository.GetServiceUrlByUserName(param.Username);
        //    var serviceCheckSum = await _iServiceUrlRepository.GetService("PORTAL2");
        //    var response = new AuthServiceResponse();

        //    if (serviceUrl != null)
        //    {
        //        //check user active?
        //        if (serviceUrl.Status != 1)
        //        {
        //            response.code = "01-001-01";
        //            response.message = "Dịch vụ bị khóa.";
        //        }
        //        //Todo: Md5 password???? --> done by BCrypt                                
        //        else
        //        {
        //            if (serviceUrl.ServiceType != 1)
        //            {
        //                var secretKey = serviceCheckSum.Var1;
        //                var publicKey = serviceCheckSum.Var2;
        //                var privateKey = serviceCheckSum.Var3;
        //                RSA rsaObjDecode = new RSA(privateKey, RSA_PEM.PEM_TYPE.PRIVATE);

        //                var checkSumSHA = rsaObjDecode.DecodeOrNull(param.CheckSum);

        //                JObject jObjSMS = new JObject();
        //                jObjSMS.Add("requestId", param.RequestId);
        //                jObjSMS.Add("requestDate", param.RequestDate);
        //                jObjSMS.Add("username", param.Username);
        //                jObjSMS.Add("password", param.Password);
        //                jObjSMS.Add("checkSum", checkSumSHA);

        //                string propName = "checkSum";
        //                if (checkSumSHA == null || !SecurityUtility.CompareChecksum(ref jObjSMS, ref propName, ref secretKey, "|"))
        //                {
        //                    response.code = "01-001-04";
        //                    response.message = "CheckSum không đúng.";
        //                    return response;
        //                }
        //            }

        //            if (BCryptNet.Verify(param.Password, serviceUrl.Var2))
        //            {
        //                var now = DateTime.UtcNow;

        //                var isContractCustomer = await _posRepository.IsContractCustomer(serviceUrl.PosId);
        //                var plxId = await _posRepository.GetPlxId(serviceUrl.PosId);
        //                var claims = new[]
        //                {
        //                    new Claim(JwtRegisteredClaimNames.Sub, serviceUrl.Var1),
        //                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(),
        //                        ClaimValueTypes.Integer64),
        //                    new Claim(JwtRegisteredClaimNames.NameId, serviceUrl.ServiceCode),
        //                    new Claim("PosId", serviceUrl.PosId.ToString()),
        //                    new Claim("UserId", serviceUrl.PosId.ToString()),
        //                    new Claim("Role", isContractCustomer ? RoleEnum.U0.ToString() : RoleEnum.U1.ToString()),
        //                    new Claim("PlxId", plxId ?? string.Empty)
        //                };

        //                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_audience.Secret));

        //                var jwt = new JwtSecurityToken(
        //                    issuer: _audience.Iss,
        //                    audience: _audience.Aud,
        //                    claims: claims,
        //                    notBefore: now,
        //                    expires: now.Add(TimeSpan.FromDays(_audience.Expires)),
        //                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512) //HmacSha256 HmacSha512
        //                );

        //                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        //                AuthServiceResponseData authServiceResponseData = new AuthServiceResponseData();


        //                authServiceResponseData.requestDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //                authServiceResponseData.serviceName = serviceUrl.ServiceName;
        //                authServiceResponseData.status = serviceUrl.Status;
        //                authServiceResponseData.token = encodedJwt;


        //                response.code = "01-001-00";
        //                response.message = "Thành công.";
        //                response.data = authServiceResponseData;
        //            }
        //            else
        //            {
        //                response.code = "01-001-03";
        //                response.message = "Mật khẩu không đúng.";
        //            }

        //        }
        //    }
        //    else
        //    {
        //        response.code = "01-001-02";
        //        response.message = "Tài khoản không đúng.";
        //    }

        //    return response;
        //}
        

        public async Task<AuthServiceResponse> AuthCreateToken(AuthServiceRequest param)
        {
            var Iuser = await _iServiceUrlRepository.GetUserByUserName(param.Username);
            var response = new AuthServiceResponse();

            if (Iuser != null)
            {
                if (param.Password == Utils.Utils.TBTDecrypt(Iuser.Password))
                {
                    var now = DateTime.UtcNow;

                    var claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, Iuser.CompanyId.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.NameId, Iuser.UserName),
                            new Claim("PosId", Iuser.PosId.ToString()),
                            new Claim("UserId", Iuser.UserName.ToString()),
                            new Claim("Role", RoleEnum.U0.ToString()),
                            new Claim("PlxId", string.Empty)
                        };

                    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_audience.Secret));

                    var jwt = new JwtSecurityToken(
                        issuer: _audience.Iss,
                        audience: _audience.Aud,
                        claims: claims,
                        notBefore: now,
                        expires: now.Add(TimeSpan.FromDays(_audience.Expires)),
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512) //HmacSha256 HmacSha512
                    );

                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                    AuthServiceResponseData authServiceResponseData = new AuthServiceResponseData();


                    authServiceResponseData.requestDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    authServiceResponseData.serviceName = "AuthCreateToken";
                    authServiceResponseData.status = 1;
                    authServiceResponseData.token = encodedJwt;


                    response.code = "00";
                    response.message = "Thành công.";
                    response.data = authServiceResponseData;
                }
                else
                {
                    response.code = "03";
                    response.message = "Mật khẩu không đúng.";
                }
            }
            else
            {
                response.code = "-1";
                response.message = "Tài khoản không Tồn tại.";
            }

            return response;
        }

        internal GetCryptKeyResponse GetCryptKey()
        {
            GetCryptKeyResponse response = new GetCryptKeyResponse(_RSACrypt);

            return response;
        } 
        public bool IsValidBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}