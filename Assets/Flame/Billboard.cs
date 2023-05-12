using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        directionToCamera.y = 0;  // This line keeps the flame's normal parallel to the ground.

        if (directionToCamera != Vector3.zero) // To avoid errors when the object is exactly at the camera position
        {
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }
}
