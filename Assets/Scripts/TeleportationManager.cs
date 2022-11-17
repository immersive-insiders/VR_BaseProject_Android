using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private Controller controller;
    private enum Controller
    {
        RightController,
        LeftController
    }

    private bool isTeleportActive = false;
    private string handActionMap;
    private InputAction thumbstickInputAction;
    private InputAction teleportActivate;
    private InputAction teleportCancel;

    void Start()
    {
        rayInteractor.enabled = false;
        
        if(controller == Controller.LeftController)
            handActionMap = "XRI LeftHand Locomotion";
        else
            handActionMap = "XRI RightHand Locomotion";

        teleportActivate = inputAction.FindActionMap(handActionMap).FindAction("Teleport Mode Activate");
        teleportActivate.Enable();
        teleportActivate.performed += OnTeleportActivate;

        teleportCancel = inputAction.FindActionMap(handActionMap).FindAction("Teleport Mode Cancel");
        teleportCancel.Enable();
        teleportCancel.performed += OnTeleportCancel;

        thumbstickInputAction = inputAction.FindActionMap(handActionMap).FindAction("Move");
        thumbstickInputAction.Enable();
    }
    
    void Update()
    {
        if (!isTeleportActive)
            return;
  
        if (!rayInteractor.enabled)
            return;

        if (thumbstickInputAction.triggered)
            return;

        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            rayInteractor.enabled = false;
            isTeleportActive = false;
            return;
        }

        TeleportRequest teleportRequest = new TeleportRequest()
        {
            destinationPosition = raycastHit.point,
        };

        teleportationProvider.QueueTeleportRequest(teleportRequest);
        rayInteractor.enabled = false;
        isTeleportActive = false;
    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        if (!isTeleportActive)
        {
            rayInteractor.enabled = true;
            isTeleportActive = true;
        }

    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        if (isTeleportActive && rayInteractor.enabled == true)
        {
            rayInteractor.enabled = false;
            isTeleportActive = false;
        }

    }
    private void OnDestroy()
    {
        teleportActivate.performed -= OnTeleportActivate;
        teleportCancel.performed -= OnTeleportCancel;
    }
}
