using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingJoints : MonoBehaviour
{
    public ConfigurableJoint configurableJoint;

    [SerializeField] private Rigidbody baseRotor;
    [SerializeField] private Rigidbody arm1;
    [SerializeField] private Rigidbody arm2;
    [SerializeField] private Rigidbody arm3;
    [SerializeField] private Rigidbody handRotor;

    public float rotationVelocity = 10f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            baseRotor.AddTorque(0, rotationVelocity, 0, ForceMode.Force);
        }
    }
}

