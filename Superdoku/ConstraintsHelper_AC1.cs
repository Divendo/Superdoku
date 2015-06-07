using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Class that helps solving a sudoku using constraints by applying the AC1 algorithm.</summary>
    class ConstraintsHelper_AC1 : ConstraintsHelper
    {
        public ConstraintsHelper_AC1(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, ulong value)
        {
            sudoku[index] = value;
            return apply(sudoku);
        }

        public override bool clean()
        {
            return apply(sudoku);
        }

        /// <summary>Runs the algorithm on the Sudoku.</summary>
        /// <param name="sudoku">The sudoku to run the algorithm on.</param>
        /// <returns>True if successful, false otherwise (e.g. in case a contradiction is reached).</returns>
        public bool apply(Sudoku sudoku)
        {
            // We will need an index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // Keep running while something has changed
            bool changed = true;
            while(changed)
            {
                // Increase the iteration count
                ++iterations;

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
                        if(sudoku.valueCount(peers[peer]) == 1 && (sudoku[square] & sudoku[peers[peer]]) != 0)
                        {
                            // Eliminate the possibility
                            sudoku[square] ^= sudoku[peers[peer]];
                            changed = true;
                            if(sudoku[square] == 0)
                                return false;
                        }
                    }
                }
            }

            // If we have come here, everything went successful
            return true;
        }
    }

    /// <summary>A factor for the ConstraintsHelper_AC1 class.</summary>
    class ConstraintsHelperFactory_AC1 : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelper_AC1(sudoku); }
    }
}
