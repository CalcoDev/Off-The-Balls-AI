using System;
using System.Linq;

public class MathC
{
    public static float Sigmoid(float n)
    {
        return 1f / (1f + (float)Math.Exp(-n));
    }

    public static float Linear(float n)
    {
        return n;
    }

    public static float LinearDerivative(float n)
    {
        return 1;
    }

    public static float SigmoidDerivative(float n)
    {
        return n * (1 - n);
    }

    public static float Tanh(float n)
    {
        // return 2f / (1f + (float)Math.Exp(-2 * n)) - 1f;
        return (float)Math.Tanh(n);
    }

    public static float TanhDerivative(float n)
    {
        return 1 - n * n;
    }
        
    public static float RELU(float n)
    {
        if (n <= 0)
            return n * 0.01f;
            
        return n;
    }

    public static float RELUDerivative(float n)
    {
        return n <= 0 ? 0.01f : 1f;
    }
    
    public static float[] Normalise(float[] inputs)
    {
        float min = inputs.Min();
        float max = inputs.Max();

        float n = max - min;
            
        for (var i = 0; i < inputs.Length; i++)
        {
            inputs[i] = (inputs[i] - min) / n;
        }

        return inputs;
    }
}