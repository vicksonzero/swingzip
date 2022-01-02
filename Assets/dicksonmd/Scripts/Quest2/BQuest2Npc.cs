using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BNpcTrigger2D))]
public class BQuest2Npc : MonoBehaviour, IQuestGiver
{
    public string npcName = "";
    public string npcKey = "";
    public SpriteRenderer npcSprite;
    public Text nameTag;

    public BInteractionButton interactionButton;

    public List<BQuest2Quest> offeredQuests = new List<BQuest2Quest>();


    public BNpcTrigger2D trigger;
    BMissionUI missionUI;

    // Start is called before the first frame update
    void Start()
    {
        if (npcSprite == null) npcSprite = GetComponentInChildren<SpriteRenderer>();
        if (nameTag == null) nameTag = GetComponentInChildren<Text>();
        if (interactionButton == null) interactionButton = GetComponentInChildren<BInteractionButton>();
        if (trigger == null) trigger = GetComponent<BNpcTrigger2D>();
        if (missionUI == null) missionUI = FindObjectOfType<BMissionUI>();

        nameTag.text = npcName;
        trigger.triggerEnter += OnPlayerTriggerEnter;
        trigger.triggerExit += OnPlayerTriggerExit;

        // interactionButton
    }

    void IQuestGiver.RegisterQuest(BQuest2Quest quest)
    {
        Debug.Log("BQuest2Npc.RegisterQuest");
        offeredQuests.Add(quest);
    }

    void IQuestGiver.UnRegisterQuest(BQuest2Quest quest)
    {
        offeredQuests.Remove(quest);
    }


    void OnPlayerTriggerEnter(Collider2D collider)
    {
        Debug.Log("BQuest2Npc.OnPlayerTriggerEnter");
        // // TODO: make interaction trigger through clicking the interaction button or pressing [UP] near the NPC
        // if (collider.GetComponent<BPlayer>().controller.collisions.below)
        // {
        //     interactionButton.isJumping = true;
        // }

        StartDialog();
    }
    void OnPlayerTriggerExit(Collider2D collider)
    {
        Debug.Log("BQuest2Npc.OnPlayerTriggerExit");
        // // TODO: make interaction trigger through clicking the interaction button or pressing [UP] near the NPC
        // interactionButton.isJumping = false;

        HideDialog();
    }

    void OnInteractionButtonPressed()
    {
        Debug.Log("BQuest2Npc.OnInteractionButtonPressed");
    }

    void StartDialog()
    {
        Debug.Log("BQuest2Npc.StartDialog");
        missionUI.deliveryQuestUi
            .SetNpcDialogs(name: this.npcName, dialog: Dialogs.GetRandomGreetings())
            .PopulateQuestList(offeredQuests)
            .Show();
    }

    void HideDialog()
    {
        missionUI.deliveryQuestUi.Hide();
    }


    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (npcName == "") npcName = Names.GetRandomName();
        if (npcSprite == null)
        {
            npcSprite = GetComponentInChildren<SpriteRenderer>();
            npcSprite.color = Random.ColorHSV(0f, 1f, 0.4f, 0.7f, 0.7f, 1f);
        }

        if (nameTag == null)
        {
            nameTag = GetComponentInChildren<Text>();
            nameTag.text = npcName;
        }
    }
#endif
}