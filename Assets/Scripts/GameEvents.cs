using System;

namespace Game 
{
    public class GameEvents
    {
        private static GameEvents _instance;

        public static GameEvents Instance
        {
            get 
            {
                if(_instance == null)
                {
                    _instance = new GameEvents();
                }
                return _instance;
            }
            private set 
            {
                _instance = value;
            }
        }

        #region handlers
        public event EventHandler<PelletCollectedEventArgs> PelletCollected;
        #endregion handlers

        #region args
        public class PelletCollectedEventArgs : EventArgs
        {
            public PelletType PelletType;
        }
        #endregion args

        #region events
        public void OnPelletCollected(PelletCollectedEventArgs args)
        {
            PelletCollected?.Invoke(this, args);
        }
        #endregion events
    }
}


