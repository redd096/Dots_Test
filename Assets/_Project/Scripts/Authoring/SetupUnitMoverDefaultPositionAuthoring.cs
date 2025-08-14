using Unity.Entities;
using UnityEngine;

/// <summary>
/// We aren't using this component, because we are settings default position already in UnitMoverAuthoring. 
/// But is possible to read how to set default values and remove components in SetupUnitMoverDefaultPositionSystem
/// </summary>
class SetupUnitMoverDefaultPositionAuthoring : MonoBehaviour
{
    class Baker : Baker<SetupUnitMoverDefaultPositionAuthoring>
    {
        public override void Bake(SetupUnitMoverDefaultPositionAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SetupUnitMoverDefaultPosition());
        }
    }
}

public struct SetupUnitMoverDefaultPosition : IComponentData
{
}