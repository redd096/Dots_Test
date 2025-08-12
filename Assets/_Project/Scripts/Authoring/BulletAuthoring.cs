using Unity.Entities;
using UnityEngine;

class BulletAuthoring : MonoBehaviour
{
    public float speed = 60;

    class BulletAuthoringBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet()
            {
                speed = authoring.speed,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float speed;
    public int damageAmount;
}