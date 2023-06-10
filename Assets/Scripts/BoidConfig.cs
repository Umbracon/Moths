using UnityEngine;

[CreateAssetMenu]
public class BoidConfig : ScriptableObject{
    [Header("Basic Parameters")]
    // Single boid characteristics configuration
    public float minSpeed = 2.0f;
    public float maxSpeed = 5.0f;
    
    public float perceptionRange = 2.5f;
    public float avoidanceRange = 1.0f;
    public float maxSteeringForce = 3.0f;

    public float separationWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;

    public float targetWeight = 1.0f;
    
    [Header("Collision")]
    // Collision parameters of a single boid
    public LayerMask collisionMask;
    public float boundsRadius = 0.27f;
    public float collisionAvoidanceWeight = 10.0f;
    public float collisionAvoidanceDistance = 5.0f;
}