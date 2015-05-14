using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the sudoku
            Sudoku sudoku = SudokuReader.readFromFile("../../sudokus/9x9.txt", 3);
            Console.WriteLine("Original sudoku:");
            printSudoku(sudoku);
            Console.WriteLine();

            // Solve the sudoku as far as we can using the SudokuConstraintsHelper
            SudokuConstraintsHelper sudokuConstraintsHelper = new SudokuConstraintsHelper(new Sudoku(sudoku.N));
            List<int> indices = new List<int>();
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku[i].Count == 1)
                    indices.Add(i);
            }
            for(int i = 0; i < indices.Count; ++i)
            {
                if(!sudokuConstraintsHelper.assign(indices[i], sudoku[indices[i]][0]))
                {
                    Console.WriteLine("ERROR: This sudoku seems not to be possible...");
                    break;
                }
            }
            Console.WriteLine("Sudoku after applying constraints:");
            printSudoku(sudokuConstraintsHelper.Sudoku);
            Console.WriteLine();

            // Solve the sudoku using depth-first search
            LocalSearcher searchMachine = new TabuSearcher();
            Sudoku solution = searchMachine.solve(sudoku); 
            /* SimulatedAnnealer searcher = new SimulatedAnnealer();
            Sudoku solution = searcher.solve(sudoku); */
           // Sudoku solution = DepthFirstSearch.search(sudokuConstraintsHelper.Sudoku);
            if (solution == null)
            {
                Console.WriteLine("This sudoku seems to be impossible to solve...");
            }
            else if (solution.isSolved())
            {
                Console.WriteLine("The solution after depth-first search:");
                printSudoku(solution);
            }
            else
            {
                Console.WriteLine("ERROR! The solution after depth-first search seems to be wrong:");
                printSudoku(solution);
            }

            // Wait for the user
            Console.ReadLine();
        }

        static void printSudoku(Sudoku sudoku)
        {
            // Loop through the rows
            for(int y = 0; y < sudoku.NN; ++y)
            {
                // Write a separator when necessary
                if(y != 0 && y % sudoku.N == 0)
                {
                    for(int x = 0; x < sudoku.NN; ++x)
                    {
                        if(x != 0 && x % sudoku.N == 0)
                            Console.Write('+');
                        Console.Write('-');
                    }
                    Console.Write('\n');
                }

                // Loop through the squares in this row
                for(int x = 0; x < sudoku.NN; ++x)
                {
                    // Write a separator when necessary
                    if(x != 0 && x % sudoku.N == 0)
                        Console.Write('|');

                    // Write an x if there are no possibilities for this square
                    // If there is only one possibility, write that possibility
                    // Otherwise we write a '.'
                    List<int> values = sudoku[x, y];
                    if(values.Count == 0)
                        Console.Write('x');
                    else if(values.Count == 1)
                    {
                        if(values[0] <= 9)
                            Console.Write(values[0]);
                        else
                            Console.Write((char) ('A' + (values[0] - 10)));
                    }
                    else
                        Console.Write('.');
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
