using Mono.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using QFSW.QC;
using QFSW.QC.Actions;
using QFSW.QC.Suggestors.Tags;
using UnityEngine.InputSystem;
using Unity.Burst.Intrinsics;
using System.Linq;

public class NewArmController : MonoBehaviour
{
    #region Variables
    public event EventHandler OnRotationChanged;

    [Header("General Configuration")]
    [SerializeField] public bool useInput;
    [SerializeField] private float period = 5;
    [Space]

    [Header("InputSystem")]
    [SerializeField] private Dictionary<Rotor, GameObject> rotorDictionary = new Dictionary<Rotor, GameObject>();
    [SerializeField] private Rotor currentRotorSelected;
    [SerializeField] private float rotationSpeed;
    public float inputCurrentPartDegrees = 0;

    bool selectedRotor = false;

    private float rotationInput;

    private InputAndCheck baseRotorInput;
    private InputAndCheck arm1Input;
    private InputAndCheck arm2Input;
    private InputAndCheck arm3Input;
    private InputAndCheck handRotorInput;


    private float executeMotionInput;
    private PlayerInput playerInput;
    private bool isPressing;
    private bool isPressing2;
    [Space]

    [Header("Rotors Config")]
    [SerializeField] public Rotor baseRotor;
    [SerializeField] public Rotor arm1;
    [SerializeField] public Rotor arm2;
    [SerializeField] public Rotor arm3;
    [SerializeField] public Rotor handRotor;

    #endregion

    #region Structs and Classes
    [System.Serializable]
    public class Rotor
    {
        public GameObject rotorObject;
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

    [System.Serializable]
    public struct InputAndCheck
    {
       public float input;
       public bool check;
    }
    #endregion

    #region Awake, Start and Update
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        StoreAllInitialRotations();

        AddAllObjectsToDictionary();

    }

    private void FixedUpdate()
    {
        UpdateAllCurrentRotations();

        SelectRotor();
    }
    #endregion

    #region Commands
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
    #endregion


    #region Logic for command Simulation
    public IEnumerator InterpreteInstructions(string rotor, string degrees)
    {

        yield return StartCoroutine(ProduceMovement(rotor.ToUpper(), float.Parse(degrees)));

        Logger.Log($"{rotor} {degrees}");
    }

    public IEnumerator ProduceMovement(string rotor, float degrees)
    {
        float rotationTime = CalculateRotateTime(degrees);

        if (rotor == baseRotor.rotorName)
        {
            EstablishPosibleRotation(degrees, rotationTime, baseRotor);
        }
        else if (rotor == arm1.rotorName)
        {
            EstablishPosibleRotation(degrees, rotationTime, arm1);
        }
        else if (rotor == arm2.rotorName)
        {
            EstablishPosibleRotation(degrees, rotationTime, arm2);
        }
        else if (rotor == arm3.rotorName)
        {
            EstablishPosibleRotation(degrees, rotationTime, arm3);
        }
        else if (rotor == handRotor.rotorName)
        {
            EstablishPosibleRotation(degrees, rotationTime, handRotor);
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

        if (rotor == baseRotor.rotorName)
        {
            EstablishPosibleRotation(DegreesToInit(baseRotor), rotationTimeM1, baseRotor);
        }
        else if (rotor == arm1.rotorName)
        {
            EstablishPosibleRotation(DegreesToInit(arm1), rotationTimeM2, arm1);
        }
        else if (rotor == arm2.rotorName)
        {
            EstablishPosibleRotation(DegreesToInit(arm2), rotationTimeM3, arm2);
        }
        else if (rotor == arm3.rotorName)
        {
            EstablishPosibleRotation(DegreesToInit(arm3), rotationTimeM4, arm3);
        }
        else if (rotor == handRotor.rotorName)
        {
            EstablishPosibleRotation(DegreesToInit(handRotor), rotationTimeM5, handRotor);
        }
        else if (rotor == "ALL")
        {
            EstablishPosibleRotation(DegreesToInit(baseRotor), rotationTimeM1, baseRotor);
            EstablishPosibleRotation(DegreesToInit(arm1), rotationTimeM2, arm1);
            EstablishPosibleRotation(DegreesToInit(arm2), rotationTimeM3, arm2);
            EstablishPosibleRotation(DegreesToInit(arm3), rotationTimeM4, arm3);
            EstablishPosibleRotation(DegreesToInit(handRotor), rotationTimeM5, handRotor);
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
                    rotor.rotor.DORotate(new Vector3(degrees, 0, 0), rotationTime, RotateMode.LocalAxisAdd).OnComplete(() => OnRotationChanged?.Invoke(this, EventArgs.Empty));
                    break;
                case "y":
                    rotor.rotor.DORotate(new Vector3(0, degrees, 0), rotationTime, RotateMode.LocalAxisAdd).OnComplete(() => OnRotationChanged?.Invoke(this, EventArgs.Empty));
                    break;
                case "z":
                default:
                    rotor.rotor.DORotate(new Vector3(0, 0, degrees), rotationTime, RotateMode.LocalAxisAdd).OnComplete(() => OnRotationChanged?.Invoke(this, EventArgs.Empty));
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"El valor de rotación para {rotor.rotorName} está fuera del rango permitido ({rotor.range.min} a {rotor.range.max}). No se realizará la rotación.");
        }

        //OnRotationChanged?.Invoke(this, EventArgs.Empty);
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

    private void StoreAllInitialRotations()
    {
        baseRotor.initialRotation = StoreInitialRotation(baseRotor);
        arm1.initialRotation = StoreInitialRotation(arm1);
        arm2.initialRotation = StoreInitialRotation(arm2);
        arm3.initialRotation = StoreInitialRotation(arm3);
        handRotor.initialRotation = StoreInitialRotation(handRotor);
    }

    private void UpdateAllCurrentRotations()
    {
        baseRotor.currentRotation = UpdateCurrentRotation(baseRotor);
        arm1.currentRotation = UpdateCurrentRotation(arm1);
        arm2.currentRotation = UpdateCurrentRotation(arm2);
        arm3.currentRotation = UpdateCurrentRotation(arm3);
        handRotor.currentRotation = UpdateCurrentRotation(handRotor);
    }

    private void AddAllObjectsToDictionary()
    {
        rotorDictionary.Add(baseRotor, baseRotor.rotorObject);
        rotorDictionary.Add(arm1, arm1.rotorObject);
        rotorDictionary.Add(arm2, arm2.rotorObject);
        rotorDictionary.Add(arm3, arm3.rotorObject);
        rotorDictionary.Add(handRotor, handRotor.rotorObject);
    }
    #endregion



    #region Logic with New Input System
    private void SelectRotor()
    {
        if (useInput == true)
        {  
            CheckInputSelectRotor(baseRotorInput, 0, "ChangeBaseRotor");
            CheckInputSelectRotor(arm1Input, 1, "ChangeArm1Rotor");
            CheckInputSelectRotor(arm2Input, 2, "ChangeArm2Rotor");
            CheckInputSelectRotor(arm3Input, 3, "ChangeArm3Rotor");
            CheckInputSelectRotor(handRotorInput, 4, "ChangeHandRotor");
        }

    }

    private void CheckInputSelectRotor(InputAndCheck inputAndCheck, int rotorIndex, string actions)
    {
        inputAndCheck.input = playerInput.actions[$"{actions}"].ReadValue<float>();
        if (inputAndCheck.input != 0)
        {
            if (rotorDictionary.ContainsKey(currentRotorSelected))
            {
                rotorDictionary[currentRotorSelected].GetComponent<Outline>().enabled = false;
            }
            inputAndCheck.check = true;
            selectedRotor = true;
            currentRotorSelected = GetRotorByIndex(rotorIndex);
            rotorDictionary[currentRotorSelected].GetComponent<Outline>().enabled = true;

        }
        else
        {
            inputAndCheck.check = false;

        }

        if (selectedRotor)
        {
            RotationWithInput(currentRotorSelected);

        }
    }

    private Rotor GetRotorByIndex(int index)
    {
        if (rotorDictionary.Count == 0)
        {
            return default(Rotor);
        }

        index = (index + rotorDictionary.Count) % rotorDictionary.Count;
        return rotorDictionary.Keys.ElementAt(index);
    }

 private void RotationWithInput(Rotor rotor)
 {
     if (useInput == true)
     {
         rotationInput = playerInput.actions["Rotation"].ReadValue<float>();
         if (rotationInput != 0)
         {
             float adjustedRotationSpeed = rotationSpeed * 0.00001f;

             float rawRotation = rotationInput * adjustedRotationSpeed * Time.fixedDeltaTime;
             if (rawRotation > 0)
             {
                 inputCurrentPartDegrees += Mathf.Ceil(rawRotation);
                 inputCurrentPartDegrees = Mathf.Clamp(inputCurrentPartDegrees, rotor.range.min, rotor.range.max);
                
             }
             else
             {
                 inputCurrentPartDegrees += Mathf.Floor(rawRotation);
                 inputCurrentPartDegrees = Mathf.Clamp(inputCurrentPartDegrees, rotor.range.min, rotor.range.max);
             }

         }

         executeMotionInput = playerInput.actions["ExecuteMotion"].ReadValue<float>();
         if(executeMotionInput != 0 )
         {
             if (executeMotionInput == 1 && !isPressing2)
             {
                 isPressing2 = true;

                 float rotationTime = CalculateRotateTime(inputCurrentPartDegrees);
                 EstablishPosibleRotation(inputCurrentPartDegrees, rotationTime, rotor);
                 Logger.Log($"{rotor.rotorName} {inputCurrentPartDegrees}");
                 rotorDictionary[currentRotorSelected].GetComponent<Outline>().enabled = false;
                 selectedRotor = false;
                 currentRotorSelected = default(Rotor);
                 inputCurrentPartDegrees = 0;
             }

         }

         if (executeMotionInput == 0)
         {
             isPressing2 = false;
         }

     }
 }
    #endregion
}
