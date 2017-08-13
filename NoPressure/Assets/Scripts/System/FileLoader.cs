using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

class DebugLogger
{
    static bool ErrorFound;
    static string ErrorString;
    static bool firstWrite = true;

    public static void Log(String logString)
    {
        ErrorFound = true;
        ErrorString = logString;
    }

    public static void LogAppend(String logString)
    {
        ErrorFound = true;
        ErrorString += logString;
    }

    public static void LogPrepend(String logString)
    {
        ErrorFound = true;
        ErrorString = logString + ErrorString;
    }

    public static bool IsErrorFound()
    {
        return ErrorFound;
    }

    public static void WriteAndFlush(String path)
    {
        if (firstWrite == true)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        if (ErrorFound == true)
        {
            using (System.IO.StreamWriter file =
                   new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(ErrorString);
            }
            ErrorFound = false;
            firstWrite = false;
            ErrorString = "";
        }
    }
}

public class FileLoader
{

    StreamReader theReader;
    string line;
    string fileName;
    int LineNumber = 0;

    public String[] getLineCommaDelim()
    {
        //using (theReader) {
        LineNumber++;
        line = theReader.ReadLine();
        if (line != null)
        {
            string[] entries = line.Split(',');
            if (entries.Length > 0)
                return entries;
            else
                theReader.Close();
            return null;
        }
        else
        {
            theReader.Close();
            return null;
        }
    }
    // Return a list of arrays each with size length x Width
    public List<int[,]> GetIntLineCommaDelim(int width, int height)
    {
        List<int[,]> returnVal = new List<int[,]>();

        int[] currentLine;

        currentLine = getIntLineCommaDelim();

        int widthDivisions = 0;

        // Find the number of arrays 
        for (int i = 0; i < currentLine.Length; i += width)
        {
            returnVal.Add(new int[width, height]);
            widthDivisions++;
        }

        int currentHeight = 0;
        int currentWidth = 0;
        int currentDivision = 0;

        int[,] currentArray;

        //currentLine = getIntLineCommaDelim();
        if (currentLine == null)
        {
            Debug.Log("found no lines left, returning null");
            return null;
        }

        while (currentLine != null)
        {
            currentWidth = -1;
            // Process current line
            for (int j = 0; j < currentLine.Length; j++)
            {
                currentWidth++;

                if (currentWidth == width)
                {
                    currentDivision++;
                    currentWidth = 0;
                }
                // Trouble Line 

                if (currentDivision < widthDivisions)
                {
                    currentArray = returnVal[currentDivision];

                    currentArray[currentWidth, currentHeight] = currentLine[j];
                    
                }
                else
                {
                    Debug.Log("Discarding: " + currentLine[j]);
                }

            }
            currentDivision = 0;

            currentHeight++;
            if (currentHeight == height)
                break;

            currentLine = getIntLineCommaDelim();
            if (currentLine == null)
            {
                currentLine = new int[width * widthDivisions];
                Debug.Log("Creating blank lines");
            }
        }
        return returnVal;
    }

    //public int[,] getIntLineCommaDelim(int length, int width)
    //{
    //    return new int[1,1];
    //}

    public int[] getIntLineCommaDelim()
    {
        line = theReader.ReadLine();
        LineNumber++;

        if (line != null)
        {
            string[] entries = line.Split(',');
            int[] intEntries = new int[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                try
                {
                    intEntries[i] = Convert.ToInt32(entries[i]);
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    intEntries[i] = 0;
                }
            }

            if (entries.Length > 0)
                return intEntries;
            else
                theReader.Close();

            return null;
        }
        else
        {
            theReader.Close();
            return null;
        }
    }

    public bool load(string lfileName)
    {
        // Handle any problems that might arise when reading the text
        try
        {
            fileName = lfileName;

            var fs = new FileStream(lfileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            theReader = new StreamReader(fs);
            return true;
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (FileNotFoundException e)
#pragma warning restore CS0168 // Variable is declared but never used
        {
            //Debug.LogException(e);
            return false;
        }
    }

}
