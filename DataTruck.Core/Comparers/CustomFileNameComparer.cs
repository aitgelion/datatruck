using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataTruck.Comparers
{
    public interface IFileNameComparer : IComparer<string> {}

    public class CustomFileNameComparer : IFileNameComparer
    {
        IComparer<string> baseComparer = StringComparer.OrdinalIgnoreCase;

        Regex number = new Regex("(\\d+)");

        public int Compare(string x, string y)
        {
            var ordinalsX = number.Matches(x).Select(m => int.Parse(m.Value)).ToArray();
            var ordinalsY = number.Matches(y).Select(m => int.Parse(m.Value)).ToArray();

            for (int i=0; i<Math.Min(ordinalsX.Length, ordinalsY.Length); i++)
            {
                var order = ordinalsX[i] - ordinalsY[i];
                if (order != 0) return order;
            }

            return ordinalsX.Length - ordinalsY.Length;
        }
    }

}
