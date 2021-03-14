using DataTruck.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Core.Models
{
    public class MigrationError
    {
        public string FileName { get; init; }

        public string Error { get; init; }
    }

    public class MigrationState
    {
        public string Name { get; set; }

        public DataTruck.Models.State State { get; set; }
        public DataTruck.Models.ApplyModes Mode { get; set; }
    }

    public class MigrationResult
    {
        public bool EnableExitCodeError { get; set; } = true;

        public List<MigrationState> Files { get; set; } = new();

        public List<MigrationError> Errors { get; set; } = new();

        public void Clear()
        {
            Files.Clear();
            Errors.Clear();
        }
    }
}
