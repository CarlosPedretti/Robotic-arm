using Mono.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using QFSW.QC;
using QFSW.QC.Actions;

public class ArmController : MonoBehaviour
{
    [Header("Rigidbodys")]
    [SerializeField] private Rigidbody baseRotorRb;
    [SerializeField] private Rigidbody arm1Rb;
    [SerializeField] private Rigidbody arm2Rb;
    [SerializeField] private Rigidbody arm3Rb;
    [SerializeField] private Rigidbody handRotorRb;

    [Header("Ranges")]
    [SerializeField] private Range rangeBaseRotor;
    [SerializeField] private Range rangeArm1;
    [SerializeField] private Range rangeArm2;
    [SerializeField] private Range rangeArm3;
    [SerializeField] private Range rangeHandRotor;

    [Header("Other")]
    [SerializeField] private float period = 5;
    [SerializeField] private TMP_InputField inputInstructions;
    [SerializeField] private string instructionsString;


    private void Update()
    {
        
    }

    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
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


    [Command("move", MonoTargetType.All)]
    public void MoveCommand(string instruction)
    {
        StartCoroutine(InterpretInstructions(instruction));
    }


    public IEnumerator InterpretInstructions(string instruction)
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



    public IEnumerator RealizarMovimiento(string eje, float grados)
    {
        float rotationTime = (period / 360) * Mathf.Abs(grados);

        switch (eje)
        {
            case "M1":
                EstablishPosibleRotationY(grados, baseRotorRb.rotation.eulerAngles.y, rangeBaseRotor, baseRotorRb, rotationTime, eje);
                break;

            case "M2":
                EstablishPosibleRotationZ(grados, arm1Rb.rotation.eulerAngles.z, rangeArm1, arm1Rb, rotationTime, eje);
                break;

            case "M3":
                EstablishPosibleRotationZ(grados, arm2Rb.rotation.eulerAngles.z, rangeArm2, arm2Rb, rotationTime, eje);
                break;

            case "M4":
                EstablishPosibleRotationZ(grados, arm3Rb.rotation.eulerAngles.z, rangeArm3, arm3Rb, rotationTime, eje);
                break;

            case "M5":
                EstablishPosibleRotationX(grados, handRotorRb.rotation.eulerAngles.x, rangeHandRotor, handRotorRb, rotationTime, eje);
                break;
        }

        yield return new WaitForSeconds(rotationTime);
    }


    //No se me ocurrio otra forma para cambiar la variable "Degrees" dentro del new Vector3, son las 2 A.M, me re queme... Ma;ana sera otro dia.
    private void EstablishPosibleRotationX(float degrees, float partActualRotation, Range range, Rigidbody rb, float rotationTime, string axis)
    {
        float newPosibleRange = partActualRotation + degrees;
        if (IsOnRange(newPosibleRange, range))
        {
            rb.DORotate(new Vector3(degrees, 0, 0), rotationTime, RotateMode.LocalAxisAdd);
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {axis} está fuera del rango permitido ({range.min} a {range.max}). No se realizará la rotación.");
        }
    }

    private void EstablishPosibleRotationY(float degrees, float partActualRotation, Range range, Rigidbody rb, float rotationTime, string axis)
    {
        float newPosibleRange = partActualRotation + degrees;
        if (IsOnRange(newPosibleRange, range))
        {
            rb.DORotate(new Vector3(0, degrees, 0), rotationTime, RotateMode.LocalAxisAdd);
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {axis} está fuera del rango permitido ({range.min} a {range.max}). No se realizará la rotación.");
        }
    }
    private void EstablishPosibleRotationZ(float degrees, float partActualRotation, Range range, Rigidbody rb, float rotationTime, string axis)
    {
        float newPosibleRange = partActualRotation + degrees;
        if (IsOnRange(newPosibleRange, range))
        {
            rb.DORotate(new Vector3(0, 0, degrees), rotationTime, RotateMode.LocalAxisAdd);
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {axis} está fuera del rango permitido ({range.min} a {range.max}). No se realizará la rotación.");
        }
    }


    private bool IsOnRange(float value, Range range)
    {
        return value >= range.min && value <= range.max;
    }


    //Fuera de uso por el momento.
    public void StopCorrutines()
    {
        Debug.Log("StopCorrutines!");
        StopAllCoroutines();
    }
}
