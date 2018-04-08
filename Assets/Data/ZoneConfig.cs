using UnityEngine;

[CreateAssetMenu(fileName = "ZoneConfig")]
public class ZoneConfig : ScriptableObject
{
    public Vector3 playZone;
    public Vector3 restZone;
    public Vector3 eatZone;
    public Vector3 rallyZone;
}
