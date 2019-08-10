using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class EntryExistsException : HttpException
    {
        public EntryExistsException() : base(400, "Entry already exists") { }
    }
}
