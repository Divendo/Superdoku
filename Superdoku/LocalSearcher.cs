using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class LocalSearcher
    {
        Tuple<int,int>[] tabuList;
        int pointer;
        int TABUSIZE = 100000;

        public LocalSearcher()
        { }
        public Sudoku solve(Sudoku sudoku)
        {
            LocalSudokuOld toSolve = new LocalSudokuOld(sudoku);
            tabuList = new Tuple<int,int>[TABUSIZE];
            pointer = 0;

            while (toSolve.heuristicValue > 50)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();

        }



        //First improvement Iteration
        private LocalSudokuOld iterate(LocalSudokuOld sudoku)
        {
            int value = sudoku.heuristicValue;
            LocalSudokuOld result;


            List<LocalSudokuOld> neighbors = this.generateNeighbors(sudoku);
            result = neighbors.First();

            foreach (LocalSudokuOld neighbor in neighbors)
            {
                if (!tabuList.Any(x => this.checkequal(x, neighbor.changed)))
                {
                    if (neighbor.heuristicValue < value)
                    {
                        tabuList[pointer] = neighbor.changed;
                        pointer++;
                        pointer = pointer % TABUSIZE;
                        return neighbor;
                    }
                    if (neighbor.heuristicValue == value)
                        result = neighbor;
                }
            }
            tabuList[pointer] = result.changed;
            pointer++;
            pointer = pointer % TABUSIZE;
            if (checkequal(sudoku.changed, result.changed))
                return null;
            return result;
        }

        private bool checkequal(Tuple<int, int> a, Tuple<int, int> b)
        {
            if (a == null)
                return false;
            else return a == b;
        }

      
        private List<LocalSudokuOld> generateNeighbors(LocalSudokuOld sudoku)
        {
            List<LocalSudokuOld> result = new List<LocalSudokuOld>();

            //Add each possible swap to the list
            foreach(List<int> square in sudoku.squares)
                for(int a = 0; a < square.Count(); ++a)
                    for(int b = a + 1; b < square.Count(); ++b)
                        if(!sudoku.isFixed[square[a]] && !sudoku.isFixed[square[b]])
                        {
                            LocalSudokuOld sample = new LocalSudokuOld(sudoku);
                            sample.swap(square[a], square[b]);
                            result.Add(sample);
                        }
            return result;
        }
    }
}
