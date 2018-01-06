using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System;

public class mainController : MonoBehaviour
{

    //left grip button
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    //left trigger button
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private Valve.VR.EVRButtonId appMenu = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;


    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_Controller.Device rightDevice;
    private SteamVR_Controller.Device leftDevice;

    public controlerWordPress interactIt;
    private bool button_allowed = true;

    private SteamVR_TrackedObject trackedObject;

    HashSet<interactivity> objectsHoveringOver = new HashSet<interactivity>();
    private interactivity closestItem;
    private interactivity interactingItem;
    private ushort pulsePower = 3000;


    // Use this for initialization
    void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        rightDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        leftDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));

    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Always have a backup plan.");
            return;
        }

        //gripButtonDown = controller.GetPressDown(gripButton);
        //gripButtonUp = controller.GetPressUp(gripButton);
        //gripButtonPressed = controller.GetPress(gripButton);

        //triggerButtonDown = controller.GetPressDown(triggerButton);
        //triggerButtonUp = controller.GetPressUp(triggerButton);
        //triggerButtonPressed = controller.GetPress(triggerButton);
        if (controller.GetPressUp(appMenu))
        {
                interactIt.plusplus();
        }

        if (controller.GetPressUp(gripButton))
        {
                interactIt.minusminus();
        }


        if (controller.GetPressDown(gripButton))
        {
                Debug.Log("click");
            float minDistance = float.MaxValue;
            controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, 100));
            float distance;
            foreach (interactivity item in objectsHoveringOver)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }

            interactingItem = closestItem;

            if (interactingItem)
            {
                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }
                interactingItem.BeginInteraction(this);
            }

            //           pickup.transform.parent = this.transform;
            //           pickup.GetComponent<Rigidbody>().useGravity = false;
        }

        if (controller.GetPressUp(gripButton) && interactingItem != null)
        {
            Debug.Log("C\'mon, press it.");
            interactingItem.EndInteraction(this);

            //           pickup.transofrm.parent = null;
            //          pickup.GetComponent<Rigidbody>().isKinematic = true;

        }

        //        if (controller.GetPressDown(triggerButton))
        //        {
        //            Debug.Log("oh yea");
        //        }
        //
        //        if (controller.GetPressUp(triggerButton))
        //        {
        //            Debug.Log("C\'mon, pull it.");
        //        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("trigger entered");
        interactivity collidedItem = collider.GetComponent<interactivity>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
        //        pickup = collider.gameObject;
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("trigger");
        interactivity collidedItem = collider.GetComponent<interactivity>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
        //        pickup = null;
    }

}
