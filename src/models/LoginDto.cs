namespace GerenciaAPI.Models
{
    public class LoginDto
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
    public class UserDto
    {
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}