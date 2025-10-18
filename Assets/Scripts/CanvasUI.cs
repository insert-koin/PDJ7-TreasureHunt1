using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasUI : MonoBehaviour
{
    [Tooltip("Assign Main Panel so it can be turned on from Player:OnStartClient")]
    public RectTransform mainPanel;
    [Tooltip("Assign Players Panel for instantiating PlayerUI as child")]
    public RectTransform playersPanel;
    // static instance that can be referenced from static methods below.
    public static CanvasUI instance;
    bool isTabPanelOpen;
    InputAction clickTab;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        clickTab = InputSystem.actions.FindAction("TabClick");
        clickTab.performed += InteractWithPlayerPanel;
        playersPanel.gameObject.SetActive(false);
        isTabPanelOpen = false;
    }
    public void SetActive(bool active)
    {
        instance.mainPanel.gameObject.SetActive(active);
    }
    public RectTransform GetPlayersPanel() => instance.playersPanel;
    public void InteractWithPlayerPanel(InputAction.CallbackContext ctx)
    {
        if (isTabPanelOpen)
        {
            playersPanel.gameObject.SetActive(false);
            isTabPanelOpen = false;
        }
        else
        {
            playersPanel.gameObject.SetActive(true);
            isTabPanelOpen = true;
        }
    }  
}
