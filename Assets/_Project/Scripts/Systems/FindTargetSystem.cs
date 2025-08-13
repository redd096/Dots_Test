using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //cycle entities with FindTarget component
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        foreach ((var localTransform, var findTarget, var target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<Target>>())
        {
            //wait timer
            if (findTarget.ValueRO.timer > SystemAPI.Time.ElapsedTime)
            {
                continue;
            }
            findTarget.ValueRW.timer = SystemAPI.Time.ElapsedTime + findTarget.ValueRW.timerMax;

            CollisionFilter collisionFilter = new CollisionFilter()
            {
                BelongsTo = ~0u,                                                                    //belong to every layer (like -1, the u is only to have uint instead of int)
                CollidesWith = 1u << GameAssets.UNITS_LAYER, //(uint)GameAssets.UnitsLayer.value,   //hit units layer
                //GroupIndex = 0                                                                    //useless, this is used to override layers
            };

            //find targets near this entity
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref hits, collisionFilter))
            {
                foreach (DistanceHit hit in hits)
                {
                    //be sure hitted entity exists
                    if (SystemAPI.Exists(hit.Entity) == false || SystemAPI.HasComponent<Unit>(hit.Entity) == false)
                        continue;

                    //if opposite faction, set target
                    Unit unit = SystemAPI.GetComponent<Unit>(hit.Entity);
                    if (unit.faction == findTarget.ValueRO.targetFaction)
                    {
                        target.ValueRW.targetEntity = hit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
