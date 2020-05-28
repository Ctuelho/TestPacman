using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        #region consts
        public readonly int SMALL_PELLET_SCORE = 10;
        public readonly int BIG_PELLET_SCORE = 50;
        public readonly float POWER_UP_DURATION = 4;
        #region speeds
        public readonly float PLAYER_DEFAULT_SPEED = 5;
        public readonly float ELF_DODGE_SPEED = 10;
        public readonly float ELF_POWERUP_SPEED_MUL = 2.5f;
        public readonly float KNIGHT_POWERUP_SPEED_MUL = 2.0f;
        public readonly float MAGE_POWERUP_SPEED_MUL = 1.5f;
        public readonly float GHOST_DEFAULT_SPEED = 3;
        public readonly float FIRE_BALL_SPEED = 40;
        public readonly float ARROW_SPEED = 50;
        #endregion speeds
        #endregion consts

        #region static fields
        //singleton
        public static GameController Instance = null;

        public static int Score { get; private set; }
        public static int Lives { get; private set; }
        public static int Level { get; private set; }
        #endregion static fields

        #region properties
        public LevelManager LevelManager => _levelManager;
        public Character Character { get; private set; } = Character.Mage;
        #endregion properties

        #region private fields
        [SerializeField]
        private LevelManager _levelManager;
        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private GameObject _elfPrefab;
        [SerializeField]
        private GameObject _knightPrefab;
        [SerializeField]
        private GameObject _magePrefab;
        [SerializeField]
        private GameObject _enemy1Prefab;
        [SerializeField]
        private GameObject _enemy2Prefab;
        [SerializeField]
        private GameObject _enemy3Prefab;
        [SerializeField]
        private GameObject _enemy4Prefab;
        private Player _player;
        private GhostA _enemyA;
        private GhostA _enemyB;
        private GhostA _enemyC;
        private GhostA _enemyD;
        private List<Navigator> _navigators;
        private Coroutine CountDownPowerUpTimeCoroutine;
        #endregion private fields

        #region unity event functions
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvents.Instance.PelletCollected += OnPelletCollectedListener;
            GameEvents.Instance.PlayerWalkedTile += OnPlayerWalkedTileListener;
        }

        private void OnDisable()
        {
            GameEvents.Instance.PelletCollected -= OnPelletCollectedListener;
            GameEvents.Instance.PlayerWalkedTile -= OnPlayerWalkedTileListener;
        }

        // Start is called before the first frame update
        void Start()
        {
            _levelManager.Initialize();

            _navigators = new List<Navigator>();

            switch (Character)
            {
                case Character.Elf:
                    _player = Instantiate(_elfPrefab).GetComponent<Player>();
                    break;
                case Character.Knight:
                    _player = Instantiate(_knightPrefab).GetComponent<Player>();
                    break;
                case Character.Mage:
                    _player = Instantiate(_magePrefab).GetComponent<Player>();
                    break;
                default:
                    _player = Instantiate(_playerPrefab).GetComponent<Player>();
                    break;
            }
            _enemyA = Instantiate(_enemy1Prefab).GetComponent<GhostA>();
            _enemyB = Instantiate(_enemy1Prefab).GetComponent<GhostA>();
            _enemyC = Instantiate(_enemy1Prefab).GetComponent<GhostA>();
            _enemyD = Instantiate(_enemy1Prefab).GetComponent<GhostA>();

            _player.Initialize(
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item1,
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item2);

            _enemyA.Initialize(
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item2);

            _enemyB.Initialize(
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item2);

            _enemyC.Initialize(
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item2);

            _enemyD.Initialize(
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item2);

            var playerInitialPath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.PlayerSpawnNode, _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item1-1,
                        _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item2));

            _player.NavEntity.SetPath(playerInitialPath);

            _navigators.Add(_player);
            _navigators.Add(_enemyA);
            _navigators.Add(_enemyB);
            _navigators.Add(_enemyC);
            _navigators.Add(_enemyD);
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < _navigators.Count; i++)
            {
                _navigators[i].NavEntity.Move(Time.deltaTime);
                Vector3 pos = new Vector3(
                    0.5f + _navigators[i].NavEntity.Position.Item1 - _levelManager.NavGraph.maxX / 2,
                    0.5f + _navigators[i].NavEntity.Position.Item2 - _levelManager.NavGraph.maxY / 2,
                    0);
                _navigators[i].transform.position = pos;
            }
            
        }
        #endregion unity event functions

        #region set functions
        public void SetLevelManagerReference(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        #endregion set functions

        #region public functions
        public Pacman.NavNode GetPlayerCurrentNode()
        {
            var playerNode = _levelManager.NavGraph.GetNode(
                    (int)_player.NavEntity.Position.Item1,
                    (int)_player.NavEntity.Position.Item2);

            return playerNode;
        }
        #endregion public functions

        #region event listeners
        private void OnPelletCollectedListener(object sender, GameEvents.PelletCollectedEventArgs e)
        {
            switch (e.PelletType) 
            {
                case PelletType.Small:
                    Score += SMALL_PELLET_SCORE;
                    break;
                case PelletType.Big:
                    Score += BIG_PELLET_SCORE;
                    _player.EnablePowerUp();
                    if(CountDownPowerUpTimeCoroutine != null)
                    {
                        StopCoroutine(CountDownPowerUpTimeCoroutine);
                    }
                    CountDownPowerUpTimeCoroutine = StartCoroutine(CountDownPowerUpTime());
                    break;
            }
        }

        private void OnPlayerWalkedTileListener(object sender, GameEvents.OnPlayerWalkedTileEventArgs e)
        {
            var collectable = _levelManager.GetCollectable(e.indexX, e.indexY);
            if(collectable != null)
            {
                if (!collectable.Collected)
                {
                    collectable.Collect();
                }
            }
        }
        #endregion event listeners

        #region coroutines
        IEnumerator CountDownPowerUpTime()
        {
            var timer = 0f;
            while(timer < POWER_UP_DURATION)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            _player.DisablePowerUp();
        }
        #endregion coroutines
    }
}

