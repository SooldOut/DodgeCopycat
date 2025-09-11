using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // === UI 패널 변수 ===
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    // === 추가된 UI 변수 ===
    public GameObject winPanel;
    public GameObject deathPanel;

    public Transform playerStartingPoint;
    public GameObject playerObject; // 플레이어 오브젝트를 연결할 변수

    public BulletSpawner[] bulletSpawners;

    // === 몬스터/다이아몬드 재생성 관련 변수 추가 ===
    public GameObject monsterPrefab;
    public GameObject diamondPrefab;
    public int numberOfMonsters = 5;
    public int numberOfDiamonds = 3;
    public Vector3 spawnAreaMin = new Vector3(-50, 10, -50);
    public Vector3 spawnAreaMax = new Vector3(50, 10, 50);

    // 현재 생성된 몬스터/다이아몬드 오브젝트들을 추적하기 위한 리스트
    private List<GameObject> currentMonsters = new List<GameObject>();
    private List<GameObject> currentDiamonds = new List<GameObject>();

    // === 몬스터 생성 시간 간격 변수 추가 ===
    public float monsterSpawnInterval = 5f;
    private int spawnedMonsterCount = 0;
    private Coroutine monsterSpawnCoroutine;

    // === 상태 변수 ===
    private bool isGameStarted = false;
    public bool isPaused = false; // <-- private을 public으로 변경!
    private bool isCountingDown = false;
    private bool isResuming = false;

    // === 배경음악 관련 변수 추가 ===
    public AudioSource backgroundMusicPlayer; // AudioSource 컴포넌트를 연결할 변수


    void Awake()
    {
        InitializeGameOnAwake();
    }

    void InitializeGameOnAwake()
    {
        Time.timeScale = 0f;
        startPanel.SetActive(true);
        pausePanel.SetActive(false);
        countdownPanel.SetActive(false);
        winPanel.SetActive(false);
        deathPanel.SetActive(false);

        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.gameObject.SetActive(false);
        }

        ClearAllGameObjects();

        // 배경음악 초기화 및 재생 (게임 시작 전 대기 화면에서 재생)
        if (backgroundMusicPlayer != null && !backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.Play();
        }
    }

    public bool IsGameStarted()
    {
        return isGameStarted;
    }

    public void StartGame()
    {
        if (isCountingDown) return;

        ResetGameElements();

        isCountingDown = true;
        isGameStarted = false;
        isResuming = false;
        isPaused = false;

        startPanel.SetActive(false);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        deathPanel.SetActive(false);
        countdownPanel.SetActive(true);

        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        int countdownTime = 3;

        Time.timeScale = 0f;

        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSecondsRealtime(1f);
            countdownTime--;
        }

        countdownText.text = "Start!";
        yield return new WaitForSecondsRealtime(1f);

        countdownPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isCountingDown = false;

        if (!isResuming)
        {
            isGameStarted = true;
            if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);
            monsterSpawnCoroutine = StartCoroutine(SpawnMonstersOverTime());
        }

        isResuming = false;

        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.gameObject.SetActive(true);
        }
    }

    public void PauseGame()
    {
        if (!isGameStarted || isCountingDown) return;

        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);
        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.gameObject.SetActive(false);
        }

        // 배경음악 일시정지
        if (backgroundMusicPlayer != null && backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.Pause();
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        pausePanel.SetActive(false);
        countdownPanel.SetActive(true);
        isResuming = true;
        isCountingDown = true;

        // 몬스터 생성 코루틴 재개
        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);
        monsterSpawnCoroutine = StartCoroutine(SpawnMonstersOverTime());

        // BulletSpawner 재개
        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.gameObject.SetActive(true);
        }

        // 배경음악 재개
        if (backgroundMusicPlayer != null && !backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.UnPause();
        }

        StartCoroutine(CountdownRoutine());
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }

    public void WinGame()
    {
        Time.timeScale = 0f;
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        isGameStarted = false;
        isPaused = false;

        ClearAllGameObjects();
        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);

        // 게임 종료/승리 시 배경음악 정지
        if (backgroundMusicPlayer != null && backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.Stop();
        }
    }

    public void LoseGame()
    {
        Time.timeScale = 0f;
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        isGameStarted = false;
        isPaused = false;

        if (playerObject != null)
        {
            playerObject.SetActive(true);
        }

        ClearAllGameObjects();
        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);

        // 게임 종료/패배 시 배경음악 정지
        if (backgroundMusicPlayer != null && backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.Stop();
        }
    }

    private void ResetGameElements()
    {
        ClearAllGameObjects();
        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);
        spawnedMonsterCount = 0;

        if (playerObject != null && playerStartingPoint != null)
        {
            playerObject.transform.position = playerStartingPoint.position;
            playerObject.transform.rotation = playerStartingPoint.rotation;
        }
        playerObject.SetActive(true);

        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.ResetPosition();
            spawner.gameObject.SetActive(false);
        }

        SpawnDiamonds();
    }

    private void ClearAllGameObjects()
    {
        Debug.Log("--- ClearAllGameObjects 호출됨 ---");

        foreach (GameObject monster in currentMonsters)
        {
            if (monster != null)
            {
                Debug.Log("Destroying monster from list: " + monster.name);
                Destroy(monster);
            }
        }
        currentMonsters.Clear();
        Debug.Log("currentMonsters 리스트 클리어됨.");


        foreach (GameObject diamond in currentDiamonds)
        {
            if (diamond != null)
            {
                Debug.Log("Destroying diamond from list: " + diamond.name);
                Destroy(diamond);
            }
        }
        currentDiamonds.Clear();
        Debug.Log("currentDiamonds 리스트 클리어됨.");

        GameObject[] looseMonsters = GameObject.FindGameObjectsWithTag("Monster");
        if (looseMonsters.Length > 0)
        {
            Debug.LogWarning("씬에서 리스트에 없던 'Monster' 태그 오브젝트 " + looseMonsters.Length + "개 발견! 파괴합니다.");
            foreach (GameObject monster in looseMonsters)
            {
                Destroy(monster);
            }
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        if (bullets.Length > 0)
        {
            Debug.Log("씬에서 'Bullet' 태그 오브젝트 " + bullets.Length + "개 파괴합니다.");
            foreach (GameObject bullet in bullets)
            {
                Destroy(bullet);
            }
        }
        Debug.Log("--- ClearAllGameObjects 완료 ---");
    }

    IEnumerator SpawnMonstersOverTime()
    {
        if (!isResuming)
        {
            spawnedMonsterCount = 0;
        }

        while (spawnedMonsterCount < numberOfMonsters)
        {
            yield return new WaitForSeconds(monsterSpawnInterval);

            if (Time.timeScale > 0 && spawnedMonsterCount < numberOfMonsters)
            {
                SpawnSingleMonster();
                spawnedMonsterCount++;
            }
        }
        Debug.Log("모든 몬스터가 생성되었습니다.");
    }

    private void SpawnSingleMonster()
    {
        if (monsterPrefab == null)
        {
            Debug.LogError("Monster Prefab이 할당되지 않았습니다! 몬스터를 생성할 수 없습니다.");
            return;
        }
        Vector3 randomPos = GetRandomSpawnPosition();
        GameObject newMonster = Instantiate(monsterPrefab, randomPos, Quaternion.identity);
        currentMonsters.Add(newMonster);
        Debug.Log("몬스터 생성: " + newMonster.name + " at " + randomPos);
    }

    private void SpawnDiamonds()
    {
        if (diamondPrefab == null)
        {
            Debug.LogError("Diamond Prefab이 할당되지 않았습니다! 다이아몬드를 생성할 수 없습니다.");
            return;
        }
        currentDiamonds.Clear();
        for (int i = 0; i < numberOfDiamonds; i++)
        {
            Vector3 randomPos = GetRandomSpawnPosition();
            GameObject newDiamond = Instantiate(diamondPrefab, randomPos, Quaternion.identity);
            currentDiamonds.Add(newDiamond);
            Debug.Log("다이아몬드 생성: " + newDiamond.name + " at " + randomPos);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);
        return new Vector3(randomX, randomY, randomZ);
    }

    void Update()
    {
        if (isGameStarted && !isCountingDown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }
}