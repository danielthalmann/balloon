using UnityEngine;
using System.Text;
using System;
using TMPro;

public class Credits : MonoBehaviour 
{

    public bool play;
    public int delay;
    public RectTransform panel;
    public TextAsset creditsFile;

    public int fontSize = 36;
    public float yDistance;
    public float scrollSpeed;

    private float y;
    private Vector2 startingPos;

    private string[] lines;

    public const string COLUMN_SEP = " - ";
    public const string ROW_SEP = "\n";

    private int current_line;

    public void Start()
    {
        current_line = 0;
                    
        //Break up our credits file into a jagged array
        //Every return (\r\n) is a new row
        lines = creditsFile.text.Split(new string[]{"\r\n", "\n" }, StringSplitOptions.None);

    }

    public void Update()
    {
        if (!play)
            return;

        float distance = Time.deltaTime * scrollSpeed; 

        y += distance;

        if (y > yDistance)
        {
            AddLine();
            y = 0;
        }

        for ( int i = 0; i < panel.transform.childCount; i++)
        {
            RectTransform rect = panel.transform.GetChild(i).GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y + distance);

            if (rect.anchoredPosition.y > panel.rect.size.y + yDistance)
            {
                Destroy(panel.transform.GetChild(i).gameObject);
            }
        }

        if (current_line >= lines.Length && panel.transform.childCount == 0)
        {
            Finished();
        }

    }

    public void AddLine()
    {
        if (current_line >= lines.Length)
            return;

        GameObject obj = new GameObject();
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.CenterGeoAligned;
        text.fontSize = fontSize;

        RectTransform trans = obj.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(0, -yDistance);
        trans.anchorMin = new Vector2();
        trans.anchorMax = new Vector2(1f, 0);
        trans.pivot = new Vector2(.5f, .5f);
        // trans.position = Vector3.zero;
        //trans.sizeDelta = new Vector2(0, 50);
        trans.sizeDelta = panel.rect.size;
        
        text.text = lines[current_line];

        obj.transform.SetParent(panel.transform);
        current_line++;
    }

    protected void Finished()
    {
        play = false;
        current_line = 0;
    }

    public void StartCredits()
    {
        play = true;
        current_line = 0;
    }



}
