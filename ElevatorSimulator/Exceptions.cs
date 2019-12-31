﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class CustomException : Exception
    {
        public new readonly string Message;

        public CustomException(string message)
        {
            this.Message = message;
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + this.Message;
        }
    }

    class InvalidSimulationConfigException : CustomException
    {
        public InvalidSimulationConfigException(string message) : base(message)
        {
        }
    }

    class InvalidWaypointException : CustomException
    {
        public InvalidWaypointException(string message) : base(message)
        {
        }
    }

    class UnknownAIException : CustomException
    {
        public UnknownAIException(string message) : base(message)
        {
        }
    }

    class UnknownRequestGeneratorException : CustomException
    {
        public UnknownRequestGeneratorException(string message) : base(message)
        {
        }
    }

}
