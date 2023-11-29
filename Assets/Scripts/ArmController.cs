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
    [SerializeField] private Transform baseRotor;
    [SerializeField] private Transform arm1;
    [SerializeField] private Transform arm2;
    [SerializeField] private Transform arm3;
    [SerializeField] private Transform handRotor;

    [Header("Ranges")]
    [SerializeField] private Range rangeBaseRotor;
    [SerializeField] private Range rangeArm1;
    [SerializeField] private Range rangeArm2;
    [SerializeField] private Range rangeArm3;
    [SerializeField] private Range rangeHandRotor;

    public Range GetRangeBaseRotor() => rangeBaseRotor;
    public Range GetRangeArm1() => rangeArm1;
    public Range GetRangeArm2() => rangeArm2;
    public Range GetRangeArm3() => rangeArm3;
    public Range GetRangeHandRotor() => rangeHandRotor;


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
            StartCoroutine(InterpreteInstructions(instructionsString));
        }
    }


    [Command("move", MonoTargetType.All)]
    public void MoveCommand(string instruction)
    {
        StartCoroutine(InterpreteInstructions(instruction));
    }


    public IEnumerator InterpreteInstructions(string instruction)
    {
        string instructionUpperCase = instruction.ToUpper();

        string[] movimientos = instructionUpperCase.Split(',');

        foreach (string movimiento in movimientos)
        {
            string[] partes = movimiento.Split('_');

            if (partes.Length == 2)
            {
                string eje = partes[0];
                float grados = float.Parse(partes[1]);

                yield return StartCoroutine(ProduceMovement(eje, grados));

            }
            else
            {
                Debug.LogError("Formato de instrucción incorrecto: " + movimiento);
            }
        }
    }


    public IEnumerator ProduceMovement(string rotor, float degrees)
    {
        float rotationTime = (period / 360) * Mathf.Abs(degrees);

        switch (rotor)
        {
            case "M1":
                EstablishPosibleRotation(degrees, baseRotor.localEulerAngles.y, rangeBaseRotor, baseRotor, rotationTime,"y", rotor);
                break;

            case "M2":
                EstablishPosibleRotation(degrees, arm1.localEulerAngles.z, rangeArm1, arm1, rotationTime, "z", rotor);
                break;

            case "M3":
                EstablishPosibleRotation(degrees, arm2.localEulerAngles.z, rangeArm2, arm2, rotationTime, "z", rotor);
                break;

            case "M4":
                EstablishPosibleRotation(degrees, arm3.localEulerAngles.z, rangeArm3, arm3, rotationTime, "z", rotor);
                break;

            case "M5":
                EstablishPosibleRotation(degrees, handRotor.localEulerAngles.x, rangeHandRotor, handRotor, rotationTime, "x", rotor);
                break;
        }

        yield return new WaitForSeconds(rotationTime);
    }


    private void EstablishPosibleRotation(float degrees, float partActualRotation, Range range, Transform transform, float rotationTime, string axis, string rotor)
    {
        float newPartActualRotation = ConvertToSignedAngle(partActualRotation);

        float newPosibleRange = newPartActualRotation + degrees;
        if (IsOnRange(newPosibleRange, range))
        {
            switch (axis.ToLower())
            {
                case "x":
                    transform.DORotate(new Vector3(degrees, 0, 0), rotationTime, RotateMode.LocalAxisAdd);
                    break;
                case "y":
                    transform.DORotate(new Vector3(0, degrees, 0), rotationTime, RotateMode.LocalAxisAdd);
                    break;
                case "z":
                default:
                    transform.DORotate(new Vector3(0, 0, degrees), rotationTime, RotateMode.LocalAxisAdd);
                    break;
            }
            /*Debug.LogWarning("partActualRotation: " + partActualRotation);
            Debug.LogWarning("newPartActualRotation: " + newPartActualRotation);
            Debug.LogWarning("newPosibleRange: " + newPosibleRange);
            Debug.LogWarning("IsOnRange: " + IsOnRange(newPosibleRange, range));*/
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {rotor} está fuera del rango permitido ({range.min} a {range.max}). No se realizará la rotación.");
            /*Debug.LogWarning("partActualRotation: " + partActualRotation);
            Debug.LogWarning("newPartActualRotation: " + newPartActualRotation);
            Debug.LogWarning("newPosibleRange: " + newPosibleRange);
            Debug.LogWarning("IsOnRange: " + IsOnRange(newPosibleRange, range));*/
        }
    }

    private bool IsOnRange(float value, Range range)
    {
        return value >= range.min && value <= range.max;
    }

    private float ConvertToSignedAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        return angle;
    }

    //Fuera de uso por el momento.
    public void StopCorrutines()
    {
        Debug.Log("StopCorrutines!");
        StopAllCoroutines();
    }
}
