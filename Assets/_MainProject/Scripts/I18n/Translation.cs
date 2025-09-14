using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Translation
{

    protected Dictionary<string, string> dictionary;
    public string lang { get; private set; } = "en";
    public string fallback_lang = "en";

    public delegate void OnChangeLanguage();
    public static OnChangeLanguage onChangeLanguage;

    public Translation()
    {
        dictionary = new Dictionary<string, string>();

        SystemLanguage currentLanguage = Application.systemLanguage;

        foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            if (culture.EnglishName.Equals(currentLanguage.ToString()))
            {
                lang = culture.Name;
                break;
            }
        }

        LoadTranslate(lang);

    }

    public void SetLanguage(string l)
    {
        lang = l.ToLower();
        LoadTranslate(lang);

        onChangeLanguage?.Invoke();
    }

    public string TranslateMulti(string text)
    {
        text = text.Replace("\r\n", "\n");
        string[] texts = text.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.None);

        if (texts.Length > 1)
        {
            for (int i = 0; i < texts.Length; ++i)
            {
                texts[i] = Translate(texts[i]);
            }
            return string.Join("\n", texts);

        }
        else
        {
            return Translate(text);
        }
    }

    public string Translate(string text)
    {
        // Debug.Log("Translate : \"" + text + "\"");
        if (dictionary.ContainsKey(text))
        {
            return dictionary[text];
        }
        else
        {
            return text;
        }
    }

    protected void LoadTranslate(string lang)
    {

        dictionary.Clear();

        TextAsset textAsset = Resources.Load<TextAsset>("I18n/" + lang);

        if (textAsset == null)
        {
            textAsset = Resources.Load<TextAsset>("I18n/" + fallback_lang);
        }

        if (textAsset == null)
        {
            Debug.LogWarning("File not found for I18n: Assets/Resources/I18n/" + lang + ".txt");
            return;
        }


        string allTexts = "";
        allTexts = textAsset.text;

        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

        string key, value;
        for (int i = 0; i < lines.Length; i++)
        {
            int pos = lines[i].IndexOf("=");
            if (pos >= 0 && !lines[i].StartsWith("#"))
            {
                key = lines[i].Substring(0, pos).Trim();
                value = lines[i].Substring(pos + 1, lines[i].Length - pos - 1).Replace("\\n", System.Environment.NewLine).Trim();

                if (!dictionary.ContainsKey(key))
                {
                    // Debug.Log("add key : \"" + key + "\" values : " + value);
                    dictionary.Add(key, value);
                }

            }
        }
    }

}
