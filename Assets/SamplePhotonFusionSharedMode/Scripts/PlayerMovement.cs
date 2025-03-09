using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _playerSpeed = 2f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravityValue = -9.81f;

    private Vector3 _velocity;
    private bool _jumpPressed;

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _camera = _camera == null ? Camera.main : _camera;
            _camera.GetComponent<FirstPersonCamera>().SetTarget(transform);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority

        //Handle Gravity
        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }
        _velocity.y += _gravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += _jumpForce;
        }

        //Handle Movement
        Quaternion cameraRotationY = 
            _camera != null
                ? Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0)
                : Quaternion.identity;
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * _playerSpeed;
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _jumpPressed = false;
    }
}
