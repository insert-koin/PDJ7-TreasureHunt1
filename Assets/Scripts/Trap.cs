using Mirror;
using UnityEngine;

public class Trap : NetworkBehaviour
{
    [SerializeField] TrapType trapType;
    [ServerCallback]
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
            case TrapType.teleport:
                {
                    PlayerMovement player = other.GetComponent<PlayerMovement>();
                    player?.TargetTeleportPlayer();
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
