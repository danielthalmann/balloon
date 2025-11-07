using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Scriptable Objects/GameParameters")]
public class GameParameter : ScriptableObject
{
    public float fallForce = 0.5f;

    public float levelTimeDuration = 300;

    public AnimationCurve groundCurve;

}
