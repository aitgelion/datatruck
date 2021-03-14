using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Interfaces
{
    public interface IFileProviderEx : IFileProvider
    {
        public string BasePath { get; init; }
    }
}
