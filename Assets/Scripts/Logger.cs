using UnityEngine;
using System.IO;
public static class Logger
{

    [SerializeField] private static string logFilePath = $"Commands/Runtime.jmp";

    public static void Log(string message)
    {
        Debug.Log(message);
        WriteToFile(message);
    }

    private static void WriteToFile(string message)
    {
        string logDirectory = Path.GetDirectoryName(logFilePath);
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(message);
        }
    }
}