using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class TestQueue
    {
        /// <summary>The tests that we should run.</summary>
        private List<TestQueueItem> tests = new List<TestQueueItem>();

        /// <summary>Adds the given test to the queue.</summary>
        /// <param name="test">The test that is added to the queue.</param>
        public void addTest(TestQueueItem test)
        { tests.Add(test); }

        /// <summary>Overloaded method, creates a test from the given parameters and adds it to the queue.</summary>
        /// <param name="test">The test that will be run.</param>
        /// <param name="sudokuFile">The file where we should load the sudokus from.</param>
        /// <param name="maxSudokus">The maximum amount of sudokus to read (or a negative value for unlimited).</param>
        /// <param name="n">The size of each sudoku (n*n squares by n*n squares).</param>
        /// <param name="exportFile">The file to export the results to (null if the results should not be exported).</param>
        public void addTest(Test test, string sudokuFile, int maxSudokus, int n, string exportFile)
        { addTest(new TestQueueItem(test, sudokuFile, maxSudokus, n, exportFile)); }

        /// <summary>Runs all the tests in the queue.</summary>
        public void run()
        {
            foreach(TestQueueItem testQueueItem in tests)
            {
                // Read the sudokus
                Sudoku[] sudokus = SudokuReader.readFromFileLines(testQueueItem.sudokuFile, testQueueItem.n, testQueueItem.maxSudokus);
                Console.WriteLine("{0} sudokus imported.", sudokus.Length);

                // Make sure the SudokuIndexHelper is cached (for fair measurements)
                SudokuIndexHelper.get(sudokus[0].N);

                // Run the test
                testQueueItem.test.runTest(sudokus, testQueueItem.exportFile);
            }
        }
    }

    class TestQueueItem
    {
        /// <summary>The test that will be run.</summary>
        public Test test;
        /// <summary>The file where we should load the sudokus from.</summary>
        public string sudokuFile;
        /// <summary>The maximum amount of sudokus to read (or a negative value for unlimited).</summary>
        public int maxSudokus;
        /// <summary>The size of each sudoku (n*n squares by n*n squares).</summary>
        public int n;
        /// <summary>The file to export the results to (null if the results should not be exported).</summary>
        public string exportFile;

        /// <summary>Constructor.</summary>
        /// <param name="test">The test that will be run.</param>
        /// <param name="sudokuFile">The file where we should load the sudokus from.</param>
        /// <param name="maxSudokus">The maximum amount of sudokus to read (or a negative value for unlimited).</param>
        /// <param name="n">The size of each sudoku (n*n squares by n*n squares).</param>
        /// <param name="exportFile">The file to export the results to (null if the results should not be exported).</param>
        public TestQueueItem(Test test, string sudokuFile, int maxSudokus, int n, string exportFile)
        {
            this.test = test;
            this.sudokuFile = sudokuFile;
            this.maxSudokus = maxSudokus;
            this.n = n;
            this.exportFile = exportFile;
        }
    }
}
