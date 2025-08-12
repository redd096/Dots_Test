using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    public static UnitSelectionManager Instance { get; private set; }

    //events
    public event System.Action OnSelectionAreaStart;
    public event System.Action OnSelectionAreaEnd;

    private Vector2 startSelectionMousePosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //select area with left click
        if (Input.GetMouseButtonDown(0))
        {
            startSelectionMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            UpdateSelectedUnits();
            OnSelectionAreaEnd?.Invoke();
        }

        //right click to set target position
        if (Input.GetMouseButtonDown(1))
        {
            SetTargetPosition();
        }
    }

    /// <summary>
    /// Return Rect from startMousePosition to currentMousePosition
    /// </summary>
    public Rect GetSelectionAreaRect()
    {
        Vector2 currentSelectionMousePosition = Input.mousePosition;

        Vector2 upLeft = new Vector2(
            Mathf.Min(startSelectionMousePosition.x, currentSelectionMousePosition.x),
            Mathf.Min(startSelectionMousePosition.y, currentSelectionMousePosition.y)
        );
        Vector2 bottomRight = new Vector2(
            Mathf.Max(startSelectionMousePosition.x, currentSelectionMousePosition.x),
            Mathf.Max(startSelectionMousePosition.y, currentSelectionMousePosition.y)
        );

        return new Rect(position: upLeft, size: bottomRight - upLeft);
    }

    private void UpdateSelectedUnits()
    {
        //disable every Selected Unit
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit, Selected>().Build(entityManager);
        NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
        for (int i = 0; i < entities.Length; i++)
        {
            entityManager.SetComponentEnabled<Selected>(entities[i], false);

            //set OnDeselected event
            var selectedElement = selectedArray[i];
            selectedElement.onDeselected = true;
            //update one by one instead of call one time CopyFromComponentDataArray, because we are updating the entityQuery list by disabling Selected component
            entityManager.SetComponentData(entities[i], selectedElement);
        }

        //if multi selection, find units in selectionArea and enable Selected
        Rect selectionRect = GetSelectionAreaRect();
        bool isMultiSelection = selectionRect.width + selectionRect.height > 40f;
        if (isMultiSelection)
        {
            //find every object with LocalTransform and Unit enabled, and Selected component (both enabled and disabled)
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
            entities = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<LocalTransform> transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (int i = 0; i < transforms.Length; i++)
            {
                //if inside selection area, enable Selected
                Vector2 screenPosition = cam.WorldToScreenPoint(transforms[i].Position);
                if (selectionRect.Contains(screenPosition))
                {
                    entityManager.SetComponentEnabled<Selected>(entities[i], true);

                    //set OnSelected event
                    Selected selectedElement = entityManager.GetComponentData<Selected>(entities[i]);
                    selectedElement.onSelected = true;
                    entityManager.SetComponentData(entities[i], selectedElement);
                }
            }
        }
        //else use raycast to select only one unit
        else
        {
            //(other way to create EntityQuery + GetSingleton method instead of cycle the array)
            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

            UnityEngine.Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput()
            {
                Start = ray.origin,
                End = ray.GetPoint(9999f),
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,                                                                    //belong to every layer (like -1, the u is only to have uint instead of int)
                    CollidesWith = 1u << GameAssets.UNITS_LAYER, //(uint)GameAssets.UnitsLayer.value,   //hit units layer
                    //GroupIndex = 0                                                                    //useless, this is used to override layers
                }
            };

            //raycast
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
            {
                if (entityManager.HasComponent<Unit>(hit.Entity) && entityManager.HasComponent<Selected>(hit.Entity))
                {
                    entityManager.SetComponentEnabled<Selected>(hit.Entity, true);

                    //set OnSelected event
                    Selected selectedElement = entityManager.GetComponentData<Selected>(hit.Entity);
                    selectedElement.onSelected = true;
                    entityManager.SetComponentData(hit.Entity, selectedElement);
                }
            }
        }
    }

    private void SetTargetPosition()
    {
        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition(cam);

        //same as SystemAPI.Query but executed from a Monobehaviour instead of Dots ISystem 

        //cycle every entity with UnitMover and Selected components (and both components enabled)
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);
        // NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<UnitMover> unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
        NativeArray<float3> positions = GenerateMovePositionArray(mouseWorldPosition, unitMovers.Length);   //generate positions instead of put mouseWorldPosition for everyone
        for (int i = 0; i < unitMovers.Length; i++)
        {
            //set target position
            UnitMover unitMover = unitMovers[i];
            unitMover.targetPosition = positions[i];

            // //and update dots component
            // entityManager.SetComponentData(entities[i], unitMover);

            //instead of update dots component, update value for every element and out of the forCycle call only one time to update every dots component
            unitMovers[i] = unitMover;
        }
        entityQuery.CopyFromComponentDataArray(unitMovers);
    }

    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if (positionCount == 0)
            return positionArray;

        //add targetPosition as first position
        positionArray[0] = targetPosition;
        if (positionCount == 1)
            return positionArray;

        float ringSize = 2.2f;  //size of every ring
        int ring = 0;
        int positionIndex = 1;  //1 because 0 is already the targetPosition

        while (positionIndex < positionCount)
        {
            //the position to the right on this ring
            float3 ringRightPoint = new float3(ringSize * (ring + 1), 0, 0);

            //3 in the first ring, then add 2 more in every ring (3+0, 3+2, 3+4, 3+6, etc...)
            int ringPositionCount = 3 + ring * 2;

            //rotate right point to get every point on this ring
            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (Mathf.PI / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), ringRightPoint);
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                //be sure to not exceed positions
                if (positionIndex >= positionCount)
                    break;
            }

            ring++;
        }

        return positionArray;
    }
}
