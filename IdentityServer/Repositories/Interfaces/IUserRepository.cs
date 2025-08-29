using IdentityServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Lấy tất cả user trong hệ thống
        /// </summary>
        Task<List<User>> GetAllUsers();

        /// <summary>
        /// Lấy thông tin user theo Id
        /// </summary>
        Task<User> GetUserById(string id);

        /// <summary>
        /// Lấy thông tin user kèm công ty theo UserId
        /// </summary>
        Task<UserCompanyDto> GetUserByUserId(string userId);

        /// <summary>
        /// Lấy thông tin user theo username (đăng nhập)
        /// </summary>
        Task<User?> GetUserByUserName(string username);

        /// <summary>
        /// Tạo mới 1 user
        /// </summary>
        Task CreateUser(User user);

        /// <summary>
        /// Cập nhật mật khẩu user
        /// </summary>
        Task UpdateUserPassword(string userId, string newPassword);

        /// <summary>
        /// Thêm user mới (sử dụng DTO UserAddNewParam)
        /// </summary>
        Task<long> AddNew(UserAddNewParam param);

        /// <summary>
        /// Đổi mật khẩu user
        /// </summary>
        Task<int> ChangePassword(ChangePasswordRequest param, long userId);

        /// <summary>
        /// Gán user với vehicle
        /// </summary>
        Task<int> MapUserVehicle(MapUserVehicleRequest request);
    }
}
