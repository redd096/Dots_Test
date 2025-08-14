using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<RaycastHit> hits = new NativeList<RaycastHit>(Allocator.Temp);

        foreach ((var localTransform, var meleeAttack, var target, var unitMover)
            in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>, RefRW<UnitMover>>().WithDisabled<MoveOverride>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            //check distance to target
            bool isCloseEnough = math.distancesq(localTransform.ValueRO.Position, targetTransform.Position) < 2f;
            if (isCloseEnough == false)
            {
                //or if it's touching it
                float3 directionToTarget = targetTransform.Position - localTransform.ValueRO.Position;
                directionToTarget = math.normalize(directionToTarget);
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + directionToTarget * meleeAttack.ValueRO.attackColliderSize,
                    Filter = CollisionFilter.Default,
                };

                hits.Clear();
                if (collisionWorld.CastRay(raycastInput, ref hits))
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.Entity == target.ValueRO.targetEntity)
                        {
                            isCloseEnough = true;
                            break;
                        }
                    }
                }
            }

            //if target too far, move to it
            if (isCloseEnough == false)
            {
                unitMover.ValueRW.targetPosition = targetTransform.Position;
            }
            //if near, attack
            else
            {
                //stop movement
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                //wait timer
                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRO.timer > 0f)
                    continue;
                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;

                //do damage
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;
            }
        }
    }
}
