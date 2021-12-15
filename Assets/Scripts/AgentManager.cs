using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class AgentManager : MonoBehaviour
{
    [Header("Network")] 
    [Range(1f, 100f)] public float SimulationSpeed = 1f;
    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;
    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    public bool loadFromFile = true;
    
    public float timeBetweenGenerations = 15f;
    public float agentSpawnRadius;
    public float generationSize = 20f;
    public GameObject AgentPrefab;

    private List<NeuralNetwork> networks;
    private List<Agent> agents;

    [Header("World")]
    public GameObject enemyPrefab;
    public Transform cam;
    public float enemyNumber;
    public float enemySpawnRadius;

    [Header("Stats")] 
    public float bestFitness;
    public int genNr = 0;
    
    private void Start()
    {
        InitNetworks();

        InvokeRepeating(nameof(NewGeneration), 1f, timeBetweenGenerations);
    }

    private void Update()
    {
        if (agents[0])
            cam.position = agents[0].transform.position;
        
        if (Math.Abs(Time.timeScale - SimulationSpeed) > 0.05f)
        {
            Time.timeScale = SimulationSpeed;
            Time.fixedDeltaTime = SimulationSpeed * 0.02f;
        }

        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            print("saved");
            SortNetworks();
            networks[networks.Count - 1].SaveToFile();
        }
    }

    private void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        for (var i = 0; i < generationSize; i++)
        {
            networks.Add(new NeuralNetwork(2 + 1, 64 + 1, 32 + 1,MathC.Sigmoid, MathC.Linear));
        }

        if (!loadFromFile) return;
        foreach (var neuralNetwork in networks)
        {
            neuralNetwork.LoadFromFile();
        }
    }

    private void NewGeneration()
    {
        genNr += 1;
        if (agents != null)
        {
            foreach (var t in agents)
            {
                Destroy(t.gameObject);
            }

            SortNetworks();
        }

        foreach (var target in FindObjectsOfType<Target>())
        {
            Destroy(target.gameObject);
        }
        for (int i = 0; i < enemyNumber; i++)
        {
            Instantiate(enemyPrefab, transform.position + new Vector3(Random.Range(-enemySpawnRadius, enemySpawnRadius), Random.Range(-enemySpawnRadius, enemySpawnRadius), 0f), Quaternion.identity);
        }
        
        agents = new List<Agent>();
        for (int i = 0; i < generationSize; i++)
        {
            Agent agent = (Instantiate(AgentPrefab, transform.position + new Vector3(Random.Range(-agentSpawnRadius, agentSpawnRadius), Random.Range(-agentSpawnRadius, agentSpawnRadius), 0f), Quaternion.identity)).GetComponent<Agent>();
            agent.net = networks[i];
            agents.Add(agent);
        }
    }

    private void SortNetworks()
    {
        for (int i = 0; i < generationSize; i++)
        {
            agents[i].UpdateFitness();
            networks[i].fitness = agents[i].net.fitness;
        }
        
        networks.Sort();
        bestFitness = networks[networks.Count - 1].fitness;
        int halfGen = (int)(generationSize * 0.5f);
        for (int i = 0; i < halfGen; i++)
        {
            networks[i] = networks[i + halfGen].Copy(new NeuralNetwork(2 + 1, 64 + 1, 32 + 1,MathC.Sigmoid, MathC.Linear));
            networks[i].Mutate(1/MutationChance, MutationStrength);
        }

        // for (int i = 0; i < generationSize; i++)
        // {
        //     networks[i] = networks[networks.Count - 1].Copy(new NeuralNetwork(4 + 1, 32 + 1, 16 + 1,MathC.RELU, MathC.Tanh));
        //     networks[i].Mutate((int)(1/MutationChance), MutationStrength);
        // }
    }
}