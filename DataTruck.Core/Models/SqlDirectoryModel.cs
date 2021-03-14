using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Models
{
    public enum ApplyModes { OneTime, Updated, Always }
    public enum State { Skipped, Applied, IgnoredModified, Error }

    public class SqlDirectoryModel
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public ApplyModes Mode { get; set; } = ApplyModes.OneTime;
    }
}
