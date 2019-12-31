using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class ElevatorCollection : IEnumerable<Elevator>
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

        public uint Tick()
        {
            uint totalEnergyUsed = 0;
            for (int i = 0; i < this.Elevators.Count(); i++)
            {
                totalEnergyUsed += this.Elevators[i].Tick();
            }

            return totalEnergyUsed;
        }

        public IEnumerator<Elevator> GetEnumerator()
        {
            return this.Elevators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
