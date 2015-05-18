using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SteepRandomHillclimbSolver : LocalSearcher
    {
         /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public SteepRandomHillclimbSolver(int maxIterations = -1)
            : base(maxIterations) { }

        private LocalSudoku best;
        //This constant makes it so it takes about 20 seconds
        private const int STEPS = 300;

        public override bool solve(LocalSudoku sudoku)
        {
            // Reset the iterations
            iterations = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // Search for the best neighbor
                List<SwapNeighbor> neighbors = generateNeighbors(sudoku);
                SwapNeighbor bestNeighbor = null;
                foreach(SwapNeighbor neighbor in neighbors)
                {
                    // We will only accept improvements
                    if(neighbor.ScoreDelta < 0)
                    {
                        if(bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                            bestNeighbor = neighbor;
                    }
                }

                // If we have found a neighbor, apply it otherwise we start the random walk
                if (bestNeighbor != null)
                    sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                //We start a random walk starting from the best point we've ever met.
                else
                {
                    if (best != null)
                        if (best.HeuristicValue < sudoku.HeuristicValue)
                            sudoku = best;
                    return solve(randomWalk(sudoku, STEPS));
                }
            }
            solution = sudoku;
            return sudoku.HeuristicValue == 0;
        }

        private LocalSudoku randomWalk(LocalSudoku sudoku, int n)
        {
            //Store the best optimum found.
            if (best == null)
                best = new LocalSudoku(sudoku);
            else if (best.HeuristicValue > sudoku.HeuristicValue)
                best = new LocalSudoku(sudoku);

            //Walk N times in a random direction
            for(int t = 0; t < n; ++t)
            {
                SwapNeighbor apply = this.generateNeighbor(sudoku);
                sudoku.swap(apply.Square1, apply.Square2);
            }

            return sudoku;
        }
    }
}
