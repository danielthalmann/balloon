using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTranslator : MonoBehaviour
{

    public string translationKey = null;
    
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();


        if (text != null)
        {
            translationKey = text.text;
        }

        UpdateText();

        Translation.onChangeLanguage += OnChangeLanguage;
    }

    protected void OnChangeLanguage()
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (text != null)
        {
            text.text = I18n.Trans().TranslateMulti(translationKey);
        }
    }

}
