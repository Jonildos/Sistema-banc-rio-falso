namespace Sistema_banc_rio_falso.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public Guid ContaId { get; set; }

        // Construtor vazio necessário para o EF Core mapear o banco
        public Transacao() { }

        // Construtor principal usado na classe Conta
        public Transacao(string tipo, decimal valor, Guid contaId)
        {
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.Now;
            ContaId = contaId;
        }
    }
}