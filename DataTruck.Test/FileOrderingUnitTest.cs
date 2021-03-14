using DataTruck.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DataTruck.Test
{
    public class FileOrderingUnitTest
    {
        [Fact]
        public void NumberedTest()
        {
            CustomFileNameComparer comparator = new ();

            // Correct order
            List<string> fileNames = new()
            {
                "1_Primero",
                "0001.00001_Segundo",
                "0001.00002_Tercero",
                "0002_Cuarto",
                "0002,00001_Quinto",
                "3_Sexto",
                "3.1.1 Septimo",
                "3.1.02 Octavo",
                "3.1.003 Noveno",
                "3.1.004_Decimo",
                "00003.0001.0005_Undecimo_0001",
                "00003.0001.0005_Doceavo_0002"
            };

            // Suffle all the things!
            Random random = new Random();
            var shuffledfileNames = fileNames.OrderBy(a => random.Next()).ToList();

            var reorder = shuffledfileNames.OrderBy(f => f, comparator).ToList();
            Assert.Equal(fileNames, reorder);
        }

        [Fact]
        public void NumberedTestInverted()
        {
            CustomFileNameComparer comparator = new();

            // Correct order
            List<string> fileNames = new()
            {
                "1_Primero",
                "0001.00001_Segundo",
                "0001.00002_Tercero",
                "0002_Cuarto",
                "0002,00001_Quinto",
                "3_Sexto",
                "3.1.1 Septimo",
                "3.1.02 Octavo",
                "3.1.003 Noveno",
                "3.1.004_Decimo",
                "00003.0001.0005_Undecimo_0002",
                "00003.0001.0005_Doceavo_0001"
            };

            // Suffle all the things!
            Random random = new Random();
            var shuffledfileNames = fileNames.OrderBy(a => random.Next()).ToList();

            var reorder = shuffledfileNames.OrderBy(f => f, comparator).ToList();
            Assert.NotEqual(fileNames, reorder);
        }
    }
}
