using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Scriptable Objects/WalkAnimation")]
public class WalkAnimation : ScriptableObject
{
    public float speed = 1f;
    public float amplitude = 1f;
    public AnimationCurve horizontalCurve;
    public AnimationCurve verticalCurve;

}
