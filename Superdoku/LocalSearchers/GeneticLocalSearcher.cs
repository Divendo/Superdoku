using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class GeneticLocalSearcher : LocalSearcherSwapCounter
    {
        /// <summary>The size of the population.</summary>
        private int populationSize = 20;

        /// <summary>The local searcher that is used to find local optima.</summary>
        private LocalSearcherSwapCounter localSearcher;

        /// <summary>Constructor.</summary>
        /// <param name="localSearcher">The local searcher that is used to find local optima.</param>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="maxIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public GeneticLocalSearcher(LocalSearcherSwapCounter localSearcher, int maxIterations = -1, int maxIterationsWithoutImprovement = -1, int populationSize = 20)
            : base(maxIterations, maxIterationsWithoutImprovement)
        {
            this.localSearcher = localSearcher;
            this.populationSize = populationSize;
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

            // Initialise the population
            List<LocalSudoku> population = initialisePopulation(sudoku);

            // Check if we have found a better solution
            foreach(LocalSudoku localSudoku in population)
            {
                if(localSudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = localSudoku;
            }

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while(bestSolution.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;

                // Choose two parents for mating (and make sure they are different)
                int mate1 = Randomizer.random.Next(populationSize);
                int mate2 = Randomizer.random.Next(populationSize - 1);
                if(mate2 >= mate1)
                    ++mate2;

                // Create a baby and find a local optimum for the baby
                LocalSudoku baby = findLocalOptimum(new LocalSudoku(population[mate1], population[mate2]));

                // Replace the worst value in the population by the baby (if the baby has a better value)
                int worstIndex = 0;
                for(int i = 1; i < population.Count; ++i)
                {
                    if(population[i].HeuristicValue > population[worstIndex].HeuristicValue)
                        worstIndex = i;
                }
                if(baby.HeuristicValue < population[worstIndex].HeuristicValue)
                {
                    iterationsWithoutImprovement = 0;
                    population[worstIndex] = baby;
                    if(baby.HeuristicValue < bestSolution.HeuristicValue)
                        bestSolution = baby;
                }
            }

            return bestSolution.HeuristicValue == 0;
        }

        /// <summary>Initialises a population of size POLULATION_SIZE using the given LocalSudoku.</summary>
        /// <param name="sudoku">The sudoku that should be used to initliase the population.</param>
        /// <returns>The newly created population.</returns>
        protected List<LocalSudoku> initialisePopulation(LocalSudoku sudoku)
        {
            List<LocalSudoku> result = new List<LocalSudoku>(populationSize);
            for(int t = 0; t < populationSize; ++t)
                result.Add(findLocalOptimum(LocalSudoku.buildRandomlyFromLocalSudoku(sudoku)));

            return result;
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
