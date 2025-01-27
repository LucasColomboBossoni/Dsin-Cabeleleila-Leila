using Microsoft.AspNetCore.Mvc;
using Dsin_CabeleleiraleiraAPI.Data;
using Dsin_CabeleleiraleiraAPI.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dsin_CabeleleiraleiraAPI.Controllers
{
    [Route("api/[controller]")] 
    public class HistoricoController : Controller 
    {   
        private readonly AppDbContext _context; 
        // LIGACAO COM BANCO
        public HistoricoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Historico()
        {
            return View(); // RETORNA UMA VIEW DO HISTORICO
        }

        // MÉTODO GET PARA BUSCAR O HISTÓRICO DE AGENDAMENTOS DO USUÁRIO.
        [HttpGet("historico")]
        public IActionResult GetHistorico()
        {   // OBTEM O TOKEN DO CABEÇALHO DA REQUISIÇAO HTTP
            var authHeader = Request.Headers["Authorization"].ToString();

            // VERIFICA SE O CABEÇALHO ESTÁ PRESENTE E SE O FORMATO É VÁLIDO.
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            var token = authHeader.Replace("Bearer ", "");
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token); // BUSCA O USUÁRIO NO BANCO PELO TOKEN.

            // VERIFICA SE O USUÁRIO FOI ENCONTRADO.
            if (usuario == null)
            {
                return Unauthorized(new { message = "Token inválido ou usuário não encontrado." });
            }

            // BUSCA TODOS OS AGENDAMENTOS ASSOCIADOS AO USUÁRIO.
            var agendamentos = _context.Agendamentos
                .Where(a => a.UsuarioId == usuario.Id)
                .Select(a => new
                {
                    a.Id, 
                    a.Servico,
                    a.DataHora
                })
                .ToList(); // CONVERTE O RESULTADO PARA UMA LISTA.

            return Ok(agendamentos); // RETORNA A LISTA DE AGENDAMENTOS EM FORMATO JSON.
        }

        // MÉTODO PUT PARA EDITAR UM AGENDAMENTO EXISTENTE.
        [HttpPut("editar/{id}")]
        public IActionResult EditarAgendamento(int id, [FromBody] Agendamento agendamentoAtualizado)
        {
            var authHeader = Request.Headers["Authorization"].ToString(); // OBTÉM O CABEÇALHO DE AUTORIZAÇÃO DA REQUISIÇÃO.

            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            var token = authHeader.Replace("Bearer ", "");
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token); // BUSCA O USUÁRIO NO BANCO PELO TOKEN.

            // VERIFICA SE O USUÁRIO FOI ENCONTRADO.
            if (usuario == null)
            {
                return Unauthorized(new { message = "Token inválido ou usuário não encontrado." }); 
            }

            // BUSCA O AGENDAMENTO PELO ID E PELO USUÁRIO.
            var agendamento = _context.Agendamentos.FirstOrDefault(a => a.Id == id && a.UsuarioId == usuario.Id);

            // VERIFICA SE O AGENDAMENTO EXISTE.
            if (agendamento == null)
            {
                return NotFound(new { message = "Agendamento não encontrado." }); // RETORNA ERRO 404 SE O AGENDAMENTO NÃO FOR ENCONTRADO.
            }

            // ATUALIZA OS DADOS DO AGENDAMENTO COM AS NOVAS INFORMAÇÕES.
            agendamento.Servico = agendamentoAtualizado.Servico;
            agendamento.DataHora = agendamentoAtualizado.DataHora;

            _context.Agendamentos.Update(agendamento); // PREPARA A ATUALIZAÇÃO NO BANCO DE DADOS.
            _context.SaveChanges(); // SALVA AS ALTERAÇÕES NO BANCO.

            return Ok(new { message = "Agendamento atualizado com sucesso." }); // RETORNA MENSAGEM DE SUCESSO.
        }
    }
}
