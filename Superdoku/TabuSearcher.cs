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
        private const int TABULENGTH = 500;
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
            helper = SudokuIndexHelper.get(sudoku.N);
        
            while (toSolve.HeuristicValue > 0)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();
        }


        protected override LocalSudoku iterate(LocalSudoku sudoku)
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
                if (!tabuList.Any(x => SwapNeighbor.equal(x, neighbor)))
                {
                    if (neighbor.Delta < 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.First, neighbor.Second);
                        tabuList[pointer] = neighbor;
                        pointer = (pointer + 1) % TABULENGTH;
                        return result;
                    }
                    if (neighbor.Delta == 0)
                    {
                        result = new LocalSudoku(sudoku);
                        result.swap(neighbor.First, neighbor.Second);
                        last = neighbor;
                    }

                    if (last == null)
                        last = neighbor;
                }
            }

            if (last != null)
            {
                tabuList[pointer] = last;
                pointer = (pointer + 1) % TABULENGTH;
                result = new LocalSudoku(sudoku);
                result.swap(last.First, last.Second);

            }

            //HOE KUN JE EEN LEGE ZOEKRUIMTE KRIJGEN
            if (last == null)
                return null;
            return result;
        }
    }
}
