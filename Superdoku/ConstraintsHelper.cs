using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>A class that can apply certain strategies for satisfying constraints.</summary>
    abstract class ConstraintsHelper
    {
        /// <summary>The sudoku this helper represents</summary>
        protected Sudoku sudoku;

        /// <summary>Constructor.</summary>
        /// <param name="sudoku">The sudoku this helper will represent.</param>
        public ConstraintsHelper(Sudoku sudoku)
        { this.sudoku = sudoku; }

        /// <summary>Assign a value to a square, and then apply the constraint satisfaction algorithm of this class.</summary>
        /// <param name="index">The index of the square we want to change.</param>
        /// <param name="value">The value the square should get.</param>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public abstract bool assign(int index, int value);

        /// <summary>Cleans the sudoku by applying assign() to every square that contains one possibility.</summary>
        /// <returns>True if succesfull, false if a contradiction is reached.</returns>
        public abstract bool clean();

        /// <summary>Property to access the sudoku we are manipulating.</summary>
        public Sudoku Sudoku
        {
            get { return sudoku; }
            set { sudoku = value; }
        }
    }

    /// <summary>A factor for the ConstraintsHelper class.</summary>
    abstract class ConstraintsHelperFactory
    {
        /// <summary>Creates an instance of the ConstraintsHelper class.</summary>
        /// <param name="sudoku">The sudoku that the ConstraintsHelper should represent.</param>
        /// <returns>The instance of the ConstraintsHelper class.</returns>
        public abstract ConstraintsHelper createConstraintsHelper(Sudoku sudoku);
    }

    /// <summary>A trivial implementation of ConstraintsHelper. This class does nothing.</summary>
    class ConstraintsHelper_Trivial : ConstraintsHelper
    {
        public ConstraintsHelper_Trivial(Sudoku sudoku)
            : base(sudoku) { }

        public override bool assign(int index, int value)
        {
            // First check if the value does not conflict with some peer
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);
            int[] peers = sudokuIndexHelper.getPeersFor(index);
            for(int peer = 0; peer < peers.Length; ++peer)
            {
                if(sudoku[peers[peer]].Count == 1 && sudoku[peers[peer]][0] == value)
                    return false;
            }

            // If everything checks out, we can set the value
            sudoku[index] = new List<int>(new int[] { value });
            return true;
        }

        public override bool clean()
        {
            // Simply check if the sudoku is still valid
            SudokuIndexHelper sudokuIndexHelper = SudokuIndexHelper.get(sudoku.N);
            for(int square = 0; square < sudoku.NN; ++square)
            {
                if(sudoku[square].Count != 1)
                    continue;

                int[] peers = sudokuIndexHelper.getPeersFor(square);
                for(int peer = 0; peer < peers.Length; ++peer)
                {
                    if(sudoku[peers[peer]].Count == 1 && sudoku[peers[peer]][0] == sudoku[square][0])
                        return false;
                }
            }
            return true;
        }
    }

    /// <summary>A factor for the ConstraintsHelper_Trivial class.</summary>
    class ConstraintsHelperFactory_Trivial : ConstraintsHelperFactory
    {
        public override ConstraintsHelper createConstraintsHelper(Sudoku sudoku)
        { return new ConstraintsHelper_Trivial(sudoku); }
    }
}
