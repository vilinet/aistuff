using System.Collections.Generic;
using System.Linq;

namespace Lib.Evolution
{
    public class Evolution<Genotype> where Genotype : IGenotype, new()
    {
        public IEnumerable<IGenotype> Population { get; set; }

        public int PopulationSize { get; set; }
        public int Generation { get; protected set; } = 1;

        public float CrossSwapProbability { get; set; } = 0.6f;
        public float IndividualMutationProbability { get; set; } = 0.20f;
        public float GenotypeMutationProbability { get; set; } = 0.20f;
        public float WeakestPercentage { get; set; } = 0.3f;
        public float MaxMutationAmount { get; set; } = 0.3f;
        public float ThreeParentsProbability { get; set; } = 0.75f;

        public int Elits { get; set; } = 10;

        public float ElitRequirementTop { get; set; } = 0.1f;

        public Evolution(int populationSize, double[] weights = null, int? seed = null)
        {
            if (seed.HasValue)
            {
                Randomizer.InitSeed(seed.Value);
            }

            PopulationSize = populationSize;

            InitPopulation(weights);
        }

        public void InitPopulation(object weights)
        {
            Generation = 1;

            var list = new List<IGenotype>();

            for (int i = 0; i < PopulationSize; i++)
            {
                var genom = new Genotype();
                genom.SetParameters(weights);
                genom.RandomizeParameters();
                list.Add(genom);
            }

            Population = list.AsEnumerable();
        }

        public IGenotype Evolve()
        {
            var filtered = Population.OrderByDescending(x => x.GetFitness()).Take(Population.Count() - (int)(Population.Count() * WeakestPercentage)).ToList();
            var best = filtered.First();
            var newPopulation = new List<IGenotype>();

            for (int i = 0; i < Elits && i < filtered.Count; i++)
            {
                var cs = filtered.ToList().Take(filtered.Count).ToList();
                cs.Remove(filtered[i]);

                var parents = new List<IGenotype>();
                parents.Add(filtered[i]);

                int parentCount = cs.Count >= 2 && Randomizer.NextDouble() <= ThreeParentsProbability ? 3 : 2;

                for (int p = 0; p < parentCount; p++)
                {
                    parents.Add(cs[(int)(Randomizer.NextDouble() * cs.Count - 1)]);
                    cs.Remove(parents.Last());
                }
                var geno = new Genotype();
                geno.CrossOver(parents, CrossSwapProbability);
                newPopulation.Add(geno);

                if (Randomizer.NextDouble() <= IndividualMutationProbability)
                {
                    geno.Mutation(GenotypeMutationProbability / 2, MaxMutationAmount / 2);
                }
            }

            while (newPopulation.Count < PopulationSize)
            {
                for (int i = 0; i < filtered.Count; i++)
                {
                    if (newPopulation.Count >= PopulationSize) break;
                    var p = Randomizer.NextDouble();
                    if (p <= IndividualMutationProbability)
                    {
                        newPopulation.Add(filtered[i].Mutation(GenotypeMutationProbability, MaxMutationAmount));
                    }
                    else if (p >= 0.95) //chance to not mutate
                    {
                        newPopulation.Add(filtered[i].Clone());
                    }
                    else
                    {
                        var cs = filtered.ToList();
                        cs.Remove(filtered[i]);
                        var parents = new List<IGenotype>();
                        int parentCount = Randomizer.NextDouble() <= ThreeParentsProbability ? 3 : 2;

                        for (int pp = 0; pp < parentCount; pp++)
                        {
                            parents.Add(cs[(int)(Randomizer.NextDouble() * (cs.Count - 1))]);
                            cs.Remove(parents.Last());
                        }
                        var geno = new Genotype();
                        geno.CrossOver(parents, CrossSwapProbability);
                        newPopulation.Add(geno);

                        if (Randomizer.NextDouble() <= IndividualMutationProbability / 3)
                        {
                            geno.Mutation(GenotypeMutationProbability / 3, MaxMutationAmount / 3);
                        }
                    }
                }
            }

            Population = newPopulation.AsEnumerable();
            Generation++;
            return best;
        }
    }
}
