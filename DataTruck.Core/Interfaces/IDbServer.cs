using DataTruck.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.Interfaces
{
    public interface IDbServer
    {
        Task Initialize(CancellationToken stoppingToken);

        Task<SqlFile> GetFileInfo(string name);
        Task<SqlFile> UpsertFileInfo(string name, string hash);

        Task ApplyFile(string connectionString, string file, int timeoutSeconds);
    }
}
