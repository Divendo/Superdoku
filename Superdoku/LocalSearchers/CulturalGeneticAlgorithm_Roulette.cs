using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Implements a cultural genetic algorithm where roulette is used to selected solutions for mating.</summary>
    class CulturalGeneticAlgorithm_Roulette : CulturalGeneticAlgorithm
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="populationSize">The size of the population in each iteration.</param>
        public CulturalGeneticAlgorithm_Roulette(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int populationSize = 42)
            : base(maxIterations, maxIterationsWithoutImprovement, populationSize) { }

        protected override LocalSudoku selectForMating(List<LocalSudoku> population)
        {
            // Determine the highest heuristic value
            int highestValue = -1;
            foreach(LocalSudoku solution in population)
            {
                if(solution.HeuristicValue > highestValue)
                    highestValue = solution.HeuristicValue;
            }

            // First we sum all the inverted heuristic values of the solutions
            int sum = 0;
            foreach(LocalSudoku solution in population)
                sum += highestValue - solution.HeuristicValue;

            // Now we randomly select an integer in the range [0, sum) to determine the solution we want to use
            int roll = Randomizer.random.Next(sum);
            sum = 0;                        // Reuse the sum variable
            foreach(LocalSudoku solution in population)
            {
                sum += highestValue - solution.HeuristicValue;
                if(sum > roll)
                    return solution;
            }

            // We should not come here
            return population[0];
        }
    }
}
