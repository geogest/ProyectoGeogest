using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TryTestWeb.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Recordarme?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un login \n")]
        [Display(Name = "Login")]
        //[EmailAddress]
        public string Login { get; set; }

        [Required(ErrorMessage = "Debe ingresar su contraseña \n")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Recordar Datos")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un Login.")]
        //[EmailAddress]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Debe ingresar un E-Mail.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña.")]
        [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} carácteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "Las contraseñas no son las mismas.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterViewModelAddUser
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas ingresadas deben ser la misma.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Nombre Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [Display(Name = "RUT Usuario")]
        [StringLength(10)]
        [ValidaRut(ErrorMessage = "El Rut ingresado es Invalido.")]
        public string RutUsuario { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un E-Mail")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña.")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} carácteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no son las mismas.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
