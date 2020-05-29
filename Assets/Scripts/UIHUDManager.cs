using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIHUDManager : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        private GameObject _highScoreCanvas;
        [SerializeField]
        private GameObject _scoreCanvas;
        [SerializeField]
        private GameObject _livesCanvas;
        [SerializeField]
        private GameObject _skillCanvas;
        [SerializeField]
        private GameObject _powerCanvas;
        [SerializeField]
        private Text _highScoreValue;
        [SerializeField]
        private Text _scoreValue;
        [SerializeField]
        private Image[] _livesImages;
        [SerializeField]
        private Image _energyBarFill;
        [SerializeField]
        private Image _powerBarFill;
        #endregion private fields

        #region public functions

        #endregion public functions
    }
}
