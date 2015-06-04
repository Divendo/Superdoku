using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class RandomWalkSearcher : LocalSearcher
    {
        /// <summary>A list of all possible neighbors.</summary>
        private LocalSearcherNeighborList allNeighbors;

        /// <summary>The amount of steps each random walk should perform.</summary>
        private const int STEPS = 300;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of random walks (negative value for unlimited).</param>
        public RandomWalkSearcher(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // The amount of random walks
            int randomWalks = 0;

            // Initialise the list of all neighbors
            allNeighbors = new LocalSearcherNeighborList(generateNeighbors(sudoku));

            // The last neighbor that was applied
            SwapNeighbor lastApplied = null;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

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

                // If we have found a neighbor, apply it otherwise we start the random walk
                if(bestNeighbor != null)
                {
                    sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                    lastApplied = bestNeighbor;
                }
                else if(randomWalks == maxIterationsWithoutImprovement)
                    break;
                else
                {
                    // Increase the random walks counter
                    ++randomWalks;

                    // We start a random walk starting from the best point we have ever found
                    sudoku = randomWalk(bestSolution, STEPS);

                    // Re-initialise the list of all neighbors
                    allNeighbors = new LocalSearcherNeighborList(generateNeighbors(sudoku));
                    lastApplied = null;
                }

                // Keep track of the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return sudoku.HeuristicValue == 0;
        }

        /// <summary>Performs a random walk of a given amount of steps starting at a given solution.</summary>
        /// <param name="sudoku">The solution to start at (will not be altered).</param>
        /// <param name="n">The amount of steps to take.</param>
        /// <returns>The solution after the random walk/</returns>
        private LocalSudoku randomWalk(LocalSudoku start, int n)
        {
            // Make a copy to work with
            LocalSudoku sudoku = new LocalSudoku(start);

            // Walk N times in a random direction
            for(int t = 0; t < n; ++t)
            {
                SwapNeighbor apply = generateRandomNeighbor(sudoku);
                sudoku.swap(apply.Square1, apply.Square2);
            }

            // Return the result
            return sudoku;
        }
    }
}
