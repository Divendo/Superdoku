using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class GeneticLocalSearcher : LocalSearcher
    {
        protected int POLULATION_SIZE = 6;

        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        /// <param name="stopAfterIterationsWithoutImprovement">The maximum amount of iterations without improvement (negative value for unlimited).</param>
        public GeneticLocalSearcher(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations)
        { }

        public override bool solve(LocalSudoku sudoku)
        {
            List<LocalSudoku> population = this.initialise(sudoku);
            Random random = new Random();

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (population[0].HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                //Choose two parents for mating
                int mate1 = random.Next(POLULATION_SIZE);
                int mate2 = random.Next(POLULATION_SIZE);
                //Make sure they're different
                while (mate1 == mate2)
                    mate2 = random.Next(POLULATION_SIZE);

                //Create a baby and execute the hillclimb algorithm
                LocalSudoku baby = new LocalSudoku(population[mate1], population[mate2]);
                baby = this.hillClimb(baby);

                //If the baby isn't worthless, drop the weakest from the population and insert the baby
                if (population[POLULATION_SIZE - 1].HeuristicValue > baby.HeuristicValue)
                {
                    population.RemoveAt(POLULATION_SIZE - 1);

                    for (int t = 0; t < POLULATION_SIZE - 1; ++t)
                        if (population[t].HeuristicValue > baby.HeuristicValue)
                        {
                            population.Insert(t, baby);
                            break;
                        }

                    if (population.Count != POLULATION_SIZE)
                        population.Add(baby);
                }


            }
            sudoku = new LocalSudoku(population[0]);
            return sudoku.HeuristicValue == 0;
        }

        protected List<LocalSudoku> initialise(LocalSudoku sudoku)
        {
            List<LocalSudoku> result = new List<LocalSudoku>(POLULATION_SIZE);

            for(int t = 0; t < POLULATION_SIZE; ++t)
            {
                result.Add(LocalSudoku.buildRandomlyFromLocalSudoku(sudoku));
                result[t] = this.hillClimb(result[t]);
            }
            result = result.OrderByDescending(x => x.HeuristicValue).ToList();
            return result;
        }

        protected LocalSudoku hillClimb(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            // Initialise the list of all neighbors
            LocalSearcherNeighborList allNeighbors = new LocalSearcherNeighborList(generateNeighbors(sudoku));

            //Last applied Swap
            SwapNeighbor lastApplied = null;

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (sudoku.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                // Increase the iteration counter
                ++iterations;

                // Update the list of all neighbors
                if (lastApplied != null)
                    allNeighbors.update(sudoku, lastApplied);

                // Search for the best neighbor
                SwapNeighbor bestNeighbor = null;
                foreach (SwapNeighbor neighbor in allNeighbors.Neighbors)
                {
                    // We will only accept improvements and equals
                    if (neighbor.ScoreDelta <= 0)
                    {
                        if (bestNeighbor == null || neighbor.ScoreDelta < bestNeighbor.ScoreDelta)
                        {
                            bestNeighbor = neighbor;

                            // We will never find a better score delta than -4
                            if (bestNeighbor.ScoreDelta == -4)
                                break;
                        }
                    }
                }

                // If we have found a neighbor, apply it otherwise we return null
                if (bestNeighbor != null)
                {
                    sudoku.swap(bestNeighbor.Square1, bestNeighbor.Square2);
                    lastApplied = bestNeighbor;
                    if (sudoku.HeuristicValue < bestSolution.HeuristicValue)
                        bestSolution = new LocalSudoku(sudoku);
                    else
                        return bestSolution;
                }
            }

            return bestSolution;
        }

        
    }
}
