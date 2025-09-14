using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Translation;

public class Translator : MonoBehaviour
{

    void Start()
    {
        UpdateText();
        Translation.onChangeLanguage += OnChangeLanguage;
    }

    protected void OnChangeLanguage()
    {
        UpdateText();
    }

    void UpdateText()
    {
        TMP_Text text = GetComponent<TMP_Text>();

        if (text != null)
        {
            text.text = I18n.Trans().Translate(text.text);
        }
    }

}
