using HRManagementSys.Models;
using System.Threading.Tasks;

namespace HRManagementSys.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<List<string>> GetUserPermissionsAsync(int userId);
    }
}
