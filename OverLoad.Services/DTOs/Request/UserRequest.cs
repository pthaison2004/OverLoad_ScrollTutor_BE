using System.ComponentModel.DataAnnotations;

namespace OverLoad.Services.DTOs.Request;

public class CreateUserRequest
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

    public string Role { get; set; } = "Student";
}

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [MaxLength(200, ErrorMessage = "Họ và tên không được vượt quá 200 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Đường dẫn ảnh đại diện không được vượt quá 500 ký tự.")]
    public string? AvatarUrl { get; set; }

    [MaxLength(1000, ErrorMessage = "Tiểu sử không được vượt quá 1000 ký tự.")]
    public string? Bio { get; set; }

    public bool IsVerified { get; set; }

    public string Role { get; set; } = "Student";
}

public class UserQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public bool SortDesc { get; set; } = false;
}
