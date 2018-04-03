using UnityEngine;

[CreateAssetMenu(fileName = "AreaConfig")]
public class AreaConfig : ScriptableObject
{
    public Vector3 playArea;
    public Vector3 restArea;
    public Vector3 eatArea;
}
