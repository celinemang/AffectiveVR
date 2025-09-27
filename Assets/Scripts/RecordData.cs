using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public static class RecordData
{
    public static void SaveData(string Inputpath, string fileName, string data)
    {
        string path = Path.Combine(Inputpath, fileName + ".csv");

        //This text is added only once to the file
        if (!File.Exists(path))
        {

            //create a file to write to.
            string createText = Environment.NewLine;
            File.WriteAllText(path, createText);
        }

        //this text is always added, making the file longer over time if it is not deleted.
        File.AppendAllText(path, data);
    }
}
