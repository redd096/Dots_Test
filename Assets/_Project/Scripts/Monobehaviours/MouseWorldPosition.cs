using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPosition(Camera cam)
    {
        Ray mouseCameraRay = cam.ScreenPointToRay(Input.mousePosition);

        // if (Physics.Raycast(mouseCameraRay, out RaycastHit hit))
        //     return hit.point;

        //instead of use raycast to hit plane in scene, create a logic Plane at Vector3.zero with Vector3.up as normal and check "hit" it
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(mouseCameraRay, out float distance))
            return mouseCameraRay.GetPoint(distance);
        else
            return Vector3.zero;
    }
}
