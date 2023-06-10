using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour {
    // Spawning parameters
    public Boid boidBlueprint;
    public float spawnRadius = 10f;
    public int spawnCount = 10;

    // Boids computing parameters
    [SerializeField] BoidConfig boidConfig;
    [SerializeField] ComputeShader computeShader;
    Boid[] boids;
    const int ThreadGroupSize = 1024;

    void Awake() {
        SpawnBoids();
    }

    void Start() {
        boids = FindObjectsOfType<Boid> ();
        foreach (var boid in boids) {
            boid.Initialize (boidConfig, null);
        }
    }

    void Update() {
        if (boids == null) return;
        ComputeBoids();
    }

    void SpawnBoids() {
        for (var i = 0; i < spawnCount; i++) {
            var position = transform.position + Random.insideUnitSphere * spawnRadius;
            var boid = Instantiate (boidBlueprint);
            boid.transform.position = position;
            boid.transform.forward = Random.insideUnitSphere;
        }
    }

    void ComputeBoids() {
        var boidsCount = boids.Length;
        var boidParameters = new BoidParameters[boidsCount];

        for (var i = 0; i < boids.Length; i++) {
            boidParameters[i].position = boids[i].position;
            boidParameters[i].direction = boids[i].forward;
        }

        var boidBuffer = new ComputeBuffer(boidsCount, sizeof(float) * 3 * 5 + sizeof(int));
        boidBuffer.SetData(boidParameters);

        computeShader.SetBuffer(0, "boids", boidBuffer);
        computeShader.SetInt("numBoids", boids.Length);
        computeShader.SetFloat("viewRadius", boidConfig.perceptionRange);
        computeShader.SetFloat("avoidRadius", boidConfig.avoidanceRange);
        var threadGroups = Mathf.CeilToInt(boidsCount / (float) ThreadGroupSize);
        computeShader.Dispatch(0, threadGroups, 1, 1);

        boidBuffer.GetData(boidParameters);

        for (var i = 0; i < boids.Length; i++) {
            boids[i].flockAim = boidParameters[i].flockAim;
            boids[i].flockCenter = boidParameters[i].flockCenter;
            boids[i].avoidanceAim = boidParameters[i].avoidanceAim;
            boids[i].neighbours = boidParameters[i].neighbours;

            boids[i].UpdateBoid();
        }

        boidBuffer.Release();
    }

    struct BoidParameters {
        public Vector3 position;
        public Vector3 direction;
        public Vector3 flockAim;
        public Vector3 flockCenter;
        public Vector3 avoidanceAim;
        public int neighbours;
    }
}
