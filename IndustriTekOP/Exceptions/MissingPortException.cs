using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Exceptions
{
    public class MissingPortException : Exception
    {
        public MissingPortException()
        {

        }

        public MissingPortException(string message) : base(message)
        {

        }

        public MissingPortException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
