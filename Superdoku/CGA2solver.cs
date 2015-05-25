using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class CGA2solver : LocalSearcher
    {
         /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public CGA2solver(int maxIterations = -1)
            : base(maxIterations) { }

        public override bool solve(LocalSudoku sudoku)
        {
            //reset the iterations
            iterations = 0;

            //The amount of children we generate
            //The roulettenumber
            int neighborhoodSize, rouletteNumber;

            //The roulette needed for the selection algorithm
            Random roulette = new Random();

            //The neighborhood
            List<SwapNeighbor> neighborhood;
            List<SwapNeighbor> focus;
            SwapNeighbor bestNeighbor;


           // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 30 && (maxIterations < 0 || iterations < maxIterations))
            {
                //increase the iteration counter
                ++iterations;
                rouletteNumber = 0;
                
                //Calculate the size of the neighborhood and create a new list
                neighborhoodSize = (int) Math.Ceiling(((float) sudoku.HeuristicValue) / 2.0); 
                neighborhood = new List<SwapNeighbor>(neighborhoodSize);
                focus = new List<SwapNeighbor>(neighborhoodSize / 4);

                //Generate the neighbors
                for (int t = 0; t < neighborhoodSize; ++t)
                    neighborhood.Add(generateNeighbor(sudoku));

                for (int t = 0; t < Math.Max(neighborhoodSize / 4,1); ++t)
                {
                    rouletteNumber = roulette.Next(0, neighborhoodSize);
                    focus.Add(neighborhood[rouletteNumber]);
                }

                bestNeighbor = focus[0];
                foreach (SwapNeighbor neighbor in focus)
                    if (neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                        bestNeighbor = neighbor;
                


                //Apply the swap
                sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);

            }
            bestSolution = sudoku;
            return true;
        }

    }
}
