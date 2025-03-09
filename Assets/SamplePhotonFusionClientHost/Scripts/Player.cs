using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _cc;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [SerializeField] private MeshRenderer _mesh;

    [Networked] private TickTimer delay { get; set; }
    [Networked] public bool spawnedProjectile { get; set; }

    private Vector3 _forward = Vector3.forward;
    private ChangeDetector _changeDetector;
    private Color _targetColor;

    private void Awake()
    {
        _forward = transform.forward;
    }

    private void Reset()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    private void Update()
    {
        var color = _mesh.material.color;
        if (color != _targetColor)
        {
            _mesh.material.color = Color.Lerp(color, _targetColor, Time.deltaTime);
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(spawnedProjectile):
                    Debug.Log($"Property spawnedProjectile changed....... {spawnedProjectile}");
                    _targetColor = spawnedProjectile ? Color.blue : Color.white;
                    break;
            }
        }
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5f * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_prefabBall,
                        transform.position + _forward,
                        Quaternion.LookRotation(_forward),
                        Object.InputAuthority,
                        (runner, o) =>
                        {
                            o.GetComponent<Ball>().Init();
                        });
                    spawnedProjectile = !spawnedProjectile;
                }
                else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                      });
                    spawnedProjectile = !spawnedProjectile;
                }
            }
        }
    }
}
