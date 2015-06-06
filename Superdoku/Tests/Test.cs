using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class Test
    {
        /// <summary>Run the test with the given sudokus.</summary>
        /// <param name="sudokus">The sudokus to run the test with.</param>
        /// <param name="filename">The file to export the results to, or null if the results should not be exported.</param>
        public abstract void runTest(Sudoku[] sudokus, string filename = null);
    }
}
