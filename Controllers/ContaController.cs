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
  }
}