using Api.Model;

namespace Api.Repository
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> BuscaUsuario();

        Task<Usuario> BuscaUsuario(int id);

        void AdicionarUsuario(Usuario usuario);
        void AtualizarUsuario(Usuario usuario);
        void DeletaUsuario(Usuario usuario);

        Task<Boolean> SaveChangesAsync();

    }
}
