using Fusion;
using UnityEngine;

public class PhysxBall : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [Networked] private TickTimer _life { get; set; }

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 velocity)
    {
        _life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        _rb.linearVelocity = velocity;
    }

    public override void FixedUpdateNetwork()
    {
        if (_life.Expired(Runner))
            Runner.Despawn(Object);
    }
}
