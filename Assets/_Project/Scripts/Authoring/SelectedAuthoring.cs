using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class SelectedAuthoring : MonoBehaviour
{
    public GameObject selectedObj;
    public Vector3 scale = new Vector3(2, 0.1f, 2);

    class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            //find entity and add component to it
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected()
            {
                selectedObjEntity = GetEntity(authoring.selectedObj, TransformUsageFlags.NonUniformScale),
                scale = authoring.scale,
            });
            //and set it disabled
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

//In inspector you can see it in Tags (instead of a new component) because this struct doesn't have variables
//Now we added variables, so it appears like a normal component but with toggle because of IEnableableComponent
public struct Selected : IComponentData, IEnableableComponent
{
    public Entity selectedObjEntity;
    public float3 scale;

    public bool onSelected;
    public bool onDeselected;
}