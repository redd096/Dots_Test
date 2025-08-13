using Unity.Entities;
using UnityEngine;

class HealthBarAuthoring : MonoBehaviour
{
    public GameObject ownerObj;
    public GameObject barPivotObj;

    class Baker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar()
            {
                ownerEntity = GetEntity(authoring.ownerObj, TransformUsageFlags.Dynamic),
                barPivotEntity = GetEntity(authoring.barPivotObj, TransformUsageFlags.NonUniformScale),
            });
        }
    }
}

public struct HealthBar : IComponentData
{
    public Entity ownerEntity;
    public Entity barPivotEntity;
}