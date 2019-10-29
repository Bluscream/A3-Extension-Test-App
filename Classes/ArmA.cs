using System.Collections.Generic;
using System.Linq;

namespace Arma
{
    public class SQFCommand
    {
        public string Command { get; set; }
        public List<string> Arguments { get; set; }

        public SQFCommand(string command, params string[] arguments)
        {
            Command = command;
            Arguments = arguments.ToList();
        }
    }
}