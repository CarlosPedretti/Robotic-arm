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

public class NewArmController : MonoBehaviour
{
    [Header("General Configuration")]
    [SerializeField] private bool useInput;
    [SerializeField] private float period = 5;
    [Space]

    [Header("InputSystem")]
    [SerializeField] private List<GameObject> rotorsList = new List<GameObject>();
    [SerializeField] private int currentRotorIndex;
    [SerializeField] private float rotationSpeed;
    public float inputCurrentPartRotation = 0;

    private int previousRotorIndex;
    private float rotationInput;
    private float changeRotorInput;
    private float executeMotionInput;
    private PlayerInput playerInput;
    private bool isPressing;
    [Space]

    [Header("Rotors Config")]
    [SerializeField] public Rotor baseRotor;
    [SerializeField] public Rotor arm1;
    [SerializeField] public Rotor arm2;
    [SerializeField] public Rotor arm3;
    [SerializeField] public Rotor handRotor;

    [System.Serializable]
    public struct Rotor
    {
        public Transform rotor;
        public string rotorName;
        public string axis;
        public Range range;
        public float initialRotation;
        public float currentRotation;

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


    private void FixedUpdate()
    {
        RotationWithInput(baseRotor);
        ChangeRotor(rotorsList);

        baseRotor.currentRotation = UpdateCurrentRotation(baseRotor);
        arm1.currentRotation = UpdateCurrentRotation(arm1);
        arm2.currentRotation = UpdateCurrentRotation(arm2);
        arm3.currentRotation = UpdateCurrentRotation(arm3);
        handRotor.currentRotation = UpdateCurrentRotation(handRotor);



    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        baseRotor.initialRotation = StoreInitialRotation(baseRotor);
        arm1.initialRotation = StoreInitialRotation(arm1);
        arm2.initialRotation = StoreInitialRotation(arm2);
        arm3.initialRotation = StoreInitialRotation(arm3);
        handRotor.initialRotation = StoreInitialRotation(handRotor);

    }


    [Command("move", MonoTargetType.All)]
    public void MoveCommand(string rotor, string degrees)
    {
        StartCoroutine(InterpreteInstructions(rotor, degrees));
    }

    [Command("init", MonoTargetType.All)]
    public void ResetRotationCommand([Suggestions("ALL", "M1", "M2", "M3", "M4", "M5")] string rotor)
    {
        StartCoroutine(RotateToInitialRotation(rotor.ToUpper()));
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
        float rotationTime = CalculateRotateTime(degrees);


        switch (rotor)
        {
            case "M1":
                EstablishPosibleRotation(degrees, rotationTime, baseRotor);
                break;

            case "M2":
                EstablishPosibleRotation(degrees, rotationTime, arm1);
                break;

            case "M3":
                EstablishPosibleRotation(degrees, rotationTime, arm2);
                break;

            case "M4":
                EstablishPosibleRotation(degrees, rotationTime, arm3);
                break;

            case "M5":
                EstablishPosibleRotation(degrees, rotationTime, handRotor);
                break;
        }

        yield return new WaitForSeconds(rotationTime);
    }

    public IEnumerator RotateToInitialRotation(string rotor)
    {
        float rotationTimeM1 = (period / 360) * Mathf.Abs(DegreesToInit(baseRotor));
        float rotationTimeM2 = (period / 360) * Mathf.Abs(DegreesToInit(arm1));
        float rotationTimeM3 = (period / 360) * Mathf.Abs(DegreesToInit(arm2));
        float rotationTimeM4 = (period / 360) * Mathf.Abs(DegreesToInit(arm3));
        float rotationTimeM5 = (period / 360) * Mathf.Abs(DegreesToInit(handRotor));

        float totalRotationTime = rotationTimeM1 + rotationTimeM2 + rotationTimeM3 + rotationTimeM4 + rotationTimeM5;

        switch (rotor)
        {
            case "M1":
                EstablishPosibleRotation(DegreesToInit(baseRotor), rotationTimeM1, baseRotor);
                break;

            case "M2":
                EstablishPosibleRotation(DegreesToInit(arm1), rotationTimeM2, arm1);
                break;

            case "M3":
                EstablishPosibleRotation(DegreesToInit(arm2), rotationTimeM3, arm2);
                break;

            case "M4":
                EstablishPosibleRotation(DegreesToInit(arm3), rotationTimeM4, arm3);
                break;

            case "M5":
                EstablishPosibleRotation(DegreesToInit(handRotor), rotationTimeM5, handRotor);
                break;

            case "ALL":

                EstablishPosibleRotation(DegreesToInit(baseRotor), rotationTimeM1, baseRotor);
                EstablishPosibleRotation(DegreesToInit(arm1), rotationTimeM2, arm1);
                EstablishPosibleRotation(DegreesToInit(arm2), rotationTimeM3, arm2);
                EstablishPosibleRotation(DegreesToInit(arm3), rotationTimeM4, arm3);
                EstablishPosibleRotation(DegreesToInit(handRotor), rotationTimeM5, handRotor);
                break;
        }

        yield return new WaitForSeconds(totalRotationTime);
    }




    private void EstablishPosibleRotation(float degrees, float rotationTime, Rotor rotor)
    {
        float newPartActualRotation = rotor.currentRotation;

        float newPosibleRange = newPartActualRotation + degrees;
        if (IsOnRange(newPosibleRange, rotor))
        {
            switch (rotor.axis.ToLower())
            {
                case "x":
                    rotor.rotor.DORotate(new Vector3(degrees, 0, 0), rotationTime, RotateMode.LocalAxisAdd);
                    break;
                case "y":
                    rotor.rotor.DORotate(new Vector3(0, degrees, 0), rotationTime, RotateMode.LocalAxisAdd);
                    break;
                case "z":
                default:
                    rotor.rotor.DORotate(new Vector3(0, 0, degrees), rotationTime, RotateMode.LocalAxisAdd);
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {rotor.rotorName} está fuera del rango permitido ({rotor.range.min} a {rotor.range.max}). No se realizará la rotación.");
        }
    }

    private bool IsOnRange(float value, Rotor rotor)
    {
        return value >= rotor.range.min && value <= rotor.range.max;
    }

    private float ConvertToSignedAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        return angle;
    }
    
    private float CalculateRotateTime(float degrees)
    {
        float rotationTime = (period / 360) * Mathf.Abs(degrees);
        return rotationTime;
    }

    private float StoreInitialRotation(Rotor rotor)
    {
        Vector3 eulerAngles = rotor.rotor.localEulerAngles;
        float axisValue = 0f;

        switch (rotor.axis)
        {
            case "x":
                axisValue = eulerAngles.x;
                break;
            case "y":
                axisValue = eulerAngles.y;
                break;
            case "z":
                axisValue = eulerAngles.z;
                break;

            default:

                break;
        }

        rotor.initialRotation = axisValue;
        rotor.initialRotation = ConvertToSignedAngle(rotor.initialRotation);
        return rotor.initialRotation;
    }

    private float UpdateCurrentRotation(Rotor rotor)
    {
        Vector3 eulerAngles = rotor.rotor.localEulerAngles;
        float axisValue = 0f;

        switch (rotor.axis)
        {
            case "x":
                axisValue = eulerAngles.x;
                break;
            case "y":
                axisValue = eulerAngles.y;
                break;
            case "z":
                axisValue = eulerAngles.z;
                break;

            default:

                break;
        }

        rotor.currentRotation = axisValue;
        rotor.currentRotation = ConvertToSignedAngle(rotor.currentRotation);
        return rotor.currentRotation;
    }

    private float DegreesToInit(Rotor rotor)
    {

        float newRotation = rotor.initialRotation - rotor.currentRotation;

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
    private void RotationWithInput(Rotor rotor)
    {
        if (useInput == true)
        {
            rotationInput = playerInput.actions["Rotation"].ReadValue<float>();
            if (rotationInput != 0)
            {
                float adjustedRotationSpeed = rotationSpeed * 0.0001f;

                float rawRotation = rotationInput * adjustedRotationSpeed * Time.fixedDeltaTime;
                Debug.Log("rawRotation: " + rawRotation);
                if (rawRotation > 0)
                {
                    inputCurrentPartRotation += Mathf.Ceil(rawRotation);
                    inputCurrentPartRotation = Mathf.Clamp(inputCurrentPartRotation, rotor.range.min, rotor.range.max);
                    Debug.Log("currentRotation: " + inputCurrentPartRotation);
                }
                else
                {
                    inputCurrentPartRotation += Mathf.Floor(rawRotation);
                    inputCurrentPartRotation = Mathf.Clamp(inputCurrentPartRotation, rotor.range.min, rotor.range.max);
                    Debug.Log("currentRotation: " + inputCurrentPartRotation);
                }

            }

            executeMotionInput = playerInput.actions["ExecuteMotion"].ReadValue<float>();
            if(executeMotionInput != 0 )
            {
                if (executeMotionInput == 1)
                {
                    float rotationTime = CalculateRotateTime(inputCurrentPartRotation);
                    EstablishPosibleRotation(inputCurrentPartRotation, rotationTime, rotor);
                }
            }

        }
    }

}
