using UnityEngine;

public class Ladder : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);

        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().SetOnLadder(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject);

        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().SetOnLadder(false);
        }
    }
}
