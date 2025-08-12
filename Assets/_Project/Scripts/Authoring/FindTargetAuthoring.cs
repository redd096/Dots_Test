using Unity.Entities;
using UnityEngine;

class FindTargetAuthoring : MonoBehaviour
{
    public float range = 7;
    public EFaction targetFaction;
    public double timerMax = 0.2;

    class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FindTarget()
            {
                range = authoring.range,
                targetFaction = authoring.targetFaction,
                timerMax = authoring.timerMax,
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float range;
    public EFaction targetFaction;
    public double timer;
    public double timerMax;
}