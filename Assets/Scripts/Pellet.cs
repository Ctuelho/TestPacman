using UnityEngine;

namespace Game
{
    public class Pellet : Collectable
    {
        #region public fields
        public PelletType PelletType;
        #endregion public fields

        #region public functions
        public override void Collect()
        {
            GameEvents.Instance.OnPelletCollected(
                new GameEvents.PelletCollectedEventArgs() {
                    PelletType = PelletType });

            base.Collect();
        }
        #endregion public functions
    }
}

