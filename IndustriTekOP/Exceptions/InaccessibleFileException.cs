using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP.Exceptions
{
    public class InaccessibleFileException : Exception
    {
        public InaccessibleFileException()
        {

        }

        public InaccessibleFileException(string message) : base(message)
        {

        }

        public InaccessibleFileException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
