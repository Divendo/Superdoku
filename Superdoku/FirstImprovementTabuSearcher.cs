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

        private int indexA = -1;
        private int indexB = -1;

        List<SwapNeighbor> neighbors = null;


        /// <summary>Calculates the length of the tabu list for a sudoku of the given size.</summary>
        /// <param name="n">The size of the sudoku (n*n by n*n squares).</param>
        /// <returns>The size of the tabu list for a sudoku of the given size.</returns>
        public int tabuListLength(int n)
        {
            return (int)(n * n);
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // The tabu list
            int tabuListSize = tabuListLength(sudoku.N);
            HashSet<SwapNeighbor> tabuList = new HashSet<SwapNeighbor>();
            Queue<SwapNeighbor> tabuQueue = new Queue<SwapNeighbor>(tabuListSize);
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);
            int[] peersA = { -1 };
            int[] peersB = { -1 };

            // Reset the iterations
            iterations = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;
                
                // Find the best neighbor that is not on the tabu list
                //List<SwapNeighbor> neighbors = generateNeighbors(sudoku);
                if (neighbors == null)
                    neighbors = generateNeighbors(sudoku);
                SwapNeighbor bestNeighbor = null;
                if (indexA != -1)
                {
                    peersA = helper.getPeersFor(indexA);
                    peersB = helper.getPeersFor(indexB);
                }
              
                

                for (int t = 0; t < neighbors.Count; t++ )
                {
                    if (!tabuList.Contains(neighbors[t]))
                    {
                        //check wheter the square is a peer of A or B, if so, update the swapneighbor
                        //WHY YOU GOTTA BE SO RUUDE
                        if (peersA.Contains(neighbors[t].Square1) || peersB.Contains(neighbors[t].Square1) ||
                            peersA.Contains(neighbors[t].Square2) || peersB.Contains(neighbors[t].Square2))
                            neighbors[t] = new SwapNeighbor(neighbors[t].Square1, neighbors[t].Square2, sudoku.heuristicDelta(neighbors[t].Square1, neighbors[t].Square2));
                        //things and stuff
                        if (bestNeighbor == null || neighbors[t].ScoreDelta < bestNeighbor.ScoreDelta)
                            bestNeighbor = neighbors[t];
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
            }
            solution = sudoku;
            return true;
        }
    }
}

