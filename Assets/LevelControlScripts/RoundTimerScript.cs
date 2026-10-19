using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class RoundTimerScript : MonoBehaviour
{
    public UnityEvent TimerEnded;
    public float maxTimeInSeconds = 5;

    private void Awake()
    {
        StartCoroutine(CollapseWalls());
    }

    IEnumerator CollapseWalls()
    {
        yield return new WaitForSeconds(maxTimeInSeconds);
        TimerEnded.Invoke();
    }
}
