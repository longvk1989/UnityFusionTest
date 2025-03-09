using Fusion;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Camera _camera;
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _camera = _camera == null ? Camera.main : _camera;
            _camera.GetComponent<FirstPersonCamera>().SetTarget(transform);
        }
    }

    private void Update()
    {
        if (HasStateAuthority && Input.GetMouseButtonDown(0))
        {
            Runner.Spawn(_bulletPrefab,
                transform.position + _camera.transform.forward.normalized * 0.2f,
                Quaternion.LookRotation(_camera.transform.forward),
                Object.InputAuthority,
                (runner, o) =>
                {
                    o.GetComponent<Bullet>().Setup();
                });
        }
    }
}
