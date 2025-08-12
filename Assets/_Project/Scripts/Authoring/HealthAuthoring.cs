using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public int healthAmount = 100;

    class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health()
            {
                healthAmount = authoring.healthAmount,
            });
        }
    }
}

public struct Health : IComponentData
{
    public int healthAmount;
}