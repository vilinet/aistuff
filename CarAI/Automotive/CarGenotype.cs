using Lib;
using Lib.Evolution;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarAI.AutoMotive
{
    public class CarGenotype : IGenotype
    {
        public static int PARAMETER_COUNT = 34;

        public double[] Parameters { get; private set; } = new double[PARAMETER_COUNT];

        public int CheckpointsPassed { get;  set; }
        public uint Checkpoint { get; internal set; }

        public CarGenotype() {}

        public CarGenotype(CarGenotype carGenotype)
        {
            carGenotype.Parameters.CopyTo(Parameters, 0);
        }

        public void CrossOver(IReadOnlyList<IGenotype> parents, float swapChance)
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = (parents[(int)Randomizer.NextDouble(parents.Count-1)] as CarGenotype).Parameters[i];
            }
        }

        public double GetFitness()
        {
            return CheckpointsPassed + Checkpoint*2;
        }

        public IGenotype Mutation(float mutationProb, float mutationAmount)
        {
            var newGenotype = new CarGenotype(this);

            for (int i = 0; i < Parameters.Length; i++)
            {
                if(Randomizer.NextDouble() <= mutationProb)
                {
                    newGenotype.Parameters[i] += Randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount;
                    if (newGenotype.Parameters[i] > 1) newGenotype.Parameters[i] = 1;
                    else if (newGenotype.Parameters[i] < -1) newGenotype.Parameters[i] = -1;
                }
            }
            
            return newGenotype;
        }

        public void RandomizeParameters()
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = Randomizer.NextDouble()* 2 - 1;
            }
        }

        public IGenotype Clone()
        {
            return new CarGenotype(this);
        }

        public override string ToString()
        {
            return GetFitness().ToString();
        }

        public void SetParameters(object weights)
        {
            if (weights != null)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    Parameters[i] = ((double[])weights)[i];
                }
            }
        }
    }
}
