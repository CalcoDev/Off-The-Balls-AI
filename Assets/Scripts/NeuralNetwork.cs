using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private Neuron[] hiddenLayer1;
    private Neuron[] hiddenLayer2;
    private Neuron outputNeuron1; // x Force
    private Neuron outputNeuron2; // y Force

    public float fitness = 0f;

    public NeuralNetwork(int nInputs, int nHidden1, int nHidden2, Func<float, float> hiddenActivation,
        Func<float, float> outputActivation)
    {
        hiddenLayer1 = new Neuron[nHidden1];
        for (var i = 0; i < hiddenLayer1.Length; i++)
        {
            hiddenLayer1[i] = new Neuron(nInputs, hiddenActivation);
        }
        
        hiddenLayer2 = new Neuron[nHidden2];
        for (var i = 0; i < hiddenLayer2.Length; i++)
        {
            hiddenLayer2[i] = new Neuron(nHidden1, hiddenActivation);
        }

        //todo bias?
        outputNeuron1 = new Neuron(nHidden2, outputActivation);
        outputNeuron2 = new Neuron(nHidden2, outputActivation);
    }

    public void Mutate(float chance, float amount)
    {
        foreach (var neuron in hiddenLayer1)
        {
            neuron.Mutate(chance, amount);
        }
        
        foreach (var neuron in hiddenLayer2)
        {
            neuron.Mutate(chance, amount);
        }

        outputNeuron1.Mutate(chance, amount);
        outputNeuron2.Mutate(chance, amount);
    }

    public float[] Predict(float[] inputs)
    {
        float[] predsL1 = CalculatePredictionsL1(inputs);
        float[] predsL2 = CalculatePredictionsL2(predsL1);
        return new [] {outputNeuron1.Predict(predsL2), outputNeuron2.Predict(predsL2)};
    }

    // public void Train(float[] inputs, float target)
    // {
    //     float[] predsL1 = CalculatePredictionsL1(inputs);
    //     float globalError = CalculateGlobalError(inputs, target);
    //
    //     float[] errorsL1 = new float[predsL1.Length];
    //     for (var i = 0; i < predsL1.Length; i++)
    //     {
    //         errorsL1[i] = MathC.RELUDerivative(predsL1[i]) * globalError * outputNeuron.weights[i];
    //     }
    //
    //     outputNeuron.AdjustWeights(predsL1, globalError);
    //     for (var i = 0; i < hiddenLayer1.Length; i++)
    //     {
    //         hiddenLayer1[i].AdjustWeights(inputs, errorsL1[i]);
    //     }
    // }

    // public float CalculateGlobalError(float[] inputs, float target)
    // {
    //     float pred = Predict(inputs);
    //     float error = MathC.TanhDerivative(pred) * (target - pred);
    //
    //     return error;
    // }

    public float[] CalculatePredictionsL1(float[] inputs)
    {
        // Bias:
        float[] preds = new float[hiddenLayer1.Length + 1];
        for (var i = 0; i < hiddenLayer1.Length; i++)
        {
            preds[i] = hiddenLayer1[i].Predict(inputs);
        }

        preds[preds.Length - 1] = 1f;
        return preds;
    }

    public float[] CalculatePredictionsL2(float[] p1)
    {
        float[] preds = new float[hiddenLayer2.Length + 1];
        for (var i = 0; i < hiddenLayer2.Length; i++)
        {
            preds[i] = hiddenLayer2[i].Predict(p1);
        }

        preds[preds.Length - 1] = 1f;
        return preds;
    }

    public NeuralNetwork Copy(NeuralNetwork nn)
    {
        for (var i = 0; i < hiddenLayer1.Length; i++)
        {
            nn.hiddenLayer1[i] = hiddenLayer1[i];
        }
        
        for (var i = 0; i < hiddenLayer2.Length; i++)
        {
            nn.hiddenLayer2[i] = hiddenLayer2[i];
        }

        nn.outputNeuron1 = outputNeuron1;
        nn.outputNeuron2 = outputNeuron2;
        // nn.fitness = fitness;

        return nn;
    }
    
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        
        if (fitness < other.fitness)
            return -1;
        
        return 0;
    }

    public void SaveToFile()
    {
        string h1 = "";
        foreach (var neuron in hiddenLayer1)
        {
            h1 += string.Join(" ", neuron.weights) + "\n";
        }
        File.WriteAllText("./Assets/Neural Network/h1", h1);
        
        string h2 = "";
        foreach (var neuron in hiddenLayer2)
        {
            h2 += string.Join(" ", neuron.weights) + "\n";
        }
        File.WriteAllText("./Assets/Neural Network/h2", h2);
        
        File.WriteAllText("./Assets/Neural Network/output1", string.Join(" ", outputNeuron1.weights));
        File.WriteAllText("./Assets/Neural Network/output2", string.Join(" ", outputNeuron2.weights));
    }

    public void LoadFromFile()
    {
        string[] wH1 = File.ReadAllLines("./Assets/Neural Network/h1");
        string[] wH2 = File.ReadAllLines("./Assets/Neural Network/h2");
        string[] wO1 = File.ReadAllLines("./Assets/Neural Network/output1");
        string[] wO2 = File.ReadAllLines("./Assets/Neural Network/output2");
        
        for (var i = 0; i < wH1.Length; i++)
        {
            string[] ws = wH1[i].Split(' ');
            for (var j = 0; j < ws.Length; j++)
            {
                hiddenLayer1[i].weights[j] = float.Parse(ws[j]);
            }
        }
        
        for (var i = 0; i < wH2.Length; i++)
        {
            string[] ws = wH2[i].Split(' ');
            for (var j = 0; j < ws.Length; j++)
            {
                hiddenLayer2[i].weights[j] = float.Parse(ws[j]);
            }
        }
        
        string[] wsO1 = wO1[0].Split(' ');
        for (var j = 0; j < wsO1.Length; j++)
        {
            outputNeuron1.weights[j] = float.Parse(wsO1[j]);
        }
        
        string[] wsO2 = wO2[0].Split(' ');
        for (var j = 0; j < wsO2.Length; j++)
        {
            outputNeuron2.weights[j] = float.Parse(wsO2[j]);
        }
    }
}