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
        /// <summary>The starting temperature.</summary>
        private double startTemp = 10.0;

        /// <summary>The cooling scheme.</summary>
        private SimulatedAnnealingCoolingScheme coolingScheme;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="startTemp">The starting temperature.</param>
        public SimulatedAnnealer(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, double startTemp = 10.0)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.startTemp = startTemp;
            coolingScheme = new SimulatedAnnealingCoolingScheme_Exponential();
        }

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="coolingScheme">The cooling scheme.</param>
        public SimulatedAnnealer(int maxIterations, int maxIterationsWithoutImprovement, double startTemp, SimulatedAnnealingCoolingScheme coolingScheme)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.startTemp = startTemp;
            this.coolingScheme = coolingScheme;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Our temperature
            double temperature = startTemp;

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
                    temperature = coolingScheme.cool(temperature);

                // If the sample is better, adopt it
                if(neighbor.ScoreDelta < 0)
                {
                    sudoku.swap(neighbor.Square1, neighbor.Square2);
                    iterationsWithoutImprovement = 0;
                }
                // Else we adopt it with a given chance
                else if(Randomizer.random.NextDouble() < Math.Exp(-neighbor.ScoreDelta / temperature))
                    sudoku.swap(neighbor.Square1, neighbor.Square2);

                // Remember the best solution
                if(sudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(sudoku);
            }

            return sudoku.HeuristicValue == 0;
        }
    }

    /// <summary>A class that defines a cooling scheme for simulated annealing.</summary>
    abstract class SimulatedAnnealingCoolingScheme
    {
        /// <summary>Cools the given temperature.</summary>
        /// <param name="currentTemp">The current temperature.</param>
        /// <returns>The cooled temperature.</returns>
        public abstract double cool(double currentTemp);
    }

    class SimulatedAnnealingCoolingScheme_Exponential : SimulatedAnnealingCoolingScheme
    {
        /// <summary>The factor with which the temperature is multiplied upon cooling.</summary>
        private double alpha;

        /// <summary>Constructor.</summary>
        /// <param name="alpha">The factor with which the temperature is multiplied upon cooling.</param>
        public SimulatedAnnealingCoolingScheme_Exponential(double alpha = 0.8)
        { this.alpha = alpha; }

        public override double cool(double currentTemp)
        { return currentTemp * alpha; }
    }

    class SimulatedAnnealingCoolingScheme_Linear : SimulatedAnnealingCoolingScheme
    {
        /// <summary>The amount with which the temperature is decreased upon cooling.</summary>
        private double delta;

        /// <summary>Constructor.</summary>
        /// <param name="delta">The amount with which the temperature is decreased upon cooling.</param>
        public SimulatedAnnealingCoolingScheme_Linear(double delta = 1.0)
        { this.delta = delta; }

        public override double cool(double currentTemp)
        { return Math.Max(0, currentTemp - delta); }
    }
}
