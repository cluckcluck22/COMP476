/* Class: getUsername.cs
 * Programmer: Eric Davies
 * Date: 12/4/2018
 * Description: A class that outputs the current username of the player in the settings main menu page.
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getUsername : MonoBehaviour {

    Text mText;

    // Use this for initialization
    void Start()
    {
        mText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        mText.text = "Current Username: " + NetworkInformation.username;
    }
}
