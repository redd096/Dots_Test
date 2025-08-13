using Unity.Entities;
using UnityEngine;

class ShootLightAuthoring : MonoBehaviour
{
    public float timer = 0.05f;

    class ShootLightAuthoringBaker : Baker<ShootLightAuthoring>
    {
        public override void Bake(ShootLightAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootLight()
            {
                timer = authoring.timer,
            });
        }
    }
}

public struct ShootLight : IComponentData
{
    public float timer;
}
