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
        abstract protected LocalSudoku iterate(LocalSudoku sudoku);
        
       

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
        protected List<SwapNeighbor> generateNeighbors(LocalSudoku sudoku)
        {
            List<SwapNeighbor> result = new List<SwapNeighbor>();
            List<int>[] squares = helper.Squares;
            SwapNeighbor sample;

            //Add each possible swap to the list
            foreach (List<int> square in squares)
                for (int a = 0; a < square.Count(); ++a)
                    for (int b = a + 1; b < square.Count(); ++b)
                        if (!sudoku.fixiated[square[a]] && !sudoku.fixiated[square[b]])
                        {
                            sample = new SwapNeighbor(sudoku, a, b);
                            result.Add(sample);
                        }
            return result;
        }
        
    }
}
