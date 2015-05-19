using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the tabu search technique.</summary>
    class TabuSearcher : LocalSearcher
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public TabuSearcher(int maxIterations = -1)
            : base(maxIterations) { }


        /// <summary>Calculates the length of the tabu list for a sudoku of the given size.</summary>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The size of the tabu list for a sudoku of the given size.</returns>
        public int tabuListLength(int n)
        {
            return (int)(n*n );
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // The tabu list
            int tabuListSize = tabuListLength(sudoku.N);
            HashSet<SwapNeighbor> tabuList = new HashSet<SwapNeighbor>();
            Queue<SwapNeighbor> tabuQueue = new Queue<SwapNeighbor>(tabuListSize);

            // Reset the iterations
            iterations = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // Find the best neighbor that is not on the tabu list
                List<SwapNeighbor> neighbors = generateNeighbors(sudoku);
                SwapNeighbor bestNeighbor = null;
                foreach(SwapNeighbor neighbor in neighbors)
                {
                    if(!tabuList.Contains(neighbor))
                    {
                        if(bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                            bestNeighbor = neighbor;
                    }
                }

                // If no neighbor can be found, we stop the search process
                if(bestNeighbor == null)
                    return false;

                // Otherwise we apply the neighbor and add it to the tabu list
                sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                if(tabuQueue.Count == tabuListSize)
                    tabuList.Remove(tabuQueue.Dequeue());
                tabuList.Add(bestNeighbor);
                tabuQueue.Enqueue(bestNeighbor);
            }
            solution = sudoku;
            return true;
        }
    }
}
