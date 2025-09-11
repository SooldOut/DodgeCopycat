using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // === UI �г� ���� ===
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    // === �߰��� UI ���� ===
    public GameObject winPanel;
    public GameObject deathPanel;

    public Transform playerStartingPoint;
    public GameObject playerObject; // �÷��̾� ������Ʈ�� ������ ����

    public BulletSpawner[] bulletSpawners;

    // === ����/���̾Ƹ�� ����� ���� ���� �߰� ===
    public GameObject monsterPrefab;
    public GameObject diamondPrefab;
    public int numberOfMonsters = 5;
    public int numberOfDiamonds = 3;
    public Vector3 spawnAreaMin = new Vector3(-50, 10, -50);
    public Vector3 spawnAreaMax = new Vector3(50, 10, 50);

    // ���� ������ ����/���̾Ƹ�� ������Ʈ���� �����ϱ� ���� ����Ʈ
    private List<GameObject> currentMonsters = new List<GameObject>();
    private List<GameObject> currentDiamonds = new List<GameObject>();

    // === ���� ���� �ð� ���� ���� �߰� ===
    public float monsterSpawnInterval = 5f;
    private int spawnedMonsterCount = 0;
    private Coroutine monsterSpawnCoroutine;

    // === ���� ���� ===
    private bool isGameStarted = false;
    public bool isPaused = false; // <-- private�� public���� ����!
    private bool isCountingDown = false;
    private bool isResuming = false;

    // === ������� ���� ���� �߰� ===
    public AudioSource backgroundMusicPlayer; // AudioSource ������Ʈ�� ������ ����


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

        // ������� �ʱ�ȭ �� ��� (���� ���� �� ��� ȭ�鿡�� ���)
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

        // ������� �Ͻ�����
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

        // ���� ���� �ڷ�ƾ �簳
        if (monsterSpawnCoroutine != null) StopCoroutine(monsterSpawnCoroutine);
        monsterSpawnCoroutine = StartCoroutine(SpawnMonstersOverTime());

        // BulletSpawner �簳
        foreach (BulletSpawner spawner in bulletSpawners)
        {
            spawner.gameObject.SetActive(true);
        }

        // ������� �簳
        if (backgroundMusicPlayer != null && !backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.UnPause();
        }

        StartCoroutine(CountdownRoutine());
    }

    public void QuitGame()
    {
        Debug.Log("���� ����!");
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

        // ���� ����/�¸� �� ������� ����
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

        // ���� ����/�й� �� ������� ����
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
        Debug.Log("--- ClearAllGameObjects ȣ��� ---");

        foreach (GameObject monster in currentMonsters)
        {
            if (monster != null)
            {
                Debug.Log("Destroying monster from list: " + monster.name);
                Destroy(monster);
            }
        }
        currentMonsters.Clear();
        Debug.Log("currentMonsters ����Ʈ Ŭ�����.");


        foreach (GameObject diamond in currentDiamonds)
        {
            if (diamond != null)
            {
                Debug.Log("Destroying diamond from list: " + diamond.name);
                Destroy(diamond);
            }
        }
        currentDiamonds.Clear();
        Debug.Log("currentDiamonds ����Ʈ Ŭ�����.");

        GameObject[] looseMonsters = GameObject.FindGameObjectsWithTag("Monster");
        if (looseMonsters.Length > 0)
        {
            Debug.LogWarning("������ ����Ʈ�� ���� 'Monster' �±� ������Ʈ " + looseMonsters.Length + "�� �߰�! �ı��մϴ�.");
            foreach (GameObject monster in looseMonsters)
            {
                Destroy(monster);
            }
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        if (bullets.Length > 0)
        {
            Debug.Log("������ 'Bullet' �±� ������Ʈ " + bullets.Length + "�� �ı��մϴ�.");
            foreach (GameObject bullet in bullets)
            {
                Destroy(bullet);
            }
        }
        Debug.Log("--- ClearAllGameObjects �Ϸ� ---");
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
        Debug.Log("��� ���Ͱ� �����Ǿ����ϴ�.");
    }

    private void SpawnSingleMonster()
    {
        if (monsterPrefab == null)
        {
            Debug.LogError("Monster Prefab�� �Ҵ���� �ʾҽ��ϴ�! ���͸� ������ �� �����ϴ�.");
            return;
        }
        Vector3 randomPos = GetRandomSpawnPosition();
        GameObject newMonster = Instantiate(monsterPrefab, randomPos, Quaternion.identity);
        currentMonsters.Add(newMonster);
        Debug.Log("���� ����: " + newMonster.name + " at " + randomPos);
    }

    private void SpawnDiamonds()
    {
        if (diamondPrefab == null)
        {
            Debug.LogError("Diamond Prefab�� �Ҵ���� �ʾҽ��ϴ�! ���̾Ƹ�带 ������ �� �����ϴ�.");
            return;
        }
        currentDiamonds.Clear();
        for (int i = 0; i < numberOfDiamonds; i++)
        {
            Vector3 randomPos = GetRandomSpawnPosition();
            GameObject newDiamond = Instantiate(diamondPrefab, randomPos, Quaternion.identity);
            currentDiamonds.Add(newDiamond);
            Debug.Log("���̾Ƹ�� ����: " + newDiamond.name + " at " + randomPos);
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