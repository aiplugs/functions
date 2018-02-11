using System.Threading.Tasks;

namespace Aiplugs.Functions.Core
{
    public interface ILockService
    {
        Task<bool> LockAsync(string name);
        Task UnlockAsync(string name);
    }
}