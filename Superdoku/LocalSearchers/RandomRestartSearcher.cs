using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the hillclimbing technique with random restarts</summary>
    class RandomRestartSearcher : LocalSearcher
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public RandomRestartSearcher(int maxIterations = -1)
            : base(maxIterations) { }

        //TESTER
        int lowest = 1000;

        public override bool solve(LocalSudoku sudoku)
        {
            
            // Reset the iterations
            iterations = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                //HERE FOR TESTING PURPOSES BTCH
                if (lowest > sudoku.HeuristicValue)
                    lowest = sudoku.HeuristicValue;

                // Search for the best neighbor
                List<SwapNeighbor> neighbors = generateNeighbors(sudoku);
                SwapNeighbor bestNeighbor = null;
                foreach (SwapNeighbor neighbor in neighbors)
                {
                    // We will only accept improvements
                    if (neighbor.ScoreDelta < 0)
                    {
                        if (bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                            bestNeighbor = neighbor;
                    }
                }

                // If we have found a neighbor, apply it otherwise we return null
                if (bestNeighbor != null)
                    sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                else //If we cannot find any better neighbors, restart
                    return this.solve(new LocalSudoku(primary));
            }
            solution = sudoku;
            return sudoku.HeuristicValue == 0;
        }
    }
}
