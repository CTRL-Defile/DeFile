using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;


public partial class CTRL_Buff : IDisposable
{
    public enum PRECONDITION_CLASS
    {
        NONE,
        HAVE_MORE
    }

    public enum PRECONDITION_TYPE
    {
        NONE,
        SYNERGY
    }

    public enum APPLY_CLASS
    {
        SYNERGY_ELF
    }

    public enum APPLY_TYPE
    {
        SPAWN_CHANGE,
        ATTACK_POINT
    }

    public enum RATIO_TYPE
    {
        PERCENT,
        FLAT
    }

    public enum DURATION_TYPE
    {
        STAGE
    }
}

[Serializable]
public class CTRL_Buff_Save : IDisposable
{
    public int Basic_index;
    public string Basic_tag;
    public string Basic_name;
    public int Basic_number;

    public string Basic_durationType;
    public float Basic_durationValue;

    //////////  Getter & Setter //////////
    public bool CTRL_Basic_GetIsSame(CTRL_Buff_Save _data)
    {
        bool res =  Basic_index.Equals(     _data.Basic_index   )   &&
                    Basic_name.Equals(      _data.Basic_name    )   &&
                    Basic_number.Equals(    _data.Basic_number  );

        //
        return res;
    }

    public CTRL_Buff.DURATION_TYPE CTRL_Basic_durationType { get { return (CTRL_Buff.DURATION_TYPE)Enum.Parse(typeof(CTRL_Buff.DURATION_TYPE), Basic_durationType); } }

    //////////  Method          //////////

    //////////  Default Method  //////////
    public CTRL_Buff_Save(Dictionary<string, object> _data)
    {
        Basic_index     = (int)_data[       "index" ];
        Basic_tag       = (string)_data[    "tag"   ];
        Basic_name      = (string)_data[    "name"  ];
        Basic_number    = (int)_data[       "num"   ];

        Basic_durationType  = (string)_data[    "duration_type" ];
        Basic_durationValue = (float)_data[     "duration_value"];
    }

    public CTRL_Buff_Save(CTRL_Buff_Save _save)
    {
        Basic_index     = _save.Basic_index;
        Basic_tag       = "" + _save.Basic_tag;
        Basic_name      = "" + _save.Basic_name;
        Basic_number    = _save.Basic_number;

        Basic_durationType  = _save.Basic_durationType;
        Basic_durationValue = _save.Basic_durationValue;
    }

    public void Dispose()
    {

    }
}

partial class CTRL_Buff
{
    public CTRL_Buff_Save Basic_data;

    public PRECONDITION_CLASS   Basic_preconditionClass;
    public PRECONDITION_TYPE    Basic_preconditionType;
    public int                  Basic_preconditionValue;

    public APPLY_CLASS  Basic_applyClass;
    public APPLY_TYPE   Basic_applyType;

    public RATIO_TYPE   Basic_ratioType;
    public int          Basic_ratioValue;

    public bool         Basic_isShop;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    public CTRL_Buff(Dictionary<string, object> _data)
    {
        Basic_data = new CTRL_Buff_Save(_data);

        Basic_preconditionClass = (PRECONDITION_CLASS)Enum.Parse(   typeof(PRECONDITION_CLASS), (string)_data[  "precondition_class"    ]);
        Basic_preconditionType  = (PRECONDITION_TYPE)Enum.Parse(    typeof(PRECONDITION_TYPE),  (string)_data[  "precondition_type"     ]);
        Basic_preconditionValue = (int)_data["precondition_value"];

        Basic_applyClass    = (APPLY_CLASS)Enum.Parse( typeof(APPLY_CLASS),   (string)_data["applyTarget_class"]);
        Basic_applyType     = (APPLY_TYPE)Enum.Parse(  typeof(APPLY_TYPE),    (string)_data["applyTarget_type"]);

        Basic_ratioType     = (RATIO_TYPE)Enum.Parse(  typeof(RATIO_TYPE),  (string)_data["ratio_type"]);
        Basic_ratioValue    = (int)_data["ratio_value"];

        Basic_isShop    = (bool)_data["isShop"];
        Debug.Log((bool)_data["isShop"]);
    }

    public void Dispose()
    {

    }
}