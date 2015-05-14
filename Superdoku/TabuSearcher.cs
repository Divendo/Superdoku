using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class TabuSearcher : LocalSearcher
    {
        //TODO: MAKE TABULIST WAAAY FASTER
        private const int TABULENGTH = 200;
        private int pointer;
        private SwapNeighbor[] tabuList;

        public TabuSearcher()
        { 
            tabuList = new SwapNeighbor[TABULENGTH];
            pointer = 0;
        }

        /// <summary>Solves the sudoku using Tabu Search </summary>
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
            SwapNeighbor last = null; ;

            foreach (SwapNeighbor neighbor in neighbors)
            {
                //Dit kan sneller!
                if (!tabuList.Any(x => SwapNeighbor.equal(x, neighbor)))
                {
                    if (neighbor.Delta < 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.First, neighbor.Second);
                        tabuList[pointer] = neighbor;
                        pointer++;
                        return result;
                    }
                    if (neighbor.Delta == 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.First, neighbor.Second);
                        last = neighbor;
                    }
                }
            }
            if(last != null)
            {
                tabuList[pointer] = last;
                pointer++;
            }
            return result;
        }
    }
}
