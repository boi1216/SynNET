using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.utils
{
    class ConfigValidationException : Exception
    {

        public ConfigValidationException() : base() { }

        public ConfigValidationException(string message) : base(message) {}
    }
}
