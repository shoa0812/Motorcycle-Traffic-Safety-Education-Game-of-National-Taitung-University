using UnityEngine;
using System.Collections.Generic;  // ✅ 為了使用 List<>
using TMPro;  // ✅ 必加！

public class ViolationLogger : MonoBehaviour
{
    public TMP_Text resultText;
    private List<string> violations = new List<string>();

    public void LogViolation(string message)
    {
        if (!violations.Contains(message))
        {
            violations.Add(message);
            
        }
    }

    public void ShowViolations()
    {
        string result = "\ud83d\udea8 \u9055\u898f\u884c\u70ba\u6e05\u55ae\uff1a\n";
        foreach (string v in violations)
        {
            result += "\u2022 " + v + "\n";
        }
        if (violations.Count == 0)
            result += "\u2705 \u5b8c\u7f8e\u99d5\u99db\uff01\u7121\u4efb\u4f55\u9055\u898f\u3002";

        if (resultText != null)
            resultText.text = result;
    }
    public bool HasViolation(string keyword)
    {
        foreach (var v in violations)
        {
            if (v.Contains(keyword))
                return true;
        }
        return false;
    }

}
