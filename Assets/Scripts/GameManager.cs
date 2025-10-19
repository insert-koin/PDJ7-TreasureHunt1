using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    [SerializeField] int MinutesPerGame = 5;
    float timer;
    [SyncVar]public bool gameIsRunning;
    public static int numberOfPlayers;
    [SyncVar] public bool teamModeOn;
    [SyncVar] public byte readyPlayers;
    public override void OnStartServer()
    {
        base.OnStartServer();
        instance = this;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(instance==null)instance = this;
        timer = 60 * MinutesPerGame;
    }
    void StartGame()
    {
        gameIsRunning = true;
    }
    void Update()
    {
        if (gameIsRunning)
        {
            timer -= Time.deltaTime;
            CanvasUI.instance.UpdateTime(timer);
            if (timer < 0)
            {
                LoseByTime();
            }
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log($"Temos {numberOfPlayers} players");
            Debug.Log($"Temos {readyPlayers} players ready ");
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdReadyPlayers(int n)
    {
        readyPlayers += (byte)n;
        if (readyPlayers == numberOfPlayers) StartGame();
    }
    [ServerCallback]
    public void LoseByTime()
    {
        gameIsRunning = false;
        RpcLose();
    }
    [ClientRpc]
    void RpcLose()
    {
        CanvasUI.instance.Lose();
    }
    [ServerCallback]
    public void Win(NetworkIdentity netId)
    {
        gameIsRunning = false;
        RpcLose();
        TargetWin(netId.connectionToClient);
    }
    [TargetRpc]
    void TargetWin(NetworkConnectionToClient conn)
    {
        CanvasUI.instance.Win();
    }
    [ServerCallback]
    public void TeamWin(NetworkIdentity[] netIds)
    {
        gameIsRunning = false;
        RpcLose();
        foreach(NetworkIdentity ni in netIds)
            TargetWin(ni.connectionToClient);
    }
    public void AddPlayer()
    {
        numberOfPlayers++;
        Debug.Log($"Temos {numberOfPlayers} players");
        if (numberOfPlayers == 4)
        {
            Debug.Log($"Temos {numberOfPlayers} players, entando no modo multiplayer");
            teamModeOn = true;
            PlayerMovement.SetTeamColors();
        }
    }
    public void RemovePlayer()
    {
        numberOfPlayers--;
    }
}
