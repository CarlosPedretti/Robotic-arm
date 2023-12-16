using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerL : MonoBehaviour
{
    [SerializeField] private NewArmController armController;
    [SerializeField] private Transform hand;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finger"))
        {
            armController.HandIsClosing = false;
            Debug.Log("finger");


        }
        else if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("Touch");

            armController.Hand_Touch_L = true;
            armController.GrabbedObject = collision.gameObject;
        }
    }


}