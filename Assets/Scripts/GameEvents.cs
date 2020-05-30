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
            public PelletTypes PelletType;
        }
        public class PowerUpActiveEventArgs : EventArgs 
        {
            public bool PowerUpStatus;
        }
        public class PlayerDamagedEventArgs : EventArgs {}
        public class GhostEatenEventArgs : EventArgs {}
        public class PlayerWalkedTileEventArgs : EventArgs 
        {
            public int indexX;
            public int indexY;
        }
        public class NavigatorWalkedTileEventArgs : EventArgs
        {
            public Navigator Navigator;
        }
        public class SkillActiveEventArgs : EventArgs 
        {
            public bool SkillStatus;
        }
        public class OnPlayerUsedSkillEventArgs : EventArgs {}
        public class OnEnemyDeadEventArgs : EventArgs 
        {
            public EnemyTypes EnemyType;
        }
        #endregion args

        #region handlers
        public event EventHandler<PelletCollectedEventArgs> PelletCollected;
        public event EventHandler<PowerUpActiveEventArgs> PowerUpActive;
        public event EventHandler<SkillActiveEventArgs> SkillActive;
        public event EventHandler<PlayerDamagedEventArgs> PlayerDamaged;
        public event EventHandler<GhostEatenEventArgs> GhostEaten;
        public event EventHandler<PlayerWalkedTileEventArgs> PlayerWalkedTile;
        public event EventHandler<NavigatorWalkedTileEventArgs> NavigatorWalkedTile;
        public event EventHandler<OnPlayerUsedSkillEventArgs> PlayerUsedSkill;
        public event EventHandler<OnEnemyDeadEventArgs> EnemyDead;
        #endregion handlers

        #region events
        public void OnPelletCollected(PelletCollectedEventArgs args)
        {
            PelletCollected?.Invoke(this, args);
        }

        public void OnPowerUpActive(PowerUpActiveEventArgs args)
        {
            PowerUpActive?.Invoke(this, args);
        }

        public void OnSkillActive(SkillActiveEventArgs args)
        {
            SkillActive?.Invoke(this, args);
        }

        public void OnPlayerDamaged(PlayerDamagedEventArgs args)
        {
            PlayerDamaged?.Invoke(this, args);
        }

        public void OnGhostEaten(GhostEatenEventArgs args)
        {
            GhostEaten?.Invoke(this, args);
        }

        public void OnPlayerWalkedTile(PlayerWalkedTileEventArgs args)
        {
            PlayerWalkedTile?.Invoke(this, args);
        }

        public void OnNavigatorWalkedTile(NavigatorWalkedTileEventArgs args)
        {
            NavigatorWalkedTile?.Invoke(this, args);
        }

        public void OnPlayerUsedSkill(OnPlayerUsedSkillEventArgs args)
        {
            PlayerUsedSkill?.Invoke(this, args);
        }

        public void OnEnemyDead(OnEnemyDeadEventArgs args)
        {
            EnemyDead?.Invoke(this, args);
        }
        #endregion events
    }
}


