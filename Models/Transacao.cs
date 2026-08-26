namespace Sistema_banc_rio_falso.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public TipoTransacao Tipo { get; set; } // Utilizando o Enum
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public Guid ContaId { get; set; }

        public Transacao() { }

        public Transacao(TipoTransacao tipo, decimal valor, Guid contaId)
        {
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.Now;
            ContaId = contaId;
        }
    }
}