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

        public override bool solve(LocalSudoku sudoku)
        {
            Random random = new Random();
            List<LocalSudoku> generation = new List<LocalSudoku>(50);
            List<LocalSudoku> generation2 = new List<LocalSudoku>(25);
            LocalSudoku best;
            LocalSudoku bestFirst, bestSecond;

            for (int i = 0; i < 50; ++i)
            {
                LocalSudoku newbie = new LocalSudoku(sudoku);

                for (int j = 0; j < sudoku.NN * sudoku.NN; ++j)
                    if (!newbie.isFixed(j))
                        newbie[j] = random.Next(9);
                newbie.calcHeuristicValue();
                generation.Add(newbie);
            }
            best = generation[0];

            // Keep running while the sudoku has not been solved yet (and we have not reached our iteration limit)
            while (best.HeuristicValue > 0 && (maxIterations < 0 || iterations < maxIterations))
            {
                generation2 = new List<LocalSudoku>(25);

                //randomly select half of the population
                for (int t = 0; t < 25; ++t)
                {
                    int next = random.Next(generation.Count);
                    generation2.Add(generation[next]);
                    generation.RemoveAt(next);
                }


                bestFirst = generation[0];
                bestSecond = generation2[0];

                for (int t = 0; t < 25; ++t)
                {
                    if (bestFirst.HeuristicValue > generation[t].HeuristicValue)
                        bestFirst = generation[t];
                    if (bestSecond.HeuristicValue > generation2[t].HeuristicValue)
                        bestSecond = generation[t];
                }

                if (bestFirst.HeuristicValue < bestSecond.HeuristicValue)
                    best = bestFirst;
                else
                    best = bestSecond;

                generation = this.generateGeneration(bestFirst, bestSecond, sudoku.NN * sudoku.NN);
            }

            solution = best;
            return true;
        }

        private List<LocalSudoku> generateGeneration(LocalSudoku a, LocalSudoku b, int N)
        {
            List<LocalSudoku> result = new List<LocalSudoku>(50);

            for (int i = 0; i < 50; ++i)
            {
                Random random = new Random();
                int n = random.Next(N);

                LocalSudoku sudoku = new LocalSudoku(a);

                //reproduce
                for (int t = 0; t < N; ++t)
                {
                    if (t < N)
                        sudoku[t] = a[t];
                    else
                        sudoku[t] = b[t];
                }

                //mutate
                for (int t = 0; t < 3; ++t)
                {
                    n = random.Next(N);
                    if (!a.isFixed(n))
                        sudoku[n] = random.Next(9);
                    else
                        --t;
                }

                result.Add(sudoku);
            }
            return result;
        }

    }
}
