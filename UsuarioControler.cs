using Api.Model;
using Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers

{

    [ApiController]
    [Route("api/[Controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioController(IUsuarioRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _repository.BuscaUsuario();
            return usuarios.Any()
             ? Ok(usuarios)
             : NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _repository.BuscaUsuario(id);
            return usuario != null
             ? Ok(usuario)
             : NotFound("Usuário não encontrado");
        }

        [HttpPost]
        public async Task<IActionResult> Post(Usuario usuario)
        {
            _repository.AdicionarUsuario(usuario);

            return await _repository.SaveChangesAsync()
            ? Ok("Usuário Adicionado com sucesso")
            : BadRequest("Erro ao salvar Usuário");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Usuario usuario)
        {
            var usuarioBanco = await _repository.BuscaUsuario(id);
            if (usuarioBanco == null) return NotFound("Usuário não encontrado");

            usuarioBanco.Nome = usuario.Nome ?? usuarioBanco.Nome;
            usuarioBanco.Saldo = usuario.Saldo == 0
            ? 0
            : usuario.Saldo;

            _repository.AtualizarUsuario(usuarioBanco);

            return await _repository.SaveChangesAsync()

           ? Ok("Usuário atualizado com sucesso")
           : BadRequest("Erro ao atualizar Usuário");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioBanco = await _repository.BuscaUsuario(id);
            if (usuarioBanco == null) return NotFound("Usuário não encontrado");

            _repository.DeletaUsuario(usuarioBanco);
            return await _repository.SaveChangesAsync()

           ? Ok("Usuário deletado com sucesso")
           : BadRequest("Erro ao deletar Usuário");

        }





    }
}