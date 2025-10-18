using Mirror;
using UnityEngine;

public class TreasureSpawner : NetworkBehaviour
{
    [SerializeField] GameObject TreasurePrefab;
    [SerializeField] GameObject[] TrapPrefabs;
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
        SpawnTraps(20);
    }
    public void SpawnTreasure(int ammount = 1)
    {
        for (int i = 0; i < ammount; i++)
        {
            Vector2 pos2 = Random.insideUnitCircle * radius;
            Vector3 pos = new Vector3(pos2.x, .5f, pos2.y);
            GameObject newTreasure = Instantiate(TreasurePrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(newTreasure);
        }
    }
    public void SpawnTraps(int ammount = 1)
    {
        int trapType;
        float height;
        for (int i = 0; i < ammount; i++)
        {
            Vector2 pos2 = Random.insideUnitCircle * radius;
            trapType = Random.Range(0, 2);
            if(trapType==0)//slow trap
            {
                height = .5f;
            }
            else
            {
                height = 1;
            }
            Vector3 pos = new Vector3(pos2.x, height, pos2.y);
            GameObject newTrap = Instantiate(TrapPrefabs[trapType], pos, Quaternion.identity);
            NetworkServer.Spawn(newTrap);
        }
    }
}
