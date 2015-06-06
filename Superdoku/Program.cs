using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class Program
    {
        static void Main(string[] args)
        {
            /***** SETUP THE TEST PARAMETERS HERE *****/

            // The test we want to run
            Test test = new Test_DepthFirstGlobal();
                   
            // The sudokus to use for the test
            string sudokusFile = "../../sudokus/project-euler-50-9x9.txt";
     
            // The maximum amount of sudokus to read, or -1 for unlimited
            int maxSudokus = -1;
         
            // The size of the sudokus (n*n by n*n squares)
            int n = 3;

            // The file to export the results to (null if the results should not be exported)
            string exportFile = "depth-first.csv";
             
            /***** END OF THE TEST SETUP, YOU DO NOT NEED TO CHANGE ANYTHING BELOW HERE *****/

            // Read the sudokus
            Sudoku[] sudokus = SudokuReader.readFromFileLines(sudokusFile, n, maxSudokus);
            Console.WriteLine("{0} sudokus imported.", sudokus.Length);

            // Make sure the SudokuIndexHelper is cached (for fair measurements)
            SudokuIndexHelper.get(sudokus[0].N);

            // Run the test
            test.runTest(sudokus, exportFile);

            // Wait for the user
            Console.WriteLine("Press <enter> to quit.");
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
                    if(sudoku[x, y] == 0)
                        Console.Write('x');
                    else
                    {
                        int singleValue = sudoku.singleValue(x, y);
                        if(singleValue != -1)
                        {
                            if(singleValue <= 9)
                                Console.Write(singleValue);
                            else
                                Console.Write((char)('A' + (singleValue - 10)));
                        }
                        else
                            Console.Write('.');
                    }
                }

                // Next row
                Console.Write('\n');
            }
        }
    }
}
