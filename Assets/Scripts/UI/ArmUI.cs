using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArmUI : MonoBehaviour
{
    [SerializeField] private ArmController armController;
    [SerializeField] private NewArmController newArmController;

    [SerializeField] private Transform baseRotor;
    [SerializeField] private Transform arm1;
    [SerializeField] private Transform arm2;
    [SerializeField] private Transform arm3;
    [SerializeField] private Transform handRotor;

    [SerializeField] private TextMeshProUGUI baseRotorText;
    [SerializeField] private TextMeshProUGUI arm1Text;
    [SerializeField] private TextMeshProUGUI arm2Text;
    [SerializeField] private TextMeshProUGUI arm3Text;
    [SerializeField] private TextMeshProUGUI handRotorText;

    [SerializeField] private TextMeshProUGUI probando;

    void Update()
    {
        UpdateText();

    }

    private float ConvertToSignedAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        return angle;
    }


    public void UpdateText()
    {
        probando.text = newArmController.inputCurrentPartRotation.ToString();

        DisplayRotationAndRange(newArmController.baseRotor, baseRotorText, "Rotation M1");
        DisplayRotationAndRange(newArmController.arm1, arm1Text, "Rotation M2");
        DisplayRotationAndRange(newArmController.arm2, arm2Text, "Rotation M3");
        DisplayRotationAndRange(newArmController.arm3, arm3Text, "Rotation M4");
        DisplayRotationAndRange(newArmController.handRotor, handRotorText, "Rotation M5");
    }

    private void DisplayRotationAndRange(NewArmController.Rotor rotor, TextMeshProUGUI textComponent, string label)
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

        textComponent.text = $"{label}: {currentRotation:F0}°   Range: {rotor.range.min}° to {rotor.range.max}°";
    }

    private void DisplayOnlyRotation(NewArmController.Rotor rotor, TextMeshProUGUI textComponent, string label)
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

        textComponent.text = $"{label}: {currentRotation:F0}°";
    }

}
