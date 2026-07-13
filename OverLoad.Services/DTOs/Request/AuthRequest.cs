using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLoad.Services.DTOs.Request
{

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        [MaxLength(256, ErrorMessage = "Email không được vượt quá 256 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(200, ErrorMessage = "Họ và tên không được vượt quá 200 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Đường dẫn ảnh đại diện không được vượt quá 500 ký tự.")]
        public string? AvatarUrl { get; set; }

        [MaxLength(1000, ErrorMessage = "Tiểu sử không được vượt quá 1000 ký tự.")]
        public string? Bio { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh Token không được để trống.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
