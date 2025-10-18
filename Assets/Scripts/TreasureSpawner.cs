using Mirror;
using UnityEngine;

public class TreasureSpawner : NetworkBehaviour
{
    [SerializeField] GameObject TreasurePrefab;
    public static TreasureSpawner instance;
    [Range(10,50)] public float radius;

    public override void OnStartClient()
    {
        base.OnStartClient();
        instance = this;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnTreasure(10);
    }
    public void SpawnTreasure(int ammount = 1)
    {
        for(int i = 0; i < ammount; i++)
        {
            Vector2 pos2 = Random.insideUnitCircle * radius;
            Vector3 pos = new Vector3(pos2.x, .5f, pos2.y);
            GameObject newTreasure = Instantiate(TreasurePrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(newTreasure);
        }
    }
}
