using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : NetworkBehaviour
{
    private IObjectPool<Bullet> pool;
    private ulong clientID;

    // Give the bullet a reference to its originating pool
    public void SetPool(IObjectPool<Bullet> pool) => this.pool = pool;

    private void OnEnable()
    {
        // Deactivate bullet after 2 seconds automatically
        //Invoke(nameof(Deactivate), 2f);
    }

    private void Deactivate()
    {
        //CancelInvoke();
        // Return this bullet to the pool instead of Destroying it
        pool.Release(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner)
        {
            Deactivate();
        }
    }
}
