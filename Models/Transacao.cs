namespace Sistema_banc_rio_falso.Models
{
    public class Transacao
    {
        public string Tipo { get; set; } = string.Empty; // "Depósito" ou "Saque"
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }

        public Transacao(string tipo, decimal valor)
        {
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.Now; // Pega a data e hora exata da operação
        }
    }
}