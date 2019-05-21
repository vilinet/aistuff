using System.Collections.Generic;

namespace Lib.Evolution
{
    public interface IGenotype
    {
        void CrossOver(IReadOnlyList<IGenotype> parents, float swapChance);

        IGenotype Mutation(float mutationProb, float mutationAmount);

        double GetFitness();

        void RandomizeParameters();

        IGenotype Clone();

        void SetParameters(object weights);
    }
}
