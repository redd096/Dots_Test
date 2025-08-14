using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class MoveOverrideAuthoring : MonoBehaviour
{
    class Baker : Baker<MoveOverrideAuthoring>
    {
        public override void Bake(MoveOverrideAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveOverride());

            //set not enabled by default
            SetComponentEnabled<MoveOverride>(entity, false);
        }
    }
}

public struct MoveOverride : IComponentData, IEnableableComponent
{
    public float3 targetPosition;
}