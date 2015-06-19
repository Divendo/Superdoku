using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class IteratedLocalSearch : LocalSearcherSwapCounter
    {
        /// <summary>The amount of random swaps that should be performed for a perturbation.</summary>
        private int perturbationSize;

        /// <summary>The local searcher that is used to find local optima.</summary>
        private LocalSearcherSwapCounter localSearcher;

        /// <summary>Constructor.</summary>
        /// <param name="localSearcher">The local searcher that is used to find local optima.</param>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="perturbationSize">The amount of random swaps that should be performed for a perturbation.</param>
        public IteratedLocalSearch(LocalSearcherSwapCounter localSearcher, int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int perturbationSize = 10)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.localSearcher = localSearcher;
            this.perturbationSize = perturbationSize;
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

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(bestSolution.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Find a local optimum for the current solution
                sudoku = findLocalOptimum(sudoku);

                // Check if we have found a better solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);

                // Perturb the solution
                for(int i = 0; i < perturbationSize; ++i)
                {
                    SwapNeighbor neighbor = generateRandomNeighbor(sudoku);
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
                    ++totalSwaps;
                }

                // Check if we have found a better solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return bestSolution.HeuristicValue == 0;
        }

        /// <summary>Uses the local searcher to find a local optimum for the given sudoku.</summary>
        /// <param name="sudoku">The sudoku to find a local optimum for.</param>
        /// <returns>The local optimum.</returns>
        private LocalSudoku findLocalOptimum(LocalSudoku sudoku)
        {
            localSearcher.solve(sudoku);
            totalSwaps += localSearcher.TotalSwaps;
            return localSearcher.Solution;
        }
    }
}
