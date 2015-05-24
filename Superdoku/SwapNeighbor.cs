using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    /// <summary>Represents a possible swap that can be performed to get a neighbor.</summary>
    class SwapNeighbor : IComparable<SwapNeighbor>
    {
        /// <summary>The first square that should be swapped.</summary>
        private int square1;
        /// <summary>The second square that should be swapped.</summary>
        private int square2;
        /// <summary>The change in score after swapping the squares.</summary>
        private int scoreDelta;

        public SwapNeighbor(int square1, int square2, int scoreDelta)
        {
            this.square1 = square1;
            this.square2 = square2;
            this.scoreDelta = scoreDelta;
        }

        public int ScoreDelta
        { get { return scoreDelta; } }

        public int Square1
        { get { return square1; } }

        public int Square2
        { get { return square2; } }

        public override bool Equals(object obj)
        {
            if(!(obj is SwapNeighbor))
                return false;

            return Equals((SwapNeighbor) obj);
        }
        
        public bool Equals(SwapNeighbor other)
        {
            // Two SwapNeighbor instances are equal if they swap the same squares
            // So we ignore the scoreDelta on purpose
            return (square1 == other.square1 && square2 == other.square2) || (square1 == other.square2 && square2 == other.square1);
        }

        public override int GetHashCode()
        {
            if(square1 < square2)
                return square1 * 1000000 + square2;
            return square2 * 1000000 + square1;
        }

        public int CompareTo(SwapNeighbor other)
        {
            if (this.scoreDelta > other.scoreDelta)
                return -1;
            if (this.scoreDelta < other.scoreDelta)
                return 1;
            return 0;
        }

        public static bool operator < (SwapNeighbor a, SwapNeighbor b)
        { return a.scoreDelta > b.scoreDelta; }

        public static bool operator >(SwapNeighbor a, SwapNeighbor b)
        { return a.scoreDelta < b.scoreDelta; }

        public static bool operator <= (SwapNeighbor a, SwapNeighbor b)
        { return a.scoreDelta >= b.scoreDelta; }

        public static bool operator >=(SwapNeighbor a, SwapNeighbor b)
        { return a.scoreDelta <= b.scoreDelta; }

        public static bool equal(SwapNeighbor a, SwapNeighbor b)
        { return a.Equals(b); }

        public static bool operator == (SwapNeighbor a, SwapNeighbor b)
        { return a.Equals(b); }

        public static bool operator != (SwapNeighbor a, SwapNeighbor b)
        { return !a.Equals(b); }
    }
}
