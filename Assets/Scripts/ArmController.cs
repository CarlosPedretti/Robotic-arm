using Mono.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using QFSW.QC;
using QFSW.QC.Actions;
using QFSW.QC.Suggestors.Tags;

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

    [Command("move", MonoTargetType.All)]
    public void MoveCommand(string rotor, string degrees)
    {
        StartCoroutine(InterpreteInstructions(rotor, degrees));
    }

    //Todavia no programado!
    [Command("init", MonoTargetType.All)]
    public void ResetRotation([Suggestions("ALL", "M1", "M2", "M3", "M4", "M5")] string rotor)
    {

    }

    [Command("period", MonoTargetType.All)]
    public void PeriodCommand(float period)
    {
        ChangePeriod(period);
    }


    public IEnumerator InterpreteInstructions(string rotor, string degrees)
    {

        yield return StartCoroutine(ProduceMovement(rotor.ToUpper(), float.Parse(degrees)));

    }


    public IEnumerator ProduceMovement(string rotor, float degrees)
    {
        float rotationTime = (period / 360) * Mathf.Abs(degrees);


        switch (rotor)
        {
            case "M1":
                EstablishPosibleRotation(degrees, baseRotor.localEulerAngles.y, rangeBaseRotor, baseRotor, rotationTime,"z", rotor);
                break;

            case "M2":
                EstablishPosibleRotation(degrees, arm1.localEulerAngles.z, rangeArm1, arm1, rotationTime, "y", rotor);
                break;

            case "M3":
                EstablishPosibleRotation(degrees, arm2.localEulerAngles.z, rangeArm2, arm2, rotationTime, "y", rotor);
                break;

            case "M4":
                EstablishPosibleRotation(degrees, arm3.localEulerAngles.z, rangeArm3, arm3, rotationTime, "y", rotor);
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
        }
        else
        {
            Debug.LogWarning($"El valor de rotaci�n para {rotor} est� fuera del rango permitido ({range.min} a {range.max}). No se realizar� la rotaci�n.");
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

    private void ChangePeriod(float value)
    {
        period = value;
    }
}
