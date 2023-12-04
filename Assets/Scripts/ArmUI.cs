using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArmUI : MonoBehaviour
{
    [SerializeField] private ArmController armController;

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
        DisplayRotationAndRange(baseRotor, armController.GetRangeBaseRotor(), baseRotorText, "Rotation M1", "z");
        DisplayRotationAndRange(arm1, armController.GetRangeArm1(), arm1Text, "Rotation M2", "y");
        DisplayRotationAndRange(arm2, armController.GetRangeArm2(), arm2Text, "Rotation M3", "y");
        DisplayRotationAndRange(arm3, armController.GetRangeArm3(), arm3Text, "Rotation M4", "y");
        DisplayRotationAndRange(handRotor, armController.GetRangeHandRotor(), handRotorText, "Rotation M5", "x");
    }

    private void DisplayRotationAndRange(Transform motor, ArmController.Range range, TextMeshProUGUI textComponent, string label, string axis)
    {
        float currentRotation = 0f;

        switch (axis.ToLower())
        {
            case "x":
                currentRotation = ConvertToSignedAngle(motor.localEulerAngles.x);
                break;
            case "y":
                currentRotation = ConvertToSignedAngle(motor.localEulerAngles.y);
                break;
            case "z":
            default:
                currentRotation = ConvertToSignedAngle(motor.localEulerAngles.z);
                break;
        }

        textComponent.text = $"{label}: {currentRotation:F0}°   Range: {range.min}° to {range.max}°";
    }

}
