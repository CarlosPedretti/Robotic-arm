using Mono.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using QFSW.QC;
using QFSW.QC.Actions;
using QFSW.QC.Suggestors.Tags;
using UnityEngine.InputSystem;

public class ArmController : MonoBehaviour
{
    [Header("General Configuration")]
    [SerializeField] private bool useInput;

    [Header("InputSystem")]
    [SerializeField] private List<GameObject> rotorsList = new List<GameObject>();
    [SerializeField] private int currentRotorIndex;
    private int previousRotorIndex;
    public float inputCurrentPartRotation = 0;
    [SerializeField] private float rotationSpeed;

    private float rotationInput;
    private float changeRotorInput;
    private PlayerInput playerInput;
    private bool isPressing;

    private InputCurrentRotation baseRotorCurrentRotation;
    private InputCurrentRotation arm1CurrentRotation;
    private InputCurrentRotation arm2CurrentRotation;
    private InputCurrentRotation arm3CurrentRotation;
    private InputCurrentRotation handCurrentRotation;
    public InputCurrentRotation GetCurrentBaseRotation() => baseRotorCurrentRotation;
    public InputCurrentRotation GetCurrentArm1Rotation() => arm1CurrentRotation;
    public InputCurrentRotation GetCurrentArm2Rotation() => arm2CurrentRotation;
    public InputCurrentRotation GetCurrentArm3Rotation() => arm3CurrentRotation;
    public InputCurrentRotation GetCurrentHandRotation() => handCurrentRotation;



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
    public struct InputCurrentRotation
    {
        public float currentRotation;
        public InputCurrentRotation(float rotation)
        {
            this.currentRotation = rotation;
        }
    }

    [System.Serializable]
    public struct InitialRotation
    {
        public float baseRotation { get; set; }
        public float arm1Rotation { get; set; }
        public float arm2Rotation { get; set; }
        public float arm3Rotation { get; set; }
        public float handRotation { get; set; }

    }

    private InitialRotation initialRotation;

    private void FixedUpdate()
    {
        RotationWithInput(rangeBaseRotor);
        ChangeRotor(rotorsList);
        /*if (Input.GetKeyDown("space"))
        {
            Debug.Log("Initial rotation: " + initialRotation.baseRotation + " " + "Actual rotation: " + baseRotor.localEulerAngles.z);
            Debug.Log("Rotation to initial rot: " + DegreesToInit(initialRotation.baseRotation, baseRotor.localEulerAngles.z));
        }*/
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        StoreInitialRotation();
        /*Debug.Log("M1: " + initialRotation.baseRotation);
        Debug.Log("M2: " + initialRotation.arm1Rotation);
        Debug.Log("M3: " + initialRotation.arm2Rotation);
        Debug.Log("M4: " + initialRotation.arm3Rotation);
        Debug.Log("M5: " + initialRotation.handRotation);*/
    }


    //[Command("move", MonoTargetType.All)]
    public void MoveCommand(string rotor, string degrees)
    {
        StartCoroutine(InterpreteInstructions(rotor, degrees));
    }

    //[Command("init", MonoTargetType.All)]
    public void ResetRotationCommand([Suggestions("ALL", "M1", "M2", "M3", "M4", "M5")] string rotor)
    {
        StartCoroutine(RotateToInitialRotation(rotor.ToUpper()));
    }

    //[Command("period", MonoTargetType.All)]
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
                EstablishPosibleRotation(degrees, baseRotor.localEulerAngles.z, rangeBaseRotor, baseRotor, rotationTime, "z", rotor);
                break;

            case "M2":
                EstablishPosibleRotation(degrees, arm1.localEulerAngles.y, rangeArm1, arm1, rotationTime, "y", rotor);
                break;

            case "M3":
                EstablishPosibleRotation(degrees, arm2.localEulerAngles.y, rangeArm2, arm2, rotationTime, "y", rotor);
                break;

            case "M4":
                EstablishPosibleRotation(degrees, arm3.localEulerAngles.y, rangeArm3, arm3, rotationTime, "y", rotor);
                break;

            case "M5":
                EstablishPosibleRotation(degrees, handRotor.localEulerAngles.x, rangeHandRotor, handRotor, rotationTime, "x", rotor);
                break;
        }

        yield return new WaitForSeconds(rotationTime);
    }

    public IEnumerator RotateToInitialRotation(string rotor)
    {
        float rotationTimeM1 = (period / 360) * Mathf.Abs(DegreesToInit(initialRotation.baseRotation, baseRotor.localEulerAngles.z));
        float rotationTimeM2 = (period / 360) * Mathf.Abs(DegreesToInit(initialRotation.arm1Rotation, arm1.localEulerAngles.y));
        float rotationTimeM3 = (period / 360) * Mathf.Abs(DegreesToInit(initialRotation.arm2Rotation, arm2.localEulerAngles.y));
        float rotationTimeM4 = (period / 360) * Mathf.Abs(DegreesToInit(initialRotation.arm3Rotation, arm3.localEulerAngles.y));
        float rotationTimeM5 = (period / 360) * Mathf.Abs(DegreesToInit(initialRotation.handRotation, handRotor.localEulerAngles.x));

        float totalRotationTime = rotationTimeM1 + rotationTimeM2 + rotationTimeM3 + rotationTimeM4 + rotationTimeM5;

        switch (rotor)
        {
            case "M1":
                EstablishPosibleRotation(DegreesToInit(initialRotation.baseRotation, baseRotor.localEulerAngles.z), baseRotor.localEulerAngles.z, rangeBaseRotor, baseRotor, rotationTimeM1, "z", rotor);
                break;

            case "M2":
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm1Rotation, arm1.localEulerAngles.y), arm1.localEulerAngles.y, rangeArm1, arm1, rotationTimeM2, "y", rotor);
                break;

            case "M3":
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm2Rotation, arm2.localEulerAngles.y), arm2.localEulerAngles.y, rangeArm2, arm2, rotationTimeM3, "y", rotor);
                break;

            case "M4":
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm3Rotation, arm3.localEulerAngles.y), arm3.localEulerAngles.y, rangeArm3, arm3, rotationTimeM4, "y", rotor);
                break;

            case "M5":
                EstablishPosibleRotation(DegreesToInit(initialRotation.handRotation, handRotor.localEulerAngles.x), handRotor.localEulerAngles.x, rangeHandRotor, handRotor, rotationTimeM5, "x", rotor);
                break;

            case "ALL":

                EstablishPosibleRotation(DegreesToInit(initialRotation.baseRotation, baseRotor.localEulerAngles.z), baseRotor.localEulerAngles.z, rangeBaseRotor, baseRotor, rotationTimeM1, "z", rotor);
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm1Rotation, arm1.localEulerAngles.y), arm1.localEulerAngles.y, rangeArm1, arm1, rotationTimeM2, "y", rotor);
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm2Rotation, arm2.localEulerAngles.y), arm2.localEulerAngles.y, rangeArm2, arm2, rotationTimeM3, "y", rotor);
                EstablishPosibleRotation(DegreesToInit(initialRotation.arm3Rotation, arm3.localEulerAngles.y), arm3.localEulerAngles.y, rangeArm3, arm3, rotationTimeM4, "y", rotor);
                EstablishPosibleRotation(DegreesToInit(initialRotation.handRotation, handRotor.localEulerAngles.x), handRotor.localEulerAngles.x, rangeHandRotor, handRotor, rotationTimeM5, "x", rotor);
                break;
        }

        yield return new WaitForSeconds(totalRotationTime);
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
            Debug.LogWarning($"El valor de rotación para {rotor} está fuera del rango permitido ({range.min} a {range.max}). No se realizará la rotación.");
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

    private void StoreInitialRotation()
    {
        initialRotation.baseRotation = baseRotor.localEulerAngles.z;
        initialRotation.arm1Rotation = arm1.localEulerAngles.y;
        initialRotation.arm2Rotation = arm2.localEulerAngles.y;
        initialRotation.arm3Rotation = arm3.localEulerAngles.y;
        initialRotation.handRotation = handRotor.localEulerAngles.x;
    }

    private float DegreesToInit(float intiailRotation, float actualRotation)
    {

        float newRotation = ConvertToSignedAngle(intiailRotation) - ConvertToSignedAngle(actualRotation);

        return newRotation;
    }

    private void ChangePeriod(float value)
    {
        period = value;
    }




    //Movimiento con Input
    private void ChangeRotor(List<GameObject> rotorList)
    {
        if (useInput == true)
        {
            changeRotorInput = playerInput.actions["ChangeRotor"].ReadValue<float>();

            if (changeRotorInput == 1 && !isPressing || changeRotorInput == -1 && !isPressing)
            {
                isPressing = true;

                int direction = Mathf.RoundToInt(changeRotorInput);

                previousRotorIndex = currentRotorIndex;

                currentRotorIndex = (currentRotorIndex + direction + rotorList.Count) % rotorList.Count;

                GameObject previousRotor = rotorList[previousRotorIndex];

                Outline previousOutlineComponent = previousRotor.GetComponent<Outline>();

                if (previousOutlineComponent != null)
                {
                    previousOutlineComponent.enabled = false;
                }

                GameObject selectedRotor = rotorList[currentRotorIndex];

                Outline outlineComponent = selectedRotor.GetComponent<Outline>();

                if (outlineComponent != null)
                {
                    outlineComponent.enabled = true;

                    // outlineComponent.OutlineColor = Color.red;
                    // outlineComponent.OutlineWidth = 2f;
                }


            }

            if (changeRotorInput == 0)
            {
                isPressing = false;
            }
        }
        else
        {
            GameObject selectedRotor = rotorList[currentRotorIndex];

            Outline outlineComponent = selectedRotor.GetComponent<Outline>();

            if (outlineComponent != null)
            {
                outlineComponent.enabled = false;
            }
        }
    }
    private void RotationWithInput(Range range)
    {
        if (useInput == true)
        {
            rotationInput = playerInput.actions["Rotation"].ReadValue<float>();
            if (rotationInput != 0)
            {
                float adjustedRotationSpeed = rotationSpeed * 0.01f;

                float rawRotation = rotationInput * adjustedRotationSpeed * Time.fixedDeltaTime;
                Debug.Log("rawRotation: " + rawRotation);
                if (rawRotation > 0)
                {
                    inputCurrentPartRotation += Mathf.Ceil(rawRotation);
                    inputCurrentPartRotation = Mathf.Clamp(inputCurrentPartRotation, range.min, range.max);
                    Debug.Log("currentRotation: " + inputCurrentPartRotation);
                }
                else
                {
                    inputCurrentPartRotation += Mathf.Floor(rawRotation);
                    inputCurrentPartRotation = Mathf.Clamp(inputCurrentPartRotation, range.min, range.max);
                    Debug.Log("currentRotation: " + inputCurrentPartRotation);
                }

            }
        }
    }

}
