using System.Threading.Tasks;

namespace ChuckNorrisFacts
{
    public interface ILoginProvider
    {
        Task<AuthInfo> LoginAsync();
    }
}
