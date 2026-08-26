using Microsoft.AspNetCore.Mvc;
using Sistema_banc_rio_falso.Services;

namespace Sistema_banc_rio_falso.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly ContaService _contaService;

        public ContaController(ContaService contaService)
        {
            _contaService = contaService;
        }

        public class CriarContaDto
        {
            public string Titular { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty; // NOVO: Campo de senha do cliente
        }

        public class LoginAdminDto
        {
            public string Email { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        public class TransferenciaDto
        {
            public string ChaveDestino { get; set; } = string.Empty; 
            public decimal Valor { get; set; }
        }
        // DTO para Login do Cliente
        public class LoginClienteDto
        {
            public string CpfOuChave { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        [HttpPost]
        public IActionResult CriarConta([FromBody] CriarContaDto request)
        {
            try
            {
                var novaConta = _contaService.CriarConta(request.Titular, request.Cpf, request.Senha);
                return Ok(novaConta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("admin/login")]
        public IActionResult LoginAdmin([FromBody] LoginAdminDto credenciais)
        {
            if (credenciais.Email == "admin@bancofalso.com" && credenciais.Cpf == "000.000.000-00" && credenciais.Senha == "admin123")
                return Ok(new { mensagem = "Login administrativo autorizado com sucesso!" });

            return Unauthorized("Credenciais de administrador inválidas.");
        }

        [HttpPost("{id}/transferir")]
        public IActionResult Transferir(Guid id, [FromBody] TransferenciaDto request)
        {
            try
            {
                var novoSaldo = _contaService.Transferir(id, request.ChaveDestino, request.Valor);
                return Ok(new { mensagem = "Transferência realizada com sucesso!", novoSaldoOrigem = novoSaldo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult ConsultarSaldo(Guid id)
        {
            try
            {
                var conta = _contaService.ObterPorId(id);
                return Ok(conta);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/extrato")]
        public IActionResult ConsultarExtrato(Guid id)
        {
            try
            {
                var extrato = _contaService.ObterExtrato(id);
                return Ok(extrato);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/depositar")]
        public IActionResult Depositar(Guid id, [FromBody] decimal valor)
        {
            try
            {
                var saldoAtual = _contaService.Depositar(id, valor);
                return Ok(new { mensagem = "Depósito realizado com sucesso!", saldoAtual });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/sacar")]
        public IActionResult Sacar(Guid id, [FromBody] decimal valor)
        {
            try
            {
                var saldoAtual = _contaService.Sacar(id, valor);
                return Ok(new { mensagem = "Saque autorizado!", saldoAtual });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public IActionResult LoginCliente([FromBody] LoginClienteDto request)
        {
            try
            {
                var conta = _contaService.LoginCliente(request.CpfOuChave, request.Senha);
                return Ok(conta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("buscar-por-cpf/{cpf}")]
        public IActionResult BuscarPorCpf(string cpf)
        {
            try
            {
                var conta = _contaService.BuscarPorCpf(cpf);
                return Ok(conta);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("buscar-por-nome/{nome}")]
        public IActionResult BuscarPorNome(string nome)
        {
            try
            {
                var contas = _contaService.BuscarPorNome(nome);
                return Ok(contas);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ListarTodas()
        {
            var contas = _contaService.ListarTodas();
            return Ok(contas);
        }
    }
}