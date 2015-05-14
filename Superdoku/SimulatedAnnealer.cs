using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SimulatedAnnealer : LocalSearcher
    {
        private double C = 2;
        private const double  alpha = 0.97;


        public SimulatedAnnealer()
        { }


        /// <summary>Solves the sudoku using simulated annealing</summary>
        /// <param name="sudoku">The sudoku</param>
        /// <returns>A solved sudoku</returns>
        public override Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
            helper = SudokuIndexHelper.get(sudoku.N);

            //return toSolve.toSudoku();
            while (toSolve.HeuristicValue > 0)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();
        }


        protected override LocalSudoku iterate(LocalSudoku sudoku)
        {
            SwapNeighbor sample = this.generateNeighbor(sudoku);
            LocalSudoku result;
            C += alpha;

            if (sample == null)
                return sudoku;

            //If the sample is better, adopt it
            if (sample.Delta < 0)
            {
                result = new LocalSudoku(sudoku);
                sudoku.swap(sample.First, sample.Second);
                return result;
            }


            //Else determine wheter you adopt it.
            double chance = Math.Exp(sample.Delta / C);
            int length = (int) (1f / chance);
            bool[] boolBag = new bool[length];
            boolBag[0] = true;

            Random random = new Random();
            int index = random.Next(0, boolBag.Length);

            if(boolBag[index])
            {
                result = new LocalSudoku(sudoku);
                sudoku.swap(sample.First, sample.Second);
                return result;
            }

            else return sudoku;

            
        }

        private SwapNeighbor generateNeighbor(LocalSudoku sudoku)
        {
            Random random = new Random();
            int square = random.Next(0, sudoku.NN);

            int first = random.Next(0, helper.Squares[square].Count());
            int second = random.Next(0, helper.Squares[square].Count());

            if (first != second && !sudoku.Fixed[first] && !sudoku.Fixed[second])
                return new SwapNeighbor(sudoku, first, second);
            else
                return null;
        }
    }
}
