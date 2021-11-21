
/// <summary>
/// A quest giver is usually an NPC, but can be a UI element
/// </summary>
public interface IQuestGiver
{
    void RegisterQuest(BQuest2Quest quest);
    void UnRegisterQuest(BQuest2Quest quest);
}