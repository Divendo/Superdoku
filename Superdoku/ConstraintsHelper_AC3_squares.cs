using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>
    /// Class that helps solving a sudoku using constraints by applying the AC3 algorithm.
    /// The algorithm is applied to squares instead of constraints.
    /// </summary>
    class ConstraintsHelper_AC3_squares : ConstraintsHelper
    {
        public ConstraintsHelper_AC3_squares(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, int value)
        {
            sudoku[index] = new List<int>(new int[] { value });
            return apply(sudoku);
        }

        public override bool clean()
        {
            return apply(sudoku);
        }

        /// <summary>Runs the algorithm on the Sudoku.</summary>
        /// <param name="sudoku">The sudoku to run the algorithm on.</param>
        /// <returns>True if successful, false otherwise (e.g. in case a contradiction is reached).</returns>
        public static bool apply(Sudoku sudoku)
        {
            // We will need an index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // Keep track of the squares we still need to check
            bool[] toCheck = new bool[sudoku.NN * sudoku.NN];
            Queue<int> toCheckQueue = new Queue<int>(sudoku.NN * sudoku.NN);

            // Add all squares to the queue
            for(int square = 0; square < sudoku.NN * sudoku.NN; ++square)
            {
                toCheck[square] = true;
                toCheckQueue.Enqueue(square);
            }

            // Keep running while there are still squares in the queue
            while(toCheckQueue.Count != 0)
            {
                // Pop the square we are going to check
                int square = toCheckQueue.Dequeue();
                toCheck[square] = false;

                // Loop through all the peers of our square
                int[] peers = sudokuIndexHelper.getPeersFor(square);
                for(int peer = 0; peer < peers.Length; ++peer)
                {
                    if(sudoku[peers[peer]].Count == 1 && sudoku[square].Contains(sudoku[peers[peer]][0]))
                    {
                        // Remove the value from the domain
                        sudoku[square].Remove(sudoku[peers[peer]][0]);

                        // Check for a contradiction
                        if(sudoku[square].Count == 0)
                            return false;
                        // Only add new constraints to the queue if it is worth checking them
                        else if(sudoku[square].Count == 1)
                        {
                            // Add new constraints to the queue
                            for(int peerToQueue = 0; peerToQueue < peers.Length; ++peerToQueue)
                            {
                                if(!toCheck[peers[peerToQueue]])
                                {
                                    toCheck[peers[peerToQueue]] = true;
                                    toCheckQueue.Enqueue(peers[peerToQueue]);
                                }
                            }
                        }
                    }
                }
            }

            // If we have come here, everything went successful
            return true;
        }
    }

    /// <summary>A factor for the ConstraintsHelper_AC3_squares class.</summary>
    class ConstraintsHelperFactory_AC3_squares : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelper_AC3_squares(sudoku); }
    }
}
