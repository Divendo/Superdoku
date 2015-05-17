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
        public override Sudoku solve(Sudoku sudoku)
        {
            // Create a LocalSudoku that can be used to perform local search on
            LocalSudoku toSolve = new LocalSudoku(sudoku);

            // Keep running while the sudoku has not been solved yet
            while(toSolve.HeuristicValue > 0)
            {
                // Search for the best neighbor
                List<SwapNeighbor> neighbors = generateNeighbors(toSolve);
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

                // If we have found a neighbor, apply it otherwise we return null
                if(bestNeighbor != null)
                    toSolve.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                return null;
            }

            // If we have come here, we have solved the sudoku
            return toSolve.toSudoku();
        }
    }
}
