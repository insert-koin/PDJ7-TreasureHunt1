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
    [SyncVar]public byte readyPlayers;
    public override void OnStartClient()
    {
        base.OnStartClient();
        instance = this;
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
}
