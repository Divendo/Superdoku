using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class LocalSearcher
    {
        protected SudokuIndexHelper helper;

        public LocalSearcher()
        { }


        abstract public Sudoku solve(Sudoku sudoku);
        



       /// <summary>Implementation of first improvement Iteration. </summary>
       /// <param name="sudoku">The sudoku to iterate over</param>
       /// <returns>An improved version.</returns>
        protected LocalSudoku iterate(LocalSudoku sudoku)
        {
            int value = sudoku.HeuristicValue;
            LocalSudoku result;

            List<LocalSudoku> neighbors = this.generateNeighbors(sudoku);
            result = neighbors.First();

            foreach (LocalSudoku neighbor in neighbors)
            {
                    if (neighbor.HeuristicValue < value)
                        return neighbor;
                    if (neighbor.HeuristicValue == value)
                        result = neighbor;
            }
            
            return result;
        }


        /// <summary>Returns wheter two tuples are equal.</summary>
        /// <param name="a">The First tuple.</param>
        /// <param name="b">The second tuple.</param>
        /// <returns>Returns wheter the two tuples are the same.</returns>
        protected bool checkequal(Tuple<int, int> a, Tuple<int, int> b)
        {
            if (a == null || b == null)
                return false;
            else return a == b;
        }

        /// <summary>Generates the Neighbors of a LocalSudoku</summary>
        /// <param name="sudoku">The sudoku</param>
        /// <returns>A list containing neighbors.</returns>
        protected List<LocalSudoku> generateNeighbors(LocalSudoku sudoku)
        {
            List<LocalSudoku> result = new List<LocalSudoku>();
            List<int>[] squares = helper.Squares;

            //Add each possible swap to the list
            foreach (List<int> square in squares)
                for (int a = 0; a < square.Count(); ++a)
                    for (int b = a + 1; b < square.Count(); ++b)
                        if (!sudoku.fixiated[square[a]] && !sudoku.fixiated[square[b]])
                        {
                            LocalSudoku sample = new LocalSudoku(sudoku);
                            sample.swap(square[a], square[b]);
                            result.Add(sample);
                        }
            return result;
        }
        
    }
}
