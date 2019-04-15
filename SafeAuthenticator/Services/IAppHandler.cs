using System.Threading.Tasks;

namespace SafeAuthenticator.Services
{
    public interface IAppHandler
    {
        Task<bool> LaunchApp(string uri);
    }
}
