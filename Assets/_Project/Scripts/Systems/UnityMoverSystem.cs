using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnityMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        // unitMoverJob.Run();
        unitMoverJob.ScheduleParallel();

        /*
        // Instead now use IJob to do the same thing

        //query to cycle entities components. This will find every entity that have LocalTransform AND MoveSpeed AND PhysicsVelocity components. And only if enabled
        //RefRW to edit the localTransform (read and write)
        //RefRO to read moveSpeed (read only)
        foreach ((var localTransform, var unitMover, var physicsVelocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
        {
            //add movement * deltaTime
            //NB RW to edit value and RO to read
            // localTransform.ValueRW.Position = localTransform.ValueRO.Position + new float3(moveSpeed.ValueRO.value, 0, 0) * SystemAPI.Time.DeltaTime;

            //calculate direction and speed
            float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
            if (math.lengthsq(moveDirection) <= 2f)
            {
                //reached point, stop and do nothing
                physicsVelocity.ValueRW.Linear = float3.zero;
                physicsVelocity.ValueRW.Angular = float3.zero;
                return;
            }
            moveDirection = math.normalize(moveDirection);

            float speed = unitMover.ValueRO.moveSpeed;
            float rotationSpeed = unitMover.ValueRO.rotationSpeed;

            //set rotation and position
            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * rotationSpeed);
            // localTransform.ValueRW.Position += moveDirection * speed * SystemAPI.Time.DeltaTime;

            //set velocity and angular velocity
            physicsVelocity.ValueRW.Linear = moveDirection * speed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
        */
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    //in IJob use "ref" instead of RefRW
    //and "in" instead of RefRO
    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        //calculate speed
        float speed = unitMover.moveSpeed;
        float rotationSpeed = unitMover.rotationSpeed;

        //and direction
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;
        if (math.lengthsq(moveDirection) <= speed * deltaTime + 0.1f)
        {
            //reached point, stop and do nothing
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        moveDirection = math.normalize(moveDirection);

        //set rotation and position
        quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
        localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, deltaTime * rotationSpeed);

        //set velocity and angular velocity
        physicsVelocity.Linear = moveDirection * speed;
        physicsVelocity.Angular = float3.zero;

    }
}
