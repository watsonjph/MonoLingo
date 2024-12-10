using UnityEngine;
using System.Diagnostics;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System;

public static class CDatabaseConnector
{
    private static string exePath = "C:\\Users\\Dyedw\\source\\repos\\KanaDB\\x64\\Debug\\KanaDB.exe";

    public static (bool success, int dayStreak, string lastPracticeDate) LoginUser(string username, string password)
    {
        string arguments = $"login {username} {password}";
        string result = RunProcess(arguments);

        // Trim the result and split by "|"
        string[] parts = result.Trim().Split('|');

        // Validate the output format
        if (parts.Length < 3)
        {
            return (false, 0, null); // Return default values
        }

        if (parts[0].ToLower() == "success")
        {
            int dayStreak = int.Parse(parts[1]);
            string lastPracticeDate = parts[2] == "NULL" ? null : parts[2]; // Convert "NULL" to null
            return (true, dayStreak, lastPracticeDate);
        }

        return (false, 0, null);
    }

    public static bool CreateUser(string username, string password)
    {
        // Call the C program with arguments
        string arguments = $"create {username} {password}";
        string result = RunProcess(arguments);

        return result.Trim().ToLower() == "success";
    }

    public static string RunProcess(string arguments)
    {
        // Start the C program as a process
        Process process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8; // Ensure UTF-8 is used
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

        process.Start();

        // Read the output from the C program
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }

    public static (string lastPracticeDate, int dayStreak) GetPracticeInfo(string username)
    {
        string arguments = $"getPracticeInfo {username}";

        string result = RunProcess(arguments);

        // Ensure the response starts with "success;"
        if (result.StartsWith("success;"))
        {
            string[] parts = result.Split(';');
            if (parts.Length == 3)
            {
                string lastPracticeDate = parts[1] == "None" ? null : parts[1];
                int dayStreak;
                if (int.TryParse(parts[2], out dayStreak))
                {
                    // Return the parsed values
                    return (lastPracticeDate, dayStreak);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Failed to parse DayStreak: {parts[2]}");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Unexpected response format: {result}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Failed response from KanaDB: {result}");
        }

        // Default fallback values
        return ("None", 0);
    }

    public static bool UpdatePracticeInfo(string username, string lastPracticeDate, int dayStreak)
    {
        string arguments = $"updatePracticeInfo {username} {lastPracticeDate} {dayStreak}";
        string result = RunProcess(arguments);
        return result.Trim() == "success";
    }

    public static int GetHighScore(string username)
    {
        string arguments = $"getHighScore {username}";
        string result = RunProcess(arguments);

        // Ensure the response starts with "success;"
        if (result.StartsWith("success;"))
        {
            string[] parts = result.Split(';');
            if (parts.Length == 2 && int.TryParse(parts[1], out int highScore))
            {
                return highScore;
            }
        }

        return 0; // Default high score if fetching fails
    }

    public static bool UpdateHighScore(string username, int highScore)
    {
        string arguments = $"updateHighScore {username} {highScore}";
        string result = RunProcess(arguments);

        if (result.Trim() == "success")
        {
            return true;
        }

        return false;
    }

}
