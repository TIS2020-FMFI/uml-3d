using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScriptParser : Singleton<ScriptParser>
{
    List<string> fromMethods;
    List<string> toMethods;
    List<string> across;
    private bool fromClass = true;
    private bool fromMethod = true;

    public void Parse(string text)
    {
        fromMethods = new List<string>();
        toMethods = new List<string>();
        across = new List<string>();

        string line;
        string[] pole = text.Split(';');

        foreach (string command in pole)
        {
            string[] sarray = command.Split(' ');
            if (sarray.Length == 7)
            {
                line = sarray[0] + " " + sarray[1] + " " + sarray[2];
                fromMethods.Add(line);
                line = sarray[3] + " " + sarray[4];
                toMethods.Add(line);
                line = sarray[5] + " " + sarray[6] + ";";
                across.Add(line);
            }
        }
    }

    public void HighlightClass(int index, TMP_InputField script)
    {
        if (fromClass)
        {
            string line = "<color=#9c2171>" + fromMethods[index].Replace("::", "</color >::");
            string text = "";
            for (int i=0; i<fromMethods.Count; i++)
            {
                if(i == index)
                {
                    text += line;
                } else
                {
                    text += fromMethods[i];
                }
                text += " " + toMethods[i] + " " + across[i];
            }
            script.text = text;
            fromClass = false;
        } else
        {
            string line = "<color=#9c2171>" + toMethods[index].Replace("::", "</color >::");
            string text = "";
            for (int i = 0; i < fromMethods.Count; i++)
            {
                text += fromMethods[i] + " ";
                if (i == index)
                {
                    text += line;
                }
                else
                {
                    text += toMethods[i];
                }
                text += " " + across[i];
            }
            script.text = text;
            index++;
            fromClass = true;
        }
    }

    public void HighlightMethod(int index, TMP_InputField script)
    {
        if (fromMethod)
        {
            string line = "<color=#9c2171>" + fromMethods[index].Replace("::", "</color>::<color=#24349c>");
            string text = "";
            for (int i = 0; i < fromMethods.Count; i++)
            {
                if (i == index)
                {
                    text += line + "</color>";
                }
                else
                {
                    text += fromMethods[i];
                }
                text += " " + toMethods[i] + " " + across[i];
            }
            script.text = text;
            fromMethod = false;
        }
        else
        {
            string line = "<color=#9c2171>" + toMethods[index].Replace("::", "</color>::<color=#24349c>");
            string text = "";
            for (int i = 0; i < fromMethods.Count; i++)
            {
                text += fromMethods[i] + " ";
                if (i == index)
                {
                    text += line + "</color>";
                }
                else
                {
                    text += toMethods[i];
                }
                text += " " + across[i];
            }
            script.text = text;
            index++;
            fromMethod = true;
        }
    }

    public void HighlightEdge(int index, TMP_InputField script)
    {
            string text = "";
            for (int i = 0; i < fromMethods.Count; i++)
            {
                text += fromMethods[i] + " " + toMethods[i] + " ";
                if (i == index)
                {
                    text += "<color=#329123>" + across[i] + "</color>";
                }
                else
                {
                    text += across[i];
                }
            }
            script.text = text;
            fromMethod = false;
    }


}
