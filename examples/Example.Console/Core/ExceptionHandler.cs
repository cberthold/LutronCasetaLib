using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Core
{
    public static class ExceptionHandler
    {
        public static void Handle(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
