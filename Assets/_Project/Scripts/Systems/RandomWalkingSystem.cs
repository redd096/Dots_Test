using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var randomWalking, var unitMover, var localTransform)
            in SystemAPI.Query<RefRW<RandomWalking>, RefRW<UnitMover>, RefRO<LocalTransform>>())
        {
            //reached random target position, so create new one
            if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition)
                <= unitMover.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime + 0.1f)
            {
                Random random = randomWalking.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                randomDirection = math.normalize(randomDirection);

                float randomDistance = random.NextFloat(randomWalking.ValueRO.minDistance, randomWalking.ValueRO.maxDistance);

                //origin + random Direction * random Distance
                randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection * randomDistance;

                //update random struct, so Next Float will be a new number instead of restart the series
                randomWalking.ValueRW.random = random;
            }
            //else, move to it
            else
            {
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
            }
        }
    }
}
