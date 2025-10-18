using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasUI : MonoBehaviour
{
    [Tooltip("Assign Main Panel so it can be turned on from Player:OnStartClient")]
    public RectTransform mainPanel;
    [Tooltip("Assign Players Panel for instantiating PlayerUI as child")]
    public RectTransform playersPanel;
    [SerializeField] GameObject painelDerrota;
    [SerializeField] GameObject painelVitoria;
    public TextMeshProUGUI textoTimer;
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
    public void UpdateTime(float timer)
    {
        if (textoTimer != null) textoTimer.text = String.Format("{0:00}:{1:00}", (int)timer / 60, timer % 60);
    }
    public void Lose()
    {
        painelDerrota.SetActive(true);
    }
    public void Win()
    {
        painelDerrota.SetActive(false);
        painelVitoria.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
