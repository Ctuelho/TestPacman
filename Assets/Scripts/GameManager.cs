using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public enum MovementDirection { None, Up, Down, Left, Right }

    public class Movable : MonoBehaviour
    {
        public Pacman.MovableEntity MovableEntity { get; protected set; }
    }

    public class GameManager : MonoBehaviour
    {
        //singleton
        public static GameManager Instance = null;

        #region properties
        public LevelManager LevelManager => _levelManager;
        #endregion properties

        #region private fields
        [SerializeField]
        private LevelManager _levelManager;

        [SerializeField]
        private Transform _testSubject;

        [SerializeField]
        private Player _player;

        private List<Movable> _movingEntities;
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

        // Start is called before the first frame update
        void Start()
        {
            _levelManager.Initialize();

            _movingEntities = new List<Movable>();
            Pacman.MovableEntity test = new Pacman.MovableEntity();
            test.SetSpeed(10);
            var startNode = _levelManager.NavGraph.GetNode(0, 0);
            test.SetCurrentPosition(startNode.Indexes.Item1, startNode.Indexes.Item2);
            var path =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.GetNode(0, 0),
                    _levelManager.NavGraph.GetNode(
                        _levelManager.NavGraph.maxX, _levelManager.NavGraph.maxY));
            test.SetPath(path);
            _levelManager.DrawPath(path.ToList());
            //_movingEntities.Add(test);

            _player.Initialize(13, 12);
            var playerInitialPath =
                _levelManager.NavGraph.ShortestPath(
                    _levelManager.NavGraph.GetNode(13, 12),
                    _levelManager.NavGraph.GetNode(12, 12));
            _player.MovableEntity.SetPath(playerInitialPath);
            _movingEntities.Add(_player);
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < _movingEntities.Count; i++)
            {
                _movingEntities[i].MovableEntity.Move(Time.deltaTime);
                Vector3 pos = new Vector3(
                    0.5f + _movingEntities[i].MovableEntity.Position.Item1 - _levelManager.NavGraph.maxX / 2,
                    0.5f + _movingEntities[i].MovableEntity.Position.Item2 - _levelManager.NavGraph.maxY / 2,
                    0);
                _movingEntities[i].transform.position = pos;
            }
            
        }
        #endregion unity event functions

        #region set functions
        public void SetLevelManagerReference(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        #endregion set functions
    }
}

