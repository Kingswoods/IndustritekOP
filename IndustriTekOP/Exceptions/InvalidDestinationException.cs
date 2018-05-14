using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Exceptions
{
    public class InvalidDestinationException : Exception
    {
        public InvalidDestinationException()
        {

        }

        public InvalidDestinationException(string message) : base(message)
        {

        }

        public InvalidDestinationException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
