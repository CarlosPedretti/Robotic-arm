using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerR : MonoBehaviour
{
    [SerializeField] private NewArmController armController;
    [SerializeField] private Rigidbody hand_Rb;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            armController.Hand_Touch_R = true;
            armController.GrabbedObject = collision.gameObject;
        }
    }
}