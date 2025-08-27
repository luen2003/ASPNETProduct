namespace ServiceAPI.Models
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Mã kết quả:
        /// 00 = Thành công,
        /// 01 = Không tìm thấy,
        /// 02 = Lỗi dữ liệu/validate,
        /// 03 = Chưa đăng nhập/không có quyền,
        /// 99 = Lỗi hệ thống.
        /// </summary>
        public string Code { get; set; } = "00";

        public string Message { get; set; } = "Thành công.";

        public T? Data { get; set; }

        /// <summary>
        /// Chi tiết lỗi (stack/info) – chỉ nên bật khi debug.
        /// </summary>
        public object? Error { get; set; }

        // ---- Static helper methods ----
        public static ApiResponse<T> Ok(T data, string? message = null)
            => new ApiResponse<T>
            {
                Code = "00",
                Message = message ?? "Thành công.",
                Data = data
            };

        public static ApiResponse<T> NotFound(string? message = null, object? error = null)
            => new ApiResponse<T>
            {
                Code = "01",
                Message = message ?? "Không tìm thấy.",
                Error = error
            };

        public static ApiResponse<T> Invalid(string? message = null, object? error = null)
            => new ApiResponse<T>
            {
                Code = "02",
                Message = message ?? "Dữ liệu không hợp lệ.",
                Error = error
            };

        public static ApiResponse<T> Unauthorized(string? message = null, object? error = null)
            => new ApiResponse<T>
            {
                Code = "03",
                Message = message ?? "Chưa đăng nhập hoặc không có quyền.",
                Error = error
            };

        public static ApiResponse<T> Fail(string? message = null, object? error = null)
            => new ApiResponse<T>
            {
                Code = "99",
                Message = message ?? "Lỗi hệ thống.",
                Error = error
            };
    }
}
