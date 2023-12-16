using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerL : MonoBehaviour
{
    [SerializeField] private NewArmController armController;
    [SerializeField] private Transform hand;
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.CompareTag("Object"))
        {
            armController.Hand_Touch_L = true;
            armController.GrabbedObject = collision.gameObject;
        }
    }


}