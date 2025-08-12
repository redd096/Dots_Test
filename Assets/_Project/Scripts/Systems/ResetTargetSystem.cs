using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//work in late update and it's the first to run
[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //if target entity doesn't exists, set it to null
        foreach (var target in SystemAPI.Query<RefRW<Target>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;
                
            if (SystemAPI.Exists(target.ValueRO.targetEntity) == false || SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity) == false)
                target.ValueRW.targetEntity = Entity.Null;
        }
    }
}
