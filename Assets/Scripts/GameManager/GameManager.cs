using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Exposed
    //[SerializeField] private GameObject[] _enemyPrefabs;
    public EnemyInfo[] _enemyinfos;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _goalPoint;
    [SerializeField] private float _spawnDelay;

    [Header("Layers for raycast")]
    [SerializeField] private LayerMask _towerLayer;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Enemy saved for victory condition")]
    public float _firstStar;
    public float _secondStar;
    public float _thirdStar;
    #endregion

    #region Events
    public delegate void OnEnemyCountChange(float enemyCount);
    public OnEnemyCountChange _onEnemyCountChange;
    public delegate void OnVictory();
    public OnVictory _onVictory;
    public delegate void OnResetElements();
    public OnResetElements _onResetElements;
    #endregion

    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        //_totalEnemy = _enemyPrefabs.Length;
        _totalEnemy = _enemyinfos.Length;        
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        _pathRenderer = GetComponentInChildren<LineRenderer>();
        _rangeRenderer = GetComponentInChildren<SpriteRenderer>();
        TransitionToState(_planningState);
        //_spawnPoint.GetComponent<NavMeshAgent>().SetDestination(_goalPoint.position);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            CleanPathAndTowerRange();
        }
        _currentState.OnStateUpdate(this);
    }

    void FixedUpdate()
    {
        _currentState.OnStateFixedUpdate(this);
    }

    void OnTriggerEnter(Collider other)
    {
        _currentState.OnStateTriggerEnter(this, other);
    }

    void OnGUI() {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 28;
        /*if (GUI.Button(new Rect(10,10,360,60), _currentState.ToString(), style)) {
            TransitionToState(_planningState);
        }*/
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(_goalPoint.position, new Vector3(0.3f,0.3f,0.3f));
    }

    #endregion

    #region Main methods
    public void TransitionToState(GameState state) {
        if (_currentState != null) _currentState.OnStateExit(this);
        _currentState = state;
        _currentState.OnStateEnter(this);
    }

    public void SetEnemySpawnInfos(string[] enemySpawnNames) {
        _enemySpawnInfos = new EnemyInfo[_enemyinfos.Length];
        for (int i = 0; i < enemySpawnNames.Length; i++) {
            for(int j = 0; j < _enemyinfos.Length; j++) {
                if (_enemyinfos[j]._name == enemySpawnNames[i]) {
                    _enemySpawnInfos[i] = _enemyinfos[j];
                    break;
                }
            }
        }
        _gameStarted = true;
    }

    public void ResetGameElements() {
        _gameStarted = false;
        _spawnFinished = false;
        _enemyId = 0;
        _enemySavedCount = 0;
        _currentScore = 0;
        if(_onEnemyCountChange != null) _onEnemyCountChange.Invoke(_enemySavedCount);
        if(_onResetElements != null) _onResetElements.Invoke();
        _selectedAgent = null;
        CleanPathAndTowerRange();
        for (int i = 0; i < _spawnPoint.childCount; i++) {
            Destroy(_spawnPoint.GetChild(i).gameObject);
        }
    }

    public void SpawnEnemies() {
        if (_enemyId >= _enemySpawnInfos.Length) {
            _spawnFinished = true;
        } else _spawnFinished = false;

        if (Time.time - _lastSpawnTime >= _spawnDelay) {
            EnemyInfo enemyInfo = _enemySpawnInfos[_enemyId];
            GameObject enemy = Instantiate(enemyInfo._enemyPrefab, _spawnPoint);

            EnemyController enmyController = enemy.GetComponent<EnemyController>();
            enmyController.SetParameters(enemyInfo);
            enmyController.SetDestination(_goalPoint.position);

            //enemy.GetComponent<NavMeshAgent>().SetDestination(_goalPoint.position);
            _enemyId++;
            _lastSpawnTime = Time.time;
        }
    }

    public void CheckGoalReached() {
        Collider[] colliders = Physics.OverlapBox(_goalPoint.position, new Vector3(0.15f,0.15f,0.15f));
        
        foreach(Collider collision in colliders) {
            if (collision.CompareTag("Enemy")) {
                _enemySavedCount++;
                UpdateScore();
                _onEnemyCountChange.Invoke(_enemySavedCount);
                Destroy(collision.gameObject);
            }
        }
    }

    public bool CheckEndGameCondition() {
        return _spawnPoint.childCount == 0;
    }

    public bool CheckVictoryCondition() {
        return CheckEndGameCondition() && _enemySavedCount >= _firstStar;
    }

    public void ActivateEnemyAndDrawPath() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, _enemyLayer)) {
                GameObject enemy = hit.collider.gameObject;
                _selectedAgent = enemy.GetComponent<NavMeshAgent>();
                EnemyController.EnemyAction action = enemy.GetComponent<EnemyController>()._enemyAction;
                if (action != null) action();
            }
        }
        if (_selectedAgent != null) {
            DrawPath(_selectedAgent.path.corners);
        } else {
            _pathRenderer.positionCount = 0;
        }
    }

    public void DrawPlanningPath() {
        _pathRenderer.positionCount = 0;
        DrawPath(_spawnPoint.GetComponent<NavMeshAgent>().path.corners);
    }

    public void DrawPath(Vector3[] points) {
        _pathRenderer.positionCount = points.Length;

        int i = 0;
        foreach(Vector3 waypoint in points) {
            _pathRenderer.SetPosition(i, waypoint + new Vector3 (0, 0.6f, 0));
            i++;
        }
        /*float width =  _pathRenderer.startWidth;
        _pathRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);*/
    }

    public void DisplayTowerRange() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, _towerLayer)) {
                _rangeRenderer.enabled = true;

                Transform towerTransform = hit.transform;
                _rangeRenderer.transform.position = towerTransform.position + new Vector3(0, 0.1f, 0);
                float range = towerTransform.GetComponent<TowerController>()._range;
                _rangeRenderer.transform.localScale = new Vector2(2*range, 2*range);
            }
        }
    }

    public void CleanPathAndTowerRange() {
        _selectedAgent = null;
        _rangeRenderer.enabled = false;
    }

    private void UpdateScore() {
        if (_enemySavedCount >= _thirdStar)
            _currentScore = 3;
        else if (_enemySavedCount >= _secondStar)
            _currentScore = 2;
        else if (_enemySavedCount >= _firstStar)
            _currentScore = 1;
    }

    public void SaveScore() {
        if (_currentScore > ScoresList.Instance.starsUnlocked[_currentSceneIndex]) {
            ScoresList.Instance.starsUnlocked[_currentSceneIndex] = _currentScore;
            SaveSystem.SaveScoresList();
        }
    }

    public void UnlockNextLevel() {
        if (_currentSceneIndex+1 < SceneManager.sceneCountInBuildSettings)
            ScoresList.Instance.starsUnlocked[_currentSceneIndex+1] = 0;
    }

    public void RestartLevel() {
        //instance = null;
        //ResetGameElements();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TransitionToState(_planningState);
    }

    public void launchNextLevel() {
        //instance = null;
        if (_currentSceneIndex+1 < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(_currentSceneIndex+1);
    }

    public void ReturnToMenu() {
        //instance = null;
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Private & Protected
    private LineRenderer _pathRenderer;
    private SpriteRenderer _rangeRenderer;
    //private GameObject[] _enemyPrefabs;
    private EnemyInfo[] _enemySpawnInfos;
    [HideInInspector] public NavMeshAgent _selectedAgent;
    [HideInInspector] public bool _gameStarted, _spawnFinished;
    [HideInInspector] public float _totalEnemy;
    [HideInInspector] public int _currentScore;
    private float _lastSpawnTime, _enemySavedCount;
    private int _enemyId, _currentSceneIndex;
    GameState _currentState;
    [HideInInspector] public GameState _planningState = new PlanningGameState();
    [HideInInspector] public GameState _spawningState = new SpawningGameState();
    [HideInInspector] public GameState _runningState = new RunningGameState();
    [HideInInspector] public GameState _victoryState = new VictoryGameState();
    #endregion
}
