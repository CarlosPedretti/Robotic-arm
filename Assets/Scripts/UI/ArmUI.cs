using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using static NewArmController;

public class ArmUI : MonoBehaviour
{
    [SerializeField] private NewArmController newArmController;

    [SerializeField] private RotorUI baseRotorUI;
    [SerializeField] private RotorUI arm1UI;
    [SerializeField] private RotorUI arm2UI;
    [SerializeField] private RotorUI arm3UI;
    [SerializeField] private RotorUI handRotorUI;


    [SerializeField] private TextMeshProUGUI selectedPartActualRotationText;
    [SerializeField] private Toggle canUseInput;



    [System.Serializable]
    public struct RotorUI
    {
        public Transform rotor;
        public TextMeshProUGUI rotationAndRangeText;
        public TextMeshProUGUI rotorNameText;
        public TMP_InputField rotorNameInputField;
        public TMP_InputField rangeMinInputField;
        public TMP_InputField rangeMaxInputField;
    }

    private void Start()
    {
        canUseInput.onValueChanged.AddListener(OnToggleChanged);
        UpdateRotationsUI(newArmController, EventArgs.Empty);
        newArmController.OnRotationChanged += UpdateRotationsUI;
    }

    void Update()
    {
        selectedPartActualRotationText.text = $"Rotation to add: {newArmController.inputCurrentPartDegrees:F0}°";
        UpdateRotationsUI(newArmController, EventArgs.Empty);
    }


    private void DisplayRotationAndRange(NewArmController.Rotor rotor, RotorUI rotorUi, string label)
    {
        float currentRotation = 0f;

        switch (rotor.axis.ToLower())
        {
            case "x":
                currentRotation = rotor.currentRotation;
                break;
            case "y":
                currentRotation = rotor.currentRotation;
                break;
            case "z":
            default:
                currentRotation = rotor.currentRotation;
                break;
        }

        rotorUi.rotationAndRangeText.text = $"{label}: {currentRotation:F0}°   Range: {rotor.range.min}° to {rotor.range.max}°";
    }

    private void DisplayOnlyRotation(NewArmController.Rotor rotor, RotorUI rotorUi, string label)
    {
        float currentRotation = 0f;

        switch (rotor.axis.ToLower())
        {
            case "x":
                currentRotation = rotor.currentRotation;
                break;
            case "y":
                currentRotation = rotor.currentRotation;
                break;
            case "z":
            default:
                currentRotation = rotor.currentRotation;
                break;
        }

        rotorUi.rotationAndRangeText.text = $"{label}: {currentRotation:F0}°";
    }

    public void ReadStringToRotorName(NewArmController.Rotor rotor, RotorUI rotorUI)
    {
        rotor.rotorName = rotorUI.rotorNameInputField.text.ToUpper();
    }

    public void ReadStringToRotorRange(NewArmController.Rotor rotor, RotorUI rotorUI)
    {
        float minRange, maxRange;
        int maxCharLimit = 3; 

        if (!string.IsNullOrEmpty(rotorUI.rangeMinInputField.text) && rotorUI.rangeMinInputField.text.Length <= maxCharLimit)
        {
            if (float.TryParse(rotorUI.rangeMinInputField.text, out minRange))
            {
                rotor.range.min = minRange;
            }
            else
            {
                //Debug.LogError("Error al convertir el valor mínimo del rango a número");
            }
        }

        if (!string.IsNullOrEmpty(rotorUI.rangeMaxInputField.text) && rotorUI.rangeMaxInputField.text.Length <= maxCharLimit)
        {
            if (float.TryParse(rotorUI.rangeMaxInputField.text, out maxRange))
            {
                rotor.range.max = maxRange;
            }
            else
            {
                //Debug.LogError("Error al convertir el valor máximo del rango a número");
            }
        }

    }

    public void OnRangeChangedUI()
    {
        ReadStringToRotorRange(newArmController.baseRotor, baseRotorUI);
        ReadStringToRotorRange(newArmController.arm1, arm1UI);
        ReadStringToRotorRange(newArmController.arm2, arm2UI);
        ReadStringToRotorRange(newArmController.arm3, arm3UI);
        ReadStringToRotorRange(newArmController.handRotor, handRotorUI);
    }

    public void OnNameChangedUI()
    {
        ReadStringToRotorName(newArmController.baseRotor, baseRotorUI);
        ReadStringToRotorName(newArmController.arm1, arm1UI);
        ReadStringToRotorName(newArmController.arm2, arm2UI);
        ReadStringToRotorName(newArmController.arm3, arm3UI);
        ReadStringToRotorName(newArmController.handRotor, handRotorUI);
    }

    public void OnToggleChanged(bool value)
    {
        newArmController.useInput = value;

        if (newArmController.useInput == true)
        {
            selectedPartActualRotationText.gameObject.SetActive(true);
        }
        else
        {
            selectedPartActualRotationText.gameObject.SetActive(false);
        }
    }

    public void UpdateRotationsUI(object sender, EventArgs e)
    {
        DisplayRotationAndRange(newArmController.baseRotor, baseRotorUI, "Rotation " + newArmController.baseRotor.rotorName.ToString());
        DisplayRotationAndRange(newArmController.arm1, arm1UI, "Rotation " + newArmController.arm1.rotorName.ToString());
        DisplayRotationAndRange(newArmController.arm2, arm2UI, "Rotation " + newArmController.arm2.rotorName.ToString());
        DisplayRotationAndRange(newArmController.arm3, arm3UI, "Rotation " + newArmController.arm3.rotorName.ToString());
        DisplayRotationAndRange(newArmController.handRotor, handRotorUI, "Rotation " + newArmController.handRotor.rotorName.ToString());
    }
}