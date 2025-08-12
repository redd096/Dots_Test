using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
    class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Target());
        }
    }
}

public struct Target : IComponentData
{
    public Entity targetEntity;
}
