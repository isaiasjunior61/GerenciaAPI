using System.Security.Cryptography;
using System.Text;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string SenhaHash { get; private set; }
    public string Telefone { get; set; }


    public void DefinirSenha(string senha)
    {
        SenhaHash = HashSenha(senha);
    }

    public bool VerificarSenha(string senha)
    {
        return SenhaHash == HashSenha(senha);
    }

    private string HashSenha(string senha)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}