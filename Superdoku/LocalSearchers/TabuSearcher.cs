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
                List<SwapNeighbor> neighbors = generateNeighbors(sudoku, tabuList);
                SwapNeighbor bestNeighbor = null;
                foreach(SwapNeighbor neighbor in neighbors)
                {
                    if(bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                        bestNeighbor = neighbor;
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

        /// <summary>Generates the Neighbors of a LocalSudoku.</summary>
        /// <param name="sudoku">The sudoku.</param>
        /// <returns>A list containing neighbors.</returns>
        protected List<SwapNeighbor> generateNeighbors(LocalSudoku sudoku, HashSet<SwapNeighbor> tabulist)
        {
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Each box contains N^2 squares, so at most there will be N^2 + (N^2 - 1) + (N^2 - 2) + ... + 1 = N^2 (N^2 + 1) / 2 combinations possible.
            // There are N^2 boxes, so that results in a capacity of N^2 * N^2 (N^2 + 1) / 2
            List<SwapNeighbor> result = new List<SwapNeighbor>(sudoku.NN * sudoku.NN * (sudoku.NN + 1) / 2);
            SwapNeighbor temp;

            // Loop through all boxes
            for (int box = 0; box < sudoku.NN; ++box)
            {
                // Within a box we loop through all squares and add all possibilities to the list
                int[,] units = helper.getUnitsFor(sudoku.N * (box % sudoku.N), sudoku.N * (box / sudoku.N));
                for (int square1 = 0; square1 < units.GetLength(1); ++square1)
                {
                    if (sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1]))
                        continue;


                    for (int square2 = square1 + 1; square2 < units.GetLength(1); ++square2)
                    {
                        if (!sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square2]))
                        {
                          temp = new SwapNeighbor(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1],
                                                  units[SudokuIndexHelper.UNIT_BOX_INDEX, square2],
                                                  0);
                           
                            //Checks wheter the tabulist contains the coordinates
                            //This ensures that we call calculateHeuristicDelta less
                            if(!tabulist.Contains(temp))
                            {
                                temp = new SwapNeighbor(
                                    units[SudokuIndexHelper.UNIT_BOX_INDEX, square1],
                                    units[SudokuIndexHelper.UNIT_BOX_INDEX, square2],
                                    sudoku.heuristicDelta(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1], units[SudokuIndexHelper.UNIT_BOX_INDEX, square2])
                                );
                                result.Add(temp);
                                
                                //We're never going to find a better score than -4, might as well return the score.
                                if (temp.ScoreDelta == -4)
                                    return new List<SwapNeighbor> { temp };
                            }
                        }
                    }
                }
            }

            // Return the list
            return result;
        }
    }
}
