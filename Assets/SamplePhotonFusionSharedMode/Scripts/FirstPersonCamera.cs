using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
