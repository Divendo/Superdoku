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
            // Setup the test queue
            TestQueue testQueue = new TestQueue();
            testQueue.addTest(new Test_CGA_SimAn(), "../../sudokus/project-euler-50-9x9.txt", 3, "siman-cga-test.csv");

            // Run the tests
            testQueue.run();

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
