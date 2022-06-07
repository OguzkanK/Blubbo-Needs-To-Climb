using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] Transform cameraTransform;

    private void Update()
    {
        if(cameraTarget)
        {
            Vector3 position = cameraTransform.position;
            position.x = cameraTarget.position.x;
            cameraTransform.position = position;
        }
        else
        {
            cameraTransform.position = new Vector3(0, 0, -10);
        }
    }
}