using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class Simulation
    {
        // Number of days the simulation will run for
        public readonly uint SimulationDays;
        // Number of ticks per day
        public readonly long TicksPerDay;

        // Current time of the simulation in ticks
        public long CurrentTick
        {
            get;
            private set;
        }

        // Building that the simulation runs on
        public Building Building
        {
            get;
            private set;
        }


        public Simulation(SimulationConfiguration config)
        {
            // Check the validity of the configuration
            config.CheckValidity();

            this.SimulationDays = config.SimulationDays;
            this.TicksPerDay = config.TicksPerDay;

            this.Building = new Building(config);
        }

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <returns> A Metrics report that shows all metrics of the simulation </returns>
        public MetricsReport StartSimulation()
        {
            MetricsReport simulationMetrics = new MetricsReport(0, 0);

            while (this.CurrentTick < this.TicksPerDay * this.SimulationDays)
            {
                if (this.CurrentTick % this.TicksPerDay == 0)
                    Console.WriteLine("Simulation start of day " + this.CurrentTick / this.TicksPerDay);

                // Increment the simulation time
                this.CurrentTick++;

                // Make the building tick and update the simulation metrics
                simulationMetrics += this.Building.Tick();
            }

            return simulationMetrics;
        }
    }

    class SimulationConfiguration
    {
        // Number of days the simulation will run for
        public readonly uint SimulationDays;
        // Number of ticks per day
        public readonly long TicksPerDay;

        // Number of elevators that are used in the simulation
        public readonly uint NumberOfElevators;
        // Energy spent per elevator per tick
        public readonly uint EnergyPerTick;

        // The type of AI that will be used during simulation
        public readonly string AIType;

        // Request Generator type
        public readonly string RequestGeneratorType;

        // Number of floors in the simulation building
        public readonly uint BuildingFloors;
        // Speed in floors per tick that elevators travel
        public readonly uint ElevatorSpeed;
        // Loading time in ticks
        public readonly uint LoadingTime;
        // Rsidents per floor in an array
        public readonly uint[] ResidentsPerFloor;
        // Average number of requests per day per residents
        public readonly uint AverageRequestsPerResidentPerDay;
        // Interest per floor (The main floor and prehaps floors with shared spaces have higher interest)
        public readonly double[] InterestPerFloor;

        public SimulationConfiguration(uint simulationDays, uint ticksPerDay, uint elevatorsNumber, uint energyPerTick, string AIType, string requestGeneratorType, uint buildingFloors, uint elevatorSpeed, uint loadingTime, uint[] residentsPerFloor, uint averageRequestsPerResidentsPerDay, uint[] interestPerFloor)
        {
            this.SimulationDays = simulationDays;
            this.TicksPerDay = ticksPerDay;

            this.NumberOfElevators = elevatorsNumber;
            this.EnergyPerTick = energyPerTick;

            this.AIType = AIType;
            this.RequestGeneratorType = requestGeneratorType;

            this.BuildingFloors = buildingFloors;
            this.ElevatorSpeed = elevatorSpeed;
            this.LoadingTime = loadingTime;

            this.ResidentsPerFloor = residentsPerFloor;

            this.AverageRequestsPerResidentPerDay = averageRequestsPerResidentsPerDay;

            long totalInterest = interestPerFloor.Sum(x => x);
            this.InterestPerFloor = interestPerFloor.Select(interest => ((double)interest / totalInterest)).ToArray();

            // Check if the configuration is valid
            this.CheckValidity();
        }

        public void CheckValidity()
        {
            if (this.BuildingFloors != this.ResidentsPerFloor.Count())
            {
                throw new InvalidSimulationConfigException("The list of residents per floor must match the number of floors in the building");
            }
            if (this.BuildingFloors != this.InterestPerFloor.Count())
            {
                throw new InvalidSimulationConfigException("The list of residents per floor must match the number of floors in the building");
            }

            if (this.TicksPerDay <= 0)
            {
                throw new InvalidSimulationConfigException("The number of ticks per day must be positive and non zero");
            }
            if (this.BuildingFloors < 1)
            {
                throw new InvalidSimulationConfigException("There must be at least more than one floor in the building");
            }
            if (this.ElevatorSpeed <= 0)
            {
                throw new InvalidSimulationConfigException("The speed of the elevator in floors per tick must be a positive number");
            }
            if (this.LoadingTime <= 0)
            {
                throw new InvalidSimulationConfigException("The loading time must be bigger than 0 ticks");
            }
            if (this.SimulationDays <= 0)
            {
                throw new InvalidSimulationConfigException("The number of simulation days must be positive and non zero");
            }
            if (this.AverageRequestsPerResidentPerDay <= 0)
            {
                throw new InvalidSimulationConfigException("The average number of requests a resident makes per day must be positive and non zero");
            }
        }
    }
}
