using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using BaseAI.cs;

public class Cattle : BaseAI {

    //Upon triggerEnter with interactable object, do thing (eat, rest, or sleep)
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided with " + col.gameObject.tag);

        if (col.gameObject.tag == "Eat") { Eat(col.gameObject); }
        else if (col.gameObject.tag == "Rest") { Rest(col.gameObject); }
        else if (col.gameObject.tag == "Play") { Play(col.gameObject); }
    }

    public override void Eat(GameObject eatTarget) { Debug.Log("I can eat."); }

    public override void Rest(GameObject restTarget) { Debug.Log("I can rest."); }

    public override void Play(GameObject playTarget) { Debug.Log("I can play."); }

    public override void Speak() { }
}
