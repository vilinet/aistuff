using CarAI.AutoMotive;
using Lib.AI;
using Lib.Evolution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CarAI
{
    public class EvolutionManager
    {
        public NeuralNetwork Network { get; private set; }

        public Evolution<CarGenotype> Evolution { get; private set; }

        List<Car> _cars = new List<Car>();
        public IEnumerable<Car> Cars => _cars.AsEnumerable();

        public Road Road { get; } = new Road();

        float lastBestFitness = 0;

        public EvolutionManager()
        {
            double[] weights = null;//  LoadWeights(); 
            uint sensors = (uint)new Car(Road, new CarGenotype()).Sensors.Count;
            Network = new NeuralNetwork(sensors, 1);
            CarGenotype.PARAMETER_COUNT = Network.WeightCount;

            Evolution = new Evolution<CarGenotype>(500, weights)
            {
                Elits = 50,
                ElitRequirementTop = 0.10f
            };

            UpdateCarEntities();
        }

        double[] LoadWeights()
        {
            var str = File.ReadAllText("c:/temp/weights.dat");
            var parts = str.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var numbers = new double[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                numbers[i] = double.Parse(parts[i]);
            }

            return numbers;
        }

        internal void Drag(int x, int y)
        {
            var ringId = -1;
            var closestVertexId = -1;
            double minDistance = 9999999999;

            for (int i = 0; i < Road.Rings.Length; i++)
            {
                for (int j = 0; j < Road.Rings[i].Length; j++)
                {
                    var p = Road.Rings[i][j];
                    var dist = Math.Sqrt(Math.Pow(p.Position.X - x, 2) + Math.Pow(p.Position.Y - y, 2));
                    if( dist<10 && dist < minDistance)
                    {
                        minDistance = dist;
                        ringId = i;
                        closestVertexId = j;
                    }
                }
            }
            if (closestVertexId >= 0) {
                Road.Rings[ringId][closestVertexId].Position = new SFML.System.Vector2f(x,y);
                    }
        }

        public void UpdateCarEntities()
        {
            _cars.Clear();

            foreach (var item in Evolution.Population)
            {
                _cars.Add(new Car(Road, (CarGenotype)item));
            }

        }

        public void NewGeneration()
        {
            var best = Evolution.Evolve() as CarGenotype;

            if( best.CheckpointsPassed > lastBestFitness )
            {
                lastBestFitness = best.CheckpointsPassed;
                var str = "";
                for (int i = 0; i < best.Parameters.Length; i++)
                {
                    str += best.Parameters[i].ToString("0.###################################") + ";";
                }
                File.WriteAllText("C:/temp/ki.txt", str);
            }

            UpdateCarEntities();
        }

        public void Reset()
        {
            Evolution.InitPopulation(null);
            UpdateCarEntities();
        }

        internal void Update()
        {
            if(/*(DateTime.Now - levelStartTime).Seconds > 30  ||*/_cars.Count == 0)
            {
                NewGeneration();
            }

            foreach (var car in Cars.ToList())
            {
                car.Update();

                var weightCounter = 0;
                foreach (var layer in Network.Layers)
                {
                    for (int i = 0; i <= layer.Weights.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j <= layer.Weights.GetUpperBound(1); j++)
                        {
                            layer.Weights[i, j] = car.Genotype.Parameters[weightCounter++];
                        }
                    }
                }

                var result = Network.ProcessInputs(car.GetInputVector());

                car.Go(GetDirection(result), (float)result[0]);

                var checkpoint = Road.Checkpoint(car);
                if (checkpoint != -1)
                {

                    if ( car.Genotype.Checkpoint < checkpoint)
                    {
                        car.Genotype.CheckpointsPassed++;
                        car.Genotype.Checkpoint = checkpoint;
                    }
                    else if(car.Genotype.Checkpoint == checkpoint && checkpoint == Road.GetMaxCheckpoint())
                    {
                        car.Genotype.Checkpoint = 0;
                        car.Genotype.CheckpointsPassed++;
                    }
                    else
                    {
                        car.Genotype.Checkpoint = checkpoint;
                    }

                }
                if(Road.Intersects(car))
                {
                    _cars.Remove(car);
                    Console.WriteLine("Alive: " + _cars.Count);
                }
            }
        }

        private Direction GetDirection(double[] array)
        {
            if (array[0] <= 0.33333333) return Direction.Right;
            else if (array[0] <= 0.66666666) return Direction.Left;
            else return Direction.Ahead;
        }

    }
}
