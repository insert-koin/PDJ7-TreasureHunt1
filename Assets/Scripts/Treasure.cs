using Mirror;
using UnityEngine;

public class Treasure : NetworkBehaviour
{
    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        player.CmdGainPoints(1);
        TreasureSpawner.instance.SpawnTreasure(1);
        Destroy(gameObject);
    }
}
