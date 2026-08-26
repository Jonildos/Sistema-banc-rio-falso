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

        public Conta CriarConta(string titular, string cpf)
        {
            if (string.IsNullOrWhiteSpace(titular) || string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("O Titular e o CPF são obrigatórios para a abertura da conta.");

            bool cpfExiste = _context.Contas.Any(c => c.Cpf == cpf);
            if (cpfExiste)
                throw new InvalidOperationException("Já existe uma conta cadastrada com este CPF no sistema.");

            var novaConta = new Conta(titular, cpf);
            _context.Contas.Add(novaConta);
            _context.SaveChanges();

            return novaConta;
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