using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class CGA1solver : LocalSearcher
    {
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public CGA1solver(int maxIterations = -1)
            : base(maxIterations) { }

        int Size;
        bool[] Fixed;

        public override bool solve(LocalSudoku sudoku)
        {
            Random random = new Random();
            List<LocalSudoku> generation = new List<LocalSudoku>(50);
            List<LocalSudoku> generation1 = new List<LocalSudoku>(50);
            List<LocalSudoku> generation2 = new List<LocalSudoku>(25);
         
            LocalSudoku best;
            LocalSudoku bestFirst, bestSecond;

            Size = sudoku.NN * sudoku.NN;
            generation = this.initialise(sudoku);

            best = generation[0];

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (best.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                generation2 = new List<LocalSudoku>(25);
                generation1 = new List<LocalSudoku>(generation);

                //randomly select half of the population
                for (int t = 0; t < 25; ++t)
                {
                    int next = random.Next(generation1.Count);
                    generation2.Add(generation1[next]);
                    generation1.RemoveAt(next);
                }

                bestFirst = generation1[0];
                bestSecond = generation2[0];
                //Select the best of each half (tournament selection)
                for (int t = 0; t < 25; ++t)
                {
                    if (bestFirst.HeuristicValue > generation1[t].HeuristicValue)
                        bestFirst = generation1[t];
                    if (bestSecond.HeuristicValue > generation2[t].HeuristicValue)
                        bestSecond = generation2[t];
                }

                if (bestFirst.HeuristicValue < bestSecond.HeuristicValue)
                    best = bestFirst;
                else
                    best = bestSecond;

                //Delete the weakest 10 of the population and replace them
                generation = generation.OrderByDescending(x => x.HeuristicValue).ToList();
                generation.RemoveRange(39, 10);
                generation.AddRange(this.generateGeneration(bestFirst, bestSecond));
            }

            bestSolution = best;
            return true;
        }

        private List<LocalSudoku> generateGeneration(LocalSudoku a, LocalSudoku b)
        {
            List<LocalSudoku> result = new List<LocalSudoku>(10);

            for (int i = 0; i < 10; ++i)
            {
                Random random = new Random();
                int n = random.Next(Size);

                LocalSudoku sudoku = new LocalSudoku(a.N);

                //reproduce
                for (int t = 0; t < Size; ++t )
                {
                    if (t < n)
                        sudoku[t] = a[t];
                    else
                        sudoku[t] = b[t];
                }

                //mutate
                for (int t = 0; t < 3; ++t)
                {
                    n = random.Next(Size);
                    if (!Fixed[n])
                        sudoku[n] = random.Next(9);
                    else
                        --t;
                }
                sudoku.calcHeuristicValue();
                result.Add(sudoku);
            }
            return result;
        }

        private List<LocalSudoku> initialise(LocalSudoku sudoku)
        {
            List<LocalSudoku> result = new List<LocalSudoku>(50);
            Fixed = sudoku.Fixed;
            Random random = new Random();
            
            //Initiate a random population of 50
            for (int t = 0; t < 50; ++t)
            {
                LocalSudoku newbie = new LocalSudoku(sudoku.N);

                for (int index = 0; index < Size; ++index)
                {
                    if (!Fixed[index])
                        newbie[index] = random.Next(9);
                    else
                        newbie[index] = sudoku[index];
                }
                newbie.calcHeuristicValue();
                result.Add(newbie);
            }

            return result;
        }

    }
}
