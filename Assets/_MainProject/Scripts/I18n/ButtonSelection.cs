using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelection : MonoBehaviour
{

    public GameObject buttonFR;
    public GameObject buttonDE;

    public Color colorActiveButton;
    public Color colorActiveText;

    public Color colorNormalButton;
    public Color colorNormalText;

    public void ChangeToLanguageFr()
    {
        UpdateButtons("fr");
        I18n.Trans().SetLanguage("fr");
    }

    public void ChangeToLanguageDe()
    {
        UpdateButtons("de");
        I18n.Trans().SetLanguage("de");
    }

    void UpdateButtons(string language)
    {
        Debug.Log("change");
        ColorBlock colors;

        TMPro.TMP_Text textFr;
        Button buttonFr;

        TMPro.TMP_Text textDe;
        Button buttonDe;

        if(buttonFR == null)
        {
            Debug.LogWarning("button fr empty");
            return;
        }
        if (buttonDE == null)
        {
            Debug.LogWarning("button de empty");
            return;
        }

        textFr = buttonFR.GetComponentsInChildren<TMP_Text>().First();
        buttonFr = buttonFR.GetComponent<Button>();

        textDe = buttonDE.GetComponentsInChildren<TMP_Text>().First();
        buttonDe = buttonDE.GetComponent<Button>();

        colors = buttonFr.colors;

        colors.normalColor = colorNormalButton;
        colors.selectedColor = colorNormalButton;
        colors.pressedColor = colorNormalButton; 
        colors.highlightedColor = colorNormalButton;

        buttonFr.colors = colors;
        textFr.color = colorNormalText;
        buttonDe.colors = colors;
        textDe.color = colorNormalText;

        colors.normalColor = colorActiveButton;
        colors.selectedColor = colorActiveButton;
        colors.pressedColor = colorActiveButton;
        colors.highlightedColor = colorActiveButton;

        if (language == "fr")
        {
            buttonFr.colors = colors;
            textFr.color = colorActiveText;
        }

        if (language == "de")
        {
            buttonDe.colors = colors;
            textDe.color = colorActiveText;
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateButtons(I18n.Trans().lang);
    }


}
