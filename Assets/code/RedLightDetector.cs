using UnityEngine;

public class RedLightDetector : MonoBehaviour
{
    public TimedTrafficLightController timedTrafficLight;
    public ViolationLogger logger;

    [HideInInspector] public bool hasEverViolated = false;

    private void OnTriggerStay(Collider other)
    {
        

        if (!other.CompareTag("Player")) return;

        if (timedTrafficLight != null && timedTrafficLight.IsRed() && !hasEverViolated)
        {
            Debug.Log("\ud83d\udea8 Red Light Violation Detected!");
            logger.LogViolation("Red Light Violation: Entered during red light");
            hasEverViolated = true;
            logger.ShowViolations();
        }

    }
}