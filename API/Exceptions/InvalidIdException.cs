using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class InvalidIdException : HttpException
    {
        public InvalidIdException() : base(423, "Invalid Id Passed") { }
    }
}
