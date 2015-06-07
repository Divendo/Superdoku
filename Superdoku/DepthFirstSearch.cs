using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>A stopwatch to limit the amount of time we search.</summary>
        private Stopwatch stopwatch;

        /// <summary>The search time limit in milliseconds.</summary>
        public const long SEARCH_TIME_LIMIT = 3 * 60 * 1000;

        /// <summary>Whether or not to use a hash map to keep track of failed nodes.</summary>
        private bool useHashMap;

        /// <summary>Nodes that we have tried but failed.</summary>
        private HashSet<Sudoku> failedNodes;

        /// <summary>The amount of nodes that were expanded.</summary>
        private long expandedNodes;

        /// <summary>The total amount of iterations the ConstraintHelper instances did.</summary>
        private long constraintIterations;

        /// <summary>The time that was used to apply the ConstraintHelper (in Stopwacht ticks).</summary>
        private long constraintTime;

        /// <summary>Used to measure the time the ConstraintHelper uses.</summary>
        private Stopwatch constraintTimeStopwatch;

        /// <summary>Constructor.</summary>
        public DepthFirstSearch()
        {
            constraintsHelperFactory = new ConstraintsHelperFactory_Trivial();
            stopwatch = null;
            useHashMap = false;
        }

        /// <summary>Constructor.</summary>
        /// <param name="helper">The factory that is to be used to create ConstraintsHelper instances, which will be used for constraint satisfaction.</param>
        public DepthFirstSearch(ConstraintsHelperFactory helperFactory)
        {
            constraintsHelperFactory = helperFactory;
            stopwatch = null;
            useHashMap = false;
        }

        /// <summary>Property to get the amount of nodes that were expanded in the last search.</summary>
        public long ExpandedNodes
        { get { return expandedNodes; } }

        /// <summary>Property to get the total amount of iterations that the constraint helpers used.</summary>
        public long ConstraintIterations
        { get { return constraintIterations; } }

        /// <summary>Property to get the total amount of Stopwacht ticks that the constraint helpers used.</summary>
        public long ContraintTime
        { get { return constraintTime; } }

        /// <summary>Convenience method. Applies clean() from the constraints helper of this instance.</summary>
        /// <param name="sudoku">The sudoku that should be cleaned.</param>
        /// <returns>The amount of iterations the constraint helper needed if succesful, or -1 if a contradiction was reached.</returns>
        public long clean(Sudoku sudoku)
        {
            ConstraintsHelper helper = constraintsHelperFactory.createConstraintsHelper(sudoku);
            if(helper.clean())
                return helper.Iterations;
            return -1;
        }

        /// <summary>Searches for a solution for the given sudoku using depth-first search.</summary>
        /// <param name="sudoku">The sudoku that should be solved.</param>
        /// <returns>The solved sudoku, or null if no solution was possible.</returns>
        public Sudoku search(Sudoku sudoku)
        {
            // Keep track of our time limit
            stopwatch = new Stopwatch();
            stopwatch.Start();

            // Reset the failed nodes set
            if(useHashMap)
                failedNodes = new HashSet<Sudoku>();

            // Do the actual searching
            expandedNodes = 0;
            constraintIterations = 0;
            constraintTime = 0;
            constraintTimeStopwatch = new Stopwatch();
            Sudoku result = search_helper(sudoku);

            // Clear the stopwatch
            stopwatch = null;

            // Clear the failed nodes set
            failedNodes = null;

            // Return the result
            return result;
        }

        /// <summary>Does the actual depth-first searching (while making deep copies).</summary>
        /// <returns>The solved sudoku if successful, null otherwise.</returns>
        private Sudoku search_helper(Sudoku sudoku)
        {
            // We can not solve a non-existing soduko
            if(sudoku == null)
                return null;

            // Check if this node was already tried
            if(useHashMap && failedNodes.Contains(sudoku))
                return null;

            // We are expanding this node
            ++expandedNodes;

            // Check if we have already solved the sudoku
            if(sudoku.isSolvedSimple())
                return sudoku;

            // Pick the square with the fewest possibilities (ignoring the squares with one possibility)
            int index = -1;
            int indexValueCount = 0;
            for(int i = 0; i < sudoku.NN * sudoku.NN; ++i)
            {
                if(sudoku.valueCount(i) == 1)
                    continue;

                if(index == -1 || sudoku.valueCount(i) < indexValueCount)
                {
                    indexValueCount = sudoku.valueCount(i);
                    index = i;
                }
            }

            // If there is a square without any possibilites, we can not find a solution
            if(indexValueCount == 0)
                return null;

            // Try all possibilities for the square we found
            ulong valuesToCheck = sudoku[index];
            for(ulong value = 1; valuesToCheck != 0; value <<= 1)
            {
                if(stopwatch.ElapsedMilliseconds > SEARCH_TIME_LIMIT)
                    throw new TimeLimitReachedException("Time limit of " + SEARCH_TIME_LIMIT + "ms reached (actual: " + stopwatch.ElapsedMilliseconds + "ms).");

                if((sudoku[index] & value) == 0)
                    continue;

                // Apply the constraint helper (and measure its performance)
                ConstraintsHelper helper = constraintsHelperFactory.createConstraintsHelper(new Sudoku(sudoku));
                constraintTimeStopwatch.Start();
                bool helperSuccess = helper.assign(index, value);
                constraintTimeStopwatch.Stop();
                constraintTime += constraintTimeStopwatch.ElapsedTicks;
                constraintTimeStopwatch.Reset();
                constraintIterations += helper.Iterations;

                // If we did not reach a contradiction, we expand this node
                if(helperSuccess)
                {
                    Sudoku result = search_helper(helper.Sudoku);
                    if(result != null)
                        return result;
                }

                // We have checked this value and no longer need to check it
                valuesToCheck ^= value;
            }

            // Remember this node will not give a solution
            if(useHashMap)
                failedNodes.Add(sudoku);

            // If we have come here, we have not found a solution
            return null;
        }

        /// <summary>Whether or not to use a hash map to keep track of failed nodes.</summary>
        public bool UseHashMap
        {
            get { return useHashMap; }
            set { useHashMap = value; }
        }
    }

    /// <summary>Exception to indicate that the time limit has been reached.</summary>
    class TimeLimitReachedException : Exception
    {
        public TimeLimitReachedException()
        { }

        public TimeLimitReachedException(string msg)
            : base(msg) { }

        public TimeLimitReachedException(string msg, Exception inner)
            : base(msg, inner) { }
    }
}
