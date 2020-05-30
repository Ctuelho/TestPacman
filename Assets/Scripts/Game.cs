using UnityEngine;

namespace Game 
{
    public enum Characters {  Elf, Knight, Mage }
    public enum MovementDirections { None, Up, Down, Left, Right }
    public enum PelletTypes { None, Small, Big, Bonus }
    public enum AnimationStates { None = -1, Idle = 0, Action1 = 1, Action2 = 2, Dead = 3, Victory = 4, Ghost = 5, Hurt = 6 }
    public enum EnemyActionState { None, Waiting, Following, Fleeing, Dead };
    public enum EnemyTypes { None, Zombie, Goblim, Ogre, Necromancer };

    public enum GameStates { None, MainMenu, CountingDownStart, Playing, Paused, GameOver, LevelCleared }

    public class Navigator : MonoBehaviour
    {
        public bool Initialized { get; private set; }

        public Pacman.NavEntity NavEntity { get; protected set; }

        public virtual void Initialize(int x, int y)
        {
            Initialized = true;
            NavEntity = new Pacman.NavEntity();
            NavEntity.SetCurrentPosition(x, y);
            NavEntity.SetSpeed(1);
            NavEntity.EnableMoving();
        }
        
        protected virtual void Update()
        {
            if (NavEntity.ReachedDestination)
            {
                GameEvents.Instance.OnNavigatorWalkedTile(new GameEvents.NavigatorWalkedTileEventArgs(){ Navigator = this });
            }
        }
    }

    public class Collectable : MonoBehaviour
    {
        public bool Collected { get; protected set; }

        public Vector2 Index;

        public virtual void Collect()
        {
            Collected = true;
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            Collected = false;
            gameObject.SetActive(true);
        }
    }
}

