using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //if target entity doesn't exists, set it to null
        foreach (var target in SystemAPI.Query<RefRW<Target>>())
        {
            if (SystemAPI.Exists(target.ValueRO.targetEntity) == false)
                target.ValueRW.targetEntity = Entity.Null;
        }
    }
}
