namespace Sistema_banc_rio_falso.Models
{
    //propriedades (O estado da conta)
    public class Conta
    {
        public Guid Id {get; private set; }
        public string Titular {get; private set; }
        public string Cpf { get; private set; } = string.Empty;
        public decimal Saldo {get; private set; }

        // Construtor (como a conta nasce)
        public Conta(string titular, string cpf)
            {
                Id = Guid.NewGuid(); //gera um id único no formato universal
                Titular = titular;
                Cpf = cpf;
                Saldo = 0; //toda conta nasce zerada
            }

        //comportamentos (regras de negócio isoladas)
        public void Depositar(decimal valor)
            {
                // A única trava: não pode depositar valor negativo ou R$ 0,00
                if (valor <= 0)
                    throw new ArgumentException("O valor de depósito deve ser maior que zero.");

                // Se a trava não gritar, o dinheiro simplesmente entra
                Saldo += valor;
            }
        public void Sacar(decimal valor)
            {
                if (valor <= 0)
                    throw new ArgumentException("O valor de saque deve ser maior que zero.");

                if (Saldo < valor)
                    throw new InvalidOperationException("Saldo insuficiente para esta transação.");

                Saldo -= valor;
            }
    }
}