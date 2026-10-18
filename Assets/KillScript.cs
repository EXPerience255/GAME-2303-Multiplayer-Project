using UnityEngine;

public class KillScript : MonoBehaviour
{
    public float time;
    private void Start()
    {
        Destroy(gameObject, time);
    }
}
