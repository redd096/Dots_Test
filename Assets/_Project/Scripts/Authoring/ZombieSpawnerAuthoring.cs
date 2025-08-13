using Unity.Entities;
using UnityEngine;

class ZombieSpawnerAuthoring : MonoBehaviour
{
    public double delay = 1.5f;
    public float randomWalkingDistanceMin = 3f;
    public float randomWalkingDistanceMax = 10f;

    class ZombieSpawnerAuthoringBaker : Baker<ZombieSpawnerAuthoring>
    {
        public override void Bake(ZombieSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawner()
            {
                timerMax = authoring.delay,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
            }); 
        }
    }
}

public struct ZombieSpawner : IComponentData
{
    public double timer;
    public double timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
}