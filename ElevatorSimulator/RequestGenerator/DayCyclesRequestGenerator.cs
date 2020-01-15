using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.RequestGenerator
{
    class DayCyclesRequestGenerator : RequestGenerator
    {
        // Keep the base probabilities of requests per floor
        private double[] BaseRequestProbabilitiesPerFloor;

        // Keep the base probability of destination per floor
        private double[] BaseDestinationProbabilitiesPerFloor;

        // Number of ticks in a day
        private readonly long TicksPerDay;

        // Current tick
        private long CurrentTick;

        private readonly double[] RCSourceFunction = {
            0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2,
            0.4, 0.4, 0.4, 0.4, 0.4, 0.4,
            0, 0, 0, 0, 0, 0, 0, 0,
            1.55, 1.55, 1.55,
            1, 1, 1,
            1.9, 1.9, 1.9, 1.9, 1.9, 1.9, 1.9, 1.9,
            1.55, 1.55, 1.55, 1.55, 1.55,
            1, 1, 1, 1, 1
        };

        private readonly double[] RCDestinationFunction =
        {
            0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2,
            0.8, 0.8, 0.8, 0.8,
            2.3, 2.3, 2.3, 2.3,
            1.8, 1.8, 1.8,
            1, 1, 1,
            1.7, 1.7, 1.7,
            1, 1, 1, 1, 1,
            0.8, 0.8, 0.8, 0.8, 0.8, 0.8,
            1.7, 1.7, 1.7, 1.7,
            1, 1, 1,
            0.4, 0.4, 0.4
        };

        private readonly int FunctionResolution = 48;

        public DayCyclesRequestGenerator(Building building, SimulationConfiguration config) : base(building, config)
        {
            this.BaseRequestProbabilitiesPerFloor = (double[]) this.RequestProbabilityPerFloor.Clone();
            this.BaseDestinationProbabilitiesPerFloor = (double[]) this.DestinationProbabilityPerFloor.Clone();

            this.TicksPerDay = config.TicksPerDay;

            this.CurrentTick = 0;

            if (this.FunctionResolution != this.RCDestinationFunction.Count())
            {
                throw new InvalidDayCycleRequestGeneraotrState("The RC Destination multiplier function must be of length " + this.FunctionResolution);
            }
            if (this.FunctionResolution != this.RCSourceFunction.Count())
            {
                throw new InvalidDayCycleRequestGeneraotrState("The RC Source multiplier function must be of length " + this.FunctionResolution);
            }
        }

        public override List<Request> GenerateRequests()
        {
            this.UpdateDistributions();

            this.CurrentTick++;

            return base.GenerateRequests();
        }

        private void UpdateDistributions()
        {
            long tickOfDay = this.CurrentTick % this.TicksPerDay;

            long ticksPerResolution = this.TicksPerDay / this.FunctionResolution;

            if (tickOfDay % ticksPerResolution == 0)
            {
                int currentTimeInterval = (int) (tickOfDay / ticksPerResolution);

                // Scale the RC floor based on the time of day
                this.DestinationProbabilityPerFloor[0] = this.BaseRequestProbabilitiesPerFloor[0] * this.RCDestinationFunction[currentTimeInterval];
                this.RequestProbabilityPerFloor[0] = this.BaseDestinationProbabilitiesPerFloor[0] * this.RCSourceFunction[currentTimeInterval];

                // Scale the other floors based on the time of day
                for (int i = 1; i < this.RequestProbabilityPerFloor.Count(); i++)
                {
                    this.RequestProbabilityPerFloor[i] = this.BaseRequestProbabilitiesPerFloor[i] * this.RCDestinationFunction[currentTimeInterval];
                    this.DestinationProbabilityPerFloor[i] = this.BaseDestinationProbabilitiesPerFloor[i] * this.RCSourceFunction[currentTimeInterval];
                }
            }
        }
    }
}
