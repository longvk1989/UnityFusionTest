using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private float _speed = 10f;

    [Networked] private TickTimer _lifeTime { get; set; }

    public override void Spawned()
    {
        _collider.enabled = HasStateAuthority;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (_lifeTime.Expired(Runner))
                Runner.Despawn(Object);
            else
                transform.position += _speed * transform.forward * Runner.DeltaTime;
        }
    }

    public void Setup(float speed = 10f, float lifeTime = 5f)
    {
        _speed = speed;
        _lifeTime = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Bullet Hit {other.name}");

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.HasStateAuthority)
        {
            playerHealth.DealDamageRpc(5f);
            Runner.Despawn(Object);
        }
    }
}
