using System;

    public class EventManager : Singleton<EventManager>
    {
        public event Action<int> onPlayerHpChanged;
        public event Action<float> onBossLightningQTE;
        public event Action<bool> onQTECompleted;

        public void PlayerHpChanged(int newHp)
        {
            onPlayerHpChanged?.Invoke(newHp);
        }

   
        public void BossLightningQTE(float duration)
        {
            onBossLightningQTE?.Invoke(duration);
            //onPlayerHpChanged?.Invoke(newHp);
        }


        public void QTECompleted(bool isSuccess)
        {
            onQTECompleted?.Invoke(isSuccess);
            //onPlayerHpChanged?.Invoke(newHp);
        }

    }
