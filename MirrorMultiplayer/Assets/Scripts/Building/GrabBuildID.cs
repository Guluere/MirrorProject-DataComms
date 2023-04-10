using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrabBuildID : MonoBehaviour
{
    public TextMeshProUGUI IDText;

    private void Start()
    {
        IDText.text = GetBuiltID();
    }

    string GetLine(string text, int lineNo)
    {
        string[] lines = text.Replace("\r", "").Split('\n');
        return lines.Length >= lineNo ? lines[lineNo - 1] : null;
    }

    public string GetBuiltID()
    {
        string WholeString = Resources.Load<TextAsset>("BuildInfo").ToString();
        string VersionNum = GetLine(WholeString, 0);
        string AlphaNum = GetLine(WholeString, 1);
        string BetaNum = GetLine(WholeString, 2);
        return "Game Version: " + VersionNum + "." + AlphaNum + "." + BetaNum;
    }    
}
