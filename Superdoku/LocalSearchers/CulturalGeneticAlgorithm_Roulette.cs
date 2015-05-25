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
        public CulturalGeneticAlgorithm_Roulette(int maxIterations = -1)
            : base(maxIterations) { }

        protected override LocalSudoku selectForMating(List<LocalSudoku> population)
        {
            // First we sum all heuristic values of the solutions
            int sum = 0;
            foreach(LocalSudoku solution in population)
                sum += solution.HeuristicValue;

            // Now we randomly select an integer in the range [0, sum) to determine the solution we want to use
            int roll = Randomizer.random.Next(sum);
            sum = 0;                        // Reuse the sum variable
            foreach(LocalSudoku solution in population)
            {
                sum += solution.HeuristicValue;
                if(sum > roll)
                    return solution;
            }

            // We should not come here
            return population[0];
        }
    }
}
