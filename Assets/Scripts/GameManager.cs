using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region consts
        const int SMALL_PELLET_SCORE = 10;
        const int BIG_PELLET_SCORE = 50;
        #endregion consts

        #region static fields
        //singleton
        public static GameManager Instance = null;

        public static int Score;
        public static int Lives;
        public static int Level;
        #endregion static fields

        #region properties
        public LevelManager LevelManager => _levelManager;
        #endregion properties

        #region private fields
        [SerializeField]
        private LevelManager _levelManager;

        [SerializeField]
        private Player _player;

        private List<Navigator> _navigators;
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
        }

        private void OnDisable()
        {
            GameEvents.Instance.PelletCollected -= OnPelletCollectedListener;
        }

        // Start is called before the first frame update
        void Start()
        {
            _levelManager.Initialize();

            _navigators = new List<Navigator>();

            var playerNodes = 
                _levelManager.NavGraph.GetNodesOfType(Pacman.NodeType.PlayerSpawn);
            if(playerNodes.Count > 0)
            {
                _player.Initialize(
                    playerNodes[0].Indexes.Item1, playerNodes[0].Indexes.Item2);

                var playerInitialPath =
                    _levelManager.NavGraph.ShortestPath(
                        playerNodes[0], _levelManager.NavGraph.GetNode(
                            playerNodes[0].Indexes.Item1-1, playerNodes[0].Indexes.Item2));

                _player.NavEntity.SetPath(playerInitialPath);

                _navigators.Add(_player);
            }
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
                    break;
            }
        }
        #endregion event listeners
    }
}

