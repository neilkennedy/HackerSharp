using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackerSharpAPI
{
    public class HackerException : Exception
    {
        public HackerException() { }
        public HackerException(string message) : base(message) { }
        public HackerException(string message, Exception inner) : base(message, inner) { }
    }
}
