using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //create ecb to destroy entities
        //there are two types of ecb: on startSimulation and on endSimulation. We need the last one to destroy entities at the end of the frame, after every simulation
        //NB On GameObject add LinkedEntityGroupAuthoring or this will destroy the logic but not the mesh in scene of the object (on spawned objects should have been added automatically)
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        //use WithEntityAccess to have also Entity as last parameter
        foreach ((RefRO<Health> health, Entity entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
        {
            //find every entity to destroy
            if (health.ValueRO.healthAmount <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
