using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Green, Yellow, Red }

    public Light redLight;
    public Light yellowLight;
    public Light greenLight;

    public LightState currentState { get; private set; }

    public float greenTime = 15f;
    public float yellowTime = 3f;
    public float redTime = 15f;

    [Tooltip("延遲幾秒啟動，讓燈號與其他路口交錯")]
    public float startDelay = 0f;

    void Start()
    {
        SetLights(LightState.Red);
        StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        StartCoroutine(TrafficLoop());
    }

    IEnumerator TrafficLoop()
    {
        while (true)
        {
            SetLights(LightState.Green);
            yield return new WaitForSeconds(greenTime);

            SetLights(LightState.Yellow);
            yield return new WaitForSeconds(yellowTime);

            SetLights(LightState.Red);
            yield return new WaitForSeconds(redTime);
        }
    }

    void SetLights(LightState state)
    {
        currentState = state;

        if (redLight != null) redLight.enabled = (state == LightState.Red);
        if (yellowLight != null) yellowLight.enabled = (state == LightState.Yellow);
        if (greenLight != null) greenLight.enabled = (state == LightState.Green);
    }

    public bool IsRed()
    {
        return currentState == LightState.Red;
    }

    public LightState GetCurrentLight()
    {
        return currentState;
    }
}