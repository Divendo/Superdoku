using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements the simulated annealing technique.</summary>
    class SimulatedAnnealer : LocalSearcher
    {
        /// <summary>The factor with which we decrease the parameter c in the search progress.</summary>
        private const double alpha = 0.8;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public SimulatedAnnealer(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations, maxIterationsWithoutImprovement) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // The size of the Markov chain should represent the size of the search space
            //maxIterations = 42700*2209;//(int)Math.Pow(sudoku.NN, 4) * 20;

            // Our temperature
            // We want to accept with an 80% chance, assuming that the average bad delta is 3, we get a c of 10
            double c = 10.0;

            // Determine after how many iterations we lower c
            int iterationsBeforeCoolingDown = sudoku.NN * sudoku.NN * 9 / 16;

            // Reset the iterations
            iterations = 0;

            // The amount of iterations since we improved our value
            int iterationsWithoutImprovement = 0;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Randomly generate a neighbor
                SwapNeighbor neighbor = generateRandomNeighbor(sudoku);
                if(neighbor == null)
                    return false;

                // Lower the temperature
                if(iterations % iterationsBeforeCoolingDown == 0)
                    c *= alpha;

                // If the sample is better, adopt it
                if(neighbor.ScoreDelta < 0)
                {
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
                    iterationsWithoutImprovement = 0;
                }
                // Else we adopt it with a given chance
                else if(Randomizer.random.NextDouble() < Math.Exp(-neighbor.ScoreDelta / c))
                    sudoku.swap(neighbor.Square1, neighbor.Square2);

                // Remember the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return sudoku.HeuristicValue == 0;
        }

    }
}
