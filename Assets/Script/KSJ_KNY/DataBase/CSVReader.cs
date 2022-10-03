using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    public enum DataType
    {
        Int, Float, String 
    }


    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;
        
        var header = Regex.Split(lines[0], SPLIT_RE);
        var dataType = Regex.Split(lines[1], SPLIT_RE);        

        for (var i = 3; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                //object finalvalue = value;
                
                switch ((DataType)System.Enum.Parse(typeof(DataType), dataType[j].ToString()))
                {
                    case DataType.Int:
                        if (!value.ToString().Equals("-"))
                            entry[header[j]] = int.Parse(value);
                        else
                            entry[header[j]] = value;
                        break;

                    case DataType.Float:
                        if (!value.ToString().Equals("-"))
                            entry[header[j]] = float.Parse(value);
                        else
                            entry[header[j]] = value;
                        break;

                    default:
                        entry[header[j]] = value;
                        break;
                }
            }
            list.Add(entry);
        }
        return list;
    }
}
