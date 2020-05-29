using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIHUDManager : MonoBehaviour
    {
        #region private fields
        [SerializeField]
        private GameObject _titleCanvas;
        [SerializeField]
        private GameObject _mainMenuCanvas;
        [SerializeField]
        private GameObject _gameplayTipsCanvas;
        [SerializeField]
        private GameObject _pauseCanvas;
        [SerializeField]
        private GameObject _gameOverCanvas;
        [SerializeField]
        private GameObject _highScoreCanvas;
        [SerializeField]
        private GameObject _scoreCanvas;
        [SerializeField]
        private GameObject _livesCanvas;
        [SerializeField]
        private GameObject _bonusCanvas;
        [SerializeField]
        private GameObject _skillCanvas;
        [SerializeField]
        private GameObject _powerCanvas;
        [SerializeField]
        private GameObject _countDownCanvas;
        [SerializeField]
        private Text _highScoreValue;
        [SerializeField]
        private Text _scoreValue;
        [SerializeField]
        private Text _bonusCollectedValue;
        [SerializeField]
        private Text _characterName;
        [SerializeField]
        private Text _countDownText;
        [SerializeField]
        private Image _characterImage;
        [SerializeField]
        private Image[] _livesImages;
        [SerializeField]
        private Image _skillBarFill;
        [SerializeField]
        private Image _powerBarFill;
        [SerializeField]
        private Animator _gameplayTipsCanvasAnimator;
        [SerializeField]
        private Sprite _elfSprite;
        [SerializeField]
        private Sprite _knightSprite;
        [SerializeField]
        private Sprite _mageSprite;
        #endregion private fields

        #region public functions
        public void SetHighScore(int value)
        {
            _highScoreValue.text = value.ToString();
        }

        public void SetScore(int value)
        {
            _scoreValue.text = value.ToString();
        }

        public void SetBonusCollected(int value)
        {
            _bonusCollectedValue.text = value.ToString();
        }

        public void SetChracter(Characters character)
        {
            _characterName.text = character.ToString();
            switch (character)
            {
                case Characters.Elf:
                    _characterImage.sprite = _elfSprite;
                    break;
                case Characters.Knight:
                    _characterImage.sprite = _knightSprite;
                    break;
                case Characters.Mage:
                    _characterImage.sprite = _mageSprite;
                    break;
            }
        }

        public void SetLives(int value, Characters character)
        {
            //max 5 lives
            value = Mathf.Clamp(value, 0, 5);
            for (int i = 0; i < 5; i++)
            {
                _livesImages[i].enabled = value > i;
                switch (character)
                {
                    case Characters.Elf:
                        _livesImages[i].sprite = _elfSprite;
                        break;
                    case Characters.Knight:
                        _livesImages[i].sprite = _knightSprite;
                        break;
                    case Characters.Mage:
                        _livesImages[i].sprite = _mageSprite;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets how much the bar is filled
        /// </summary>
        /// <param name="fill">0 means empty, 1 means full</param>
        public void SetSkillBarFill(float fill)
        {
            _skillBarFill.fillAmount = fill;
        }

        /// <summary>
        /// Sets how much the bar is filled
        /// </summary>
        /// <param name="fill">0 means empty, 1 means full</param>
        public void SetPowerBarFill(float fill)
        {
            _powerBarFill.fillAmount = fill;
        }

        public void ShowInGameUIHUD()
        {
            _highScoreCanvas.SetActive(true);
            _scoreCanvas.SetActive(true);
            _livesCanvas.SetActive(true);
            _bonusCanvas.SetActive(true);
            _skillCanvas.SetActive(true);
            _powerCanvas.SetActive(true);
        }

        public void HideInGameUIHUD()
        {
            _highScoreCanvas.SetActive(false);
            _scoreCanvas.SetActive(false);
            _livesCanvas.SetActive(false);
            _bonusCanvas.SetActive(false);
            _skillCanvas.SetActive(false);
            _powerCanvas.SetActive(false);
        }

        public void ShowMainMenuScreen()
        {
            _titleCanvas.SetActive(true);
            _mainMenuCanvas.SetActive(true);
        }

        public void HideMainMenu()
        {
            _titleCanvas.SetActive(false);
            _mainMenuCanvas.SetActive(false);
        }

        public void ShowGameplayTipsCanvas()
        {
            _gameplayTipsCanvas.SetActive(true);
        }

        public void HideGameplayTipsCanvas()
        {
            _gameplayTipsCanvas.SetActive(true);
            _gameplayTipsCanvasAnimator.Play("ShowInOut _GameplayTips");
        }

        public void ShowGameOverCanvas()
        {
            _gameOverCanvas.SetActive(true);
        }

        public void HideGameOverCanvas()
        {
            _gameOverCanvas.SetActive(false);
        }

        public void ShowPauseMenu()
        {
            _pauseCanvas.SetActive(true);
        }

        public void HidePauseMenu()
        {
            _pauseCanvas.SetActive(false);
        }

        public void ShowCountDown(string text)
        {
            _countDownCanvas.SetActive(true);
            _countDownText.text = text;
        }

        public void HideCountDown()
        {
            _countDownCanvas.SetActive(false);
        }

        public void HideAll()
        {
            HideInGameUIHUD();
            HideMainMenu();
            HideGameOverCanvas();
            HidePauseMenu();
            HideCountDown();
        }
        #endregion public functions
    }
}
