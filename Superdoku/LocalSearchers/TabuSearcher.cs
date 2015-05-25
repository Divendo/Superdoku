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
        /// <summary>A list of all possible neighbors.</summary>
        private List<SwapNeighbor> allNeighbors;

        /// <summary>The neighbor we last applied.</summary>
        private SwapNeighbor lastApplied;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public TabuSearcher(int maxIterations = -1)
            : base(maxIterations) { }

        /// <summary>Calculates the length of the tabu list for a sudoku of the given size.</summary>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The size of the tabu list for a sudoku of the given size.</returns>
        public int tabuListLength(int n)
        {
            return n*n;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // Initialise the list of all neighbors
            allNeighbors = generateNeighbors(sudoku);
            lastApplied = null;

            // The tabu list
            int tabuListSize = tabuListLength(sudoku.N);
            HashSet<SwapNeighbor> tabuList = new HashSet<SwapNeighbor>();
            Queue<SwapNeighbor> tabuQueue = new Queue<SwapNeighbor>(tabuListSize);

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // Find the best neighbor that is not on the tabu list
                SwapNeighbor bestNeighbor = generateBestNeighbor(sudoku, tabuList);

                // If no neighbor can be found, we stop the search process
                if(bestNeighbor == null)
                    return false;

                // Otherwise we apply the neighbor and add it to the tabu list
                sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                if(tabuQueue.Count == tabuListSize)
                    tabuList.Remove(tabuQueue.Dequeue());
                tabuList.Add(bestNeighbor);
                tabuQueue.Enqueue(bestNeighbor);
                lastApplied = bestNeighbor;

                // Remember the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return sudoku.HeuristicValue == 0;
        }

        /// <summary>Generates the best neighbor for a given solution, keeping the tabu list in mind.</summary>
        /// <param name="sudoku">The solution to generate a neighbor for.</param>
        /// <returns>The best neighbor that could be found or null if no neighbor could be found.</returns>
        private SwapNeighbor generateBestNeighbor(LocalSudoku sudoku, HashSet<SwapNeighbor> tabulist)
        {
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // The best neighbor we found so far
            SwapNeighbor bestNeighbor = null;

            // Get the peers of the squares that were swapped in the last iteration
            List<int> peers = new List<int>(2 * helper.getPeerCount() + 2);
            if(lastApplied != null)
            {
                peers.AddRange(helper.getPeersFor(lastApplied.Square1));
                peers.Add(lastApplied.Square1);
                peers.AddRange(helper.getPeersFor(lastApplied.Square2));
                peers.Add(lastApplied.Square2);
            }

            // Loop through all neighbors
            for(int neighbor = 0; neighbor < allNeighbors.Count; ++neighbor)
            {
                // Check if this neighbor needs to have its score delta updated
                if(peers.Contains(allNeighbors[neighbor].Square1) || peers.Contains(allNeighbors[neighbor].Square2))
                    allNeighbors[neighbor] = new SwapNeighbor(allNeighbors[neighbor].Square1, allNeighbors[neighbor].Square2, sudoku.heuristicDelta(allNeighbors[neighbor].Square1, allNeighbors[neighbor].Square2));

                // If this neighbor is not on the tabu list, it is a candidate for a better neighbor
                if(!tabulist.Contains(allNeighbors[neighbor]))
                {
                    if(bestNeighbor == null || allNeighbors[neighbor].ScoreDelta < bestNeighbor.ScoreDelta)
                    {
                        bestNeighbor = allNeighbors[neighbor];

                        // We will never be able to improve a score delta of -4
                        if(bestNeighbor.ScoreDelta == -4)
                            return bestNeighbor;
                    }
                }
            }

            // Return the best neighbor
            return bestNeighbor;
        }
    }
}
