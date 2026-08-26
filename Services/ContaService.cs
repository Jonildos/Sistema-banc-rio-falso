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
            
            if (!Sistema_banc_rio_falso.Utils.ValidadorCpf.Validar(cpf))
                throw new ArgumentException("CPF inválido! Verifique os dígitos informados ou se digitou caracteres inválidos.");

            bool cpfExiste = _context.Contas.Any(c => c.Cpf == cpf);
            if (cpfExiste)
                throw new InvalidOperationException("Já existe uma conta cadastrada com este CPF no sistema.");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            var novaConta = new Conta(titular, cpf, senhaHash);
            _context.Contas.Add(novaConta);
            _context.SaveChanges();

            return novaConta;
        }

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

            // Identificador único que une o "trem" de ida (origem -> destino)
            Guid transferenciaGrupoId = Guid.NewGuid();

            // 1. O trem sai da origem (debitando o saldo)
            contaOrigem.Sacar(valor);
            
            // 2. O trem chega ao destino (creditando o saldo de forma real)
            contaDestino.Depositar(valor);

            // Captura as transações geradas no final das listas de cada conta
            var transacaoSaida = contaOrigem.Transacoes.LastOrDefault();
            var transacaoEntrada = contaDestino.Transacoes.LastOrDefault();

            if (transacaoSaida != null) 
            {
                transacaoSaida.ContaDestinoId = contaDestino.Id;
                transacaoSaida.TransferenciaId = transferenciaGrupoId;
            }
            if (transacaoEntrada != null) 
            {
                transacaoEntrada.ContaDestinoId = contaOrigem.Id;
                transacaoEntrada.TransferenciaId = transferenciaGrupoId;
            }

            _context.SaveChanges();
            return contaOrigem.Saldo;
        }

        public decimal AjustarSaldoAdmin(Guid id, decimal valor, string motivo)
        {
            var conta = ObterPorId(id);

            if (valor == 0)
                throw new ArgumentException("O valor do ajuste não pode ser zero.");

            if (valor > 0)
            {
                conta.Depositar(valor);
            }
            else
            {
                conta.Sacar(Math.Abs(valor));
            }

            _context.SaveChanges();
            return conta.Saldo;
        }

        // Estorno Automático, Atômico e Bidirecional (O trem faz o caminho de volta)
        public void EstornarTransacao(int transacaoId)
        {
            var transacaoAlvo = _context.Transacoes.FirstOrDefault(t => t.Id == transacaoId)
                ?? throw new KeyNotFoundException("Transação não encontrada.");

            // 1. Estorno de Depósito simples
            if (transacaoAlvo.Tipo == TipoTransacao.Deposito)
            {
                var conta = ObterPorId(transacaoAlvo.ContaId);
                if (conta.Saldo < transacaoAlvo.Valor)
                    throw new InvalidOperationException("Não é possível estornar este depósito, pois o titular já gastou o saldo.");
                
                conta.Sacar(transacaoAlvo.Valor);
                _context.Transacoes.Remove(transacaoAlvo);
            }
            // 2. Estorno de Saque simples
            else if (transacaoAlvo.Tipo == TipoTransacao.Saque)
            {
                var conta = ObterPorId(transacaoAlvo.ContaId);
                conta.Depositar(transacaoAlvo.Valor);
                _context.Transacoes.Remove(transacaoAlvo);
            }
            // 3. Estorno de Transferência (Faz o trem voltar exatamente para a origem)
            else if (transacaoAlvo.Tipo == TipoTransacao.Transferencia)
            {
                if (transacaoAlvo.TransferenciaId.HasValue)
                {
                    // Busca as duas pontas da transferência no banco
                    var transacoesDoGrupo = _context.Transacoes
                        .Where(t => t.TransferenciaId == transacaoAlvo.TransferenciaId.Value)
                        .ToList();

                    if (transacoesDoGrupo.Count == 2)
                    {
                        // Identifica com precisão quem é a conta que enviou e quem é a conta que recebeu
                        // A transação onde a ContaId é a origem do envio vs o destino
                        var tOrigem = transacoesDoGrupo.FirstOrDefault(t => t.Id != transacaoAlvo.Id) ?? transacaoAlvo;
                        
                        // Descobre qual transação representou a saída (quem enviou) e qual representou a entrada (quem recebeu)
                        var contaA = ObterPorId(transacaoAlvo.ContaId);
                        var outraTransacao = transacoesDoGrupo.First(t => t.Id != transacaoAlvo.Id);
                        var contaB = ObterPorId(outraTransacao.ContaId);

                        // Como definimos quem é quem: na transferência, a conta que teve o saldo decrementado é a que enviou.
                        // Para o estorno: quem recebeu o dinheiro perde o valor, e quem enviou recupera o valor.
                        // Vamos determinar o destinatário atual (quem está com o dinheiro da transferência) e o remetente original:
                        
                        Conta contaRemetente;
                        Conta contaDestinatario;

                        // Descobrimos quem é o dono da conta de destino comparando com o ContaDestinoId registrado
                        if (transacaoAlvo.ContaDestinoId.HasValue && transacaoAlvo.ContaId != transacaoAlvo.ContaDestinoId.Value)
                        {
                            // Se a transação alvo tem o destino, ela foi a saída da origem
                            contaRemetente = contaA;
                            contaDestinatario = contaB;
                        }
                        else
                        {
                            // Caso contrário, ela foi a entrada no destino, invertemos os papéis
                            contaRemetente = contaB;
                            contaDestinatario = contaA;
                        }

                        // Validação crucial: o destinatário ainda tem saldo suficiente para o estorno?
                        if (contaDestinatario.Saldo < transacaoAlvo.Valor)
                        {
                            throw new InvalidOperationException("Impossível estornar: o destinatário já utilizou ou gastou o valor recebido.");
                        }

                        // O TREM VOLTA:
                        // Retira o dinheiro da conta de quem recebeu
                        contaDestinatario.Sacar(transacaoAlvo.Valor);
                        
                        // Devolve o dinheiro integralmente para a conta de quem enviou
                        contaRemetente.Depositar(transacaoAlvo.Valor);

                        // Remove ambos os registros de histórico das duas contas instantaneamente
                        _context.Transacoes.RemoveRange(transacoesDoGrupo);
                    }
                    else
                    {
                        // Caso de segurança se houver apenas uma ponta órfã
                        var contaAtual = ObterPorId(transacaoAlvo.ContaId);
                        contaAtual.Depositar(transacaoAlvo.Valor);
                        _context.Transacoes.Remove(transacaoAlvo);
                    }
                }
                else
                {
                    // Fallback para transferências legadas
                    var contaAtual = ObterPorId(transacaoAlvo.ContaId);
                    contaAtual.Depositar(transacaoAlvo.Valor);
                    _context.Transacoes.Remove(transacaoAlvo);
                }
            }

            _context.SaveChanges();
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