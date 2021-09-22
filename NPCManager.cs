using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCManager : MonoBehaviour
{

    /// <summary>
    /// This file is very Yolocode. It's ignoring complete reusability and future proofing for some up front ease.
    /// If the game is developed longer than a week perhaps I'll come back and fix it but it should work for now.
    /// </summary>



    public enum npcType { //what type of npc is this?

        sign,
        character

    }

    private bool interactable = false;
    [SerializeField]
    private List<string> dialogue = new List<string>(); //this will be a list of the current npcs dialogue strings.
    private int dialogueIndex = 0; //This keeps track of the current index of the dialogue.
    private int dialogueLength;
    [SerializeField]
    private MapManager mapManager;



    public npcType type = npcType.sign; //What type of npc are we?
    public int InteractionNum = 0; //Which interaction should we use?
    public TextMeshProUGUI dialogueText; //What we use to display dialogue text
    public Image dialogueImage; //Where we want to display this chars image
    public GameObject dialoguePanel; //What we open and close to display dialogue/image box.
    public Sprite characterImage; //What image we want to dispaly in dialogue
    public Sprite playerImage; //The frame image of the player to make it look like they're talking back.

    //This is all for the ability to have multiple characters talk

    [SerializeField]
    private string[] WhoShouldTalk; //If this array is set in the inspector it indicates that there are different speakers. This array contains the names of those speakers in the same index as the dialogue they own.

    [SerializeField]
    private Sprite[] spriteArray; //This will be essentially the same functionality as whoShouldTalk. I'm really upset about this but unity doesn't have an 'easy' way to do this that I'm aware of at the moment.
    //It's essentially not worth the time to code this properly for what might be used twice in this week long project. I tried dictionaries but they can't be serialized by unity so it's not 'easy'.

    private void Awake()
    {
        dialogueLength = dialogue.Count;
        
        //print(dialogueLength);
    }

    private void Update()
    {
        
        if(Input.GetButtonDown("Interact")) {

            //FindInteraction(type, InteractionNum);
            if (type == npcType.sign && interactable == true)
            {
                signInteraction();
            } else if (type == npcType.character && interactable == true)
            {
                characterInteraction(InteractionNum);
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.name == "Player")
        {
            interactable = true; //can the player interact with something.
        }
        Debug.Log("Collided with " + collision.name);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            interactable = false; //the player can no longer interact with something.
            dialoguePanel.SetActive(false);
            dialogueIndex = 0; //if the player left mid dialogue reset it.
        }
    }

    //This assumes the interaction will be a dialogue
    private void signInteraction()
    {

        if(dialogue != null)
        {

            if(dialogueIndex == 0)
            {

                //we need to open a dialogue panel
                dialoguePanel.SetActive(true);
                dialogueImage.sprite = characterImage;
                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;

            } else if (dialogueIndex != dialogueLength) {

                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;
                //display text

            } else //it's finish it's dialogue
            {

                dialogueIndex = 0; //reset the index
                dialoguePanel.SetActive(false); //close the panel

            }
            
        } else
        {
            print("The sign stares at you expectantly");
        }

    }

    private void characterInteraction(int specialInteraction)
    {
        if (dialogue != null && WhoShouldTalk == null) //In this case we don't need to do anything special and can use the previous iteration for now.
        {

            if (dialogueIndex == 0)
            {

                //we need to open a dialogue panel
                dialoguePanel.SetActive(true);
                dialogueImage.sprite = characterImage; //set the chars image in the dialogue box
                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;

            }
            else if (dialogueIndex != dialogueLength)
            {

                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;
                //display text

            }
            else //finish it's dialogue
            {

                dialogueIndex = 0; //reset the index
                dialoguePanel.SetActive(false); //close the panel
                startSpecialInteraction(specialInteraction);

            }

        }
        else if (dialogue != null && WhoShouldTalk != null)
        {

            if (dialogueIndex == 0)
            {

                dialoguePanel.SetActive(true);
                dialogueImage.sprite = spriteArray[0];
                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;

            }
            else if (dialogueIndex != dialogueLength)
            {

                dialogueImage.sprite = spriteArray[dialogueIndex];
                dialogueText.text = dialogue[dialogueIndex];
                dialogueIndex++;

            } else
            {

                dialogueIndex = 0; //reset the index
                dialoguePanel.SetActive(false); //close the panel
                startSpecialInteraction(specialInteraction);

            }


        }
        else
        {
            print("The sign stares at you expectantly");
        }
    }

    private void startSpecialInteraction(int specialInteraction)
    {

        switch(specialInteraction)
        {

            case 0: //This is the slime in 1-1 making the bridge
                //do stuff
                mapManager.makeSlimeBridge();
                break;

            default:
                print("Did nothing on special interaction!");
                break;

        }

    }

    /* Too complicated for rn. Time spent on this isn't worth it yet. Maybe if the game ever gets bigger.

    /// <summary>
    /// Finds and carries out the interaction based on type.
    /// </summary>
    /// <param name="type"> Takes a npcType to look up. </param>
    /// <param name="iNum"> Takes an interaction number to look up. </param>
    private void FindInteraction(npcType type, int iNum)
    {

        //First we look up the interaction which will likely be stored in dictionaries for each npcType

        //Second we need to determine what type of interaction it is
            //Is it a dialogue
            //Is it getting an item like a treasure chest
            //Maybe more? but not rn.




    }
    */

}
