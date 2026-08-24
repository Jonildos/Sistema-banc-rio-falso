using Microsoft.AspNetCore.Mvc;
using Sistema_banc_rio_falso.Models;

namespace Sistema_banc_rio_falso.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ContaController : ControllerBase
  {
    private static List<Conta> _bancoDeDados = new List<Conta>();

    [HttpPost("{titular}")]
    public IActionResult CriarConta(string titular)
    {
        var novaConta = new Conta(titular);
        _bancoDeDados.Add(novaConta);
        
        return Ok(novaConta); // status 200
    }

    [HttpGet("{id}")]
    public IActionResult ConsultarSaldo(Guid id)
    {
        var conta = _bancoDeDados.FirstOrDefault(c => c.Id == id);
        if (conta == null)
            return NotFound("Conta não encontrada no sistema."); //erro 404

        return Ok(conta);
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
            // Tenta acionar a regra matemática que fizemos na classe Conta
            conta.Depositar(valor);
            return Ok(new { mensagem = "Depósito realizado com sucesso!", saldoAtual = conta.Saldo });
        }
        catch (ArgumentException ex)
        {
            // Se a classe Conta recusar o valor (ex: depósito negativo), cai aqui
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
            // Captura tanto o erro de valor negativo quanto o de saldo insuficiente
            return BadRequest(ex.Message);
        }
    }
  }
}