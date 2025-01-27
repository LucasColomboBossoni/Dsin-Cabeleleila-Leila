using Microsoft.AspNetCore.Mvc; 
using Dsin_CabeleleiraleiraAPI.Data;
using Dsin_CabeleleiraleiraAPI.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dsin_CabeleleiraleiraAPI.Controllers 
{
    [Route("[controller]")] 
    [ApiController] 
    public class UsuarioController : Controller 
    {
        private readonly AppDbContext _context;

        // LIGACAO COM BANCO
        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Usuario()
        {
            return View(); // MÉTODO QUE RETORNA A VIEW DO USUARIO
        }

        // MÉTODO PARA REGISTRO DE NOVOS USUÁRIOS.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            // VERIFICA SE OS DADOS ENVIADOS PELO CLIENTE SÃO INVÁLIDOS COM BASE NAS ANOTAÇÕES DO MODELO.
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Campos obrigatórios não preenchidos." });
            }

            // VERIFICA SE O E-MAIL INFORMADO JÁ EXISTE NO BANCO DE DADOS.
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                return BadRequest(new { error = "E-mail já cadastrado." });
            }

            usuario.Token = usuario.Token ?? string.Empty; // GARANTE QUE O TOKEN NÃO SEJA NULO

            _context.Usuarios.Add(usuario); // ADICIONA O NOVO USUÁRIO AO BANCO DE DADOS.
            await _context.SaveChangesAsync(); // SALVA AS ALTERAÇÕES NO BANCO DE DADOS.

            return Ok(new { message = "Usuário registrado com sucesso." });
        }

        // MÉTODO PARA LOGIN DE USUÁRIOS.
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login login)
        {
            // BUSCA UM USUÁRIO NO BANCO DE DADOS PELO E-MAIL INFORMADO.
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email);

            // VERIFICA SE O USUÁRIO EXISTE E SE A SENHA INFORMADA É VÁLIDA.
            if (user == null || user.Senha != login.Senha)
                return Unauthorized(new { error = "Credenciais inválidas." });

            var token = Guid.NewGuid().ToString(); // GERA UM TOKEN ÚNICO PARA O USUÁRIO.

            user.Token = token; // ATRIBUI O TOKEN GERADO AO USUÁRIO.
            _context.SaveChanges(); // SALVA A ALTERAÇÃO NO BANCO DE DADOS.

            // RETORNA O TOKEN GERADO PARA O FRONTEND JUNTO COM UMA MENSAGEM DE SUCESSO.
            return Ok(new { message = "Login realizado com sucesso.", token = token });
        }
    }
}

