using DataTruck.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.Core
{
    public interface IRunner
    {
        Task<MigrationResult> Run(CancellationToken stoppingToken = default);
    }
}