using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var localTransform, var moveOverride, var moveOverrideEnabled, var unitMover)
            in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MoveOverride>, EnabledRefRW<MoveOverride>, RefRW<UnitMover>>())
        {
            //move to override position
            if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > unitMover.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime + 0.1f)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            //if reached, disable override
            else
            {
                moveOverrideEnabled.ValueRW = false;
                // SystemAPI.SetComponentEnabled<MoveOverride>(entity, false);
            }
        }
    }
}
