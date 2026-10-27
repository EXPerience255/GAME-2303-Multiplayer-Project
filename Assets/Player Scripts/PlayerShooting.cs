using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    public InputActionReference shootAction;

    [Header("Spawn Setup")]
    public Transform bulletSpawnPoint;

    [Header("Bullet Setup")]
    public float shotForce = 10;
    [SerializeField] NetworkObject baseBullet; //Used to be Rigidbody, but bullets weren't moving in multiplayer
    
    [Header("Set Shoot Timing")]
    public float shootSpeed = 0.2f;
    public bool canShoot;
    public bool currentlyShooting = false;

    private void OnEnable()
    {
        if (IsLocalPlayer)
        {
            shootAction.action.Enable();

            shootAction.action.performed += OnShootPerformed;
            shootAction.action.canceled += OnShootCancelled;
        }
    }

    private void OnDisable()
    {
        if (IsLocalPlayer)
        {
            shootAction.action.Disable();

            shootAction.action.performed -= OnShootPerformed;
            shootAction.action.canceled -= OnShootCancelled;
        }
    }
    void Awake()
    {
        canShoot = true;
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            if (canShoot && currentlyShooting)
                StartCoroutine(PlayerShoot());
        }
    }

    IEnumerator PlayerShoot()
    {
        canShoot = false;
        /*Rigidbody shot;

        shot = Instantiate(baseBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        shot.AddForce(bulletSpawnPoint.forward * shotForce, ForceMode.Impulse);*/
        ShootServerRpc();

        yield return new WaitForSeconds(shootSpeed);
        canShoot = true;
    }

    [Rpc(SendTo.Server)]
    private void ShootServerRpc() //Only server can spawn NetworkObjects
    {
        /*Rigidbody shot;
        shot = Instantiate(baseBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        shot.AddForce(bulletSpawnPoint.forward * shotForce, ForceMode.Impulse);*/

        NetworkObject bullet = Instantiate(baseBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.Spawn();
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.forward * shotForce, ForceMode.Impulse);
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
