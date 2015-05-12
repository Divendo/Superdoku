using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class LocalSearcher
    {
        LocalSudoku[] tabuList;
        int pointer;
        int TABUSIZE = 1000;

        public LocalSearcher()
        { }
        public Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
            tabuList = new LocalSudoku[TABUSIZE];
            pointer = 0;

            while (toSolve.heuristicValue > 85)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();

        }


        //I know it's double, will make a nice family tree soon. SOOOOOON.
        public Sudoku tabuSolve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
            tabuList = new LocalSudoku[TABUSIZE];
            pointer = 0;

            while (toSolve.heuristicValue > 85)
                toSolve = iterate(toSolve);

            return toSolve.toSudoku();
        }

        //First improvement Iteration
        private LocalSudoku iterate(LocalSudoku sudoku)
        {
            int value = sudoku.heuristicValue;
            LocalSudoku equal = sudoku;


            List<LocalSudoku> neighbors = this.generateNeighbors(sudoku);
            foreach (LocalSudoku neighbor in neighbors)
            {
                if (!tabuList.Any(x => this.checkequal(x, neighbor)))
                {
                    if (neighbor.heuristicValue < value)
                    {
                        tabuList[pointer] = neighbor;
                        pointer++;
                        pointer = pointer % TABUSIZE;
                        return neighbor;
                    }
                    if (neighbor.heuristicValue == value)
                        equal = neighbor;
                    if (checkequal(equal, sudoku))
                        equal = neighbor;
                }
            }
            tabuList[pointer] = equal;
            pointer++;
            pointer = pointer % TABUSIZE;
            if (checkequal(equal, sudoku))
                return null;
            return equal;
        }

        private bool checkequal(LocalSudoku a, LocalSudoku b)
        {
            if (a == null)
                return false;
            else return a.values == b.values;
        }

      
        private List<LocalSudoku> generateNeighbors(LocalSudoku sudoku)
        {
            List<LocalSudoku> result = new List<LocalSudoku>();

            //Add each possible swap to the list
            foreach(List<int> square in sudoku.squares)
                for(int a = 0; a < square.Count(); ++a)
                    for(int b = a + 1; b < square.Count(); ++b)
                        if(!sudoku.isFixed[square[a]] && !sudoku.isFixed[square[b]])
                        {
                            LocalSudoku sample = new LocalSudoku(sudoku);
                            sample.swap(square[a], square[b]);
                            result.Add(sample);
                        }
            return result;
        }
    }
}
