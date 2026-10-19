using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public InputActionReference shootAction;

    [Header("Spawn Setup")]
    public Transform bulletSpawnPoint;

    [Header("Bullet Setup")]
    public float shotForce = 10;
    [SerializeField] Rigidbody baseBullet;
    //private BulletSpawner bulletSpawner; //Not working yet
    
    [Header("Set Shoot Timing")]
    public float shootSpeed = 0.2f;
    public bool canShoot;
    public bool currentlyShooting = false;

    private void OnEnable()
    {
        shootAction.action.Enable();

        shootAction.action.performed += OnShootPerformed;
        shootAction.action.canceled += OnShootCancelled;
    }

    private void OnDisable()
    {
        shootAction.action.Disable();

        shootAction.action.performed -= OnShootPerformed;
        shootAction.action.canceled -= OnShootCancelled;
    }
    void Awake()
    {
        canShoot = true;
        //bulletSpawner = FindAnyObjectByType<BulletSpawner>();
    }
    private void FixedUpdate()
    {
        if (canShoot && currentlyShooting)
            StartCoroutine(PlayerShoot());
    }

    IEnumerator PlayerShoot()
    {
        canShoot = false;
        Rigidbody shot;

        //shot = bulletSpawner.bulletPool.Get().GetComponent<Rigidbody>();
        shot = Instantiate(baseBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        shot.AddForce(bulletSpawnPoint.forward * shotForce, ForceMode.Impulse);

        yield return new WaitForSeconds(shootSpeed);
        canShoot = true;
    }

    void OnShootPerformed(InputAction.CallbackContext context)
    {
        currentlyShooting = true;
    }

    void OnShootCancelled(InputAction.CallbackContext context)
    {
        currentlyShooting = false;
    }
}
