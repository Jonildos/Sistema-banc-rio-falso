namespace Sistema_banc_rio_falso.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public TipoTransacao Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public Guid ContaId { get; set; }
        
        // Propriedades essenciais de rastreamento para o estorno cruzado e em grupo
        public Guid? ContaDestinoId { get; set; }
        public Guid? TransferenciaId { get; set; }

        public Transacao() { }

        public Transacao(TipoTransacao tipo, decimal valor, Guid contaId, Guid? contaDestinoId = null, Guid? transferenciaId = null)
        {
            Tipo = tipo;
            Valor = valor;
            ContaId = contaId;
            ContaDestinoId = contaDestinoId;
            TransferenciaId = transferenciaId;
            DataHora = DateTime.Now;
        }
    }
}