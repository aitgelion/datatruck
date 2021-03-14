using DataTruck.Interfaces;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Services
{
    public class PhysicalFileProviderEx : PhysicalFileProvider, IFileProviderEx
    {
        public string BasePath { get; init; }

        public PhysicalFileProviderEx(string root) : base(root)
        {
            BasePath = root;
        }
    }
}
