using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class MetricsReport
    {
        // Waiting time in ticks
        public uint WaitingTime
        {
            get;
            private set;
        }

        // Energy used by elevators
        public uint EnergyUsed
        {
            get;
            private set;
        }

        public MetricsReport(uint waitingTime, uint energyUsed)
        {
            this.WaitingTime = waitingTime;
            this.EnergyUsed = energyUsed;
        }

        // Override the addition and subtraction operators
        public static MetricsReport operator +(MetricsReport a, MetricsReport b) 
            => new MetricsReport(a.WaitingTime + b.WaitingTime, a.EnergyUsed + b.EnergyUsed);

        public static MetricsReport operator -(MetricsReport a, MetricsReport b)
            => new MetricsReport(a.WaitingTime - b.WaitingTime, a.EnergyUsed - b.EnergyUsed);
    }
}
