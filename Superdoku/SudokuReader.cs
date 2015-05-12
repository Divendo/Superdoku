using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Superdoku
{
    /* Format of a sudoku grid:
     *  The string is read from left to right. For every character the following is done:
     *      .           Add all possible numbers for this square
     *      digit       Only one possible number for this square, the given digit
     *      alpha       Only one possibly number for this square, namely: 10 + charachter - 'A'
     *      x           No possibilities for this square
     *      otherwise   Any other character is ignored
     */

    /// <summary>Class that reads sudokus from a string or file.</summary>
    class SudokuReader
    {
        /// <summary>Reads a sudoku from a string.</summary>
        /// <param name="grid">The string to read the sudoku from.</param>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The parsed sudoku</returns>
        public static Sudoku readFromString(string grid, int n)
        {
            Sudoku sudoku = new Sudoku(n);
            int index = 0;
            for(int i = 0; i < grid.Length; ++i)
            {
                if(Char.IsDigit(grid[i]))
                    sudoku.setValue(index++, grid[i] - '0');
                else if(Char.IsUpper(grid[i]))
                    sudoku.setValue(index++, 10 + grid[i] - 'A');
                else if(grid[i] == 'x')
                    sudoku[index++].Clear();
                else if(grid[i] == '.')
                    ++index;
            }
            return sudoku;
        }

        /// <summary>Reads a sudoku from a file.</summary>
        /// <param name="filename">The file to read the sudoku from.</param>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The parsed sudoku</returns>
        public static Sudoku readFromFile(string filename, int n)
        {
            return readFromString(File.ReadAllText(filename), n);
        }
    }
}
