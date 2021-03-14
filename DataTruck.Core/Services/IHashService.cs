using System.Threading.Tasks;

namespace DataTruck.Core.Services
{
    public interface IHashService
    {
        Task<string> HashBase64(string filePath);
    }
}