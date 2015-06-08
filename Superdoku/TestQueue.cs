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
        /// <param name="n">The size of each sudoku (n*n squares by n*n squares).</param>
        /// <param name="exportFile">The file to export the results to (null if the results should not be exported).</param>
        /// <param name="startSudoku">The index of the sudoku to start the testing.</param>
        /// <param name="testSudokuCount">The amount of sudokus to test (negative value for unlimited).</param>
        public void addTest(Test test, string sudokuFile, int n, string exportFile, int startSudoku = 0, int testSudokuCount = -1)
        { addTest(new TestQueueItem(test, sudokuFile, n, exportFile, startSudoku, testSudokuCount)); }

        /// <summary>Runs all the tests in the queue.</summary>
        public void run()
        {
            foreach(TestQueueItem testQueueItem in tests)
            {
                // Read the sudokus
                Sudoku[] sudokus = SudokuReader.readFromFileLines(testQueueItem.sudokuFile, testQueueItem.n);
                
                // Copy the sudokus we want to test
                if(testQueueItem.startSudoku >= sudokus.Length)
                    continue;
                int testSudokuCount = testQueueItem.testSudokuCount;
                if(testQueueItem.testSudokuCount == -1 || testQueueItem.startSudoku + testQueueItem.testSudokuCount > sudokus.Length)
                    testSudokuCount = sudokus.Length - testQueueItem.startSudoku;
                if(testSudokuCount == 0)
                    continue;
                Sudoku[] sudokusToTest = new Sudoku[testSudokuCount];
                Array.Copy(sudokus, testQueueItem.startSudoku, sudokusToTest, 0, testSudokuCount);

                // Make sure the SudokuIndexHelper is cached (for fair measurements)
                SudokuIndexHelper.get(sudokusToTest[0].N);

                // Run the test
                testQueueItem.test.runTest(sudokusToTest, testQueueItem.exportFile);
            }
        }
    }

    class TestQueueItem
    {
        /// <summary>The test that will be run.</summary>
        public Test test;
        /// <summary>The file where we should load the sudokus from.</summary>
        public string sudokuFile;
        /// <summary>The size of each sudoku (n*n squares by n*n squares).</summary>
        public int n;
        /// <summary>The file to export the results to (null if the results should not be exported).</summary>
        public string exportFile;
        /// <summary>The index of the sudoku to start the testing.</summary>
        public int startSudoku;
        /// <summary>The amount of sudokus to test (negative value for unlimited).</summary>
        public int testSudokuCount;

        /// <summary>Constructor.</summary>
        /// <param name="test">The test that will be run.</param>
        /// <param name="sudokuFile">The file where we should load the sudokus from.</param>
        /// <param name="n">The size of each sudoku (n*n squares by n*n squares).</param>
        /// <param name="exportFile">The file to export the results to (null if the results should not be exported).</param>
        /// <param name="startSudoku">The index of the sudoku to start the testing.</param>
        /// <param name="testSudokuCount">The amount of sudokus to test (negative value for unlimited).</param>
        public TestQueueItem(Test test, string sudokuFile, int n, string exportFile, int startSudoku, int testSudokuCount)
        {
            this.test = test;
            this.sudokuFile = sudokuFile;
            this.n = n;
            this.exportFile = exportFile;
            this.startSudoku = startSudoku;
            this.testSudokuCount = testSudokuCount;
        }
    }
}
