using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class InvalidSimulationConfigException : Exception
    {
        public new readonly string Message;

        public InvalidSimulationConfigException(string message)
        {
            this.Message = message;
        }

        public override string ToString()
        {
            return "InvalidSimulationConfigException: " + this.Message;
        }
    }
}
