using Unity.Entities;
using UnityEngine;

class FriendlyAuthoring : MonoBehaviour
{
    class Baker : Baker<FriendlyAuthoring>
    {
        public override void Bake(FriendlyAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Friednly());
        }
    }
}

public struct Friednly : IComponentData
{
}
