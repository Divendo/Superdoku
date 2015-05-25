using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Implements a cultural genetic algorithm where tournament is used to selected solutions for mating.</summary>
    class CulturalGeneticAlgorithm_Tournament : CulturalGeneticAlgorithm
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="stopAfterIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public CulturalGeneticAlgorithm_Tournament(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations, maxIterationsWithoutImprovement) { }

        protected override LocalSudoku selectForMating(List<LocalSudoku> population)
        {
            // Search for the best contestant in the tournament
            LocalSudoku bestSoFar = null;
            foreach(LocalSudoku solution in population)
            {
                // About a third will compete in the tournament
                if(Randomizer.random.Next() % 3 == 0 && (bestSoFar == null || solution.HeuristicValue < bestSoFar.HeuristicValue))
                    bestSoFar = solution;
            }

            // Return the result, or in the exceptional case that the tournament was empty we return a random solution
            if(bestSoFar != null)
                return bestSoFar;
            else
                return population[Randomizer.random.Next(population.Count)];
        }
    }
}
