using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Exceptions
{
    public class OngoingActivityException : Exception
    {
        public OngoingActivityException()
        {

        }

        public OngoingActivityException(string message) : base(message)
        {

        }

        public OngoingActivityException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
