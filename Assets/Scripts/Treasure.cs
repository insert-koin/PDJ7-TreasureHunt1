using Mirror;
using UnityEngine;

public class Treasure : NetworkBehaviour
{
    [Server]
    void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;
        player.CmdGainPoints(1);
        TreasureSpawner.instance.SpawnTreasure(1);
        Destroy(gameObject);
    }
}
