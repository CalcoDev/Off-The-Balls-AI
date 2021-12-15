using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour
{
    [Header("Locating")]
    public float locateArea;
    public LayerMask targetLayer;

    public NeuralNetwork net;
    private Rigidbody2D rb;

    public float fitness = 0f;

    private bool canMove = true;

    private float[] preds = {0f, 0f};
    private Vector2 startPos = Vector2.zero;
    private Vector2 actualTargetPos = Vector2.zero;
    
    private Vector2 targetForce = Vector2.zero;
    private Vector2 predictedForce = Vector2.zero;

    private float modifier = 1f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void UpdateFitness()
    {
        net.fitness = fitness;
    }
    
    private void Update()
    {
        Debug.DrawLine(startPos, startPos + targetForce, Color.yellow);
        Debug.DrawLine(startPos, startPos + predictedForce, Color.red);
        Debug.DrawLine(startPos, startPos + Vector2.right, Color.white);
        
        // Find nearest target:
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, locateArea, targetLayer);
        Collider2D closest = null;
        float d = locateArea + 1f;
        foreach (var target in targets)
        {
            float cD = Vector2.Distance(transform.position, target.transform.position);
            if (cD < d)
            {
                d = cD;
                closest = target;
            }
        }

        if (closest != null)
        {
            if (canMove)
            {
                // Get target force.
                // Calculate angle based on force and Vector.right
                // Predict angle
                // Get predicted force using some unity function
                
                startPos = transform.position;
                actualTargetPos = closest.transform.position;
                targetForce = (actualTargetPos - startPos).normalized;

                float targetAngle = Vector2.SignedAngle(Vector2.right, targetForce);
                if (targetAngle < 0f)
                    targetAngle = 360 + targetAngle;
                
                float cosTa = Mathf.Cos(targetAngle * Mathf.Deg2Rad);
                float sinTa = Mathf.Sin(targetAngle * Mathf.Deg2Rad);

                float[] inputs = {sinTa, cosTa, 1f};
                preds = net.Predict(inputs);

                float predictedAngle = Mathf.Atan2(preds[0], preds[1]) * Mathf.Rad2Deg;
                predictedForce = new Vector2(cosTa, sinTa);
                
                fitness += Mathf.Cos(predictedAngle * Mathf.Deg2Rad);
                // print($"Target angle: {targetAngle} | Predicted angle: {predictedAngle}");
                
                rb.velocity = Vector2.zero;
                rb.AddForce(predictedForce * 20f, ForceMode2D.Impulse);
                canMove = false;
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            // net.fitness += 10;
            Destroy(col.gameObject);
            
            // rb.AddForce(Vector2.up * 1.25f);
            // rb.AddForce(Vector2.right * -rb.velocity * .5f);

            canMove = true;
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            transform.position = new Vector3(Random.Range(-400, 400), Random.Range(-400, 400), 0f);
            rb.velocity = Vector2.zero;
            fitness -= .25f;
            canMove = true;
        }
    }
}