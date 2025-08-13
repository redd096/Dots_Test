using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]  //work in LateUpdate instead of Update
[UpdateBefore(typeof(ResetEventsSystem))]           //this is used to call update before than the system that will reset entities events 
partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //(set NonUniformScale in SelectedAuthoring to obtain PostTransformMatrix)
        var transformMatrixLookup = SystemAPI.GetComponentLookup<PostTransformMatrix>(isReadOnly: false);

        //update scale
        SelectedVisualJob job = new SelectedVisualJob()
        {
            transformMatrixLookup = transformMatrixLookup,
        };
        job.ScheduleParallel();

        /*
        // Instead now use only one Job

        var localTransformsLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);

        //set scale for every Enabled Selected
        EnabledSelectedVisualJob enabledSelectedJob = new EnabledSelectedVisualJob()
        {
            localTransformsLookup = localTransformsLookup,
        };
        enabledSelectedJob.ScheduleParallel();

        //remove scale for every Disabled Selected
        DisabledSelectedVisualJob disabledSelectedJob = new DisabledSelectedVisualJob()
        {
            localTransformsLookup = localTransformsLookup,
        };
        disabledSelectedJob.ScheduleParallel();

        /******
        // Instead now use IJob to do the same thing
        
        //set scale for every Enabled Selected
        foreach (var selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> selectedObjTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.selectedObjEntity);
            selectedObjTransform.ValueRW.Scale = 1f;
        }

        //remove scale for every Disabled Selected
        foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> selectedObjTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.selectedObjEntity);
            selectedObjTransform.ValueRW.Scale = 0f;
        }
        */
    }
}

[WithPresent(typeof(Selected))]    //this to do this Job on both enabled and disabled Selected
[BurstCompile]
public partial struct SelectedVisualJob : IJobEntity
{
    [NativeDisableParallelForRestriction]   //add this tag to enable ReadAndWrite. Else parallel jobs want only ReadOnly ComponentLookup
    public ComponentLookup<PostTransformMatrix> transformMatrixLookup;

    public void Execute(in Selected selected)
    {
        //onSelect or onDeselect
        if (selected.onSelected || selected.onDeselected)
        {
            //use PostTransformMatrix instead of LocalTransform because now this job is called only onSelected or onDeselected event, 
            //so we set the transform localscale to 0 in the prefab to hide by default.
            //With this component we are setting the scale, instead of a multiplier like in LocalTransform
            RefRW<PostTransformMatrix> selectedObjTransform = transformMatrixLookup.GetRefRW(selected.selectedObjEntity);
            selectedObjTransform.ValueRW.Value = selected.onSelected ? float4x4.Scale(selected.scale) : float4x4.Scale(0f);
        }
    }
}

/*
// Instead now use only one Job

[BurstCompile]
public partial struct EnabledSelectedVisualJob : IJobEntity
{
    [NativeDisableParallelForRestriction]   //add this tag to enable ReadAndWrite. Else parallel jobs want only ReadOnly ComponentLookup
    public ComponentLookup<LocalTransform> localTransformsLookup;

    public void Execute(in Selected selected)
    {
        RefRW<LocalTransform> selectedObjTransform = localTransformsLookup.GetRefRW(selected.selectedObjEntity);
        selectedObjTransform.ValueRW.Scale = 1f;
    }
}

[WithDisabled(typeof(Selected))]    //this to do this Job on disabled Selected
[BurstCompile]
public partial struct DisabledSelectedVisualJob : IJobEntity
{
    [NativeDisableParallelForRestriction]   //add this tag to enable ReadAndWrite. Else parallel jobs want only ReadOnly ComponentLookup
    public ComponentLookup<LocalTransform> localTransformsLookup;

    public void Execute(in Selected selected)
    {
        RefRW<LocalTransform> selectedObjTransform = localTransformsLookup.GetRefRW(selected.selectedObjEntity);
        selectedObjTransform.ValueRW.Scale = 0f;
    }
}

*/