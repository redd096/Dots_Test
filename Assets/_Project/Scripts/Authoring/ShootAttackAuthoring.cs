using Unity.Entities;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public double timerMax = 0.2;
    
    class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack()
            {
                timerMax = authoring.timerMax,
            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    public double timer;
    public double timerMax;
}