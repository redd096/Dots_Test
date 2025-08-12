using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((var localTransform, var shootAttack, var target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>>())
        {
            //be sure there is a target
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;

            //wait timer
            if (shootAttack.ValueRO.timer > SystemAPI.Time.ElapsedTime)
                continue;
            shootAttack.ValueRW.timer = SystemAPI.Time.ElapsedTime + shootAttack.ValueRO.timerMax;

            //instantiate bullet at position
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefab);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            //and initialize
            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}
