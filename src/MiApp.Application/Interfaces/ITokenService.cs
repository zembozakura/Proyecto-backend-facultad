using System.Threading.Tasks;
using MiApp.Domain.Entities;

namespace MiApp.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
