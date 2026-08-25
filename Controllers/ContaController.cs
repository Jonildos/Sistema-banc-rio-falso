using Microsoft.AspNetCore.Mvc;
using Sistema_banc_rio_falso.Models;

namespace Sistema_banc_rio_falso.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private static List<Conta> _bancoDeDados = new List<Conta>();

        // Classe molde (DTO) para receber os dados com segurança no corpo da requisição
        public class CriarContaDto
        {
            public string Titular { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
        }

        // Rota POST: Criar conta exigindo Titular e CPF via JSON
        [HttpPost]
        public IActionResult CriarConta([FromBody] CriarContaDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Titular) || string.IsNullOrWhiteSpace(request.Cpf))
                return BadRequest("O Titular e o CPF são obrigatórios para a abertura da conta.");

            var novaConta = new Conta(request.Titular, request.Cpf);
            _bancoDeDados.Add(novaConta);
            
            return Ok(novaConta); // Status 200
        }

        [HttpGet("{id}")]
        public IActionResult ConsultarSaldo(Guid id)
        {
            var conta = _bancoDeDados.FirstOrDefault(c => c.Id == id);
            if (conta == null)
                return NotFound("Conta não encontrada no sistema.");

            return Ok(conta);
        }
        //Rota GET: Consultar extrato de uma conta pelo ID
        [HttpGet("{id}/extrato")]
        public IActionResult ConsultarExtrato(Guid id)
        {
            var conta = _bancoDeDados.FirstOrDefault(c => c.Id == id);
            if (conta == null)
                return NotFound("Conta não encontrada no sistema.");

            return Ok(conta.Transacoes);
        }

        // Rota POST: Depositar dinheiro
        [HttpPost("{id}/depositar")]
        public IActionResult Depositar(Guid id, [FromBody] decimal valor)
        {
            var conta = _bancoDeDados.FirstOrDefault(c => c.Id == id);
            
            if (conta == null)
                return NotFound("Conta não localizada no sistema.");

            try
            {
                conta.Depositar(valor);
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
            var conta = _bancoDeDados.FirstOrDefault(c => c.Id == id);
            
            if (conta == null)
                return NotFound("Conta não localizada no sistema.");

            try
            {
                conta.Sacar(valor);
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
            var conta = _bancoDeDados.FirstOrDefault(c => c.Cpf == cpf);
            if (conta == null)
                return NotFound("Nenhuma conta encontrada com este CPF.");

            return Ok(conta);
        }

        // Rota GET: Buscar conta por Nome (Área Administrativa)
        [HttpGet("buscar-por-nome/{nome}")]
        public IActionResult BuscarPorNome(string nome)
        {
            var contas = _bancoDeDados.Where(c => c.Titular.Contains(nome, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!contas.Any())
                return NotFound("Nenhuma conta encontrada com este nome.");

            return Ok(contas);
        }
        // Rota GET: Listar todas as contas cadastradas (Painel Administrativo)
        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_bancoDeDados);
        }
    }
}