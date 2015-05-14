using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SwapNeighbor
    {
        private int delta;
        private int first;
        private int second;

        public SwapNeighbor(LocalSudoku sudoku, int index_a, int index_b)
        {
            first = index_a;
            second = index_b;
            delta = sudoku.heuristicDelta(index_a, index_b);
        }

        public int Delta
        { get { return delta; } }

        public int First
        { get { return first; } }

        public int Second
        { get { return second; } }

        public static bool equal(SwapNeighbor a, SwapNeighbor b)
        {
            if (a == null)
                return false;

            if (b == null)
                return false;

            return (a.first == b.first && a.second == b.second) ||
                    (a.first == b.Second && a.second == b.first);
        }

        /*
        public static bool operator ==(SwapNeighbor a, SwapNeighbor b)
        {
            if (a == null)
                return false;

            if (b == null)
                return false;

            return  (a.first == b.first && a.second == b.second) ||
                    (a.first == b.Second && a.second == b.first);
        }

        public static bool operator !=(SwapNeighbor a, SwapNeighbor b)
        {
            return !(a == b);
        }*/
    }
}
