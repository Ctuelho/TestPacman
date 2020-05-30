using System;
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
        public readonly int BONUS_PELLET_SCORE = 500;
        public readonly int BONUS_PELLET_THRESHOLD = 100;
        public readonly int EXTRA_LIFE_BY_POINTS_THRESHOLD = 10000;
        public readonly int ZOMBIE_SCORE = 100;
        public readonly int GOBLIM_SCORE = 200;
        public readonly int OGRE_SCORE = 400;
        public readonly int NECROMANCER_SCORE = 800;
        #region speeds
        public readonly float PLAYER_DEFAULT_SPEED = 5;
        public readonly float ELF_DODGE_SPEED = 10;
        public readonly float ELF_POWERUP_SPEED_MUL = 2.0f;
        public readonly float KNIGHT_POWERUP_SPEED_MUL = 1.75f;
        public readonly float MAGE_POWERUP_SPEED_MUL = 1.5f;
        public readonly float ENEMY_SLOW_SPEED = 4;
        public readonly float ENEMY_MEDIUM_SPEED = 4.75f;
        public readonly float ENEMY_HIGH_SPEED = 5.5f;
        public readonly float FIRE_BALL_SPEED = 20;
        public readonly float ARROW_SPEED = 30;
        public readonly float NECRO_MAGIC_SPEED = 9;
        #endregion speeds
        #region intervals
        public readonly float POWER_UP_DURATION = 4;
        public readonly float SKILL_INTERVAL = 4;
        public readonly float ELF_DODGE_DURATION = 0.25f;
        public readonly float KNIGHT_SLASH_DURATION = 0.5f;
        public readonly float ARROW_CAST_INTERVAL = 0.2f;
        public readonly float RECOVER_INVERVAL = 0.5f;
        public readonly float DEATH_ANIMATION_INTERVAL = 1.5f;
        public readonly float NECRO_MAGIC_COOLDOWN = 3;
        public readonly int START_COUNTDOWN = 2;
        public readonly float BONUS_PELLET_DURATION = 6;
        public readonly float CLEAR_LEVEL_INTERVAL = 2;
        #endregion intervals
        #region layers
        public readonly float PLAYER_LAYER = 10;
        public readonly float WALLS_LAYER = 8;
        public readonly float SPELLS_LAYER = 11;
        public readonly float ENEMIES_LAYER = 9;
        public readonly float ENEMIES_SPELLS_LAYER = 12;
        #endregion layers
        #region life values
        public readonly int START_LIVES = 3;
        public readonly int MAX_LIVES = 5;
        public readonly int SMALL_LIFE = 1;
        public readonly int MEDIUM_LIFE = 2;
        public readonly int LARGE_LIFE = 3;
        #endregion life values
        #region enemies delays
        public readonly float MIN_RELEASE_DELAY = 2;
        public readonly float ENEMY_RELEASE_DELAY = 11;
        public readonly float MIN_RESPAWN_DELAY = 2;
        public readonly float ENEMY_RESPAWN_DELAY = 11;
        #endregion enemies delays
        #endregion consts

        #region static fields
        //singleton
        public static GameController Instance = null;
        private static bool ShowedInGameTooltipAlready = false;
        #endregion static fields

        #region properties
        public GameStates GameState { get; private set; } = GameStates.MainMenu;
        public LevelManager LevelManager => _levelManager;
        public Characters Character { get; private set; } = Characters.Knight;
        public int Score { get; private set; }
        public int BonusCollected { get; private set; }
        public int Lives { get; private set; }
        public int Level { get; private set; }
        public float PowerUpTimeLeft { get; private set; }
        public float SkillCoowlDown { get; private set; }
        public bool GameControlsEnabled { get; private set; } = false;
        #endregion properties

        #region private fields
        [SerializeField]
        private UIHUDManager _UIHUDManger;
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
        private GameObject _ogrePrefab;
        [SerializeField]
        private GameObject _necromancerPrefab;
        private Player _player;
        private Zombie _zombie;
        private Goblim _goblim;
        private Ogre _ogre;
        private Necromancer _necromancer;
        private Collectable _bonusPellet;
        private List<Navigator> _navigators;
        private Coroutine CountDownPowerUpTimeCoroutine;
        private Coroutine CountDownSkillCoolDownCoroutine;
        private Coroutine EnemyProgressionCoroutine;
        private int _timesZombieWasKilled;
        private int _timesGoblimWasKilled;
        private int _timesOgreWasKilled;
        private int _timesNecromancerWasKilled;
        private int _totalOfPellets;
        private int _pelletsLeftToBeCollected;
        private int _pelletsCollectedToSpawnBonus;
        private int _pointsCollected;
        private Vector2 _worldTranslationOffset = new Vector2();
        #endregion private fields

        #region unity event functions
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnEnable()
        {
            GameEvents.Instance.PelletCollected += OnPelletCollectedListener;
            GameEvents.Instance.PlayerWalkedTile += OnPlayerWalkedTileListener;
            GameEvents.Instance.PlayerUsedSkill += OnPlayerUsedSkillEventListener;
            GameEvents.Instance.PlayerDamaged += OnPlayerDamagedEventListener;
            GameEvents.Instance.EnemyDead += OnEnemyDeadListener;
            GameEvents.Instance.NavigatorWalkedTile += OnNavigatorWalkedTileListener;
        }

        void OnDisable()
        {
            GameEvents.Instance.PelletCollected -= OnPelletCollectedListener;
            GameEvents.Instance.PlayerWalkedTile -= OnPlayerWalkedTileListener;
            GameEvents.Instance.PlayerUsedSkill -= OnPlayerUsedSkillEventListener;
            GameEvents.Instance.PlayerDamaged -= OnPlayerDamagedEventListener;
            GameEvents.Instance.EnemyDead -= OnEnemyDeadListener;
            GameEvents.Instance.NavigatorWalkedTile -= OnNavigatorWalkedTileListener;
        }

        // Start is called before the first frame update
        void Start()
        {
            #region initialize game
            _levelManager.Initialize();

            _worldTranslationOffset.x = 0.5f - _levelManager.NavGraph.maxX / 2;
            _worldTranslationOffset.y = 0.5f - _levelManager.NavGraph.maxY / 2;

            //exclude the bonus one
            _totalOfPellets = _levelManager.Collectables.Count -1;

            //initialize bonus pellet
            _bonusPellet = _levelManager.GetCollectable(
                _levelManager.NavGraph.BonusPointNode.Indexes.Item1,
                _levelManager.NavGraph.BonusPointNode.Indexes.Item2);
            _bonusPellet.gameObject.SetActive(false);
            _bonusPellet.transform.position =
                new Vector3(
                    _levelManager.NavGraph.BonusPointNode.Indexes.Item1 + _worldTranslationOffset.x,
                    _levelManager.NavGraph.BonusPointNode.Indexes.Item2 + _worldTranslationOffset.y, 0);

            _navigators = new List<Navigator>();
            #endregion initialize game

            SetState(GameStates.MainMenu);
        }

        // Update is called once per frame
        void Update()
        {
            MoveEntities(Time.deltaTime);

            //check UI input
            switch (GameState)
            {
                case GameStates.MainMenu:
                    var charactersValues = Enum.GetValues(typeof(Characters));
                    var charactersTypes = new List<Characters>();
                    foreach (Characters charType in charactersValues)
                    {
                        charactersTypes.Add(charType);
                    }
                    var currentChracterIndex = charactersTypes.IndexOf(Character);
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //start countdown
                        RefreshLevel();
                        SetState(GameStates.CountingDownStart);
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        //leave game
                        Application.Quit();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        currentChracterIndex--;
                        if (currentChracterIndex < 0)
                        {
                            currentChracterIndex = charactersTypes.Count - 1;
                        }

                        Character = charactersTypes[currentChracterIndex];
                        _UIHUDManger.SetChracter(Character);
                        _UIHUDManger.SetLives(Lives, Character);
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        currentChracterIndex++;
                        if (currentChracterIndex > charactersTypes.Count - 1)
                        {
                            currentChracterIndex = 0;
                        }

                        Character = charactersTypes[currentChracterIndex];
                        _UIHUDManger.SetChracter(Character);
                        _UIHUDManger.SetLives(Lives, Character);
                    }
                    break;
                case GameStates.CountingDownStart:
                    //do nothing
                    break;
                case GameStates.Playing:
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //pause the game
                        SetState(GameStates.Paused);
                    }
                    break;
                case GameStates.Paused:
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //unpause the game
                        SetState(GameStates.Playing);
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        //back to menu
                        SetState(GameStates.MainMenu);
                    }
                    break;
                case GameStates.GameOver:
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //play again
                        RefreshHUD();
                        RefreshLevel();
                        SetState(GameStates.CountingDownStart);
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        //back to menu
                        SetState(GameStates.MainMenu);
                    }
                    break;
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

        #region private functions
        private void RefreshHUD()
        {
            _UIHUDManger.SetScore(0);

            if (PlayerPrefs.HasKey("highScore"))
            {
                _UIHUDManger.SetHighScore(PlayerPrefs.GetInt("highScore"));
            }
            else
            {
                _UIHUDManger.SetHighScore(0);
            }

            _pelletsCollectedToSpawnBonus = 0;
            _pelletsLeftToBeCollected = _totalOfPellets;
            BonusCollected = 0;
            _pointsCollected = 0;
            Score = 0;
            Level = 1;
            Lives = START_LIVES;

            _timesZombieWasKilled = 0;
            _timesGoblimWasKilled = 0;
            _timesOgreWasKilled = 0;
            _timesNecromancerWasKilled = 0;

            _UIHUDManger.SetBonusCollected(0);
            _UIHUDManger.SetLives(START_LIVES, Character);
            _UIHUDManger.SetSkillBarFill(1);
            _UIHUDManger.SetPowerBarFill(0);
        }

        private void RefreshLevel(bool refreshPellets = true)
        {
            //player
            if (_player != null)
            {
                Destroy(_player.gameObject);
            }
            switch (Character)
            {
                case Characters.Elf:
                    _player = Instantiate(_elfPrefab).GetComponent<Player>();
                    break;
                case Characters.Knight:
                    _player = Instantiate(_knightPrefab).GetComponent<Player>();
                    break;
                case Characters.Mage:
                    _player = Instantiate(_magePrefab).GetComponent<Player>();
                    break;
                default:
                    _player = Instantiate(_playerPrefab).GetComponent<Player>();
                    break;
            }
            _player.Initialize(
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item1,
                _levelManager.NavGraph.PlayerSpawnNode.Indexes.Item2);
            _player.NavEntity.SetPath(null);
               
            //fix the _player position on the world
            Vector3 playerPos = new Vector3(
                        _player.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                        _player.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
            _player.transform.position = playerPos;

            _player.NavEntity.DisableMoving();

            //enemies
            RespawnZombie();
            RespawnGoblim();
            RespawnOgre();
            RespawnNecromancer();

            //pellets
            if (refreshPellets)
            {
                _levelManager.Collectables.ForEach(c => { c.gameObject.SetActive(true); c.Reset(); });

                _bonusPellet.gameObject.SetActive(false);
            }
        }

        private void RespawnZombie()
        {
            if (_zombie != null)
            {
                _navigators.Remove(_zombie);
                Destroy(_zombie.gameObject);
            }

            _zombie = Instantiate(_zombiePrefab).GetComponent<Zombie>();
            _zombie.SetEnemyType(EnemyTypes.Zombie);
            _zombie.Initialize(
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition1Node.Indexes.Item2);
            _navigators.Add(_zombie);

            //fix the entity positions on the world
            _zombie.NavEntity.SetPath(null);
            Vector3 pos = new Vector3(
                 _zombie.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                 _zombie.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
            _zombie.transform.position = pos;

            _zombie.NavEntity.DisableMoving();       
        }

        private void RespawnGoblim()
        {
            if (_goblim != null)
            {
                _navigators.Remove(_goblim);
                Destroy(_goblim.gameObject);
            }

            _goblim = Instantiate(_goblimPrefab).GetComponent<Goblim>();
            _goblim.SetEnemyType(EnemyTypes.Goblim);
            _goblim.Initialize(
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition2Node.Indexes.Item2);
            _navigators.Add(_goblim);

            //fix the entity positions on the world
            _goblim.NavEntity.SetPath(null);
            Vector3 pos = new Vector3(
                 _goblim.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                 _goblim.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
            _goblim.transform.position = pos;

            _goblim.NavEntity.DisableMoving();
        }

        private void RespawnOgre()
        {
            if (_ogre != null)
            {
                _navigators.Remove(_ogre);
                Destroy(_ogre.gameObject);
            }

            _ogre = Instantiate(_ogrePrefab).GetComponent<Ogre>();
            _ogre.SetEnemyType(EnemyTypes.Ogre);
            _ogre.Initialize(
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition3Node.Indexes.Item2);
            _navigators.Add(_ogre);

            //fix the entity positions on the world
            _ogre.NavEntity.SetPath(null);
            Vector3 pos = new Vector3(
                 _ogre.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                 _ogre.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
            _ogre.transform.position = pos;

            _ogre.NavEntity.DisableMoving();
        }


        private void RespawnNecromancer()
        {
            if (_necromancer != null)
            {
                _navigators.Remove(_necromancer);
                Destroy(_necromancer.gameObject);
            }

            _necromancer = Instantiate(_necromancerPrefab).GetComponent<Necromancer>();
            _necromancer.SetEnemyType(EnemyTypes.Necromancer);
            _necromancer.Initialize(
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item1,
                _levelManager.NavGraph.EnemyPosition4Node.Indexes.Item2);
            _navigators.Add(_necromancer);

            //fix the entity positions on the world
            _necromancer.NavEntity.SetPath(null);
            Vector3 pos = new Vector3(
                 _necromancer.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                 _necromancer.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
            _necromancer.transform.position = pos;

            _necromancer.NavEntity.DisableMoving();
        }


        private void StartGameplay()
        {
            _player.NavEntity.EnableMoving();
            _player.EnableSkill();
            _player.SetCanBeDamaged(true);
        }

        private void MoveEntities(float deltaTime)
        {
            //update the navis positions
            if (GameState == GameStates.Playing)
            {
                _player.NavEntity.Move(deltaTime);
                Vector3 playerPos = new Vector3(
                        _player.NavEntity.Position.Item1 + _worldTranslationOffset.x,
                        _player.NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
                _player.transform.position = playerPos;

                for (int i = 0; i < _navigators.Count; i++)
                {
                    _navigators[i].NavEntity.Move(deltaTime);
                    Vector3 pos = new Vector3(
                        _navigators[i].NavEntity.Position.Item1 + _worldTranslationOffset.x,
                        _navigators[i].NavEntity.Position.Item2 + _worldTranslationOffset.y, 0);
                    _navigators[i].transform.position = pos;
                }
            }
        }

        private void SetState(GameStates state)
        {
            GameState = state;
            _UIHUDManger.HideAll();
            Time.timeScale = 1;
            switch (GameState)
            {
                case GameStates.MainMenu:
                    _UIHUDManger.ShowMainMenuScreen();
                    RefreshHUD();
                    GameControlsEnabled = false;
                    break;
                case GameStates.CountingDownStart:
                    if (!ShowedInGameTooltipAlready)
                    {
                        ShowedInGameTooltipAlready = true;
                        _UIHUDManger.ShowGameplayTipsCanvas();
                    }
                    _player.EnableSkill();
                    _UIHUDManger.ShowInGameUIHUD();
                    _UIHUDManger.SetSkillBarFill(1);
                    _UIHUDManger.SetPowerBarFill(0);
                    GameControlsEnabled = false;
                    StartCoroutine(CountDownStartGame(START_COUNTDOWN));
                    break;
                case GameStates.Playing:
                    _UIHUDManger.ShowInGameUIHUD();
                    GameControlsEnabled = true;
                    break;
                case GameStates.Paused:
                    _UIHUDManger.ShowInGameUIHUD();
                    _UIHUDManger.ShowPauseMenu();
                    Time.timeScale = 0;
                    GameControlsEnabled = false;
                    break;
                case GameStates.LevelCleared:
                    _UIHUDManger.ShowInGameUIHUD();
                    _UIHUDManger.ShowCountDown("Cleared!");
                    GameControlsEnabled = false;
                    _player.DisablePowerUp();
                    _player.NavEntity.DisableMoving();
                    foreach (var nav in _navigators)
                    {
                        nav.NavEntity.DisableMoving();
                    }
                    StopAllCoroutines();
                    StartCoroutine(WaitToStartNextLevel());
                    break;
                case GameStates.GameOver:
                    _UIHUDManger.ShowInGameUIHUD();
                    _UIHUDManger.ShowGameOverCanvas();
                    StopAllCoroutines();
                    GameControlsEnabled = false;
                    _player.NavEntity.DisableMoving();
                    foreach (var nav in _navigators)
                    {
                        nav.NavEntity.DisableMoving();
                    }
                    break;
            }

        }

        
        #endregion private functions

        #region event listeners
        private void OnPelletCollectedListener(object sender, GameEvents.PelletCollectedEventArgs e)
        {
            switch (e.PelletType)
            {
                case PelletTypes.Small:
                    _pelletsLeftToBeCollected--;
                    _pelletsCollectedToSpawnBonus++;
                    _pointsCollected += SMALL_PELLET_SCORE;
                    AddScore(SMALL_PELLET_SCORE);
                    break;
                case PelletTypes.Big:
                    _pelletsLeftToBeCollected--;
                    _pelletsCollectedToSpawnBonus++;
                    _pointsCollected += BIG_PELLET_SCORE;
                    AddScore(BIG_PELLET_SCORE);
                    _player.EnablePowerUp();
                    if (CountDownPowerUpTimeCoroutine != null)
                    {
                        StopCoroutine(CountDownPowerUpTimeCoroutine);
                    }
                    CountDownPowerUpTimeCoroutine = StartCoroutine(CountDownPowerUpTime());
                    GameEvents.Instance.OnPowerUpActive(
                        new GameEvents.PowerUpActiveEventArgs() { PowerUpStatus = true });
                    _UIHUDManger.SetPowerBarFill(1);
                    break;
                case PelletTypes.Bonus:
                    if (_bonusPellet.gameObject.activeSelf)
                    {
                        BonusCollected++;
                        _pointsCollected += BONUS_PELLET_SCORE;
                        AddScore(BONUS_PELLET_SCORE);
                    }
                    break;
            }

            if (_pelletsCollectedToSpawnBonus >= BONUS_PELLET_THRESHOLD)
            {
                //spawn a bonus pellet
                _pelletsCollectedToSpawnBonus -= BONUS_PELLET_THRESHOLD;
                _bonusPellet.Reset();
                _bonusPellet.gameObject.SetActive(true);
            }
            if (_pointsCollected >= EXTRA_LIFE_BY_POINTS_THRESHOLD)
            {
                //give extra life
                _pointsCollected -= EXTRA_LIFE_BY_POINTS_THRESHOLD;
                Lives = Mathf.Clamp(++Lives, 0, MAX_LIVES);
                _UIHUDManger.SetLives(Lives, Character);
            }

            if (_pelletsLeftToBeCollected == 0)
            {
                _pelletsLeftToBeCollected = _totalOfPellets;
                Level++;
                SetState(GameStates.LevelCleared);
            }
        }

        private void AddScore(int score)
        {
            Score += score;
            //check and udpdate highscore
            if (PlayerPrefs.HasKey("highScore"))
            {
                var hscr = PlayerPrefs.GetInt("highScore");
                if (Score > hscr)
                {
                    hscr = Score;
                    PlayerPrefs.SetInt("highScore", hscr);
                }
                _UIHUDManger.SetHighScore(hscr);
            }
            else
            {
                PlayerPrefs.SetInt("highScore", Score);
                _UIHUDManger.SetHighScore(Score);
            }

            _UIHUDManger.SetScore(Score);
            _UIHUDManger.SetBonusCollected(BonusCollected);
        }

        private void OnPlayerWalkedTileListener(object sender, GameEvents.PlayerWalkedTileEventArgs e)
        {
            var collectable = _levelManager.GetCollectable(e.indexX, e.indexY);
            if (collectable != null)
            {
                if (!collectable.Collected)
                {
                    collectable.Collect();
                }
            }
        }

        private void OnNavigatorWalkedTileListener(object sender, GameEvents.NavigatorWalkedTileEventArgs e)
        {
            var navigator = e.Navigator;
            var lastTile = _levelManager.NavGraph.GetNode(
                navigator.NavEntity.LastIndexes.Item1,
                navigator.NavEntity.LastIndexes.Item2);

            if (lastTile != null)
            {
                var slow = navigator != _player;
                if (lastTile == _levelManager.NavGraph.WarperLeft)
                {
                    //warp from left to right
                    //create a path from right warper to -4 left
                    var warpedLeftTargetTile = _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.WarperRight.Indexes.Item1 - 4,
                        _levelManager.NavGraph.WarperRight.Indexes.Item2);
                    var warpedLeftPath =
                        _levelManager.NavGraph.ShortestPath(
                            _levelManager.NavGraph.WarperRight,
                            warpedLeftTargetTile);
                    navigator.NavEntity.SetPath(warpedLeftPath);
                    if(slow) StartCoroutine(ModifyNavigatorSpeedTemporarily(1, 0.5f, navigator.NavEntity));
                    navigator.NavEntity.SetCurrentPosition(
                        _levelManager.NavGraph.WarperRight.Indexes.Item1,
                        _levelManager.NavGraph.WarperRight.Indexes.Item2);
                }
                else if (lastTile == _levelManager.NavGraph.WarperRight)
                {
                    //warp from right to left
                    //create a path from left warper to +4 right
                    var warpedRightTargetTile = _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.WarperLeft.Indexes.Item1 + 4,
                        _levelManager.NavGraph.WarperLeft.Indexes.Item2);
                    var warpedRightPath =
                        _levelManager.NavGraph.ShortestPath(
                            _levelManager.NavGraph.WarperLeft,
                            warpedRightTargetTile);
                    navigator.NavEntity.SetPath(warpedRightPath);
                    if (slow) StartCoroutine(ModifyNavigatorSpeedTemporarily(1, 0.5f, navigator.NavEntity));
                    navigator.NavEntity.SetCurrentPosition(
                        _levelManager.NavGraph.WarperLeft.Indexes.Item1,
                        _levelManager.NavGraph.WarperLeft.Indexes.Item2);
                }
                else if (lastTile == _levelManager.NavGraph.WarperUp)
                {
                    //warp from up to dow
                    //create a path from down warper to +2 down
                    var warpedDownTargetTile = _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.WarperDown.Indexes.Item1,
                        _levelManager.NavGraph.WarperDown.Indexes.Item2 + 2);
                    var warpedDownPath =
                        _levelManager.NavGraph.ShortestPath(
                            _levelManager.NavGraph.WarperDown,
                            warpedDownTargetTile);
                    navigator.NavEntity.SetPath(warpedDownPath);
                    if (slow) StartCoroutine(ModifyNavigatorSpeedTemporarily(1, 0.5f, navigator.NavEntity));
                    navigator.NavEntity.SetCurrentPosition(
                        _levelManager.NavGraph.WarperDown.Indexes.Item1,
                        _levelManager.NavGraph.WarperDown.Indexes.Item2);
                }
                else if (lastTile == _levelManager.NavGraph.WarperDown)
                {
                    //warp from down to up
                    //create a path from up warper to -2 up
                    var warpedUpTargetTile = _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.WarperUp.Indexes.Item1,
                        _levelManager.NavGraph.WarperUp.Indexes.Item2 - 2);
                    var warpedUpPath =
                        _levelManager.NavGraph.ShortestPath(
                            _levelManager.NavGraph.WarperUp,
                            warpedUpTargetTile);
                    navigator.NavEntity.SetPath(warpedUpPath);
                    if (slow) StartCoroutine(ModifyNavigatorSpeedTemporarily(1, 0.5f, navigator.NavEntity));
                    navigator.NavEntity.SetCurrentPosition(
                        _levelManager.NavGraph.WarperUp.Indexes.Item1,
                        _levelManager.NavGraph.WarperUp.Indexes.Item2);
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
            _UIHUDManger.SetSkillBarFill(0);
        }

        private void OnPlayerDamagedEventListener(object sender, GameEvents.PlayerDamagedEventArgs e)
        {
            if (_player.CanBeDamaged && !_player.IsDead)
            {
                Lives--;
                _UIHUDManger.SetLives(Lives, Character);
                _player.Die();
                StopAllCoroutines();
                if (Lives > 0)
                {
                    //damage player
                    //wait death animation and reset monsters positions
                    StartCoroutine(RepositionCharacters());
                }
                else
                {
                    //game over
                    StartCoroutine(WaitDeathAnimationComplete());
                }
            }
        }

        private void OnEnemyDeadListener(object sender, GameEvents.OnEnemyDeadEventArgs args)
        {
            switch (args.EnemyType)
            {
                case EnemyTypes.Zombie:
                    _timesZombieWasKilled++;
                    AddScore(ZOMBIE_SCORE);
                    StartCoroutine(WaitToRespawnZombie());
                    break;
                case EnemyTypes.Goblim:
                    _timesZombieWasKilled++;
                    AddScore(GOBLIM_SCORE);
                    StartCoroutine(WaitToRespawnGoblim());
                    break;
                case EnemyTypes.Ogre:
                    _timesZombieWasKilled++;
                    AddScore(OGRE_SCORE);
                    StartCoroutine(WaitToRespawnOgre());
                    break;
                case EnemyTypes.Necromancer:
                    _timesZombieWasKilled++;
                    AddScore(NECROMANCER_SCORE);
                    StartCoroutine(WaitToRespawnNecromancer());
                    break;
            }
        }
        #endregion event listeners

        #region coroutines
        IEnumerator RepositionCharacters()
        {
            var timer = 0f;
            while (timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RefreshLevel(false);
            SetState(GameStates.CountingDownStart);
        }

        IEnumerator WaitDeathAnimationComplete()
        {
            var timer = 0f;
            while (timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            SetState(GameStates.GameOver);
        }

        IEnumerator WaitToStartNextLevel()
        {
            float timer = 0;
            while (timer < CLEAR_LEVEL_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RefreshLevel();
            SetState(GameStates.CountingDownStart);
        }

        IEnumerator CountDownStartGame(int countTimer)
        {
            float timer = 0;
            int seconds = 0;
            while (countTimer - seconds > 0)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    seconds++;
                    timer = 0;
                }
                _UIHUDManger.ShowCountDown((countTimer - seconds).ToString());
                yield return null;
            }
            timer = 0;
            _UIHUDManger.ShowCountDown("GO!");
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            _UIHUDManger.HideCountDown();
            SetState(GameStates.Playing);
            //initialize level progression and IAs
            StartGameplay();
            if (EnemyProgressionCoroutine != null)
            {
                StopCoroutine(EnemyProgressionCoroutine);
            }
            EnemyProgressionCoroutine = StartCoroutine(EnemyProgression());
        }

        IEnumerator CountDownPowerUpTime()
        {
            PowerUpTimeLeft = POWER_UP_DURATION;
            while (PowerUpTimeLeft > 0)
            {
                _UIHUDManger.SetPowerBarFill(PowerUpTimeLeft / POWER_UP_DURATION);
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
                _UIHUDManger.SetSkillBarFill(SkillCoowlDown / SKILL_INTERVAL);
                SkillCoowlDown += Time.deltaTime;
                yield return null;
            }
            _player.EnableSkill();
        }

        IEnumerator ModifyNavigatorSpeedTemporarily(float duration, float mod, Pacman.NavEntity navi)
        {
            navi.SetSpeedMod(mod);
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                if (navi != null)
                    navi.SetSpeedMod(mod);
                yield return null;
            }
            if (navi != null)
                navi.SetSpeedMod(1);
        }

        //releases enemies from spawn zone as time passes
        //they will be released faster at higher levels
        IEnumerator EnemyProgression()
        {
            float enemyProgressionTimer = 0;

            while (enemyProgressionTimer < MIN_RELEASE_DELAY)
            {
                enemyProgressionTimer += Time.deltaTime;
                yield return null;
            }
            //release the zombie
            _zombie.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition1Node,
                    _levelManager.NavGraph.EnemyExitTargetNode));

            enemyProgressionTimer = 0;
            var goblinReleaseDelay = Math.Max(MIN_RELEASE_DELAY, ENEMY_RELEASE_DELAY - Level);
            while (enemyProgressionTimer < goblinReleaseDelay)
            {
                enemyProgressionTimer += Time.deltaTime;
                yield return null;
            }
            //release the goblim
            _goblim.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition2Node,
                    _levelManager.NavGraph.EnemyExitTargetNode));

            enemyProgressionTimer = 0;
            var ogreReleaseDelay = Math.Max(MIN_RELEASE_DELAY, ENEMY_RELEASE_DELAY - Level);
            while (enemyProgressionTimer < ogreReleaseDelay)
            {
                enemyProgressionTimer += Time.deltaTime;
                yield return null;
            }
            //release the ogre
            _ogre.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition3Node,
                    _levelManager.NavGraph.EnemyExitTargetNode));

            enemyProgressionTimer = 0;
            var necromancerReleaseDelay = Math.Max(MIN_RELEASE_DELAY, ENEMY_RELEASE_DELAY - Level);
            while (enemyProgressionTimer < necromancerReleaseDelay)
            {
                enemyProgressionTimer += Time.deltaTime;
                yield return null;
            }
            //release the necromancer
            _necromancer.WalkOutsideSpawnZone(
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition4Node,
                    _levelManager.NavGraph.EnemyExitTargetNode));
        }

        IEnumerator WaitToRespawnZombie()
        {
            var timer = 0f;
            while(timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RespawnZombie();

            timer = 0;
            var zombieRespawnTime =
                Math.Max(MIN_RESPAWN_DELAY, ENEMY_RESPAWN_DELAY - _timesZombieWasKilled);
            while (timer < zombieRespawnTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            var outsidePath = 
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition1Node,
                    _levelManager.NavGraph.EnemyExitTargetNode);
            _zombie.WalkOutsideSpawnZone(outsidePath);
        }

        IEnumerator WaitToRespawnGoblim()
        {
            var timer = 0f;
            while (timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RespawnGoblim();

            timer = 0;
            var goblimRespawnTime =
                Math.Max(MIN_RESPAWN_DELAY, ENEMY_RESPAWN_DELAY - _timesGoblimWasKilled);
            while (timer < goblimRespawnTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            var outsidePath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition2Node,
                    _levelManager.NavGraph.EnemyExitTargetNode);
            _goblim.WalkOutsideSpawnZone(outsidePath);
        }

        IEnumerator WaitToRespawnOgre()
        {
            var timer = 0f;
            while (timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RespawnOgre();

            timer = 0;
            var orgreRespawnTime =
                Math.Max(MIN_RESPAWN_DELAY, ENEMY_RESPAWN_DELAY - _timesOgreWasKilled);
            while (timer < orgreRespawnTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            var outsidePath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition3Node,
                    _levelManager.NavGraph.EnemyExitTargetNode);
            _ogre.WalkOutsideSpawnZone(outsidePath);
        }

        IEnumerator WaitToRespawnNecromancer()
        {
            var timer = 0f;
            while (timer < DEATH_ANIMATION_INTERVAL)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            RespawnNecromancer();

            timer = 0;
            var necromancerRespawnTime =
                Math.Max(MIN_RESPAWN_DELAY, ENEMY_RESPAWN_DELAY - _timesNecromancerWasKilled);
            while (timer < necromancerRespawnTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            var outsidePath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.EnemyPosition4Node,
                    _levelManager.NavGraph.EnemyExitTargetNode);
            _necromancer.WalkOutsideSpawnZone(outsidePath);
        }
        #endregion coroutines
    }
}

