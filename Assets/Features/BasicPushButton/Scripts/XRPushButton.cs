using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
[RequireComponent(typeof(BoxCollider))]
public class XRPushButton : XRBaseInteractable
{
    private float previousPushDepth, minimalPushDepth, maximumPushDepth = 0.0f;
    private XRBaseInteractor pushInteractor = null;
    private bool previouslyPushed = false;

    [SerializeField] private UnityEvent OnPushed = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartPush);
        hoverExited.AddListener(EndPush);
    }

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        minimalPushDepth = transform.localPosition.y;
        maximumPushDepth = transform.localPosition.y - (boxCollider.bounds.size.y * 0.5f);
    }

    public void StartPush(HoverEnterEventArgs arg0)
    {
        pushInteractor = arg0.interactor;
        previousPushDepth = GetLocalYPosition(arg0.interactor.transform.position);
    }

    private float GetLocalYPosition(Vector3 interactorPos)
    {
        return transform.root.InverseTransformDirection(interactorPos).y;
    }

    private void SetYPosition(float yPos)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(yPos, maximumPushDepth, minimalPushDepth);
        transform.localPosition = newPosition;
    }

    private void PushButtonTriggered()
    {
        if (PushedDown() && !previouslyPushed)
        {
            OnPushed.Invoke();
        }

        previouslyPushed = PushedDown();
    }

    private bool PushedDown()
    {
        return transform.localPosition.y == Mathf.Clamp(transform.localPosition.y, maximumPushDepth, maximumPushDepth + 0.01f);
    }

    public void EndPush(HoverExitEventArgs arg0)
    {
        pushInteractor = null;
        previousPushDepth = 0.0f;

        previouslyPushed = false;
        SetYPosition(minimalPushDepth);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(pushInteractor)
        {
            float newPushPos = GetLocalYPosition(pushInteractor.transform.position);
            float pushDelta = previousPushDepth - newPushPos;
            previousPushDepth = newPushPos;

            float newPos = transform.localPosition.y - pushDelta;
            SetYPosition(newPos);

            PushButtonTriggered();
        }
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();
        hoverEntered.RemoveListener(StartPush);
        hoverExited.RemoveListener(EndPush);
    }

    public void DisableButton(float seconds)
    {
        StartCoroutine(DisableButtonRoutine(seconds));
    }

    public IEnumerator DisableButtonRoutine(float seconds)
    {
        this.enabled = false;
        yield return new WaitForSeconds(seconds);
        this.enabled = true;
    }


}
