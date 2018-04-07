using UnityEngine;

[CreateAssetMenu(fileName = "AnimalConfig")]
public class AnimalConfig : ScriptableObject
{

    public Species species;

    public float randomMinRatio;
    public float randomMaxRatio;

    public int maxHunger;
    [Tooltip("Amount recovered per second while eating")]
    public int hungerRecuperation;
    [Tooltip("Amount loss per second")]
    public int hungerLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve hungerNeedCurve;

    public int maxFatigue;
    [Tooltip("Amount recovered per second while resting")]
    public int fatigueRecuperation;
    [Tooltip("Amount loss per second")]
    public int fatigueLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve fatigueNeedCurve;

    public int maxBoredom;
    [Tooltip("Amount recovered per second while playing")]
    public int boredomRecuperation;
    [Tooltip("Amount loss per second")]
    public int boredomLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve boredomNeedCurve;

    [Tooltip("Normalized need score wrt avg distance from friends")]
    public AnimationCurve affinityProximityNeedCurve;

    public int maxHealth;

    public float idleScore;

    public float fov;
    public float sightDistance;
    public float lkpSightDistance;
    public Vector3 headOffset;
    public float lkpExpire;
}
