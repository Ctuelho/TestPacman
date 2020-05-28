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

        #region args
        public class PelletCollectedEventArgs : EventArgs
        {
            public PelletType PelletType;
        }

        public class OnPlayerDamagedEventArgs : EventArgs {}
        public class OnGhostEatenEventArgs : EventArgs {}
        public class OnPlayerWalkedTileEventArgs : EventArgs 
        {
            public int indexX;
            public int indexY;
        }
        #endregion args

        #region handlers
        public event EventHandler<PelletCollectedEventArgs> PelletCollected;
        public event EventHandler<OnPlayerDamagedEventArgs> PlayerDamaged;
        public event EventHandler<OnGhostEatenEventArgs> GhostEaten;
        public event EventHandler<OnPlayerWalkedTileEventArgs> PlayerWalkedTile;
        #endregion handlers

        #region events
        public void OnPelletCollected(PelletCollectedEventArgs args)
        {
            PelletCollected?.Invoke(this, args);
        }

        public void OnPlayerDamaged(OnPlayerDamagedEventArgs args)
        {
            PlayerDamaged?.Invoke(this, args);
        }

        public void OnGhostEaten(OnGhostEatenEventArgs args)
        {
            GhostEaten?.Invoke(this, args);
        }

        public void OnPlayerWalkedTile(OnPlayerWalkedTileEventArgs args)
        {
            PlayerWalkedTile?.Invoke(this, args);
        }
        #endregion events
    }
}


