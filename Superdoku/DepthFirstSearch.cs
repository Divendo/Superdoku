using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>This class implements a depth first search to search for a solution of the sudoku.</summary>
    class DepthFirstSearch
    {
        /// <summary>A factory for the constraints helper that is applied to each node.</summary>
        private ConstraintsHelperFactory constraintsHelperFactory;

        /// <summary>Default constructor.</summary>
        public DepthFirstSearch()
        {
            constraintsHelperFactory = new ConstraintsHelperFactory_Trivial();
        }

        /// <summary>Constructor.</summary>
        /// <param name="helper">The factory that is to be used to create ConstraintsHelper instances, which will be used for constraint satisfaction.</param>
        public DepthFirstSearch(ConstraintsHelperFactory helperFactory)
        {
            constraintsHelperFactory = helperFactory;
        }

        /// <summary>Convenience method. Applies clean() from the constraints helper of this instance.</summary>
        /// <param name="sudoku">The sudoku that clean() should be applied to.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public bool clean(Sudoku sudoku)
        {
            ConstraintsHelper helper = constraintsHelperFactory.createConstraintsHelper(sudoku);
            return helper.clean();
        }

        /// <summary>Searches for a solution for the given sudoku using depth-first search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>The solved sudoku, or null if no solution was possible.</returns>
        public Sudoku search(Sudoku sudoku)
        {
            // We can not solve a non-existing soduko
            if(sudoku == null)
                return null;

            // Check if we have already solved the sudoku
            if(sudoku.isSolvedSimple())
                return sudoku;

            // Pick the square with the fewest possibilities (ignoring the squares with one possibility)
            int index = -1;
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku[i].Count == 1)
                    continue;

                if(index == -1 || sudoku[i].Count < sudoku[index].Count)
                    index = i;
            }

            // If there is a square without any possibilites, we can not find a solution
            if(sudoku[index].Count == 0)
                return null;

            // Try all possibilities for the square we found
            for(int i = 0; i < sudoku[index].Count; ++i)
            {
                ConstraintsHelper helper = constraintsHelperFactory.createConstraintsHelper(new Sudoku(sudoku));
                if(helper.assign(index, sudoku[index][i]))
                {
                    Sudoku result = search(helper.Sudoku);
                    if(result != null)
                        return result;
                }
            }

            // If we have come here, we have not found a solution
            return null;
        }
    }
}
