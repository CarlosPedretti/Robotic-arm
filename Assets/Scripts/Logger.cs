using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;

public static class Logger
{

    static DateTime currentDate = DateTime.Now;
    static string formattedDate = currentDate.ToString("dd-MM-yy");
    public static string logFilePath = $"Commands/Runtime_{formattedDate}.jmp";

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