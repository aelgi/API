using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class HttpException : Exception
    {
        public int HttpStatusCode { get; }

        public HttpException(int httpStatusCode, string message = "Internal Error Occurred") : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
