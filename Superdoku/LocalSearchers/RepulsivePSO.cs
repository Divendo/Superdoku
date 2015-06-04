using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class RepulsivePSO : LocalSearcher
    {
        int[] velocities;
        int POPULATION_SIZE = 32;


        /// <summary>The maximum amount of iterations without improvement.</summary>
        private int maxIterationsWithoutImprovement;

                public RepulsivePSO(int maxIterations = -1, int maxIterationsWithoutImprovement = -1)
            : base(maxIterations)
        {
            this.maxIterationsWithoutImprovement = maxIterationsWithoutImprovement;
        }

   

        public override bool solve(LocalSudoku sudoku)
        {
            // Initialise the best solution
            bestSolution = new LocalSudoku(sudoku);

            // Reset the iterations
            iterations = 0;

            //The random generator
            Random random = new Random();

            // The amount of iterations since we improved our value
            int iterationsWithoutImprovement = 0;

            // Initialise the first population
            List<Particle> population = new List<Particle>(POPULATION_SIZE);
            for (int i = 0; i < POPULATION_SIZE; ++i)
            {
                LocalSudoku newLocalSudoku = LocalSudoku.buildRandomlyFromLocalSudoku(sudoku);
                population.Add(new Particle(newLocalSudoku));

                if (newLocalSudoku.HeuristicValue < bestSolution.HeuristicValue)
                    bestSolution = new LocalSudoku(newLocalSudoku);
            }

              // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (bestSolution.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations) && (maxIterationsWithoutImprovement < 0 || iterationsWithoutImprovement < maxIterationsWithoutImprovement))
            {
                // Increase the iteration counter
                ++iterations;
                ++iterationsWithoutImprovement;


                //Update each particle handing a random best from another particle and a random velocitytable
                foreach (Particle p in population)
                {
                    int a = random.Next(POPULATION_SIZE);
                    int b = random.Next(POPULATION_SIZE);
                    p.Step(population[a].best, population[b].velocities);

                    if (p.bestHeuristic < bestSolution.HeuristicValue)
                        bestSolution = p.sudoku;
                }

            }

            return bestSolution.HeuristicValue == 0;

        }
    }

    class Particle
    {
        public LocalSudoku sudoku;
        public int[] velocities, best;
        private double r1, r2, r3, OMEGA = 0.1;
        public int bestHeuristic;
        Random random;

        public Particle(LocalSudoku origin)
        {
            bestHeuristic = origin.HeuristicValue;
            random = new Random();
            sudoku = origin;
            velocities = new int[origin.NN * origin.NN];
            best = new int[origin.NN * origin.NN];

            for(int t = 0; t < best.Length; ++t)
            {
                velocities[t] = 0;
                best[t] = origin[t];
            }
        }


        //Makes the particle take a step in each dimension
        public void Step(int[] randomBest, int[] randomVelocity)
        {
            for(int t = 0; t < sudoku.NN*sudoku.NN; ++t)
            {
                if(!sudoku.isFixed(t))
                {
                    r1 = random.NextDouble();
                    r2 = random.NextDouble();
                    r2 = random.NextDouble();
                    //Update the velocity according to the paper
                    velocities[t] = (int) Math.Round( OMEGA * velocities[t] + OMEGA * 2 * r1 * (best[t] - sudoku[t]) +
                                    OMEGA * -2 * r2 * (randomBest[t] - sudoku[t]) + OMEGA * 2 * r3 * randomVelocity[t]);
                    //Update the position
                    sudoku[t] = (sudoku[t] + velocities[t]) % 9;
                }
            }
            sudoku.calcHeuristicValue();
            //In case we've improved, alter the best solution.
            if(bestHeuristic > sudoku.HeuristicValue)
            {
                bestHeuristic = sudoku.HeuristicValue;
                for(int t = 0; t < sudoku.NN*sudoku.NN; ++t)
                {
                    best[t] = sudoku[t];
                }
            }

        }
    }
}
