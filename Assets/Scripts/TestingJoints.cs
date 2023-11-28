using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestingJoints : MonoBehaviour
{
    [SerializeField] private float rotationVelocity = 10f;

    [SerializeField] private ConfigurableJoint baseRotorJoint;
    [SerializeField] private ConfigurableJoint arm1Joint;
    [SerializeField] private ConfigurableJoint arm2Joint;
    [SerializeField] private ConfigurableJoint arm3Joint;
    [SerializeField] private ConfigurableJoint handRotorJoint;

    [SerializeField] private Rigidbody baseRotorRb;
    [SerializeField] private Rigidbody arm1Rb;
    [SerializeField] private Rigidbody arm2Rb;
    [SerializeField] private Rigidbody arm3Rb;
    [SerializeField] private Rigidbody handRotorRb;

    [SerializeField] private float period = 5;
    [SerializeField] private TMP_InputField inputInstructions;
    [SerializeField] private string instructionsString;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //baseRotor.AddTorque(0, rotationVelocity * 2, 0, ForceMode.Force);
        }
    }

    private void ChangeRotationY(float degrees, Rigidbody rb, ConfigurableJoint configurableJoint)
    {
        Debug.Log("ChangeRotation Y");


        float currentRotation = rb.rotation.eulerAngles.y;

        float torque = rotationVelocity * Time.deltaTime;

        float rotationDifference = degrees - currentRotation;

        // Aplicar torque gradualmente usando una interpolación lineal
        float torqueMultiplier = Mathf.Clamp01(Mathf.Abs(rotationDifference) / 180f); // Ajusta el divisor según tus necesidades
        rb.AddTorque(0, torque * torqueMultiplier * Mathf.Sign(rotationDifference), 0, ForceMode.Force);

        SoftJointLimit limit = configurableJoint.angularYLimit;
        limit.limit = degrees;

        configurableJoint.angularYLimit = limit;

    }

    private void ChangeRotationZ(float degrees, Rigidbody rb, ConfigurableJoint configurableJoint)
    {
        Debug.Log("ChangeRotation Z");
        float torque = rotationVelocity * Time.deltaTime;

        rb.AddTorque(0, torque, 0, ForceMode.Force);

        SoftJointLimit limit = configurableJoint.angularZLimit;
        limit.limit = degrees;

        configurableJoint.angularZLimit = limit;

    }

    public void RunCommand()
    {
        if (inputInstructions.text != null)
        {
            instructionsString = inputInstructions.text;
            //hay que comprobar el formato correcto del comando
            //los comandos deben tener el formato"M1_45,M2_-30"
            StartCoroutine(InterpretInstructions(instructionsString));
        }
    }

    private IEnumerator InterpretInstructions(string instruction)
    {
        string[] movimientos = instruction.Split(',');

        foreach (string movimiento in movimientos)
        {
            string[] partes = movimiento.Split('_');

            if (partes.Length == 2)
            {
                string eje = partes[0];
                float grados = float.Parse(partes[1]);

                yield return StartCoroutine(RealizarMovimiento(eje, grados));

            }
            else
            {
                Debug.LogError("Formato de instrucción incorrecto: " + movimiento);
            }
        }
    }

    private IEnumerator RealizarMovimiento(string eje, float grados)
    {
        float rotationTime = (period / 360) * Mathf.Abs(grados);

        switch (eje)
        {
            case "M1":
                Debug.Log("Rotate Base");
                ChangeRotationY(grados, baseRotorRb, baseRotorJoint);
                break;

            case "M2":
                Debug.Log("Rotate Arm1");
                ChangeRotationZ(grados, arm1Rb, arm1Joint);
                break;

            case "M3":
                Debug.Log("Rotate Arm2");
                ChangeRotationZ(grados, arm2Rb, arm2Joint);
                break;

            case "M4":
                Debug.Log("Rotate Arm3");
                ChangeRotationZ(grados, arm3Rb, arm3Joint);
                break;

            case "M5":
                Debug.Log("Rotate Hand");
                ChangeRotationZ(grados, handRotorRb, handRotorJoint);
                break;

        }

        yield return new WaitForSeconds(rotationTime);
    }

    public void StopCorrutines()
    {
        Debug.Log("StopCorrutines!");
        StopAllCoroutines();
    }

}

