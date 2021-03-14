using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Data.Models
{
    public class SqlFile
    {
        public int Id { get; set; }

        // Metadata
        public string Name { get; set; }
        public string Hash { get; set; }

        // Timestamps
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}

