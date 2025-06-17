using System.ComponentModel.DataAnnotations;

namespace AplicacaoDTI.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
    }

    namespace SeuProjeto.Models
    {
        public class RegisterViewModel
        {
            public string Email { get; set; }
            public string Senha { get; set; }
            public string ConfirmarSenha { get; set; }
            public string Role { get; set; }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; }
    }

}
