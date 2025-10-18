using Mirror;
using UnityEngine;

public class Trap : NetworkBehaviour
{
    [SerializeField] TrapType trapType;
    [Server]
    void OnTriggerEnter(Collider other)
    {
        switch (trapType)
        {
            case TrapType.slow:
                {
                    PlayerMovement player = other.GetComponent<PlayerMovement>();
                    player.TargetSlowPlayer();
                }
                break;
        }
    }
}
public enum TrapType
{
    slow,
    teleport
}
