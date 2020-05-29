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
        #region speeds
        public readonly float PLAYER_DEFAULT_SPEED = 5;
        public readonly float ELF_DODGE_SPEED = 10;
        public readonly float ELF_POWERUP_SPEED_MUL = 2.5f;
        public readonly float KNIGHT_POWERUP_SPEED_MUL = 2.0f;
        public readonly float MAGE_POWERUP_SPEED_MUL = 1.5f;
        public readonly float ENEMY_SLOW_SPEED = 4;
        public readonly float ENEMY_MEDIUM_SPEED = 5;
        public readonly float ENEMY_HIGH_SPEED = 6;
        public readonly float FIRE_BALL_SPEED = 20;
        public readonly float ARROW_SPEED = 40;
        public readonly float NECRO_MAGIC_SPEED = 8;
        #endregion speeds
        #region intervals
        public readonly float POWER_UP_DURATION = 4;
        public readonly float SKILL_INTERVAL = 5f;
        public readonly float ELF_DODGE_DURATION = 0.25f;
        public readonly float KNIGHT_SLASH_DURATION = 0.5f;
        public readonly float ARROW_CAST_INTERVAL = 0.25f;
        public readonly float RECOVER_INVERVAL = 0.3f;
        public readonly float DEATH_ANIMATION_INTERVAL = 1.0f;
        public readonly float NECRO_MAGIC_COOLDOWN = 3;
        #endregion intervals
        #region layers
        public readonly float PLAYER_LAYER = 10;
        public readonly float WALLS_LAYER = 8;
        public readonly float SPELLS_LAYER = 11;
        public readonly float ENEMIES_LAYER = 9;
        #endregion layers
        #region life values
        public readonly int SMALL_LIFE = 1;
        public readonly int MEDIUM_LIFE = 2;
        public readonly int LARGE_LIFE = 3;
        #endregion life values
        #endregion consts

        #region static fields
        //singleton
        public static GameController Instance = null;
        #endregion static fields

        #region properties
        public LevelManager LevelManager => _levelManager;
        public Character Character { get; private set; } = Character.Knight;
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Level { get; private set; }
        public float PowerUpTimeLeft { get; private set; }
        public float SkillCoowlDown { get; private set; }
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
        private GameObject _zombiePrefab;
        [SerializeField]
        private GameObject _goblimPrefab;
        [SerializeField]
        private GameObject _necromancerPrefab;
        [SerializeField]
        private GameObject _ogrePrefab;
        [SerializeField]
        private GameObject _extraPointPrefab;
        private Player _player;
        private Zombie _zombie;
        private Goblim _goblim;
        private Necromancer _necromancer;
        private Ogre _ogre;
        private List<Navigator> _navigators;
        private Coroutine CountDownPowerUpTimeCoroutine;
        protected Coroutine CountDownSkillCoolDownCoroutine;
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
            GameEvents.Instance.PlayerUsedSkill += OnPlayerUsedSkillEventListener;
            GameEvents.Instance.PlayerDamaged += OnPlayerDamagedEventListener;
        }

        private void OnDisable()
        {
            GameEvents.Instance.PelletCollected -= OnPelletCollectedListener;
            GameEvents.Instance.PlayerWalkedTile -= OnPlayerWalkedTileListener;
            GameEvents.Instance.PlayerUsedSkill -= OnPlayerUsedSkillEventListener;
            GameEvents.Instance.PlayerDamaged -= OnPlayerDamagedEventListener;
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
            _zombie = Instantiate(_zombiePrefab).GetComponent<Zombie>();
            _goblim = Instantiate(_goblimPrefab).GetComponent<Goblim>();
            _necromancer = Instantiate(_necromancerPrefab).GetComponent<Necromancer>();
            _ogre = Instantiate(_ogrePrefab).GetComponent<Ogre>();

            _player.Initialize(
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item1,
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item2);
            _player.EnableSkill();

            _zombie.Initialize(
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item2);
            _zombie.WalkOutsideSpawnZone(null);

            _goblim.Initialize(
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item2);
            _goblim.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition2Node,
                    _levelManager.NavGraph.EnemyPosition1Node
                ));

            _necromancer.Initialize(
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item2);
            _necromancer.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition3Node,
                    _levelManager.NavGraph.EnemyPosition1Node
                ));

            _ogre.Initialize(
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item2);
            _ogre.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition4Node,
                    _levelManager.NavGraph.EnemyPosition1Node
                ));

            var playerInitialPath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.PlayerSpawnNode, _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item1-1,
                        _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item2));

            _player.NavEntity.SetPath(playerInitialPath);

            _navigators.Add(_player);
            _navigators.Add(_zombie);
            _navigators.Add(_goblim);
            _navigators.Add(_necromancer);
            _navigators.Add(_ogre);
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
                    GameEvents.Instance.OnPowerUpActive(
                        new GameEvents.PowerUpActiveEventArgs() { PowerUpStatus = true });
                    break;
            }
        }

        private void OnPlayerWalkedTileListener(object sender, GameEvents.PlayerWalkedTileEventArgs e)
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

        private void OnPlayerUsedSkillEventListener(object sender, GameEvents.OnPlayerUsedSkillEventArgs e)
        {
            _player.DisableSkill();
            if (CountDownSkillCoolDownCoroutine != null)
            {
                StopCoroutine(CountDownSkillCoolDownCoroutine);
            }
            CountDownSkillCoolDownCoroutine = StartCoroutine(CoolDownSkill());
        }

        private void OnPlayerDamagedEventListener(object sender, GameEvents.PlayerDamagedEventArgs e)
        {
            if (_player.CanBeDamaged)
            {
                //kill player
                _player.Die();
            }
        }
        #endregion event listeners

        #region coroutines
        IEnumerator CountDownPowerUpTime()
        {
            PowerUpTimeLeft = POWER_UP_DURATION;
            while (PowerUpTimeLeft > 0)
            {
                PowerUpTimeLeft -= Time.deltaTime;
                yield return null;
            }
            _player.DisablePowerUp();
            GameEvents.Instance.OnPowerUpActive(
                        new GameEvents.PowerUpActiveEventArgs() { PowerUpStatus = false });
        }

        IEnumerator CoolDownSkill()
        {
            SkillCoowlDown = 0f;
            while (SkillCoowlDown < SKILL_INTERVAL)
            {
                SkillCoowlDown += Time.deltaTime;
                yield return null;
            }
            _player.EnableSkill();     
        }
        #endregion coroutines
    }
}

