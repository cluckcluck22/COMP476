using UnityEngine;

[CreateAssetMenu(fileName = "BaseAnimalConfig")]
public class BaseAnimalConfig : ScriptableObject
{

    public TextAsset xmlTree;
    public float m_randomMinRatio;    
    public float m_randomMaxRatio;    
                                                      
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve m_hungerNeedCurve;    
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve m_fatigueNeedCurve;
    [Tooltip("Normalized need score wrt current value")]
    public AnimationCurve m_boredomNeedCurve;
    [Tooltip("Normalized need score wrt avg distance from friends")]
    public AnimationCurve m_affinityProximityNeedCurve;


    public float m_idleScore;             

    public float m_fov;                   
    public float m_sightDistance;         
    public float m_lkpSightDistance;      
    public Vector3 m_headOffset;          
    public float m_lkpExpire;             

}
