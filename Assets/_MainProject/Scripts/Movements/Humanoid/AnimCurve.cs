using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Scriptable Objects/AnimationCurve")]
public class AnimCurve : ScriptableObject
{
    public float speed = 1f;
    public float amplitude = 1f;
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve zCurve;
}
