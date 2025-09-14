using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;


public class I18n
{
    protected static Translation trans;

    public static Translation Trans()
    {
        if (trans == null)
        {
            trans = new Translation();
        }
        return trans;
    }
}

