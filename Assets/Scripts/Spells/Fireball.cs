using UnityEngine;

namespace Game
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        public void Initialize(Vector3 position, MovementDirections direction, float speed)
        {
            transform.position = position;

            Vector2 velocity = new Vector2();
            switch (direction)
            {
                case MovementDirections.Up:
                    velocity = Vector2.up * speed;
                    break;
                case MovementDirections.Down:
                    velocity = Vector2.down * speed;
                    break;
                case MovementDirections.Left:
                    velocity = Vector2.left * speed;
                    break;
                case MovementDirections.Right:
                    velocity = Vector2.right * speed;
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
            Dissolve();
        }
    }
}