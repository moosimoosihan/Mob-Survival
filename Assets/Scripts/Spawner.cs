using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("소환 정보")]
    public Transform[] spawnPoint;
    int monsterIndex;

    [SerializeField]
    TextAsset enemyDatabase;
    [SerializeField]
    List<enemySpawnData> enemySpawnDataList = new List<enemySpawnData>();

    [Header("소환 시퀀스 정보")]
    float timer;
    [SerializeField]
    TextAsset enemySpwanDatabase;
    List<spawnData> spawnDataList = new List<spawnData>();
    public int curSequence;
    public int bossCount = 0;
    private IObjectPool<Enemy> _Pool;
    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        //몬스터 데이터 불러오기
        string seperator = "\r\n";
        string[] lines = enemyDatabase.text.Substring(0).Split(seperator);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] rows = lines[i].Split('\t');

            //아이템 생성
            enemySpawnData tempEnemySpawnData = new enemySpawnData();
            tempEnemySpawnData.enemyName = rows[0];
            tempEnemySpawnData.health = System.Convert.ToInt32(rows[1]);
            tempEnemySpawnData.speed = System.Convert.ToSingle(rows[2]);
            tempEnemySpawnData.attackDamage = System.Convert.ToSingle(rows[3]);
            tempEnemySpawnData.monsterPrefab = GetSpinePrefabObjFromResourceFolder("Enemy", tempEnemySpawnData.enemyName);
            tempEnemySpawnData.power = 1;

            enemySpawnDataList.Add(tempEnemySpawnData);
        }
        
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
            for(int y=0;y<monTypes.Length;y++){
                tempSpawnData.monTypes[y] = System.Convert.ToInt32(monTypes[y]);
            }
            tempSpawnData.power = System.Convert.ToSingle(rows[5]);

            spawnDataList.Add(tempSpawnData);
        }
        _Pool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, collectionCheck: false, maxSize: 20);
    }
    void Update()
    {
        // 테스트를 위하여 스페이스바를 누를 시 소환
        // if (Input.GetButtonDown("Jump"))
        // {
        //     Spawn();
        // }
        timer += Time.deltaTime;
        if(timer>=spawnDataList[curSequence].curTime){
            StartCoroutine(spwanCo(spawnDataList[curSequence].spwanInterval, spawnDataList[curSequence].monsterAmount, spawnDataList[curSequence].monTypes));
            curSequence++;
        }
    }

    void Spawn(int index)
    {
        monsterIndex = index-1;
        
        Enemy enemy = _Pool.Get();
        enemy.transform.parent = GameManager.instance.pool.transform;

        int randPointIndex = Random.Range(1, spawnPoint.Length);
        enemy.transform.position = spawnPoint[randPointIndex].position;
        if(enemy.GetComponent<Enemy>()){
            enemy.GetComponent<Enemy>().Init(enemySpawnDataList[monsterIndex]);
        }
    }
    Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(enemySpawnDataList[monsterIndex].monsterPrefab).GetComponent<Enemy>();
        enemy.SetManagedPool(_Pool);
        return enemy;
    }
    void OnGetEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }
    void OnReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }


    //Sprite GetSpriteImgFromResourceFolder(string _folderName, string _spriteName)
    //{
    //    string imgPath = $"{_folderName}/{_spriteName}";
    //    Sprite loadedSprite = Resources.Load<Sprite>(imgPath);

    //    return loadedSprite;
    //}

    GameObject GetSpinePrefabObjFromResourceFolder(string _folderName, string _prefabName)
    {
        string tempPath = $"{_folderName}/{_prefabName}";
        GameObject loadedObj = Resources.Load<GameObject>(tempPath);

        return loadedObj;
    }
    IEnumerator spwanCo(float interval, int amount, int[] type) {
        bossCount = 0;
        float spawnInterval = interval / amount; // 몬스터 생성 간격
        for (int i = 0; i < amount; i++) {
            int index = i % type.Length;
            if((type[index]>9)){
                if(bossCount<1){ // 보스몹 소환 가능
                    int monsterType = type[index];
                    Spawn(monsterType);
                    bossCount++;
                } else {
                    int monsterType = type[index - 1];
                    Spawn(monsterType);
                }
            } else {
                index = i % type.Length;
                int monsterType = type[index];
                Spawn(monsterType);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

[System.Serializable]
public class enemySpawnData
{
    public string enemyName;
    public int health;
    public float speed;
    public float attackDamage;
    public GameObject monsterPrefab;
    public float power;
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
