using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // instance of QuestManager
    public static QuestManager Instance{get; private set;}

    // list of quests
    public List<Quest> quests = new List<Quest>();

    // Dictionary to track the completion status of each quest
    private Dictionary<Quest, bool> questCompletionStatus = new Dictionary<Quest, bool>();

    // Dictionary to hold quests and their active status
    private Dictionary<Quest, bool> questActiveStatus = new Dictionary<Quest, bool>();

    // starter quest
    public Quest startingQuest;

    // function to start at beginning of game to make sure there is only one instance
    // of this manager
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ensures persistence across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // destroys any duplicate instances
            Destroy(gameObject);
        }
        AddQuest(startingQuest);
        ActivateQuest(startingQuest);
    }

    // function to add quest to the quest list
    public void AddQuest(Quest questToAdd)
    {
        if (!quests.Contains(questToAdd))
        {
            quests.Add(questToAdd);
            questCompletionStatus[questToAdd] = false;
            Debug.Log($"Quest '{questToAdd.questName}' added to the list.");
        }
    }

    // function to complete a quest
    // it looks in the quest list to find a matching title
    // if it finds one, it completes the quest and the removes it from the list
    public void CompleteQuest(Quest questToComplete)
    {
        if (questToComplete == null)
        {
            return;
        }

        if (questCompletionStatus.ContainsKey(questToComplete))
        {
            questCompletionStatus[questToComplete] = true; // Mark as complete
        }
    }

    // function to check quest completion status
    public bool IsQuestComplete(Quest questToCheck)
    {
        if (questCompletionStatus.ContainsKey(questToCheck))
        {
            bool isComplete = questCompletionStatus[questToCheck]; // Get the completion status
            return isComplete; // Return the completion status
        }

        return false; // If quest isn't in the dictionary, return false
    }

    // Function to activate a quest
    public void ActivateQuest(Quest questToActivate)
    {
        if (questToActivate != null && !IsQuestActive(questToActivate))
        {
            questActiveStatus[questToActivate] = true;
            Debug.Log($"Quest '{questToActivate.questName}' is now active.");
        }
    }

    // Function to deactivate a quest
    public void DeactivateQuest(Quest questToDeactivate)
    {
        if (questToDeactivate != null && IsQuestActive(questToDeactivate))
        {
            questActiveStatus[questToDeactivate] = false;
            Debug.Log($"Quest '{questToDeactivate.questName}' has been deactivated.");
        }
    }

    // New function similar to IsQuestComplete to check if a quest is active
    public bool IsQuestActive(Quest questToCheck)
    {
        if (questActiveStatus.TryGetValue(questToCheck, out bool isActive))
        {
            return isActive; // Return the isActive status
        }

        return false; // If the quest is null, return false
    }

    public void SaveQuestState(Quest quest)
    {
        PlayerPrefs.SetInt("QuestActive_" + quest.questName, IsQuestActive(quest) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadQuestState(Quest quest)
    {
        if (PlayerPrefs.HasKey("QuestActive_" + quest.questName))
        {
            questActiveStatus[quest] = PlayerPrefs.GetInt("QuestActive_" + quest.questName) == 1;
        }
    }
}