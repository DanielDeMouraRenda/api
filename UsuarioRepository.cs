using Api.Data;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UsuarioDbcontext _context;

        public UsuarioRepository(UsuarioDbcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> BuscaUsuario()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> BuscaUsuario(int id)
        {
            return await _context.Usuarios.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public void AdicionarUsuario(Usuario usuario)
        {
            _context.Add(usuario);
        }

        public void AtualizarUsuario(Usuario usuario)
        {
            _context.Update(usuario);
        }

        public void DeletaUsuario(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}