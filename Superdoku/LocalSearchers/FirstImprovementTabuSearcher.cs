using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class FirstImprovementTabuSearcher : LocalSearcher
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public FirstImprovementTabuSearcher(int maxIterations = -1)
            : base(maxIterations) { }

        /// <summary>Calculates the length of the tabu list for a sudoku of the given size.</summary>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The size of the tabu list for a sudoku of the given size.</returns>
        public int tabuListLength(int n)
        {
            return n * n;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // The tabu list
            int tabuListSize = tabuListLength(sudoku.N);
            HashSet<SwapNeighbor> tabuList = new HashSet<SwapNeighbor>();
            Queue<SwapNeighbor> tabuQueue = new Queue<SwapNeighbor>(tabuListSize);

            // Keep track of which squares were swapped in the last iteration, to prevent having to generate a list of all neighbors every time
            int indexA = -1;
            int indexB = -1;
            int[] peersA = {};  // The peers of square A that was swapped last iteration
            int[] peersB = {};  // The peers of square B that was swapped last iteration

            // We will need a SudokuIndexHelper
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Generate a list of neigbors once
            List<SwapNeighbor> neighbors = generateNeighbors(sudoku);

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // If we swapped two squares in the last iteration, we want to know their peers
                if (indexA != -1)
                {
                    peersA = helper.getPeersFor(indexA);
                    peersB = helper.getPeersFor(indexB);
                }

                // Find the best neighbor that is not on the tabu list
                SwapNeighbor bestNeighbor = null;

                // Loop through all neighbors
                for(int t = 0; t < neighbors.Count; t++ )
                {
                    // This neighbor may need to be updated if one of its squares is a peer of one of the squares that were swapped in the last iteration
                    if(peersA.Contains(neighbors[t].Square1) || peersB.Contains(neighbors[t].Square1) || peersA.Contains(neighbors[t].Square2) || peersB.Contains(neighbors[t].Square2))
                        neighbors[t] = new SwapNeighbor(neighbors[t].Square1, neighbors[t].Square2, sudoku.heuristicDelta(neighbors[t].Square1, neighbors[t].Square2));

                    // Only check neighbors that are not on the tabu list
                    if (!tabuList.Contains(neighbors[t]))
                    {
                        // We are searching for the best neighbor
                        if (bestNeighbor == null || neighbors[t].ScoreDelta < bestNeighbor.ScoreDelta)
                            bestNeighbor = neighbors[t];

                        // We accept any improvement
                        if (neighbors[t].ScoreDelta < 0)
                            break;
                    }
                }

                // If no neighbor can be found, we stop the search process
                if (bestNeighbor == null)
                    return false;

                // Otherwise we apply the neighbor and add it to the tabu list
                sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                if (tabuQueue.Count == tabuListSize)
                    tabuList.Remove(tabuQueue.Dequeue());
                tabuList.Add(bestNeighbor);
                tabuQueue.Enqueue(bestNeighbor);
                indexA = bestNeighbor.Square1;
                indexB = bestNeighbor.Square2;

                // Remember the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return sudoku.HeuristicValue == 0;
        }
    }
}

