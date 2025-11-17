using System.ComponentModel.DataAnnotations;

namespace StajPortal.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol seçimi gereklidir")]
        public string Role { get; set; } = "Student"; // Student, Company
    }
}

