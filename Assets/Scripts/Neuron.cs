using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Neuron
{
    public float[] weights { get; set; }

    private Func<float, float> activation;

    public Neuron(int nInputs, Func<float, float> activation)
    {
        this.activation = activation;

        weights = new float[nInputs];
        for (var i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(-.3f, .3f);
        }
    }
    
    public float Predict(float[] inputs)
    {
        float wSum = 0f;
        for (var i = 0; i < weights.Length; i++)
        {
            wSum += inputs[i] * weights[i];
        }

        return activation(wSum);
    }

    public void Mutate(float chance, float amount)
    {
        for (var i = 0; i < weights.Length; i++)
        {
            if (Random.Range(0f, chance) <= 5)
            {
                weights[i] += Random.Range(-amount, amount);
            }
        }
    }

    public void AdjustWeights(float[] inputs, float error)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] += error * inputs[i];
        }
    }
}
