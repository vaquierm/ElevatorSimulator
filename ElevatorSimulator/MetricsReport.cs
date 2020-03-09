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
        public double WaitingTime
        {
            get;
            private set;
        }

        // The time spent in the elevator
        public double TravelTime
        {
            get;
            private set;
        }

        // The total time interacting with the elevator
        public double TotalTime
        {
            get
            {
                return this.WaitingTime + this.TravelTime;
            }
        }

        // Energy used by elevators
        public double EnergyUsed
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

        public MetricsReport(double waitingTime, double travelTime, double energyUsed)
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
            return "Waiting Time: " + Math.Round(this.WaitingTime, 3) + " ticks, Travel Time: " + Math.Round(this.TravelTime, 3) + " ticks, Total Time: " + Math.Round(this.TotalTime, 3) + " ticks, Energy Used: " + Math.Round(this.EnergyUsed, 3) + " energy units";
        }
    }
}
