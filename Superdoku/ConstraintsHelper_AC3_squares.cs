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

        public override bool assign(int index, ulong value)
        {
            sudoku[index] = value;
            return apply(sudoku, index);
        }

        public override bool clean()
        {
            return apply(sudoku);
        }

        /// <summary>Runs the algorithm on the Sudoku.</summary>
        /// <param name="sudoku">The sudoku to run the algorithm on.</param>
        /// <param name="changedSquare">The square that has been changed, or -1 if all squares should be added to the queue.</param>
        /// <returns>True if successful, false otherwise (e.g. in case a contradiction is reached).</returns>
        public bool apply(Sudoku sudoku, int changedSquare = -1)
        {
            // Nothing to do if changedSquare is set and it has more than 1 possibility left
            if(changedSquare != -1 && sudoku.valueCount(changedSquare) != 1)
                return true;

            // We will need an index helper
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);

            // Keep track of the squares we still need to check
            bool[] toCheck = new bool[sudoku.NN * sudoku.NN];
            Queue<int> toCheckQueue = new Queue<int>(sudoku.NN * sudoku.NN);

            // Add all squares to the queue
            if(changedSquare != -1)
            {
                int[] peers = sudokuIndexHelper.getPeersFor(changedSquare);
                for(int peer = 0; peer < peers.Length; ++peer)
                {
                    toCheck[peers[peer]] = true;
                    toCheckQueue.Enqueue(peers[peer]);
                }
            }
            else
            {
                for(int square = 0; square < sudoku.NN * sudoku.NN; ++square)
                {
                    toCheck[square] = true;
                    toCheckQueue.Enqueue(square);
                }
            }

            // Keep running while there are still squares in the queue
            while(toCheckQueue.Count != 0)
            {
                // Increase the iteration count
                ++iterations;

                // Pop the square we are going to check
                int square = toCheckQueue.Dequeue();
                toCheck[square] = false;

                // Loop through all the peers of our square
                int[] peers = sudokuIndexHelper.getPeersFor(square);
                for(int peer = 0; peer < peers.Length; ++peer)
                {
                    if(sudoku.valueCount(peers[peer]) == 1 && (sudoku[square] & sudoku[peers[peer]]) != 0)
                    {
                        // Remove the value from the domain
                        sudoku[square] ^= sudoku[peers[peer]];

                        // Check for a contradiction
                        if(sudoku[square] == 0)
                            return false;
                        // Only add new constraints to the queue if it is worth checking them
                        else if(sudoku.valueCount(square) == 1)
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
