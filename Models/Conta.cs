namespace Sistema_banc_rio_falso.Models
{
    //propriedades (O estado da conta)
    public class Conta
    {
        public Guid Id {get; private set; }
        public string Titular {get; private set; }
        public decimal Saldo {get; private set; }

        // Construtor (como a conta nasce)
        public Conta(string titular)
        {
            Id = Guid.NewGuid(); //gera um id único no formato universal
            Titular = titular;
            Saldo = 0; //toda conta nasce zerada
        }

        //comportamentos (regras de negócio isoladas)
        public void Depositar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do depósito deve ser maior que zero.");   
            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para o depósito.");
            Saldo -= valor; // Subtrai o valor do depósito do saldo
        }
    }
}