using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Collections;
public class RoundTimerScript : NetworkBehaviour
{
    public UnityEvent<string> PassTimeToTimerText;
    public UnityEvent TimerEnded;
    public float maxTimeInSeconds = 5;
    public float timePassed = 0;
    NetworkVariable<float> timerNumber;
    NetworkVariable<FixedString64Bytes> timerText = new NetworkVariable<FixedString64Bytes>();

    private void Awake()
    {
        timerNumber = new NetworkVariable<float>();


    }
    
    public void Start()
    {
        SetTimerTextRpc();
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
        SetTimerTextRpc();
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

    [Rpc(SendTo.Server)]
    void SetTimerTextRpc()
    {
        timerNumber.Value = maxTimeInSeconds - timePassed;
        timerText.Value = "Time to Wall Collapse: " + timerNumber.Value;

        PassTimeToTimerText.Invoke(timerText.Value.ToString());
    }
}
