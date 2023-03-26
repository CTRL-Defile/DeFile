using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    public enum DataType
    {
        Int, Float, String, Bool
    }

    static string[] header;
    static string[] dataType;

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read_Dictionary(string file)
    {
        if (header == null || dataType == null)
            return null;

        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        for (var i = 1; i < lines.Length; i++)
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
                        {
                            switch (value)
                            {
                                // bool
                                case "TRUE":    { entry[header[j]] = true;  }   break;
                                case "FALSE":   { entry[header[j]] = false; }   break;
                                // string
                                default:        { entry[header[j]] = value; }   break;
                            }
                        }
                        break;
                }
            }
            list.Add(entry);
        }

        return list;
    }

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;
        
        header = Regex.Split(lines[0], SPLIT_RE);
        dataType = Regex.Split(lines[1], SPLIT_RE); // !

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
                        switch (value)
                        {
                            // bool
                            case "TRUE":    { entry[header[j]] = true;  }   break;
                            case "FALSE":   { entry[header[j]] = false; }   break;
                            // string
                            default:        { entry[header[j]] = value; }   break;
                        }
                        break;
                }
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, object>> Read_OneHeader(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        Debug.Log(lines.Length);

        for (var i = 1; i < lines.Length; i++)
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
                        switch (value)
                        {
                            // bool
                            case "TRUE":    { entry[header[j]] = true;  }   break;
                            case "FALSE":   { entry[header[j]] = false; }   break;
                            // string
                            default:        { entry[header[j]] = value; }   break;
                        }
                        break;
                }
            }
            list.Add(entry);
        }
        return list;
    }
}

// 얘 안 써도 될 듯
public class LSY_CSV_Reader
{
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
        for (var i = 3; i < lines.Length; i++)  // 시작 위치는 csv 파일마다 다를 수 있으니 수정하면 됨
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}