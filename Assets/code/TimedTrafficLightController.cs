using UnityEngine;
public class TimedTrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Yellow, Green }
    public LightState currentLight = LightState.Red;

    public GameObject redLightObj;
    public GameObject yellowLightObj;
    public GameObject greenLightObj;

    public float redTime = 20f;
    public float yellowTime = 3f;
    public float greenTime = 15f;

    private float timer = 0f;

    void Start()
    {
        SetLightState(LightState.Red);
    }

    void Update()
    {
      

        timer += Time.deltaTime;
        switch (currentLight)
        {
            case LightState.Red:
                if (timer >= redTime) SetLightState(LightState.Green);
                break;
            case LightState.Green:
                if (timer >= greenTime) SetLightState(LightState.Yellow);
                break;
            case LightState.Yellow:
                if (timer >= yellowTime) SetLightState(LightState.Red);
                break;
        }
    }

    void SetLightState(LightState state)
    {
        currentLight = state;
        timer = 0f;
        redLightObj.SetActive(state == LightState.Red);
        yellowLightObj.SetActive(state == LightState.Yellow);
        greenLightObj.SetActive(state == LightState.Green);
    }

    public bool IsRed()
    {
        return currentLight == LightState.Red;
    }
}