using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the simulated annealing technique.</summary>
    class SimulatedAnnealer : LocalSearcher
    {
        /// <summary>The factor with which we decrease the parameter c in the search progress.</summary>
        private const double  alpha = 0.97;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public SimulatedAnnealer(int maxIterations = -1)
            : base(maxIterations) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // We will need a random generator
            Random random = new Random();

            // Parameter to decrease the chances of accepting neighbor that does not improve the sudoku
            double c = 2.0;

            // Reset the iterations
            iterations = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // Randomly generate a neighbor
                SwapNeighbor neighbor = generateNeighbor(sudoku);
                if(neighbor == null)
                    return false;

                // Decrease the value of c
                c *= alpha;

                // If the sample is better, adopt it
                if(neighbor.ScoreDelta < 0)
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
                // Else we adopt it with a given chance
                else if(random.NextDouble() < Math.Exp(-neighbor.ScoreDelta / c))
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
            }

            return sudoku.HeuristicValue == 0;
        }

        /// <summary>Randomly generates a neighbor for the given sudoku.</summary>
        /// <param name="sudoku">The sudoku to generate a neighbor for.</param>
        /// <returns>The generated neighbor, or null if no neighbor could be generated.</returns>
        private SwapNeighbor generateNeighbor(LocalSudoku sudoku)
        {
            Random random = new Random();
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Pick a random box 
            int box = random.Next(0, sudoku.NN);
            
            List<int> squares = null;
            int currBox = box;
            do
            {
                // Make a list of all squares that can be swapped within this box
                int[,] units = helper.getUnitsFor(sudoku.N * (box % sudoku.N), sudoku.N * (box / sudoku.N));
                squares = new List<int>(units.GetLength(1));
                for(int square = 0; square < units.GetLength(1); ++square)
                {
                    if(!sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square]))
                        squares.Add(units[SudokuIndexHelper.UNIT_BOX_INDEX, square]);
                }

                // Check if we have enough options
                if(squares.Count >= 2)
                    break;

                // We do not have enough options, so we try the next box
                if(++currBox >= sudoku.NN)
                    currBox -= sudoku.NN;
                squares = null;
            } while(currBox != box);

            // We may not have succeeded in finding two squares to swap
            if(squares == null)
                return null;

            // Randomly pick two squares
            int square1 = random.Next(0, squares.Count);
            int square2 = random.Next(0, squares.Count - 1);
            if(square2 >= square1)
                ++square2;

            // Return the neighbor
            return new SwapNeighbor(squares[square1], squares[square2], sudoku.heuristicDelta(squares[square1], squares[square2]));
        }
    }
}
