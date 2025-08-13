using Unity.Entities;
using UnityEngine;

class ZombieSpawnerAuthoring : MonoBehaviour
{
    public double delay = 1.5f;

    class ZombieSpawnerAuthoringBaker : Baker<ZombieSpawnerAuthoring>
    {
        public override void Bake(ZombieSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawner()
            {
                timerMax = authoring.delay,
            }); 
        }
    }
}

public struct ZombieSpawner : IComponentData
{
    public double timer;
    public double timerMax;
}