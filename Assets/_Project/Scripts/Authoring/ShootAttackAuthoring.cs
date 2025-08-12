using Unity.Entities;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public double rateOfFire = 0.3;
    public int damageAmount = 5;
    
    class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack()
            {
                timerMax = authoring.rateOfFire,
                damageAmount = authoring.damageAmount,
            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    public double timer;
    public double timerMax;
    public int damageAmount;
}