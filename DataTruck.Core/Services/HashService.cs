using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataTruck.Core.Services
{
    public class HashService : IHashService
    {
        public async Task<string> HashBase64(string filePath)
        {
            using (FileStream fop = File.OpenRead(filePath))
            using (var sha = SHA1.Create()) // not necessary to use: SHA256
            {
                var hs = await sha.ComputeHashAsync(fop);
                return Convert.ToBase64String(hs);
            }
        }
    }
}
