using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    //remove BurstCompile to use Camera.main 
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Camera.main ? Camera.main.transform.forward : default;

        foreach ((var localTransform, var healthBar) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
        {
            //rotate to look always to the camera
            if (localTransform.ValueRO.Scale == 1f) //only if heealth bar is visible
            {
                LocalTransform ownerTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.ownerEntity);
                quaternion localForwardRotation = quaternion.LookRotation(cameraForward, math.up());
                localTransform.ValueRW.Rotation = ownerTransform.InverseTransformRotation(localForwardRotation);
            }

            //if owner health changed
            Health ownerHealth = SystemAPI.GetComponent<Health>(healthBar.ValueRO.ownerEntity);
            if (ownerHealth.onHealthChanged == false)
                continue;

            //get owner health
            float healthNormalized = (float)ownerHealth.healthAmount / ownerHealth.healthmax;

            //show only if owner is damaged
            localTransform.ValueRW.Scale = healthNormalized == 1f ? 0f : 1f;

            // //update health bar size
            // RefRW<LocalTransform> healthBarTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barPivotEntity);
            // healthBarTransform.ValueRW.Scale = healthNormalized;

            //update health bar size (set NonUniformScale in HealthBarAuthoring to obtain PostTransformMatrix)
            RefRW<PostTransformMatrix> healthBarTransform = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barPivotEntity);
            healthBarTransform.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}
