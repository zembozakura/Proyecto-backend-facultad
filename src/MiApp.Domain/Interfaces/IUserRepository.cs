using System;
using System.Threading.Tasks;
using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
    }
}
