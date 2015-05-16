using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints by applying the AC1 algorithm.</summary>
    class SudokuConstraintsAC1
    {
        /// <summary>Runs the algorithm on the Sudoku.</summary>
        /// <param name="sudoku">The sudoku to run the algorithm on.</param>
        /// <returns>True if successful, false otherwise (e.g. in case a contradiction is reached).</returns>
        public static bool apply(Sudoku sudoku)
        {
            // We will need an index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // Keep running while something has changed
            bool changed = true;
            while(changed)
            {
                // Initially nothing has changed in this loop
                changed = false;

                // Loop through all squares
                for(int square = 0; square < sudoku.NN * sudoku.NN; ++square)
                {
                    // We will check all the peers of the square
                    int[] peers = sudokuIndexHelper.getPeersFor(square);
                    for(int peer = 0; peer < peers.Length; ++peer)
                    {
                        // Only squares that have one possibility can eliminate values from the domain of our square
                        if(sudoku[peers[peer]].Count == 1 && sudoku[square].Contains(sudoku[peers[peer]][0]))
                        {
                            // Eliminate the possibility
                            sudoku[square].Remove(sudoku[peers[peer]][0]);
                            changed = true;
                            if(sudoku[square].Count == 0)
                                return false;
                        }
                    }
                }
            }

            // If we have come here, everything went successful
            return true;
        }
    }
}
