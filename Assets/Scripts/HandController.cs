using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private ArmController armController;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter!");

        if (other.gameObject.tag == "Ground")
        {
            if (armController != null)
            {
                armController.StopCorrutines();
            }
        }
    }
}
