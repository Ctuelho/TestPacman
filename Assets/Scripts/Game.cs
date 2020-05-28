using UnityEngine;

namespace Game 
{
    public enum Character {  Elf, Knight, Mage }
    public enum MovementDirection { None, Up, Down, Left, Right }
    public enum PelletType { None, Small, Big }
    public enum PlayerAnimationState { None = -1, Idle = 0, Action1 = 1, Action2 = 2, Dead = 3, Victory = 4 }

    public class Navigator : MonoBehaviour
    {
        public bool Initialized { get; private set; }

        public Pacman.NavEntity NavEntity { get; protected set; }

        public virtual void Initialize(int x, int y)
        {
            Initialized = true;
            NavEntity = new Pacman.NavEntity();
            NavEntity.SetCurrentPosition(x, y);
            NavEntity.SetSpeed(5);
            NavEntity.EnableMoving();
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

