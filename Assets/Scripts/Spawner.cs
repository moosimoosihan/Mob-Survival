using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using olimsko;
using Cysharp.Threading.Tasks;

public class Spawner : MonoBehaviour
{
    [Header("소환 정보")]
    public Transform[] spawnPoint;
    int monsterIndex;

    [SerializeField]
    TextAsset enemyDatabase;

    private List<MonsterTable> MonsterTable => OSManager.GetService<DataManager>().GetData<MonsterTableSO>().MonsterTable;
    public int enemyPoolMaxSize;

    [Header("소환 시퀀스 정보")]
    float timer;
    [SerializeField]
    TextAsset enemySpwanDatabase;
    List<spawnData> spawnDataList = new List<spawnData>();
    public int curSequence;
    public int bossCount = 0;
    private IObjectPool<Enemy> _Pool;
    public GameObject enemyPrefab;
    public GameObject[] bossPrefab;
    float difficulty;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        //몬스터 데이터 불러오기
        string seperator = "\r\n";
        string[] lines = enemyDatabase.text.Substring(0).Split(seperator);

        // 몬스터 스폰 데이터 불러오기
        string spwanSeperator = "\r\n";
        string[] spwanlines = enemySpwanDatabase.text.Substring(0).Split(spwanSeperator);

        for (int i = 0; i < spwanlines.Length; i++)
        {
            string[] rows = spwanlines[i].Split('\t');

            // 스폰 데이터 생성
            spawnData tempSpawnData = new spawnData();
            string[] timePart = rows[0].Split('~');
            string[] timeParts = timePart[0].Split(':');
            int minutes = int.Parse(timeParts[0]);
            int seconds = int.Parse(timeParts[1]);
            tempSpawnData.curTime = minutes * 60 + seconds;
            tempSpawnData.sequence = System.Convert.ToInt32(rows[1]);
            tempSpawnData.spwanInterval = System.Convert.ToSingle(rows[2]);
            tempSpawnData.monsterAmount = System.Convert.ToInt32(rows[3]);
            string[] monTypes = rows[4].Split(',');
            tempSpawnData.monTypes = new int[monTypes.Length];
            for (int y = 0; y < monTypes.Length; y++)
            {
                tempSpawnData.monTypes[y] = System.Convert.ToInt32(monTypes[y]);
            }
            tempSpawnData.power = System.Convert.ToSingle(rows[5]);

            spawnDataList.Add(tempSpawnData);
        }
        _Pool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize : enemyPoolMaxSize);
    }
    void Start()
    {
        difficulty = GameManager.instance.StageContext.SelectedDifficulty==1? 1: GameManager.instance.StageContext.SelectedDifficulty==2? 1.5f: 2;
    }
    void Update()
    {
        // 테스트를 위하여 스페이스바를 누를 시 소환
        // if (Input.GetButtonDown("Jump"))
        // {
        //     Spawn();
        // }
        timer += Time.deltaTime;
        if (timer >= spawnDataList[curSequence].curTime)
        {
            StartCoroutine(spwanCo(spawnDataList[curSequence].spwanInterval, spawnDataList[curSequence].monsterAmount, spawnDataList[curSequence].monTypes));
            curSequence++;
        }
    }

    async UniTask Spawn(int index)
    {
        monsterIndex = index - 1;

        Enemy enemy = _Pool.Get();
        enemy.transform.parent = GameManager.instance.pool.transform;
        int randPointIndex = Random.Range(1, spawnPoint.Length);
        enemy.transform.position = spawnPoint[randPointIndex].position;
        enemy.skeletonAnimation.skeletonDataAsset = await MonsterTable[monsterIndex].GetSkeletonDataAsset();
        enemy.skeletonAnimation.ClearState();
        enemy.skeletonAnimation.Initialize(true);

        if (enemy != null)
        {
            enemy.Init(MonsterTable[monsterIndex], difficulty);
        }
        enemy.gameObject.SetActive(true);
    }
    void SpawnBoss(int index)
    {
        monsterIndex = index - 1;
        int randPointIndex = Random.Range(1, spawnPoint.Length);
        GameObject boss = null;
        switch (monsterIndex)
        {
            case 9:
                GameObject silmeBoss = Instantiate(bossPrefab[0], GameManager.instance.pool.transform);
                boss = silmeBoss;
                break;
            case 10:
                GameObject golemBoss = Instantiate(bossPrefab[1], GameManager.instance.pool.transform);
                boss = golemBoss;
                break;
            case 11:
                GameObject goblinMage = Instantiate(bossPrefab[2], GameManager.instance.pool.transform);
                boss = goblinMage;
                break;
            case 12:
                GameObject goblinKing = Instantiate(bossPrefab[3], GameManager.instance.pool.transform);
                boss = goblinKing;
                break;
            case 16:
                GameObject giantEnt = Instantiate(bossPrefab[4], GameManager.instance.pool.transform);
                boss = giantEnt;
                break;
            case 17:
                GameObject basilisk = Instantiate(bossPrefab[5], GameManager.instance.pool.transform);
                boss = basilisk;
                break;
            case 18:
                GameObject twinHeadOgre = Instantiate(bossPrefab[6], GameManager.instance.pool.transform);
                boss = twinHeadOgre;
                break;
            case 19:
                GameObject medusa = Instantiate(bossPrefab[7], GameManager.instance.pool.transform);
                boss = medusa;
                break;
        }
        boss.transform.parent = GameManager.instance.pool.transform;
        boss.transform.position = spawnPoint[randPointIndex].position;
        boss.GetComponent<Enemy>().Init(MonsterTable[monsterIndex], difficulty);
    }
    // 일반 몬스터
    Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.SetManagedPool(_Pool);
        return enemy;
    }
    void OnGetEnemy(Enemy enemy)
    {

    }
    void OnReleaseEnemy(Enemy enemy)
    {
        if (enemy.gameObject.activeSelf)
            enemy.gameObject.SetActive(false);
    }
    void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    GameObject GetSpinePrefabObjFromResourceFolder(string _folderName, string _prefabName)
    {
        string tempPath = $"{_folderName}/{_prefabName}";
        GameObject loadedObj = Resources.Load<GameObject>(tempPath);

        return loadedObj;
    }
    IEnumerator spwanCo(float interval, int amount, int[] type)
    {
        bossCount = 0;
        float spawnInterval = interval / amount; // 몬스터 생성 간격
        for (int i = 0; i < amount; i++)
        {
            int index = i % type.Length;
            if (type[index] == 10 || type[index] == 11 || type[index] == 12 || type[index] == 13 || type[index] == 17 || type[index] == 18 || type[index] == 19 || type[index] == 20)
            {
                if (bossCount < 1)
                { // 보스몹 소환 가능
                    int monsterType = type[index];
                    SpawnBoss(monsterType);
                    bossCount++;
                }
                else
                {
                    int monsterType = type[Random.Range(0, index - 1)];
                    Spawn(monsterType).Forget();
                }
            }
            else
            {
                index = i % type.Length;
                int monsterType = type[index];
                Spawn(monsterType).Forget();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

[System.Serializable]
public class spawnData
{
    public float curTime;
    public int sequence;
    public float spwanInterval;
    public int monsterAmount;
    public int[] monTypes;
    public float power;
}
