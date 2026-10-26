using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private float updateTimer;

    [SerializeField] NavMeshAgent _agent;
    private static readonly Collider[] neighborBuffer = new Collider[16];
    
    public float stoppingDistance = 1;

    [UnitHeaderInspectable("Seperation Settings")]
    public float separationRadius = 2f;
    public float separtionStrength = 3f;
    public LayerMask enemyLayer;


    public ZoneScript currentZone;
    private void Awake()
    {
       
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        if(currentZone != null && _player == !_player.gameObject.activeSelf)
            AquireTarget();

        if (_agent == null || !_agent.isOnNavMesh)
            return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f && _player != null)
        {
            updateTimer = updateInterval;
            Vector3 _separation = GetSeperationVector();
            Vector3 _destination = _player.position + _separation;
            if (NavMesh.SamplePosition(_destination, out NavMeshHit hit, 
                separationRadius +1, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
            else
            {
                _agent.SetDestination(_destination);
            }
        }
        
    }

    private Vector3 GetSeperationVector()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position,
            separationRadius, 
            neighborBuffer,
            enemyLayer);
        Vector3 push = Vector3.zero;
        int _neighbors = 0;
        for(int i = 0; i < count; i++)
        {
            Collider other= neighborBuffer[i];
            if(other.transform == transform)
            {
                continue;
            }

            Vector3 differnce = transform.position - other.transform.position;
            float _dist = differnce.magnitude;

            if(_dist > 0.001f && _dist < separationRadius)
            {
                push += differnce.normalized  * (separationRadius - _dist);
                _neighbors++;
            }

            if (_neighbors > 0)
            {
                push /= _neighbors;
                push *= separtionStrength;
            }


        }

        return push;
    }

    public void AquireTarget()
    {
        _player = currentZone.GrabPlayer();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
