using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class ElevatorCollection
    {
        // List of elevators
        public List<Elevator> Elevators
        {
            get;
            private set;
        }
        
        public ElevatorCollection(SimulationConfiguration config)
        {
            this.Elevators = new List<Elevator>();

            for (int i = 0; i < config.NumberOfElevators; i++)
            {
                this.Elevators.Add(new Elevator(config));
            }
        }
    }
}
