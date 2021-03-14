using DataTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Configuration
{
    public class MigrationOptions
    {
        public string SqlFilesPath { get; set; }

        public string DbServer { get; set; } = "SqlServer"; // "Oracle", "InMemory"

        public string ConnectionString { get; set; }

        public string DbContext { get; set; } = "LOCAL";

        public List<SqlDirectoryModel> Directories { get; set; } = new ();

        // Errors behaviour configuration
        public bool ModifiedOneTimeScriptRaiseError { get; set; } = false;
        public bool EnableExitCodeError { get; set; } = true;

        /// <summary>
        /// Timeout for the command in Seconds
        /// </summary>
        public int CommandTimeout { get; set; } = 300;
    }
}
