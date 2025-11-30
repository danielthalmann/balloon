using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject[] tutorals;

    int current;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableAllTuto()
    {
        foreach(GameObject go in tutorals)
        {
            go.SetActive(false);
        }
    }

    public void StartTuto()
    {
        current = 0;
        tutorals[current].SetActive(true);
    }

    public bool NextTuto()
    {
        current++;
        DisableAllTuto();
        if (current < tutorals.Length)
        {
            tutorals[current].SetActive(true);
            return true;
        }
        return false;
    }
   

}
