using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class SudokuNotPossibleException : Exception
    {
        public SudokuNotPossibleException() { }
        
        public SudokuNotPossibleException(String message)
            : base(message) { }

        public SudokuNotPossibleException(String message, Exception exception)
            : base(message, exception) { }
    }
}
