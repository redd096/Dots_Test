using Unity.Entities;
using UnityEngine;

class EnemyAuthoring : MonoBehaviour
{
    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Enemy());
        }
    }
}

public struct Enemy : IComponentData
{
}
