using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    public IObjectPool<Bullet> bulletPool;

    private void Awake()
    {
        // Initialize the pool with its configurations
        bulletPool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,          // How to create a new item if the pool is empty
            actionOnGet: OnGetBullet,          // What to do when an item is retrieved
            actionOnRelease: OnReleaseBullet,  // What to do when an item returns to the pool
            actionOnDestroy: OnDestroyBullet,  // What to do if the pool overflows past MaxSize
            collectionCheck: true,             // Throws errors if you try to return an item already in the pool
            defaultCapacity: 75,               // Initial array size allocation
            maxSize: 100                       // Absolute limit of items allowed to live in memory
        );
    }

    // --- Pool Action Delegates ---

    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab);
        bullet.SetPool(bulletPool);
        return bullet;
    }

    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true); // Turn it on
        bullet.gameObject.transform.position = firePoint.position; // Set it's position and rotation
        bullet.gameObject.transform.rotation = firePoint.rotation;
    }

    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false); // Turn it off
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject); // Cleanup if over MaxSize
    }
}
