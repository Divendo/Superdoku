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


        /// <summary>Solves the sudoku using hillclimbing </summary>
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
            int value = sudoku.HeuristicValue;
            LocalSudoku result = sudoku;
            List<SwapNeighbor> neighbors = this.generateNeighbors(sudoku);

            foreach (SwapNeighbor neighbor in neighbors)
            {
                if (neighbor.Delta < 0)
                {
                    result = new LocalSudoku(sudoku);
                    result.swap(neighbor.First, neighbor.Second);
                    return result;
                }
                if (neighbor.Delta == 0)
                {
                    result = new LocalSudoku(sudoku);
                    result.swap(neighbor.First, neighbor.Second);
                }
            }

            return result;
        }
    }
}
