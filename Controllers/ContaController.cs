using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_banc_rio_falso.Data;
using Sistema_banc_rio_falso.Models;

namespace Sistema_banc_rio_falso.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly BancoDbContext _context;

        // Injeção de Dependência do Contexto do Banco de Dados
        public ContaController(BancoDbContext context)
        {
            _context = context;
        }

        // DTO para Criar Conta
        public class CriarContaDto
        {
            public string Titular { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
        }

        // DTO para Login do Administrador
        public class LoginAdminDto
        {
            public string Email { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        // Rota POST: Criar conta (Com validação de CPF duplicado no banco)
        [HttpPost]
        public IActionResult CriarConta([FromBody] CriarContaDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Titular) || string.IsNullOrWhiteSpace(request.Cpf))
                return BadRequest("O Titular e o CPF são obrigatórios para a abertura da conta.");

            // Regra de Negócio: Barrar contas com o mesmo CPF
            bool cpfExiste = _context.Contas.Any(c => c.Cpf == request.Cpf);
            if (cpfExiste)
                return BadRequest("Já existe uma conta cadastrada com este CPF no sistema.");

            var novaConta = new Conta(request.Titular, request.Cpf);
            
            _context.Contas.Add(novaConta);
            _context.SaveChanges(); // Salva permanentemente no arquivo SQLite
            
            return Ok(novaConta);
        }

        // Rota POST: Login do Administrador (Validando E-mail, CPF e Senha)
        [HttpPost("admin/login")]
        public IActionResult LoginAdmin([FromBody] LoginAdminDto credenciais)
        {
            var admin = _context.Administradores.FirstOrDefault(a => 
                a.Email == credenciais.Email && 
                a.Cpf == credenciais.Cpf && 
                a.Senha == credenciais.Senha
            );

            if (admin == null)
                return Unauthorized("Credenciais de administrador inválidas.");

            return Ok(new { mensagem = "Login administrativo autorizado com sucesso!" });
        }

        // Rota GET: Consultar saldo/dados da conta por ID (incluindo transações do banco)
        [HttpGet("{id}")]
        public IActionResult ConsultarSaldo(Guid id)
        {
            var conta = _context.Contas
                .Include(c => c.Transacoes) // Traz as transações relacionadas do banco
                .FirstOrDefault(c => c.Id == id);

            if (conta == null)
                return NotFound("Conta não encontrada no sistema.");

            return Ok(conta);
        }

        // Rota GET: Consultar extrato de uma conta pelo ID
        [HttpGet("{id}/extrato")]
        public IActionResult ConsultarExtrato(Guid id)
        {
            var conta = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Id == id);

            if (conta == null)
                return NotFound("Conta não encontrada no sistema.");

            return Ok(conta.Transacoes);
        }

        // Rota POST: Depositar dinheiro
        [HttpPost("{id}/depositar")]
        public IActionResult Depositar(Guid id, [FromBody] decimal valor)
        {
            var conta = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Id == id);
            
            if (conta == null)
                return NotFound("Conta não localizada no sistema.");

            try
            {
                conta.Depositar(valor);
                _context.SaveChanges(); // Salva a alteração de saldo e a nova transação no banco
                
                return Ok(new { mensagem = "Depósito realizado com sucesso!", saldoAtual = conta.Saldo });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Rota POST: Sacar dinheiro
        [HttpPost("{id}/sacar")]
        public IActionResult Sacar(Guid id, [FromBody] decimal valor)
        {
            var conta = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Id == id);
            
            if (conta == null)
                return NotFound("Conta não localizada no sistema.");

            try
            {
                conta.Sacar(valor);
                _context.SaveChanges(); // Salva o novo saldo e o registro do saque
                
                return Ok(new { mensagem = "Saque autorizado!", saldoAtual = conta.Saldo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Rota GET: Buscar conta por CPF (Área Administrativa)
        [HttpGet("buscar-por-cpf/{cpf}")]
        public IActionResult BuscarPorCpf(string cpf)
        {
            var conta = _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefault(c => c.Cpf == cpf);

            if (conta == null)
                return NotFound("Nenhuma conta encontrada com este CPF.");

            return Ok(conta);
        }

        // Rota GET: Buscar conta por Nome (Área Administrativa)
        [HttpGet("buscar-por-nome/{nome}")]
        public IActionResult BuscarPorNome(string nome)
        {
            var contas = _context.Contas
                .Include(c => c.Transacoes)
                .Where(c => c.Titular.Contains(nome))
                .ToList();
            
            if (!contas.Any())
                return NotFound("Nenhuma conta encontrada com este nome.");

            return Ok(contas);
        }

        // Rota GET: Listar todas as contas cadastradas (Painel Administrativo)
        [HttpGet]
        public IActionResult ListarTodas()
        {
            var contas = _context.Contas
                .Include(c => c.Transacoes)
                .ToList();

            return Ok(contas);
        }
    }
}