namespace Sistema_banc_rio_falso.Models
{
    //propriedades (O estado da conta)
    public class Conta
    {
        public Guid Id {get; private set; }
        public string Titular {get; private set; }
        public string Cpf { get; private set; } = string.Empty;
        public string ChavePix { get; private set; } = string.Empty;
        public decimal Saldo {get; private set; }
        public List<Transacao> Transacoes { get; private set; } = new List<Transacao>();
        

        // Construtor (como a conta nasce)
        public Conta(string titular, string cpf)
            {
                Id = Guid.NewGuid(); //gera um id único no formato universal
                Titular = titular;
                Cpf = cpf;
                Saldo = 0; //conta nasce zerada
                ChavePix = "PIX-" + Id.ToString().Substring(0, 8).ToUpper();
            }

        //comportamentos (regras de negócio isoladas)
       public void Depositar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor de depósito deve ser maior que zero.");
            
            Saldo += valor;
            Transacoes.Add(new Transacao(TipoTransacao.Deposito, valor, this.Id));
        }

        public void Sacar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor de saque deve ser maior que zero.");
            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para esta transação.");
            
            Saldo -= valor;
            Transacoes.Add(new Transacao(TipoTransacao.Saque, valor, this.Id));
        }
    }
}