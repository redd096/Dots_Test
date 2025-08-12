using Unity.Burst;
using Unity.Entities;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((var shootAttack, var target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
        {
            //be sure there is a target
            if (target.ValueRO.targetEntity == Entity.Null)
                continue;

            //wait timer
            if (shootAttack.ValueRO.timer > SystemAPI.Time.ElapsedTime)
                continue;
            shootAttack.ValueRW.timer = SystemAPI.Time.ElapsedTime + shootAttack.ValueRO.timerMax;

            RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
            targetHealth.ValueRW.healthAmount -= 1;
        }
    }
}
