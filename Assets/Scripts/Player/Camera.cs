using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(5, 7, -5);
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothed;

        transform.LookAt(target.position);
    }
}
