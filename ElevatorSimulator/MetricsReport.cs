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

        // The time spent in the elevator
        public uint TravelTime
        {
            get;
            private set;
        }

        // The total time interacting with the elevator
        public uint TotalTime
        {
            get
            {
                return this.WaitingTime + TravelTime;
            }
        }

        // Energy used by elevators
        public uint EnergyUsed
        {
            get;
            private set;
        }

        public MetricsReport(uint waitingTime, uint travelTime, uint energyUsed)
        {
            this.WaitingTime = waitingTime;
            this.TravelTime = travelTime;
            this.EnergyUsed = energyUsed;
        }

        // Override the addition and subtraction operators
        public static MetricsReport operator +(MetricsReport a, MetricsReport b) 
            => new MetricsReport(a.WaitingTime + b.WaitingTime, a.TravelTime + b.TravelTime, a.EnergyUsed + b.EnergyUsed);

        public static MetricsReport operator -(MetricsReport a, MetricsReport b)
            => new MetricsReport(a.WaitingTime - b.WaitingTime, a.TravelTime - b.TravelTime, a.EnergyUsed - b.EnergyUsed);

        public static MetricsReport operator /(MetricsReport a, MetricsReport b)
            => new MetricsReport(a.WaitingTime / b.WaitingTime, a.TravelTime / b.TravelTime, a.EnergyUsed / b.EnergyUsed);

        public override string ToString()
        {
            return "Waiting Time: " + this.WaitingTime + " ticks, Travel Time: " + this.TravelTime + " ticks, Total Time: " + this.TotalTime + " ticks, Energy Used: " + this.EnergyUsed + " energy units";
        }
    }
}
