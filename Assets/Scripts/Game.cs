using UnityEngine;

namespace Game 
{
    public enum MovementDirection { None, Up, Down, Left, Right }
    public enum PelletType { None, Small, Big }

    public class Navigator : MonoBehaviour
    {
        public Pacman.NavEntity NavEntity { get; protected set; }
    }
}

