using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((var localTransform, var bullet, var target, var entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>, RefRO<Target>>().WithEntityAccess())
        {
            //be sure it has a target
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            //get target position
            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
            float3 targetPosition = targetTransform.TransformPoint(targetShootVictim.hitLocalPosition);

            //move bullet to target
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            float distanceBeforeMove = math.distancesq(localTransform.ValueRO.Position, targetPosition);
            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            //be sure doesn't exceed target position
            float distanceAfterMove = math.distancesq(localTransform.ValueRO.Position, targetPosition);
            if (distanceAfterMove > distanceBeforeMove)
                localTransform.ValueRW.Position = targetPosition;

            //when reach it, damage it and destroy bullet
            float destroyDistanceSq = 0.2f;
            if (distanceAfterMove < destroyDistanceSq)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;

                ecb.DestroyEntity(entity);
            }
        }
    }
}
