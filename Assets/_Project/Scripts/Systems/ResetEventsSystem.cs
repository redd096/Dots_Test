using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]      //work in LateUpdate instead of Update, so every other system can do things before we reset the events
partial struct ResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //find every element with Selected both enabled or disabled
        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            //and reset events
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }
    }
}
