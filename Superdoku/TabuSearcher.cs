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
        private const int TABULENGTH = 50;
        private int pointer;
        private SwapNeighbor[] tabuList;

        //Just for now
        private int iterations;
        private int round;

        public TabuSearcher()
        { 
            tabuList = new SwapNeighbor[TABULENGTH];
            pointer = 0;
            round = 0;
            iterations = 0;
        }

        /// <summary>Solves the sudoku using Tabu Search </summary>
        /// <param name="sudoku">The sudoku</param>
        /// <returns>A solved sudoku</returns>
        public override Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);
        
            while (toSolve.HeuristicValue > 0)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();
        }


        protected LocalSudoku iterate(LocalSudoku sudoku)
        {
            int value = sudoku.HeuristicValue;
            LocalSudoku result = sudoku;
            List<SwapNeighbor> neighbors = this.generateNeighbors(sudoku);
            SwapNeighbor last = null;

            round++;
            if(round > 1000)
            {
                round = 0;
                iterations++;
            }

            foreach (SwapNeighbor neighbor in neighbors)
            {
                //Dit kan sneller!
                if (!tabuList.Contains(neighbor))
                   
                {
                    if(neighbor.ScoreDelta < 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.Square1, neighbor.Square2);
                        tabuList[pointer] = neighbor;
                        if (pointer + 1 >= TABULENGTH)
                            pointer = 0;
                        else pointer++;
                        return result;
                    }
                    if (neighbor.ScoreDelta == 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.Square1, neighbor.Square2);
                        last = neighbor;
                    }

                    if (last == null)
                        last = neighbor;
                }
            }

            if (last != null)
            {
                tabuList[pointer] = last;
                if (pointer + 1 >= TABULENGTH)
                    pointer = 0;
                else
                    pointer++;
                result = new LocalSudoku(sudoku);
                result.swap(last.Square1, last.Square2);

            }

            //SANITYCHECK
            if (last == null)
                return null;
            return result;
        }
    }
}
