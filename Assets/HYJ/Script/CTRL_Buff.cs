using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;


public enum Buff_precondition_CLASS
{
    NONE,
    HAVE_MORE
}

public enum Buff_precondition_TYPE
{
    NONE,
    SYNERGY
}

public enum Buff_apply_CLASS
{
    SYNERGY_ELF
}

public enum Buff_apply_TYPE
{
    SPAWN_CHANGE,
    ATTACK_POINT
}

public enum Buff_value_TYPE
{
    PERCENT,
    FLAT
}

public enum Buff_duration_TYPE
{
    STAGE
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

    //////////  Method          //////////

    //////////  Default Method  //////////
    public CTRL_Buff_Save(Dictionary<string, object> _data)
    {
        Basic_index     = (int)_data[       "valueControlBuff_index"    ];
        Basic_tag       = (string)_data[    "valueControlBuff_tag"      ];
        Basic_name      = (string)_data[    "valueControlBuff_name"     ];
        Basic_number    = (int)_data[       "valueControlBuff_num"      ];

        Basic_durationType  = (string)_data[    "valueControlBuff_duration_type"    ];
        Basic_durationValue = (float)_data[     "valueControlBuff_duration_value"   ];
    }

    public CTRL_Buff_Save(CTRL_Buff_Save _save)
    {
        Basic_index     = _save.Basic_index;
        Basic_tag       = "" + _save.Basic_tag;
        Basic_name      = "" + _save.Basic_name;
        Basic_number    = _save.Basic_number;
    }

    public void Dispose()
    {

    }
}

public class CTRL_Buff : IDisposable
{
    public CTRL_Buff_Save Basic_data;

    public Buff_precondition_CLASS  Basic_preconditionClass;
    public Buff_precondition_TYPE   Basic_preconditionType;
    public int                      Basic_preconditionValue;

    public Buff_apply_CLASS Basic_applyClass;
    public Buff_apply_TYPE  Basic_applyType;

    public Buff_value_TYPE  Basic_valueType;
    public int              Basic_valueValue;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    public CTRL_Buff(Dictionary<string, object> _data)
    {
        Basic_data = new CTRL_Buff_Save(_data);

        Basic_preconditionClass = (Buff_precondition_CLASS)Enum.Parse(  typeof(Buff_precondition_CLASS),    (string)_data[    "valueControlBuff_precondition_class" ]);
        Basic_preconditionType  = (Buff_precondition_TYPE)Enum.Parse(   typeof(Buff_precondition_TYPE),     (string)_data[    "valueControlBuff_precondition_type"  ]);
        Basic_preconditionValue = (int)_data[   "valueControlBuff_precondition_value"];

        Basic_applyClass    = (Buff_apply_CLASS)Enum.Parse( typeof(Buff_apply_CLASS),   (string)_data["valueControlBuff_applyTarget_applyClass" ]);
        Basic_applyType     = (Buff_apply_TYPE)Enum.Parse(  typeof(Buff_apply_TYPE),    (string)_data["valueControlBuff_applyTarget_applyType"  ]);

        Basic_valueType     = (Buff_value_TYPE)Enum.Parse(  typeof(Buff_value_TYPE),    (string)_data["valueControlBuff_valueType"]);
        Basic_valueValue    = (int)_data["valueControlBuff_value"];
    }

    public void Dispose()
    {

    }
}