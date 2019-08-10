using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException() : base(404, "Access to data could not be found") { }
        public NotFoundException(string message) : base(404, message) { }
    }
}
