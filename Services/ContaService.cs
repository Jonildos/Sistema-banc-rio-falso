using Microsoft.EntityFrameworkCore;
using Sistema_banc_rio_falso.Data;
using Sistema_banc_rio_falso.Models;

namespace Sistema_banc_rio_falso.Services
{
    public class ContaService
    {
        private readonly BancoDbContext _context;

        public ContaService(BancoDbContext context)
        {
            _context = context;
        }

        public Conta CriarConta(string titular, string cpf, string senha)
        {
            if (string.IsNullOrWhiteSpace(titular) || string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("O Titular, o CPF e a Senha são obrigatórios para a abertura da conta.");
            
            // 👉 NOVO: Validação Matemática Rigorosa de CPF
            if (!Sistema_banc_rio_falso.Utils.ValidadorCpf.Validar(cpf))
                throw new ArgumentException("CPF inválido! Verifique os dígitos informados ou se digitou caracteres inválidos.");

            bool cpfExiste = _context.Contas.Any(c => c.Cpf == cpf);
            if (cpfExiste)
                throw new InvalidOperationException("Já existe uma conta cadastrada com este CPF no sistema.");

            // Gera o hash seguro da senha do cliente utilizando BCrypt
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            var novaConta = new Conta(titular, cpf, senhaHash);
            _context.Contas.Add(novaConta);
            _context.SaveChanges();

            return novaConta;
        }

        // NOVO: Método de Login do Cliente
        public Conta LoginCliente(string cpfOuChave, string senha)
        {
            if (string.IsNullOrWhiteSpace(cpfOuChave) || string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Informe o CPF ou Chave Única e a senha para entrar.");

            var conta = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Cpf == cpfOuChave || c.ChavePix == cpfOuChave);

            if (conta == null)
                throw new KeyNotFoundException("Conta não encontrada com estes dados.");

            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, conta.SenhaHash);
            if (!senhaValida)
                throw new UnauthorizedAccessException("Senha incorreta.");

            return conta;
        }

        public Conta ObterPorId(Guid id)
        {
            return _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException("Conta não encontrada no sistema.");
        }

        public List<Transacao> ObterExtrato(Guid id)
        {
            var conta = ObterPorId(id);
            return conta.Transacoes;
        }

        public decimal Depositar(Guid id, decimal valor)
        {
            var conta = ObterPorId(id);
            conta.Depositar(valor);
            _context.SaveChanges();
            return conta.Saldo;
        }

        public decimal Sacar(Guid id, decimal valor)
        {
            var conta = ObterPorId(id);
            conta.Sacar(valor);
            _context.SaveChanges();
            return conta.Saldo;
        }

        public decimal Transferir(Guid idOrigem, string chaveDestino, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor da transferência deve ser maior que zero.");

            var contaOrigem = ObterPorId(idOrigem);

            var contaDestino = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.ChavePix == chaveDestino || c.Id.ToString() == chaveDestino)
                ?? throw new KeyNotFoundException("Conta de destino não localizada com esta chave única.");

            if (contaOrigem.Id == contaDestino.Id)
                throw new InvalidOperationException("Você não pode transferir dinheiro para a sua própria conta.");

            if (contaOrigem.Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para realizar esta transferência.");

            contaOrigem.Sacar(valor);
            contaDestino.Depositar(valor);
            _context.SaveChanges();

            return contaOrigem.Saldo;
        }

        public Conta BuscarPorCpf(string cpf)
        {
            return _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Cpf == cpf) ?? throw new KeyNotFoundException("Nenhuma conta encontrada com este CPF.");
        }

        public List<Conta> BuscarPorNome(string nome)
        {
            var contas = _context.Contas
                .Include(c => c.Transacoes)
                .Where(c => c.Titular.Contains(nome))
                .ToList();

            if (!contas.Any())
                throw new KeyNotFoundException("Nenhuma conta encontrada com este nome.");

            return contas;
        }

        public List<Conta> ListarTodas()
        {
            return _context.Contas
                .Include(c => c.Transacoes)
                .ToList();
        }
    }
}