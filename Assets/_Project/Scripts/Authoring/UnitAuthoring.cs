using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public EFaction faction;

    public class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit()
            {
                faction = authoring.faction,
            });
        }
    }
}

public struct Unit : IComponentData
{
    public EFaction faction;
}
