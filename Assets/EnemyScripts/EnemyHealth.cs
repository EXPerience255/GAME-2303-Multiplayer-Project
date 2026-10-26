using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    PlayerShooting _playerShootScript;
    [Header("Material Types")]
    [SerializeField] MeshRenderer[] _mRenderer;
    public Material baseMat;
    public Material hitFlashMat;

    [Header("Enemy Health")]
    public int enemyHealth = 5;

    

   

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerBullet")
        {
            Destroy(other.gameObject);
            StartCoroutine(takeDamage());
        }
        if (other.gameObject.tag == "PlayerBigBullet")
        {
            
            StartCoroutine(takeDamage());
        }
    }

    void Start()
    {
        _playerShootScript = GameObject.FindAnyObjectByType<PlayerShooting>();
        foreach (MeshRenderer m in _mRenderer)
        {
            m.material = baseMat;
        }
    }

   
    void Update()
    {
        
    }

    IEnumerator takeDamage()
    {
        enemyHealth--;
        if (enemyHealth <= 0)
        {
            //GameManager.Instance._enemiesInPlay--;
            //GameManager.Instance.EnemyKillCount();
            Destroy(gameObject);
        }
        else
        {
            foreach (MeshRenderer m in _mRenderer)
            {
                m.material =hitFlashMat;
            }
        }
        yield return new WaitForSeconds(0.1f);
        foreach (MeshRenderer m in _mRenderer)
        {
            m.material = baseMat;
        }
    }
}
