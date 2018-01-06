using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactivity : MonoBehaviour
{
    public Rigidbody rigidbody;

    private bool currentlyInteracting;

    private float velocityFactor = 2000f;
    private Vector3 posDelta;


    private float angle;
    private Vector3 axis;
    private float rotationFactor = 400f;
    private Quaternion rotationDelta;


    private mainController attachedController;

    private Transform interactionPoint;


    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform;
        velocityFactor /= rigidbody.mass;
        AudioSource kick = GetComponent<AudioSource>();
        AudioSource perc = GetComponent<AudioSource>();
        AudioSource melody = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {


        if (attachedController && currentlyInteracting)
        {
            posDelta = attachedController.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;


            rotationDelta = attachedController.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;

        }


    }

    public void BeginInteraction(mainController wand)
    {

        attachedController = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);
        currentlyInteracting = true;
    }

    public void EndInteraction(mainController wand)
    {
        if (wand == attachedController)
        {
            attachedController = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}
