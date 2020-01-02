using ElevatorSimulator.Elevator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.ElevatorAI
{
    abstract class ElevatorAI : ElevatorController
    {
        

        public ElevatorCollection Elevators;

        public Building Building;

        public ElevatorAI(Building building, ElevatorCollection elevators)
        {
            this.Building = building;
            this.Elevators = elevators;

        }

        public abstract void HandleRequests();
    }

    class ElevatorAIFactory
    {
        public static ElevatorAI CreateElevatorAI(string AIType, Building building, ElevatorCollection elevators)
        {
            switch (AIType)
            {
                case "BENCHMARK":
                    return new BenchmarkAI(building, elevators);
                default:
                    throw new UnknownAIException("The Elevator AI: " + AIType + " is unknown.");

            }
        }
    }

    interface ElevatorController
    {
        void HandleRequests();
    }
}
