using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    public static AudioScript AudioInstance { get; private set; } //Create audio instance
           
    private void Awake() //Check to see if this is the only audio instance
    {
        if (AudioInstance != null && AudioInstance != this)
        {
            Destroy(this.gameObject); //if there's another instance, destroy this one
        }
        else
        {
            AudioInstance = this; //if there isn't, don't destroy me, please.
        }
    }

    [SerializeField]
    private GameObject AudioHolder; //Holds the game audioclip
    [SerializeField]
    private GameObject speakerSprite; //UI Audio Button containing speaker sprite
    [SerializeField]
    private Sprite speakerOnSprite; //Displayed when audio is on
    [SerializeField]
    private Sprite speakerOffSprite; //Displayed when audio is off
    private static bool gameAudio = true; //Set game audio on/off

    private void Start() //Control the audio start state
    {
        if (gameAudio)
        {
            AudioHolder.SetActive(true);
            speakerSprite.GetComponent<Image>().sprite = speakerOnSprite;
        }
        if (!gameAudio)
        {
            AudioHolder.SetActive(false);
            speakerSprite.GetComponent<Image>().sprite = speakerOffSprite;
        }
    }

    public void AudioControl() //turns the audio and audio UI on/off
    {
        bool audioClick = false;

        if (gameAudio && !audioClick)
        {
            audioClick = true;
            gameAudio = false;
            AudioHolder.SetActive(false);
            speakerSprite.GetComponent<Image>().sprite = speakerOffSprite;
        }
        if (!gameAudio && !audioClick)
        {
            audioClick = true;
            gameAudio = true;
            AudioHolder.SetActive(true);
            speakerSprite.GetComponent<Image>().sprite = speakerOnSprite;
        }
        audioClick = false;
    }
}

//To display in game credits, etc.
//Music from https://filmmusic.io
//"Digital Lemonade" by Kevin MacLeod(https://incompetech.com)
//License: CC BY (http://creativecommons.org/licenses/by/4.0/)
//Special Thanks to VR with Andrew, Edwin of Sentiens Studios, and flaticon.com