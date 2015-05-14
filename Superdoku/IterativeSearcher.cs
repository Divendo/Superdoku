using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class IterativeSearcher : LocalSearcher
    {
        public IterativeSearcher()
        { }

        public override Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
            helper = SudokuIndexHelper.get(sudoku.N);
            return toSolve.toSudoku();
            while (toSolve.HeuristicValue > 11000)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();
        }
    }
}
