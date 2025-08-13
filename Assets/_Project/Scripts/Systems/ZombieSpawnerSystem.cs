using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((var localTransform, var zombieSpawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ZombieSpawner>>())
        {
            //wait timer (rate of fire)
            if (zombieSpawner.ValueRO.timer > SystemAPI.Time.ElapsedTime)
                continue;
            zombieSpawner.ValueRW.timer = SystemAPI.Time.ElapsedTime + zombieSpawner.ValueRO.timerMax;

            //instantiate zombie
            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefab);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            ecb.AddComponent(zombieEntity, new RandomWalking()
            {
                originPosition = localTransform.ValueRO.Position,
                targetPosition = localTransform.ValueRO.Position,                   //reached position to generate a new random one
                minDistance = zombieSpawner.ValueRO.randomWalkingDistanceMin,
                maxDistance = zombieSpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)zombieEntity.Index),    //randomize seed with entity index
            });
        }
    }
}
