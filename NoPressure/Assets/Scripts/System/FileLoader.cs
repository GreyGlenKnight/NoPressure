using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;  
using System;

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

	public static void WriteAndFlush()
	{
		if (firstWrite == true) {
			if (System.IO.File.Exists(@"C:\Users\Greg\Documents\Pokemon\Assets\ImportData\DebugLog.csv"))
				System.IO.File.Delete(@"C:\Users\Greg\Documents\Pokemon\Assets\ImportData\DebugLog.csv");
		}

		if (ErrorFound == true) {
			using (System.IO.StreamWriter file = 
			       new System.IO.StreamWriter(@"C:\Users\Greg\Documents\Pokemon\Assets\ImportData\DebugLog.csv", true))
			{
				file.WriteLine(ErrorString);
			}
			ErrorFound = false;
			firstWrite = false;
			ErrorString = "";
		}
	}

}


public class FileLoader {

	StreamReader theReader;
	string line;
    string fileName;
    int LineNumber = 0;

	public String[] getLineCommaDelim()
	{
        //using (theReader) {
        LineNumber++;
            line = theReader.ReadLine ();
			if (line != null) {
				string[] entries = line.Split (',');
				if (entries.Length > 0)
					return entries;
				else
					theReader.Close();
					return null;
			}
			else{
				theReader.Close();
				return null;
			}
		//}
	}

    public int[] getIntLineCommaDelim()
    {
        //using (theReader) {
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
                catch(Exception e)
                {
                    //Debug.Log("Not an Int: "+ entries[i] + ",Line: " + LineNumber + ",Col: " + i + ",file: "+ fileName);
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
        //}
    }

    public bool load(string lfileName)
	{
		
		// Handle any problems that might arise when reading the text
		try
		{
            //Debug.Log("Attempting to load file: " + lfileName);
            //string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            fileName = lfileName;

			var fs = new FileStream(lfileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); 
			theReader = new StreamReader(fs);
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
//			using (theReader)
//			{
//				// While there's lines left in the text file, do this:
//				do
//				{
//					//line = theReader.ReadLine();
//					
//					if (line != null)
//					{
//						// Do whatever you need to do with the text line, it's a string now
//						// In this example, I split it into arguments based on comma
//						// deliniators, then send that array to DoStuff()
//						string[] entries = line.Split(',');
//						if (entries.Length > 0)
//							doStuff(entries);
//					}
//				}
//				while (line != null);
//				// Done reading, close the reader and return true to broadcast success    
//				theReader.Close();
				return true;
			//}
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			//Debug.LogException(e,this);

			return false;
		}
	}


//	private void doStuff(string[] entries)
//	{
//		//levelText.Text = "";
//
//		if (entries != null & entries.Length > 0) 
//		{
//			for(int i = 0 ; i< entries.Length; i++)
//			{
//				//levelText.Text = levelText.Text + ";" + entries[i];
//				Debug.Log (entries[0]);
//			}
//		}
//			
//	}
	
}
