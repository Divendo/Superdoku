using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Implements a cultural genetic algorithm where the selection procedure is abstracted.</summary>
    abstract class CulturalGeneticAlgorithm : LocalSearcher
    {
        /// <summary>The size of the population in each iteration.</summary>
        private int populationSize = 32;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        /// <param name="populationSize">The size of the population in each iteration.</param>
        public CulturalGeneticAlgorithm(int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int populationSize = 32)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.populationSize = populationSize;
        }

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // The amount of iterations since we improved our value
            int iterationsWithoutImprovement = 0;

            // Initialise the first population
            List<LocalSudoku> population = new List<LocalSudoku>(populationSize);
            for(int i = 0; i < populationSize; ++i)
            {
                LocalSudoku newLocalSudoku = LocalSudoku.buildRandomlyFromLocalSudoku(sudoku);
                population.Add(newLocalSudoku);

                if(newLocalSudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(newLocalSudoku);
            }

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(bestSolution.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Generate a new population
                List<LocalSudoku> newPopulation = new List<LocalSudoku>(populationSize);
                for(int i = 0; i < populationSize; ++i)
                {
                    // Select two solutions for mating
                    LocalSudoku mate1 = selectForMating(population);
                    LocalSudoku mate2 = selectForMating(population);

                    // Let them create a baby
                    LocalSudoku baby = new LocalSudoku(mate1, mate2);
                    if(baby.HeuristicValue < bestSolution.HeuristicValue)
                    {
                        bestSolution = new LocalSudoku(baby);
                        iterationsWithoutImprovement = 0;
                    }

                    // Perform some mutations
                    int mutationCount = Randomizer.random.Next((baby.HeuristicValue + 1) / 2) + 1;
                    while(--mutationCount >= 0)
                    {
                        SwapNeighbor mutation = generateRandomNeighbor(baby);
                        baby.swap(mutation.Square1, mutation.Square2);
                    }

                    // Add the baby to the new population
                    if(baby.HeuristicValue < bestSolution.HeuristicValue)
                    {
                        bestSolution = new LocalSudoku(baby);
                        iterationsWithoutImprovement = 0;
                    }
                    newPopulation.Add(baby);
                }

                // Remember the new population
                population = newPopulation;
            }

            return bestSolution.HeuristicValue == 0;
        }

        /// <summary>Selects a solution from the population for mating.</summary>
        /// <param name="population">The population to select a solution from.</param>
        /// <returns>The solution that was selected.</returns>
        protected abstract LocalSudoku selectForMating(List<LocalSudoku> population);
    }
}
