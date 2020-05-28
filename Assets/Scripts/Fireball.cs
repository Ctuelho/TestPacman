using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        public void Initialize(Vector3 position, MovementDirection direction)
        {
            transform.position = position;

            Vector2 velocity = new Vector2();
            switch (direction)
            {
                case MovementDirection.Up:
                    velocity = Vector2.up * GameController.Instance.FIRE_BALL_SPEED;
                    break;
                case MovementDirection.Down:
                    velocity = Vector2.down * GameController.Instance.FIRE_BALL_SPEED;
                    break;
                case MovementDirection.Left:
                    velocity = Vector2.left * GameController.Instance.FIRE_BALL_SPEED;
                    break;
                case MovementDirection.Right:
                    velocity = Vector2.right * GameController.Instance.FIRE_BALL_SPEED;
                    break;
            }

            _rigidbody.velocity = velocity;

            Invoke("Dissolve", GameController.Instance.POWER_UP_DURATION);
        }

        private void Dissolve()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

        }
    }
}