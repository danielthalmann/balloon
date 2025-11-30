using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Scriptable Objects/GameParameters")]
public class GameParameter : ScriptableObject
{
    public float fallForce = 0.5f;

    public float levelDistance = 300;

    public float limitOfBestFly = .8f;

    public AnimationCurve groundCurve;

}
