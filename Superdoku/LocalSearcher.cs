using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class LocalSearcher
    {
        /// <summary>Tries to solve the sudoku using local search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>The solved sudoku, or null if no solution could be found.</returns>
        abstract public Sudoku solve(Sudoku sudoku);
        
        /// <summary>Returns wheter two tuples are equal.</summary>
        /// <param name="a">The First tuple.</param>
        /// <param name="b">The second tuple.</param>
        /// <returns>Returns wheter the two tuples are the same.</returns>
        protected bool checkequal(Tuple<int, int> a, Tuple<int, int> b)
        {
            if (a == null || b == null)
                return false;
            else return a == b;
        }

        /// <summary>Generates the Neighbors of a LocalSudoku.</summary>
        /// <param name="sudoku">The sudoku.</param>
        /// <returns>A list containing neighbors.</returns>
        protected List<SwapNeighbor> generateNeighbors(LocalSudoku sudoku)
        {
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Each box contains N^2 squares, so at most there will be N^2 + (N^2 - 1) + (N^2 - 2) + ... + 1 = N^2 (N^2 + 1) / 2 combinations possible.
            // There are N^2 boxes, so that results in a capacity of N^2 * N^2 (N^2 + 1) / 2
            List<SwapNeighbor> result = new List<SwapNeighbor>(sudoku.NN * sudoku.NN * (sudoku.NN + 1) / 2);

            // Loop through all boxes
            for(int box = 0; box < sudoku.NN; ++box)
            {
                // Within a box we loop through all squares and add all possibilities to the list
                int[,] units = helper.getUnitsFor(sudoku.N * (box % sudoku.N), sudoku.N * (box / sudoku.N));
                for(int square1 = 0; square1 < units.GetLength(1); ++square1)
                {
                    if(sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1]))
                        continue;

                    for(int square2 = square1 + 1; square2 < units.GetLength(1); ++square2)
                    {
                        if(!sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square2]))
                        {
                            result.Add(new SwapNeighbor(
                                units[SudokuIndexHelper.UNIT_BOX_INDEX, square1],
                                units[SudokuIndexHelper.UNIT_BOX_INDEX, square2],
                                sudoku.heuristicDelta(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1], units[SudokuIndexHelper.UNIT_BOX_INDEX, square2])
                            ));
                        }
                    }
                }
            }

            // Return the list
            return result;
        }
        
    }
}
