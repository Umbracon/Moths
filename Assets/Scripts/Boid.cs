using Unity.Collections;
using UnityEngine;

public class Boid : MonoBehaviour {
    BoidConfig config;

    [ReadOnly] public Vector3 position;
    [ReadOnly] public Vector3 forward;
    [ReadOnly] public Vector3 flockAim;
    [ReadOnly] public Vector3 avoidanceAim;
    [ReadOnly] public Vector3 flockCenter;
    [ReadOnly] public int neighbours;

    Vector3 velocity;
    Vector3 acceleration;
    
    Transform cachedTransform;
    Transform target;
    
    void Awake () {
        cachedTransform = transform;
    }

    public void Initialize(BoidConfig config, Transform target) {
        this.config = config;
        this.target = target;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        var speedRange = config.minSpeed + config.maxSpeed;
        var initialSpeed = speedRange / 2;
        velocity = transform.forward * initialSpeed;
    }

    public void UpdateBoid() {
        Vector3 acceleration = Vector3.zero;

        if (target != null) {
            acceleration = HeadTo(target.position - position) * config.targetWeight;
        }

        if (neighbours != 0) {
            flockCenter /= neighbours;

            var separation = HeadTo(avoidanceAim) * config.separationWeight;
            var alignment = HeadTo(flockAim) * config.alignmentWeight;
            var cohesion = HeadTo(flockCenter - position) * config.cohesionWeight;

            acceleration += alignment;
            acceleration += separation;
            acceleration += cohesion;

        }

        if (IsGoingToCollide()) {
            var collisionAvoidanceDirection = CollisionRaycast();
            var collisionAvoidanceForce = HeadTo(collisionAvoidanceDirection) * config.collisionAvoidanceWeight;
            acceleration += collisionAvoidanceForce;
        }

        velocity += acceleration * Time.deltaTime;
        var speed = velocity.magnitude;
        var direction = velocity / speed;
        speed = Mathf.Clamp(speed, config.minSpeed, config.maxSpeed);
        velocity = direction * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = direction;
        position = cachedTransform.position;
        forward = direction;
    }

    Vector3 HeadTo(Vector3 aim) {
        return Vector3.ClampMagnitude(aim.normalized * config.maxSpeed - velocity, config.maxSteeringForce);
    }

    bool IsGoingToCollide() {
        RaycastHit hit;
        return Physics.SphereCast(
            position,
            config.boundsRadius,
            forward,
            out hit,
            config.collisionAvoidanceDistance,
            config.collisionMask);
    }

    Vector3 CollisionRaycast() {
        var rayDirections = Utilities.directions;

        foreach (var t in rayDirections) {
            var direction = cachedTransform.TransformDirection(t);
            var ray = new Ray(position, direction);
            if (!Physics.SphereCast(ray, config.boundsRadius, config.collisionAvoidanceDistance, config.collisionMask)) {
                return direction;
            }
        }
        return forward;
    }
}