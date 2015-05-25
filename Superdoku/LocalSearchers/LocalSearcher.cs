using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    abstract class LocalSearcher
    {
        /// <summary>The best solution that was found while solving the sudoku.</summary>
        protected LocalSudoku bestSolution;
        /// <summary>The maximum amount of iterations the searcher should perform (negative value for unlimited).</summary>
        protected int maxIterations;

        /// <summary>The amount of iterations performed in the last run.</summary>
        protected int iterations;
        
        /// <summary>Constructor.</summary>
        /// <param name="maxIterations">The maximum amount of iterations the searcher should perform (negative value for unlimited).</param>
        public LocalSearcher(int maxIterations = -1)
        {
            this.maxIterations = maxIterations;
        }

        /// <summary>The best solution that was found after applying solve().</summary>
        public LocalSudoku Solution
        { get { return bestSolution; } }

        /// <summary>The amount of iterations performed in the last run.</summary>
        public int Iterations
        { get { return iterations; } }

        /// <summary>Convenience function. Tries to solve the sudoku using local search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>The solved sudoku, or null if no solution could be found.</returns>
        public Sudoku solve(Sudoku sudoku)
        {
            LocalSudoku localSudoku = new LocalSudoku(sudoku);
            if(solve(localSudoku))
                return bestSolution.toSudoku();
   
            return null;
        }

        /// <summary>Tries to solve the sudoku using local search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>True if a solution could be found, false otherwise.</returns>
        abstract public bool solve(LocalSudoku sudoku);

        /// <summary>Randomly generates a neighbor for the given sudoku.</summary>
        /// <param name="sudoku">The sudoku to generate a neighbor for.</param>
        /// <returns>The generated neighbor, or null if no neighbor could be generated.</returns>
        protected SwapNeighbor generateNeighbor(LocalSudoku sudoku)
        {
            Random random = new Random();
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Pick a random box 
            int box = random.Next(0, sudoku.NN);

            List<int> squares = null;
            int currBox = box;
            do
            {
                // Make a list of all squares that can be swapped within this box
                int[,] units = helper.getUnitsFor(sudoku.N * (box % sudoku.N), sudoku.N * (box / sudoku.N));
                squares = new List<int>(units.GetLength(1));
                for (int square = 0; square < units.GetLength(1); ++square)
                {
                    if (!sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square]))
                        squares.Add(units[SudokuIndexHelper.UNIT_BOX_INDEX, square]);
                }

                // Check if we have enough options
                if (squares.Count >= 2)
                    break;

                // We do not have enough options, so we try the next box
                if (++currBox >= sudoku.NN)
                    currBox -= sudoku.NN;
                squares = null;
            } while (currBox != box);

            // We may not have succeeded in finding two squares to swap
            if (squares == null)
                return null;

            // Randomly pick two squares
            int square1 = random.Next(0, squares.Count);
            int square2 = random.Next(0, squares.Count - 1);
            if (square2 >= square1)
                ++square2;

            // Return the neighbor
            return new SwapNeighbor(squares[square1], squares[square2], sudoku.heuristicDelta(squares[square1], squares[square2]));
        }

        /// <summary>Generates the Neighbors of a LocalSudoku.</summary>
        /// <param name="sudoku">The sudoku.</param>
        /// <returns>A list containing neighbors.</returns>
        protected List<SwapNeighbor> generateNeighbors(LocalSudoku sudoku)
        {
            SudokuIndexHelper helper = SudokuIndexHelper.get(sudoku.N);

            // Each box contains N^2 squares, so at most there will be N^2 + (N^2 - 1) + (N^2 - 2) + ... + 1 = N^2 (N^2 + 1) / 2 combinations possible.
            // There are N^2 boxes, so that results in a capacity of N^2 * N^2 (N^2 + 1) / 2
            List<SwapNeighbor> result = new List<SwapNeighbor>(sudoku.NN * sudoku.NN * (sudoku.NN + 1) / 2);

            // Loop through all boxes
            for(int box = 0; box < sudoku.NN; ++box)
            {
                // Within a box we loop through all squares and add all possibilities to the list
                int[,] units = helper.getUnitsFor(sudoku.N * (box % sudoku.N), sudoku.N * (box / sudoku.N));
                for(int square1 = 0; square1 < units.GetLength(1); ++square1)
                {
                    if(sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1]))
                        continue;

                    for(int square2 = square1 + 1; square2 < units.GetLength(1); ++square2)
                    {
                        if(!sudoku.isFixed(units[SudokuIndexHelper.UNIT_BOX_INDEX, square2]))
                        {
                            SwapNeighbor temp = new SwapNeighbor(
                                units[SudokuIndexHelper.UNIT_BOX_INDEX, square1],
                                units[SudokuIndexHelper.UNIT_BOX_INDEX, square2],
                                sudoku.heuristicDelta(units[SudokuIndexHelper.UNIT_BOX_INDEX, square1], units[SudokuIndexHelper.UNIT_BOX_INDEX, square2])
                            );
                            result.Add(temp);
                        }
                    }
                }
            }

            // Return the list
            return result;
        }
        
    }
}
