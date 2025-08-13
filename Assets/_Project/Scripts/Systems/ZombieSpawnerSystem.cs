using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var zombieSpawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ZombieSpawner>>())
        {
            //wait timer (rate of fire)
            if (zombieSpawner.ValueRO.timer > SystemAPI.Time.ElapsedTime)
                continue;
            zombieSpawner.ValueRW.timer = SystemAPI.Time.ElapsedTime + zombieSpawner.ValueRO.timerMax;

            //instantiate zombie
            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefab);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
        }
    }
}
