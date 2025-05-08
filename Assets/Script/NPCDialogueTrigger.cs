using UnityEngine;
using System.Collections;
public class NPCDialogueTrigger : MonoBehaviour
{
    public GameObject interactionPrompt;
    public DialogueManager dialogueManager; // Gán trong Inspector
    public string[] npcDialogues = {
        "NPC: Chào ngươi, lữ khách!",
        "NPC: Nghe nói có một con quái vật ở phía Bắc...",
        "Player: Cảm ơn, ta sẽ đi xem!"
    }; // Hội thoại tùy chỉnh trong Inspector

    private bool canTalk = false;

    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.E))
        {
            dialogueManager.StartDialogue(npcDialogues, null); // Gọi hội thoại, không cần Boss
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }

}