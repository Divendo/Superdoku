using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the hillclimbing technique.</summary>
    class IterativeSearcher : LocalSearcher
    {
        /// <summary>A list of all possible neighbors.</summary>
        private LocalSearcherNeighborList allNeighbors;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public IterativeSearcher(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations, maxIterationsWithoutImprovement) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // The amount of iterations since we improved our value
            int iterationsWithoutImprovement = 0;

            // Initialise the list of all neighbors
            allNeighbors = new LocalSearcherNeighborList(generateNeighbors(sudoku));

            // The last neighbor that was applied
            SwapNeighbor lastApplied = null;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Update the list of all neighbors
                if(lastApplied != null)
                    allNeighbors.update(sudoku, lastApplied);

                // Search for the best neighbor
                SwapNeighbor bestNeighbor = null;
                foreach(SwapNeighbor neighbor in allNeighbors.Neighbors)
                {
                    // We will only accept improvements and equals
                    if(neighbor.ScoreDelta <= 0)
                    {
                        if(bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                        {
                            bestNeighbor = neighbor;

                            // We will never find a better score delta than -4
                            if(bestNeighbor.ScoreDelta == -4)
                                break;
                        }
                    }
                }

                // If we have found a neighbor, apply it otherwise we return false
                if(bestNeighbor != null)
                {
                    if(bestNeighbor.ScoreDelta < 0)
                        iterationsWithoutImprovement = 0;

                    sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                    lastApplied = bestNeighbor;
                    if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                        bestSolution = new LocalSudoku(sudoku);
                }
                else
                    return false;
            }

            return sudoku.HeuristicValue == 0;
        }
    }
}
