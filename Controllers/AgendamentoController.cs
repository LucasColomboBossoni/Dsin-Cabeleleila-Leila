using Microsoft.AspNetCore.Mvc;
using Dsin_CabeleleiraleiraAPI.Data;
using Dsin_CabeleleiraleiraAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace Dsin_CabeleleiraleiraAPI.Controllers
{
    [Route("agendamentos")]
    [ApiController]
    public class AgendamentoController : Controller
    {
        private readonly AppDbContext _context;
        // LIGACAO COM BANCO
        public AgendamentoController(AppDbContext context)
        {
            _context = context;
        }
        // CHAMA A VIEW DE AGENDAMENTO
        public IActionResult Agendamento()
        {
            return View();
        }


        //CRIA UM AGENDAMENTO
        [HttpPost]
        public IActionResult Create([FromBody] Agendamento agendamento)
        {
            // VERIFICA SE O AGENDAMENTO NN É NULO
            if (agendamento == null)
            {
                return BadRequest(new { message = "Dados inválidos." });
            }
            //NAO DEIXA CRIAR UM AGENDAMENTO ANTES DA DATA ATUAL
            if (agendamento.DataHora.CompareTo(DateTime.Now) < 0)
            {
                return BadRequest(new { message = "Não é possível criar um agendamento para o passado." });
            }

            // OBTEM O TOKEN DO CABEÇALHO DA REQUISIÇAO HTTP
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // VERIFICA SE O TOKEN EXISTE
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            // BUSCA O USUARIO NO BANCO APARTIR DO TOKEN
            var user = _context.Usuarios.FirstOrDefault(u => u.Token == token);

            if (user == null)
            {
                return Unauthorized(new { message = "Usuário não encontrado ou token inválido." });
            }

            // ASSOCIA O ID DO USUARIO AO AGENDAMENTO
            agendamento.UsuarioId = user.Id;

            // ADICIONA O ID AO BANCO DE DADOS
            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();

            // RETORNA AO FRONT UMA MENSAGEM
            return Ok(new { message = "Agendamento realizado com sucesso." });
        }


        // VERIFICA SE HÁ AGENDAMENTO EXISTENTE NA MESMA SEMANA
        [HttpGet("verificar")]
        public IActionResult VerificarAgendamento([FromQuery] DateTime data)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            
            var user = _context.Usuarios.FirstOrDefault(u => u.Token == token);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuário não encontrado ou token inválido." });
            }

            // CALCULANDO O COMECO DA SEMANA BASEADO NA DATA DO AGENDAMENTO
            var startOfWeek = data.AddDays(-(int)data.DayOfWeek);
            // CACULANDO O FIM DESTA MESMA SEMANA BASEADO NO COMECO DA SEMANA
            var endOfWeek = startOfWeek.AddDays(7);

            // VERIFICA SE JA EXISTE UM AGENDAMENTO ASSOCIADO AO USUARIO NA MESMA SEMANA
            var agendamentoExistente = _context.Agendamentos
            .Where(a => a.UsuarioId == user.Id && a.DataHora >= startOfWeek && a.DataHora <= endOfWeek)
            .FirstOrDefault();

            if (agendamentoExistente != null)
            {
                return Ok(new { existeAgendamento = true, dataExistente = agendamentoExistente.DataHora });
            }

            return Ok(new { existeAgendamento = false });
        }

    }
}
