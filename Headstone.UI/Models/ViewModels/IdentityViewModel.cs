using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "E-mail alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email alanı doğru formatta girilmelidir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş geçilemez.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : BaseViewModel
    {
        [Display(Name = "İsim")]
        public string Firstname { get; set; }

        [Display(Name = "Soyisim")]
        public string Lastname { get; set; }

        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Telefon")]
        [Phone]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Cep Telefon")]
        [Phone]
        [MaxLength(10)]
        public string MobileNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email alanı doğru formatta girilmelidir.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "En az 8 karakterden oluşan, en az 1 büyük harf, 1 küçük harf ve 1 sayı içeren bir şifre girmelisiniz.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "En az 8 karakterden oluşan, en az 1 büyük harf, 1 küçük harf ve 1 sayı içeren bir şifre girmelisiniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar alanı boş geçilemez.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class ForgotPasswordViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email alanı doğru formatta girilmelidir.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Token { get; set; }
    }

    public class UpdatePasswordViewModel : BaseViewModel
    {
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Eski şifre")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifre tekrar")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmNewPassword { get; set; }

    }
}