using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [field: SerializeField] public DialogueScriptableObject[] bartenderDialogues {get; private set;}
    [field: SerializeField] public DialogueScriptableObject[] skeletonWizardDialogues {get; private set;}
    [field: SerializeField] public DialogueScriptableObject[] skeletonFighterDialogues {get; private set;}
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DialogueScriptableObject[] GetDialogues(NPCName npcName)
    {
        return npcName switch
        {
            NPCName.Bartender => bartenderDialogues,
            NPCName.SkeletonWizard => skeletonWizardDialogues,
            NPCName.SkeletonFighter => skeletonFighterDialogues,
            _ => null
        };
    }
}
