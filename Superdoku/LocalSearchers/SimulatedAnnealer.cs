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

        // Parameter to decrease the chances of accepting neighbor that does not improve the sudoku
        //We want to accept with an 80% chance, assuming that the average bad delta is 3, we get a c of 10
        double c = 10;

        int rounds = 0;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public SimulatedAnnealer(int maxIterations = -1)
            : base(maxIterations) { }

        public override bool solve(LocalSudoku sudoku)
        {
            // We will need a random generator
            Random random = new Random();


            //The size of the Markov chain should represent the size of the search space
            maxIterations = 42700*2209;//(int)Math.Pow(sudoku.NN, 4) * 20;

            int maxRounds = sudoku.NN * sudoku.NN * 9 / 16;

            // Reset the iterations
            iterations = 0;
            rounds = 0;


            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;
                ++rounds;

                // Randomly generate a neighbor
                SwapNeighbor neighbor = generateNeighbor(sudoku);
                if(neighbor == null)
                    return false;


                //Alter the temperature
                if (rounds == maxRounds)
                {
                    c *= alpha;
                    rounds = 0;
                }

                // If the sample is better, adopt it
                if(neighbor.ScoreDelta < 0)
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
                // Else we adopt it with a given chance
                else if(random.NextDouble() < Math.Exp(-neighbor.ScoreDelta / c))
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
            }
            solution = sudoku;
            return sudoku.HeuristicValue == 0;
        }

    }
}
