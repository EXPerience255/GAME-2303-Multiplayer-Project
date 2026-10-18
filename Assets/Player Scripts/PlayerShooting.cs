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

    

    private void OnEnable()
    {
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
    }
    void Awake()
    {
        canShoot = true;
    }

    void Update()
    {
        if (shootAction.action.IsPressed() && canShoot)
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
}
