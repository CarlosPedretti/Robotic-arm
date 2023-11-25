using Mono.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class ArmController : MonoBehaviour
{
    [SerializeField] private Transform baseRotor;
    [SerializeField] private Transform arm1;
    [SerializeField] private Transform arm2;
    [SerializeField] private Transform arm3;
    [SerializeField] private Transform handRotor;

    [SerializeField] private float period = 5;
    [SerializeField] private TMP_InputField inputInstructions;
    [SerializeField] private string instructionsString;



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
                baseRotor.DORotate(new Vector3(0, grados, 0), rotationTime, RotateMode.LocalAxisAdd);
                break;

            case "M2":
                arm1.DORotate(new Vector3(0, 0, grados), rotationTime, RotateMode.LocalAxisAdd);
                break;

            case "M3":
                arm2.DORotate(new Vector3(0, 0, grados), rotationTime, RotateMode.LocalAxisAdd);
                break;

            case "M4":
                arm3.DORotate(new Vector3(0, 0, grados), rotationTime, RotateMode.LocalAxisAdd);
                break;

            case "M5":
                handRotor.DORotate(new Vector3(0, 0, grados), rotationTime, RotateMode.LocalAxisAdd);
                break;
        }

        yield return new WaitForSeconds(rotationTime);
    }

    public void StopCorrutines()
    {
        StopAllCoroutines();
    }
}
