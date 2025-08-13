using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class ShootVictimAuthoring : MonoBehaviour
{
    public Transform hitPositionTransform;

    class Baker : Baker<ShootVictimAuthoring>
    {
        public override void Bake(ShootVictimAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootVictim()
            {
                hitLocalPosition = authoring.hitPositionTransform.localPosition,
            });
        }
    }
}

public struct ShootVictim : IComponentData
{
    public float3 hitLocalPosition;
}