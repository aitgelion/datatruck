using DataTruck.Core.Services;
using DataTruck.Data;
using DataTruck.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataTruck.DbManagers
{
    public class OracleManager : BaseEfServerManager, IDbServer
    {
        private readonly ILogger<OracleManager> _logger;

        public OracleManager(ILogger<OracleManager> logger, DataContext dataContext): base (logger, dataContext)
        {
            _logger = logger;
        }

        public override Task ApplyFile(string connectionString, string file, int timeoutSeconds)
        {
            throw new NotImplementedException();
            // TODO: 
        }
    }
}
