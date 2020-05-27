using UnityEngine;

namespace Game
{
    public class Pellet : MonoBehaviour
    {
        #region public fields
        public PelletType PelletType;
        #endregion public fields

        #region public functions
        public void Collect()
        {
            GameEvents.Instance.OnPelletCollected(
                new GameEvents.PelletCollectedEventArgs() {
                    PelletType = PelletType });

            gameObject.SetActive(false);
        }
        #endregion public functions
    }
}

