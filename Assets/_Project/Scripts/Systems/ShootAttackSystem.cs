using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var shootAttack, var target, var unitMover)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>, RefRW<UnitMover>>())
        {
            //be sure there is a target
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;

            //if target is still too far, move to it
            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            if (math.distance(localTransform.ValueRO.Position, targetTransform.Position) > shootAttack.ValueRO.attackDistance)
            {
                unitMover.ValueRW.targetPosition = targetTransform.Position;
                continue;
            }

            //else it's close enough, so stop move
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

            //look to the target (we stopped moving, so UnitMover isn't updating our rotation, we have to do it here)
            float3 aimDirection = targetTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);
            quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            //and start attack

            //wait timer (rate of fire)
            if (shootAttack.ValueRO.timer > SystemAPI.Time.ElapsedTime)
                continue;
            shootAttack.ValueRW.timer = SystemAPI.Time.ElapsedTime + shootAttack.ValueRO.timerMax;

            //instantiate bullet at position
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefab);
            float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            //and initialize (damage and target)
            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}
