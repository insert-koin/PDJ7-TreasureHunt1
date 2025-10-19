using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {
    [SerializeField] float speed = 5;
    [SerializeField] float currentSpeed;
    [SerializeField] Camera myCamera;
    [SerializeField] AudioListener myAudioListener;
    InputAction moveAction;
    InputAction lookAction;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector3 moveDir;
    [SerializeField]public float mouseSensitivity = 100f;
    [SerializeField] private float xRotation = 0f;
    // Events that the PlayerUI will subscribe to
    public event System.Action<byte> OnPlayerNumberChanged;
    public event System.Action<Color32> OnPlayerColorChanged;
    public event System.Action<byte> OnPlayerPointsChanged;
    // Players List to manage playerNumber
    static readonly List<PlayerMovement> playersList = new List<PlayerMovement>();
    //[SyncVar] public byte[] teamPoints = new byte[2];
    [SyncVar]public byte teamIndex;
    [Header("Player UI")]
    public GameObject playerUIPrefab;
    GameObject playerUIObject;
    PlayerUI playerUI = null;
    bool isReady;
    #region SyncVars

    [Header("SyncVars")]
    /// <summary>
    /// This is appended to the player name text, e.g. "Player 01"
    /// </summary>
    [SyncVar(hook = nameof(PlayerNumberChanged))]public byte playerNumber = 0;

    /// <summary>
    /// Random color for the playerData text, assigned in OnStartServer
    /// </summary>
    [SyncVar(hook = nameof(PlayerColorChanged))]public Color32 playerColor = Color.white;
    [SyncVar/* (hook = nameof(PlayerPointsChanged)) */]public byte points = 0;
    // This is called by the hook of playerNumber SyncVar above
    void PlayerNumberChanged(byte _, byte newPlayerNumber)
    {
        OnPlayerNumberChanged?.Invoke(newPlayerNumber);
    }
    // This is called by the hook of playerColor SyncVar above
    void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
    {
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.color = newPlayerColor;
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }
    [Command(requiresAuthority =false)]
    public void CmdGainPoints(byte pointsGained)
    {
        points += 1;
        if (!GameManager.instance.teamModeOn)
        {
            if (points >= 10)
            {
                GameManager.instance.Win(netIdentity);
            }
        }
        else
        {
            foreach (PlayerMovement p in playersList)
            {
                if (p.teamIndex == teamIndex)
                {
                    if (p != this)
                    {
                        p.points = points;
                        p.playerUI.playerPointsText.text=string.Format("Points: {0:00}", p.points);
                    }
                }
            }
            if (points > 10)
            {
                NetworkIdentity[] networkIdentities = new NetworkIdentity[playersList.Count / 2];
                int i = 0;
                foreach (PlayerMovement p in playersList)
                {
                    if (p.teamIndex == teamIndex)
                    {
                        networkIdentities[i] = p.netIdentity;
                        i++;
                    }
                }
                GameManager.instance.TeamWin(networkIdentities);
            }
        }
        RpcDisplayPoints();
    }
    [ClientRpc]
    public void RpcDisplayPoints()
    {
        StartCoroutine(DisplayPoints());
    }
    IEnumerator DisplayPoints()
    {
        yield return new WaitForSeconds(.1f);
        OnPlayerPointsChanged?.Invoke(points);
    }
    // This is called by the hook of playerData SyncVar above
    void PlayerPointsChanged(byte _, byte newPoints)
    {
        if (!GameManager.instance.teamModeOn)
        {
            OnPlayerPointsChanged?.Invoke(newPoints);
            if (newPoints >= 10)
            {
                GameManager.instance.Win(netIdentity);
            }
        }
        else
        {
            int teamPoints = 0;
            foreach (PlayerMovement p in playersList)
            {
                if (p.teamIndex == teamIndex)
                {
                    teamPoints += p.points;
                }
            }
            if (teamPoints >= 10)
            {
                NetworkIdentity[] networkIdentities = new NetworkIdentity[playersList.Count / 2];
                int i = 0;
                foreach (PlayerMovement p in playersList)
                {
                    if (p.teamIndex == teamIndex)
                    {
                        networkIdentities[i] = p.netIdentity;
                        i++;
                    }
                }
                GameManager.instance.TeamWin(networkIdentities);
            }
        }
    }
        /* int teamPoints = 0;
        foreach (PlayerMovement p in playersList)
        {
            if (p.teamIndex == teamIndex)
            {
                teamPoints += p.points;
            }
        }
        OnPlayerPointsChanged.Invoke((byte)teamPoints); */
    /* [Command]
    void CmdIncreaseTeamPoints()
    {
        teamPoints[teamIndex]++;
    } */
    #endregion
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        // Add this to the static Players List
        playersList.Add(this);
        // set the Player Color SyncVar
        playerColor = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);
        // set the initial player data
        points = 0;

    }
    // This is called from BasicNetManager OnServerAddPlayer and OnServerDisconnect
    // Player numbers are reset whenever a player joins / leaves
    [ServerCallback]
    public static void ResetPlayerNumbers()
    {
        byte playerNumber = 1;
        foreach (PlayerMovement player in playersList)
            player.playerNumber = playerNumber++;
    }
    [ServerCallback]
    public static void SetTeamColors()
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            if (i < playersList.Count / 2)
            {
                playersList[i].playerColor = new Color32(255, 0, 0, 255);
                playersList[i].teamIndex = 0;
            }
            else
            {
                playersList[i].playerColor = new Color32(0, 0, 255, 255);
                playersList[i].teamIndex = 1;
            }
        }
    }
    #endregion
    #region  Client
    public override void OnStartAuthority()
    {
        if (myCamera != null)
        {
            myCamera.enabled = true;
        }
        if (myAudioListener != null)
        {
            myAudioListener.enabled = true;
        }
    }
    public override void OnStartClient()
    {
        // Instantiate the player UI as child of the Players Panel
        playerUIObject = Instantiate(playerUIPrefab, CanvasUI.instance.GetPlayersPanel());
        playerUI = playerUIObject.GetComponent<PlayerUI>();
        // wire up all events to handlers in PlayerUI
        OnPlayerNumberChanged = playerUI.OnPlayerNumberChanged;
        OnPlayerColorChanged = playerUI.OnPlayerColorChanged;
        OnPlayerPointsChanged = playerUI.OnPlayerPointsChanged;
        // Invoke all event handlers with the initial data from spawn payload
        OnPlayerNumberChanged.Invoke(playerNumber);
        OnPlayerColorChanged.Invoke(playerColor);
        OnPlayerPointsChanged.Invoke(points);
        currentSpeed = speed;
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.color = playerColor;
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        // Activate the main panel
        CanvasUI.instance.SetActive(true);
    }
    public override void OnStopLocalPlayer()
    {
        // Disable the main panel for local player
        CanvasUI.instance.SetActive(false);
    }
    public override void OnStopClient()
    {
        // disconnect event handlers
        OnPlayerNumberChanged = null;
        OnPlayerColorChanged = null;
        OnPlayerPointsChanged = null;
        // Remove this player's UI object
        Destroy(playerUIObject);
    }
    #endregion
    #region Unity
    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) return; //funcionou sem isso, mas por via das duvidas ta ai né
        //mover
        moveInput = moveAction.ReadValue<Vector2>();
        moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(moveDir * currentSpeed * Time.deltaTime);
        //Olhar
        lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        myCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Apply vertical look to camera
        transform.Rotate(Vector3.up * mouseX);
        // Apply horizontal look to player/parent
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isReady)
            {
                GameManager.instance.CmdReadyPlayers(-1);
                isReady = false;
            }
            else
            {
                GameManager.instance.CmdReadyPlayers(+1);
                isReady = true;
            }
        }
    }
    #endregion
    [TargetRpc]
    public void TargetTeleportPlayer()
    {
        float radius;
        if (TreasureSpawner.instance != null)
        {
            radius = TreasureSpawner.instance.radius;
        }
        else
        {
            radius = 20f;
            Debug.LogWarning($"Não achamos o treasureSpawner, então settando o raio do Tp pra {radius}");
        }   
        Vector2 pos2 = Random.insideUnitCircle * radius;
        Vector3 pos = new Vector3(pos2.x, 1, pos2.y);
        transform.position = pos;
        Physics.SyncTransforms();
    }
    [TargetRpc]
    public void TargetSlowPlayer()
    {
        currentSpeed = speed / 2;
        Invoke(nameof(ReturnSpeed), 3f);
    }
    public void ReturnSpeed()
    {
        currentSpeed = speed;
    }
}
