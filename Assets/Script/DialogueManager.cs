using UnityEngine;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public PlayerInventoryUI inventoryUI;
    public bool isDialogueActive { get; private set; }
    private string[] currentDialogues;
    private int currentDialogueIndex = 0;
    private Boss boss;
    private PlayerController playerController;
    public event Action OnDialogueComplete;
    [SerializeField] private AudioClip dialogueStartSFX;
    [SerializeField] private AudioClip dialogueNextSFX;

    void Start()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("DialogueManager: Không tìm thấy PlayerController!");
        }
    }

    public void StartDialogue(string[] dialogues, Boss bossScript)
    {
        currentDialogues = dialogues;
        boss = bossScript;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        if (inventoryUI != null)
        {
            inventoryUI.SetUIVisible(false);
        }
        if (playerController != null)
        {
            playerController.SetCanControl(false);
        }
        currentDialogueIndex = 0;
        AudioManager.Instance?.PlaySFX(dialogueStartSFX);
        DisplayNextDialogue();
    }

    public void StartDialogue(Boss bossScript)
    {
        string[] defaultDialogues = {
            "Boss: Ha ha, ta không ngờ lại gặp được ngươi ở nơi như thế này đấy!",
            "Boss: Hãy chuẩn bị cho sự trả thù của ta đi",
            "Player: Ta có quen ngươi sao?",
            "Boss: Ha ha, không những quen mà rất thân thuộc lại là đằng khác!",
            "Boss: Nếu ngươi muốn biết thì trước hết phải bước qua xác ta!!!"
        };
        StartDialogue(defaultDialogues, bossScript);
    }

    void DisplayNextDialogue()
    {
        if (currentDialogueIndex < currentDialogues.Length)
        {
            dialogueText.text = currentDialogues[currentDialogueIndex];
            currentDialogueIndex++;
            AudioManager.Instance?.PlaySFX(dialogueNextSFX);
        }
        else
        {
            EndDialogue();
        }
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        if (inventoryUI != null)
        {
            inventoryUI.SetUIVisible(true);
        }
        if (playerController != null)
        {
            playerController.SetCanControl(true);
        }
        OnDialogueComplete?.Invoke();
    }
}