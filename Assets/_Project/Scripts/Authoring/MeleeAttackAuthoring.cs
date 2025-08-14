using Unity.Entities;
using UnityEngine;

class MeleeAttackAuthoring : MonoBehaviour
{
    public float delay = 0.5f;
    public int damageAmount = 5;
    public float attackColliderSize = 0.8f;

    class Baker : Baker<MeleeAttackAuthoring>
    {
        public override void Bake(MeleeAttackAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAttack()
            {
                timerMax = authoring.delay,
                damageAmount = authoring.damageAmount,
                attackColliderSize = authoring.attackColliderSize,
            });
        }
    }
}

public struct MeleeAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damageAmount;
    public float attackColliderSize;
}