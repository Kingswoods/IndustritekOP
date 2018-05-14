using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Exceptions
{
    public class InvalidMessageException : Exception
    {
        public InvalidMessageException()
        {

        }

        public InvalidMessageException(string message) : base(message)
        {

        }

        public InvalidMessageException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
