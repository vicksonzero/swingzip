using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BQuest2DeliveryQuestUi : MonoBehaviour
{
    public BQuest2DeliveryListItem listItemPrefab;
    public Button listItemButtonPrefab;
    public RectTransform listingPage;
    public Text nameLabel;
    public Text dialogLabel;
    public ScrollRect scrollView;

    public RectTransform questPage;
    public BQuest2DeliveryListItem questOverviewItem;
    public Text descriptionsLabel;
    public Button acceptButton;

    public BQuest2Quest selectedQuest = null;


    BMissionUI missionUI;
    BQuest2Rider rider;

    void Start()
    {
        if (missionUI == null) missionUI = FindObjectOfType<BMissionUI>();
        if (rider == null) rider = FindObjectOfType<BQuest2Rider>();

        acceptButton.onClick.AddListener(() => AcceptSelectedQuest());
    }

    public BQuest2DeliveryQuestUi SetNpcDialogs(string name, string dialog)
    {
        nameLabel.text = name;
        dialogLabel.text = dialog;
        return this;
    }
    public BQuest2DeliveryQuestUi PopulateQuestList(List<BQuest2Quest> quests)
    {
        Debug.Log("BQuest2DeliveryQuestUi.PopulateQuestList");
        Vector2 pointer = Vector2.zero;

        foreach (Transform child in scrollView.content)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var quest in quests)
        {

            var button = Instantiate(listItemButtonPrefab, Vector3.zero, Quaternion.identity, scrollView.content);
            button.transform.localPosition = pointer;

            button.onClick.AddListener(() => ViewQuestDetails(quest));
            var item = Instantiate(listItemPrefab, Vector3.zero, Quaternion.identity, button.transform);
            item.transform.localPosition = Vector3.zero;

            item.titleLabel.text = quest.title;
            item.descriptionLabel.text = quest.description;


            // TODO: find better place to put handling for different Quests
            if (quest is BQuest2DeliveryQuest)
            {
                var order = (quest as BQuest2DeliveryQuest).order;
                item.descriptionLabel.text = $"From: {order.fromNpc.npcName}\nTo: {order.toNpc.npcName}";
            }

            item.priceLabel.text = quest.reward;
            item.icon.sprite = quest.icon;
            item.iconPanel.color = quest.iconBg;

            pointer.y -= 80;
        }

        return this;
    }


    public BQuest2DeliveryQuestUi Show()
    {
        this.gameObject.SetActive(true);
        ViewListingPage();
        return this;
    }
    public BQuest2DeliveryQuestUi Hide()
    {
        missionUI.routeCamera.SetTarget(rider.transform);
        this.gameObject.SetActive(false);
        return this;
    }


    private void ViewListingPage()
    {
        listingPage.gameObject.SetActive(true);
        questPage.gameObject.SetActive(false);
    }
    private void ViewQuestDetails(BQuest2Quest quest)
    {
        selectedQuest = quest;
        questPage.gameObject.SetActive(true);

        descriptionsLabel.text = quest.description;


        questOverviewItem.titleLabel.text = quest.title;
        // TODO: find better place to put handling for different Quests
        if (quest is BQuest2DeliveryQuest)
        {
            var order = (quest as BQuest2DeliveryQuest).order;
            questOverviewItem.descriptionLabel.text = $"From: {order.fromNpc.npcName}\nTo: {order.toNpc.npcName}";

            // missionUI.routeCamera.SetTarget(order.toNpc.transform);
            // missionUI.routeCamera.SetTargets(rider.transform, order.toNpc.transform);
            missionUI.routeCamera.SetTargets(new Transform[] {
                rider.transform,
                order.toNpc.transform
            });
        }
        questOverviewItem.priceLabel.text = quest.reward;
        questOverviewItem.icon.sprite = quest.icon;
        questOverviewItem.iconPanel.color = quest.iconBg;



        listingPage.gameObject.SetActive(false);
    }
    private void AcceptSelectedQuest()
    {
        if (selectedQuest == null) return;
        Debug.Log("BQuest2DeliveryQuestUi.AcceptSelectedQuest" + selectedQuest);

        // Do something with rider
    }
}
