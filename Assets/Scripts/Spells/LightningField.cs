using UnityEngine;

namespace Game
{
    public class LightningField : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Invoke("Dissolve", GameController.Instance.POWER_UP_DURATION);
        }

        private void Dissolve()
        {
            Destroy(gameObject);
        }      
    }
}