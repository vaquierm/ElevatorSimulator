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
        
        // Elevators that can be controlled by the AI
        public ElevatorCollection Elevators;

        // The building that the elevators are in
        public Building Building;

        // The Smart relocator is optional
        public PredictiveRelocatorPluggin Relocator;

        public ElevatorAI(Building building, ElevatorCollection elevators, SimulationConfiguration config)
        {
            this.Building = building;
            this.Elevators = elevators;

            if (config.SmartRelocation)
            {
                this.Relocator = new PredictiveRelocatorPluggin(elevators, config);
            }
            else
            {
                this.Relocator = null;
            }
            
        }

        public abstract void HandleRequests();

        public void NotifyPickUp(Request request)
        {
        }

        public void NotifyDropOff(Elevator.Elevator elevator, Request request)
        {
            if (elevator.IsIdle)
            {
                this.Relocator?.RelocateIdleElevator(elevator);
            }
        }
    }

    class ElevatorAIFactory
    {
        public static ElevatorAI CreateElevatorAI(Building building, ElevatorCollection elevators, SimulationConfiguration config)
        {
            switch (config.AIType)
            {
                case "BENCHMARK":
                    return new BenchmarkAI(building, elevators, config);
                case "OPTIMIZED":
                    return new OptimizedAI(building, elevators, config);
                case "REGULAR":
                    return new RegularAI(building, elevators, config);
                default:
                    throw new UnknownAIException("The Elevator AI: " + config.AIType + " is unknown.");
            }
        }
    }

    interface ElevatorController
    {
        void HandleRequests();

        void NotifyPickUp(Request request);

        void NotifyDropOff(Elevator.Elevator elevator, Request request);
    }
}
