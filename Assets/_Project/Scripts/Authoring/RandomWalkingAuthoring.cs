using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This authoring is used only for tests.
/// Enemy Units don't have this script attached, it's attached by ZombieSpawnerSystem with Random generated with unique seed
/// </summary>
class RandomWalkingAuthoring : MonoBehaviour
{
    public float3 targetPosition;
    public float3 originPosition;
    public float minDistance = 3;
    public float maxDistance = 10;

    class Baker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomWalking()
            {
                targetPosition = authoring.targetPosition,
                originPosition = authoring.originPosition,
                minDistance = authoring.minDistance,
                maxDistance = authoring.maxDistance,
                random = new Unity.Mathematics.Random(1),
            });
        }
    }
}

public struct RandomWalking : IComponentData
{
    public float3 targetPosition;
    public float3 originPosition;
    public float minDistance;
    public float maxDistance;
    //can't use classes in Burst (UnityEngine.Random) so we use mathematics Random struct
    //but mathematicsRandom works with seed, then generate a random series with that seed.
    //We store it as a variable, to continue the series instead of restart it from zero anytime
    public Unity.Mathematics.Random random;
}
