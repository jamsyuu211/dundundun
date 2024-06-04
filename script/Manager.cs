using System;
using System.IO;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;
    GameObject player;
    playerMove playerScript;
    PlayerHealth playerHpScript;

    public GameObject exitMenu;

    public GameObject[] movingBlock;
    public FlyingBlock[] movingScript;

    public GameObject[] dog;
    public Dog[] dogScript;
    public GameObject[] bird;
    public Bird[] birdScript;
    public GameObject[] bomber;
    public Bomber[] bomberScript;
    public GameObject[] GroundBomber;
    public GroundBomber[] GroundBomberScript;
    public GameObject LongArm;
    public LongArm longArmScript;

    public int Stage = 1;

    //json���� ����
    private string filePath;
    GameData dataToSave;


    //weapon ���� ���� ����
    public bool Restart = false;
    float restartT = 0f;

    //longArm ���� ���� ����
    bool isSpawn = true;

    public static Manager Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("Manager");
                instance = go.AddComponent<Manager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance != null && instance != this)//�̹� ���ӸŴ����� ������
        {
            Destroy(gameObject);//�̸� �����ߴ� ���ӿ�����Ʈ�� �ı�
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        playerHpScript = player.GetComponent<PlayerHealth>();

        exitMenu.SetActive(false);

        //json���� ���� �ڵ�
        filePath = Application.persistentDataPath + "/gameData.json";

        // ������ �ҷ����� ����
        LoadData();
    }

    // ������ ������ Ŭ���� ����
    [Serializable]
    public class GameData
    {
        public float playerDamage;
        public float playerHp;
        public float playerMaxHp;
        public int stage;
    }
    // �����͸� JSON �������� �����ϴ� �޼���
    public void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("Data Saved: " + json);
    }

    // JSON ���Ͽ��� �����͸� �ҷ����� �޼���
    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            dataToSave = data;
            Stage = data.stage;
            playerScript.Damage = data.playerDamage;
            playerScript.hp = data.playerHp;
            playerScript.maxHp = data.playerMaxHp;

            Debug.Log("Data Loaded: " + json);
            Debug.Log("player Dmage: " + data.playerDamage);
            Debug.Log("Player MaxHp: " + data.playerMaxHp);
            Debug.Log("Player Hp: " + data.playerHp);
            Debug.Log("stage: " + data.stage);
        }
        else
        {
            Debug.Log("No data found");
        }
    }

    public void ResetGame()
    {
        dataToSave = new GameData();
        dataToSave.playerDamage = 0.5f;
        dataToSave.playerHp = 3f;
        dataToSave.stage = 1;
        dataToSave.playerMaxHp = 3f;

        SaveData(dataToSave);

        LoadData();
        playerScript.publicReset();

        QuitMenu();
    }

    public void QuitGame()
    {
        // ������ ���� ����
        dataToSave = new GameData();
        dataToSave.playerDamage = playerScript.Damage;
        dataToSave.playerHp = playerScript.hp;
        dataToSave.stage = Stage;
        dataToSave.playerMaxHp = playerScript.maxHp;

        SaveData(dataToSave);
        Application.Quit();

#if UNITY_EDITOR
        // �����Ϳ��� ���� ���� ��� ������ ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void QuitMenu()
    {
        Time.timeScale = 1;
        exitMenu.SetActive(false);
    }

    //���� ����� �� �÷��̾� Ȱ�� ���� ���� ���� ����
    void Update()
    {
        //���� ���� �ڵ�
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(exitMenu.activeInHierarchy)
            {
                Time.timeScale = 1;
                exitMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                exitMenu.SetActive(true);
            }
        }


        if (playerScript.enemyActive)//�÷��̾� ����� true
        {
            Restart = true;
            playerScript.enemyActive = false;
            ActiveFalse();


            ActiveTrue();
        }
        else
        {
            RespawnDog();
            RespawnBird();
            Respawnbomber();
            RespawnGroundBomber();
        }

        if (playerScript.isBossZone)
        {
            if(!isSpawn)
            {
                isSpawn = true;
                RespawnLongArm();
            }
        }
        else
        {
            if (isSpawn)
            {
                isSpawn = false;
                LongArm.SetActive(false);
            }
        }
        
        if(Restart)
        {
            restartT += Time.deltaTime;
            if(restartT > 0.5f)
            {
                restartT = 0f;
                Restart = false;
            }
        }
    }

    //Dog ������ �Լ�
    void RespawnDog()
    {
        int dogIndex = 0;
        foreach (GameObject dogs in dog)
        {
            if (!dogs.activeInHierarchy)
            {
                dogScript[dogIndex].SpawnT += Time.deltaTime;
                if (dogScript[dogIndex].SpawnT > dogScript[dogIndex].SpawnDuration)
                {
                    dogs.SetActive(true);
                    dogScript[dogIndex].publicReset();
                }
            }
            dogIndex++;
        }
    }

    void RespawnBird()
    {

        int birdIndex = 0;
        foreach (GameObject birds in bird)
        {
            if (!birds.activeInHierarchy)
            {
                birdScript[birdIndex].SpawnT += Time.deltaTime;
                if (birdScript[birdIndex].SpawnT > birdScript[birdIndex].SpawnDuration)
                {
                    birds.SetActive(true);
                    birdScript[birdIndex].publicReset();
                }
            }
            birdIndex++;
        }
    }

    void Respawnbomber()
    {
        int bomberIndex = 0;
        foreach(GameObject bombers in bomber)
        {
            if (!bombers.activeInHierarchy)
            {
                bomberScript[bomberIndex].spawnT += Time.deltaTime;
                if (bomberScript[bomberIndex].spawnT > bomberScript[bomberIndex].SpawnDuration)
                {
                    bombers.SetActive(true);
                    bomberScript[bomberIndex].publicReset();
                }
            }
            bomberIndex++;
        }
    }

    void RespawnGroundBomber()
    {
        int gbIndex = 0;
        foreach(GameObject gbs in GroundBomber)
        {
            if (!gbs.activeInHierarchy)
            {
                GroundBomberScript[gbIndex].SpawnT += Time.deltaTime;
                if (GroundBomberScript[gbIndex].SpawnT > GroundBomberScript[gbIndex].SpawnDuration)
                {
                    gbs.SetActive(true);
                    GroundBomberScript[gbIndex].publicReset();
                }
            }
            gbIndex++;
        }
    }

  void RespawnLongArm()
    {
        if (!LongArm.activeInHierarchy)
        {
            LongArm.SetActive(true);
            longArmScript.tmpDamage = 1.2513f * Mathf.Exp(0.5573f * Stage);
            longArmScript.tmpHp = 7.4983f * Mathf.Exp(0.4394f * Stage);
            longArmScript.publicReset();
        }
    }

    //��ü Ȱ��ȭ �Լ�
    void ActiveTrue()
    {
        player.SetActive(true);
        playerScript.publicReset();
        playerScript.Trail.SetActive(false);
        playerScript.hp = playerScript.maxHp;
        playerHpScript.publicReset();
        playerScript.Damage = dataToSave.playerDamage;

        int blockIndex = 0;
        foreach(GameObject blocks in movingBlock)
        {
            movingScript[blockIndex++].publicReset();
        }

        int DogIndex = 0;
        foreach(GameObject dogs in dog)
        {
            dogs.SetActive(true);
            dogScript[DogIndex].tmpHp = 2.1184f * Mathf.Exp(0.4136f * Stage);
            dogScript[DogIndex].tmpDamage = 3.7857f * Mathf.Exp(0.3067f * Stage) / 10f;
            dogScript[DogIndex++].publicReset();
        }

        int BirdIndex = 0;
        foreach (GameObject birds in bird)
        {
            birds.SetActive(true);
            birdScript[BirdIndex].tmpHp = 1.3818f * Mathf.Exp(0.4946f * Stage);
            birdScript[BirdIndex].tmpDamage = 5.4945f * Mathf.Exp(0.2508f * Stage) / 10f;
            birdScript[BirdIndex++].publicReset();
        }

        int bomberIndex = 0;
        foreach(GameObject bombers in bomber)
        {
            bombers.SetActive(true);
            bomberScript[bomberIndex].tmpHp = 8.1717f * Mathf.Exp(0.1982f * Stage) / 10f;
            bomberScript[bomberIndex].tmpDamage = 8.1717f * Mathf.Exp(0.1982f * Stage) / 10f;
            bomberScript[bomberIndex++].publicReset();
        }

        int GbIndex = 0;
        foreach (GameObject gbs in GroundBomber)
        {
            gbs.SetActive(true);
            GroundBomberScript[GbIndex].tmpHp = 12.811f * Mathf.Exp(0.1478f * Stage) / 10f;
            GroundBomberScript[GbIndex].tmpDamage = 12.811f * Mathf.Exp(0.1478f * Stage) / 10f;
            GroundBomberScript[GbIndex++].publicReset();
        }
    }

    //��ü ��Ȱ��ȭ �Լ�
    void ActiveFalse()
    {
        foreach(GameObject dogs in dog)
        {
            dogs.SetActive(false);
        }
        foreach(GameObject birds in bird)
        {
            birds.SetActive(false);
        }
        foreach(GameObject bombers in bomber)
        {
            bombers.SetActive(false);
        }
        foreach(GameObject gbs in GroundBomber)
        {
            gbs.SetActive(false);
        }
    }
}
