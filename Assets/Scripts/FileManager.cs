using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    [SerializeField] private TMP_Text infoTx;
    [SerializeField] private TMP_InputField pathIf;
    [SerializeField] private Button uploadBt;
    [SerializeField] private Button downloadBt;
    [SerializeField] private Button runBt;
    private string infoText;
    private List<string> commandsTxt;

    private void Start()
    {
        commandsTxt = new List<string>();
        runBt.gameObject.SetActive(false);
    }

    public void OnClickUpload()
    {
        string sourcePath = pathIf.text;
        try
        {
            if (File.Exists(sourcePath))
            {
                using (StreamReader sr = new StreamReader(sourcePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        commandsTxt.Add(line);
                    }
                }
                runBt.gameObject.SetActive(true);
                infoText += $"File Uploaded: {sourcePath}\n";
            }
            else
            {
                infoText += $"Source File does not Exist\n";
            }
            infoTx.text = infoText;
        }
        catch
        {
            infoText += $"Error to Upload";
            infoTx.text = infoText;
        }
    }

    public void OnClickDownload()
    {
        string sourcePath = Logger.logFilePath;
        string destinationPath = pathIf.text;
        string fullPath = null;

        try
        {
            if (Directory.Exists(destinationPath))
            {
                fullPath = Path.Combine(destinationPath, "Runtime.jmp");
            }
            else
            {
                infoText += $"Directory does not Exist\n";
            }

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, fullPath, true);
                infoText += $"File Created: {fullPath}\n";
            }
            else
            {
                infoText += $"Source File does not Exist\n";
            }
            infoTx.text = infoText;
        }
        catch
        {
            infoText += $"Error to Download";
            infoTx.text = infoText;
        }
    }

    public void OnClickRun()
    {
       StartCoroutine(CommandsRuntime());
    }

    public IEnumerator CommandsRuntime()
    {
        Debug.Log("En test");
        NewArmController armController = GameObject.FindGameObjectWithTag("Arm").GetComponent<NewArmController>();
        foreach (string comand in commandsTxt)
        {
            string rotor = comand.Split(" ")[0];
            string degrees = comand.Split(" ")[1];
            infoText += $">>{comand}\n";
            infoTx.text = infoText;
            yield return StartCoroutine(armController.InterpreteInstructions(rotor, degrees));
        }
    }
}
