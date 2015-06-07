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
            testQueue.addTest(new Test_DepthFirstGlobal(), "../../sudokus/norvig-and-euler-145-9x9.txt", -1, 3, "depth-first-3.csv");
            testQueue.addTest(new Test_DepthFirstGlobal(), "../../sudokus/maatec-30-16x16.txt", -1, 4, "depth-first-4.csv");
            testQueue.addTest(new Test_DepthFirstGlobal(), "../../sudokus/maatec-20-25x25.txt", -1, 5, "depth-first-5.csv");
            testQueue.addTest(new Test_DepthFirstGlobal(), "../../sudokus/maatec-15-36x36.txt", -1, 6, "depth-first-6.csv");

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
