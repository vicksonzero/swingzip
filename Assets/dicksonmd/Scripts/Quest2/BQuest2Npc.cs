using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BQuest2Npc : MonoBehaviour
{
    public string npcName = "";
    public string npcKey = "";
    public SpriteRenderer npcSprite;
    public Text nameTag;

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
    // Start is called before the first frame update
    void Start()
    {
        if (npcSprite == null) npcSprite = GetComponentInChildren<SpriteRenderer>();
        if (nameTag == null) nameTag = GetComponentInChildren<Text>();

        nameTag.text = npcName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}