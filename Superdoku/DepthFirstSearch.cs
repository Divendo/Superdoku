using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements a depth first search to search for a solution of the sudoku.</summary>
    class DepthFirstSearch
    {
        /// <summary>Searches for a solution for the given sudoku using depth-first search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>The solved sudoku, or null if no solution was possible.</returns>
        public static Sudoku search(Sudoku sudoku)
        {
            TabuSearcher searcher = new TabuSearcher();
            return searcher.solve(sudoku);

            // We can not solve a non-existing soduko
            if(sudoku == null)
                return null;

            // Check if we have already solved the sudoku
            if(sudoku.isSolvedSimple())
                return sudoku;

            // Pick the square with the fewest possibilities (ignoring the squares with one possibility)
            int index = -1;
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku[i].Count == 1)
                    continue;

                if(index == -1 || sudoku[i].Count < sudoku[index].Count)
                    index = i;
            }

            // If there is a square without any possibilites, we can not find a solution
            if(sudoku[index].Count == 0)
                return null;

            // Try all possibilities for the square we found
            for(int i = 0; i < sudoku[index].Count; ++i)
            {
                SudokuConstraintsHelper helper = new SudokuConstraintsHelper(new Sudoku(sudoku));
                if(helper.assign(index, sudoku[index][i]))
                {
                    Sudoku result = search(helper.Sudoku);
                    if(result != null)
                        return result;
                }
            }

            // If we have come here, we have not found a solution
            return null;
        }
    }
}
