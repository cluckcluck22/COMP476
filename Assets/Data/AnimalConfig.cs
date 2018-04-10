using UnityEngine;

[CreateAssetMenu(fileName = "AnimalConfig")]
public class AnimalConfig : ScriptableObject
{
    public Species species;
    public BaseAnimalConfig baseAnimalConfig;

    public TextAsset xmlTree
    {
        get { return baseAnimalConfig.xmlTree; }
    }

    public float randomMinRatio
    {
        get { return baseAnimalConfig.m_randomMinRatio; }
    }   

    public float randomMaxRatio
    {
        get { return baseAnimalConfig.m_randomMaxRatio; }
    }

    public int maxHunger;
    [Tooltip("Amount recovered per second while eating")]
    public int hungerRecuperation;
    [Tooltip("Amount loss per second")]
    public int hungerLossRate;

    public AnimationCurve hungerNeedCurve
    {
        get { return baseAnimalConfig.m_hungerNeedCurve; }
    }

    public int maxFatigue;
    [Tooltip("Amount recovered per second while resting")]
    public int fatigueRecuperation;
    [Tooltip("Amount loss per second")]
    public int fatigueLossRate;

    public AnimationCurve fatigueNeedCurve
    {
        get { return baseAnimalConfig.m_fatigueNeedCurve; }
    }

    public int maxBoredom;
    [Tooltip("Amount recovered per second while playing")]
    public int boredomRecuperation;
    [Tooltip("Amount loss per second")]
    public int boredomLossRate;

    public AnimationCurve boredomNeedCurve
    {
        get { return baseAnimalConfig.m_boredomNeedCurve; }
    }

    public AnimationCurve affinityProximityNeedCurve
    {
        get { return baseAnimalConfig.m_affinityProximityNeedCurve; }
    }

    public int maxHealth;

    public float idleScore
    {
        get { return baseAnimalConfig.m_idleScore; }
    }

    public float fov
    {
        get { return baseAnimalConfig.m_fov; }
    }

    public float sightDistance
    {
        get { return baseAnimalConfig.m_sightDistance; }
    }   
    
    public float lkpSightDistance
    {
        get { return baseAnimalConfig.m_lkpSightDistance; }
    }    
    
    public Vector3 headOffset
    {
        get { return baseAnimalConfig.m_headOffset; }
    }    
    
    public float lkpExpire
    {
        get { return baseAnimalConfig.m_lkpExpire; }
    }            
}
