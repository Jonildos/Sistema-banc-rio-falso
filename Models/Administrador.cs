namespace Sistema_banc_rio_falso.Models
{
    public class Administrador
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // Numa aplicação real usariamos Hash, em breve aplicamos!

        public Administrador(string email, string cpf, string senha)
        {
            Email = email;
            Cpf = cpf;
            Senha = senha;
        }
    }
}