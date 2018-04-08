using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiritProject.WebUI.Exceptions
{
    public class CommonException : Exception
    {

        public CommonException() { }
        public CommonException(string message)
            : base(message)
        {
        }

        public CommonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}