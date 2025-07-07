using UnityEngine;

public class ZoneEffect : MonoBehaviour
{
    public enum ZoneType { Ascension, Slow }
    
    [SerializeField] private ZoneType type;           // Type de zone
    [SerializeField] private float effectIntensity;   // Intensité de l'effet (force d'ascension ou facteur de ralentissement)
    
    public ZoneType GetZoneType() { return type; }
    public float GetEffectIntensity() { return effectIntensity; }
}
