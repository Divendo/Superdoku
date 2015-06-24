using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the tabu search technique.</summary>
    class TabuSearcher : LocalSearcherSwapCounter
    {
        /// <summary>A list of all possible neighbors.</summary>
        private LocalSearcherNeighborList allNeighbors;

        /// <summary>The length of the tabu list.</summary>
        private int tabuListSize;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="tabuListSize">The length of the tabu list.</param>
        public TabuSearcher(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int tabuListSize = 5)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.tabuListSize = tabuListSize;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // Reset the total amount of swaps
            totalSwaps = 0;

            // The amount of iterations since we improved our value
            int iterationsWithoutImprovement = 0;

            // Initialise the list of all neighbors
            allNeighbors = new LocalSearcherNeighborList(generateNeighbors(sudoku));

            // The last neighbor that was applied
            SwapNeighbor lastApplied = null;

            // The tabu list
            HashSet<SwapNeighbor> tabuList = new HashSet<SwapNeighbor>();
            Queue<SwapNeighbor> tabuQueue = new Queue<SwapNeighbor>(tabuListSize);

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Update the list of all neighbors
                if(lastApplied != null)
                    allNeighbors.update(sudoku, lastApplied);

                // Find the best neighbor that is not on the tabu list
                SwapNeighbor bestNeighbor = null;
                foreach(SwapNeighbor neighbor in allNeighbors.Neighbors)
                {
                    // If this neighbor is not on the tabu list, it is a candidate for a better neighbor
                    if(!tabuList.Contains(neighbor))
                    {
                        if(bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                        {
                            bestNeighbor = neighbor;

                            // We will never be able to improve a score delta of -4
                            if(bestNeighbor.ScoreDelta == -4)
                                break;
                        }
                    }
                }

                // If no neighbor can be found, we stop the search process
                if(bestNeighbor == null)
                    return false;

                // Otherwise we apply the neighbor and add it to the tabu list
                sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                lastApplied = bestNeighbor;
                ++totalSwaps;
                if(tabuQueue.Count == tabuListSize)
                    tabuList.Remove(tabuQueue.Dequeue());
                tabuList.Add(bestNeighbor);
                tabuQueue.Enqueue(bestNeighbor);

                // Remember the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                {
                    bestSolution = new LocalSudoku(sudoku);
                    iterationsWithoutImprovement = 0;
                }
            }

            return sudoku.HeuristicValue == 0;
        }
    }
}
