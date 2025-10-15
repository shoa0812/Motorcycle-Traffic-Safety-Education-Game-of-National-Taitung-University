
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemInteractionDisplay : MonoBehaviour
{
    public TMP_Text interactionText; 
    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {

        interactionText.text = "grab!";
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        interactionText.text = "release!";
    }
}
