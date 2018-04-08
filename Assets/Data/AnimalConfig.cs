using UnityEngine;

[CreateAssetMenu(fileName = "AnimalConfig")]
public class AnimalConfig : ScriptableObject
{

    public Species species;
    public AnimalConfig baseAnimalConfig; // placehodler type

    public float randomMinRatio;    // move this
    public float randomMaxRatio;    // move this

    public AnimationCurve hungerNeedCurve
    {
        get { return baseAnimalConfig.m_hungerNeedCurve; }
    }

    public int maxHunger;
    [Tooltip("Amount recovered per second while eating")]
    public int hungerRecuperation;
    [Tooltip("Amount loss per second")]
    public int hungerLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve m_hungerNeedCurve;    // move this

    public int maxFatigue;
    [Tooltip("Amount recovered per second while resting")]
    public int fatigueRecuperation;
    [Tooltip("Amount loss per second")]
    public int fatigueLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve fatigueNeedCurve;     // move this

    public int maxBoredom;
    [Tooltip("Amount recovered per second while playing")]
    public int boredomRecuperation;
    [Tooltip("Amount loss per second")]
    public int boredomLossRate;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve boredomNeedCurve;     // move this

    [Tooltip("Normalized need score wrt avg distance from friends")]
    public AnimationCurve affinityProximityNeedCurve;   // move this

    public int maxHealth;

    public float idleScore;             // move this

    public float fov;                   // move this
    public float sightDistance;         // move this
    public float lkpSightDistance;      // move this
    public Vector3 headOffset;          // move this
    public float lkpExpire;             // move this
}
