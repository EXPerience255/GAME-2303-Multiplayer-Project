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

        shot = Instantiate(baseBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation) as Rigidbody;
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
