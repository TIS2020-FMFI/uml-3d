using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AnimationScriptCreator : Singleton<AnimationScriptCreator>
{
    public string fileName;
    private string[] callsArray;
    public string ScriptText = "";

    public void initConstructing()
    {
        string[] lines = readFile(getFileName(fileName));

        foreach (string line in lines)
        {
            string fname = getFileName(line.Remove(line.Length - 1));
            constructScript(fname);
        }
    }

    public void constructScript(string currentFile)
    {
        string[] lines = readFile(currentFile);

        foreach (string line in lines)
        {
            if (line.IndexOf(' ') < 0)
            {
                ScriptText += line + '\n';
                continue;
            }

            bool skipLine = false;
            string[] words = line.Split(' ');

            for (int i=0; i<words.Length; i++)
            {
                if (words[i] == "to")
                {
                    string toFile = getFileName(words[i + 1]);
                    skipLine = true;
                    ScriptText += line + '\n';
                    constructScript(toFile);
                    break;
                }
            }

            if (skipLine == false)
            {
                ScriptText += line + '\n';
            }
        }
    }

    public string getFileName(string method)
    {
        string result = method;
        result = result.Remove(result.IndexOf(":"), 1).Replace(":", "_");
        return result;
    }

    public string[] readFile(string fname)
    {
        try
        {
            return File.ReadAllLines(@"Methods/" + fname + ".oal");
        }
        catch (IOException e)
        {
            return new string[0];
        }
    }
}
