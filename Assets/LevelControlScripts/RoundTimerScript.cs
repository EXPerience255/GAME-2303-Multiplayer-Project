using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.VisualScripting;
public class RoundTimerScript : NetworkBehaviour
{
    public UnityEvent<string> PassTimeToTimerText;
    public UnityEvent TimerEnded;
    public float maxTimeInSeconds = 5;
    public float timePassed = 0;
    NetworkVariable<float> timerNumber;

    private void Awake()
    {
        timerNumber = new NetworkVariable<float>();


    }
    
    public void Start()
    {
        SetTimerText();
        StartCoroutine(CollapseWalls());
        StartCoroutine(TrackTime());
    }

    IEnumerator CollapseWalls()
    {
        yield return new WaitForSeconds(maxTimeInSeconds);
        TimerEnded.Invoke();
    }

    IEnumerator TrackTime()
    {
        yield return new WaitForSeconds(1);
        timePassed++;
        SetTimerText();
        if (timePassed! <= maxTimeInSeconds)
            StartCoroutine(TrackTime());
        if(timePassed >= maxTimeInSeconds)
            StartCoroutine(WallDownText());
    }

    IEnumerator WallDownText()
    {
        PassTimeToTimerText.Invoke("Walls Down FIGHT!");
        yield return new WaitForSeconds(2);
        PassTimeToTimerText.Invoke("");
    }

    void SetTimerText()
    {
        timerNumber.Value = maxTimeInSeconds - timePassed;
        PassTimeToTimerText.Invoke("Time to Wall Collapse: " + timerNumber.Value);
    }
}
