using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class LocalSearcher
    {
        public LocalSearcher()
        { }
        public Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku toSolve = new LocalSudoku(sudoku);
 

            while (toSolve.heuristicValue > 100)
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
                if (neighbor.heuristicValue < value)
                    return neighbor;
                if (neighbor.heuristicValue == value)
                    equal = neighbor;
            }
            return equal;
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
