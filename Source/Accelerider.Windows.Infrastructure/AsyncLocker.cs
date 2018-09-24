using System;

namespace Accelerider.Windows.Infrastructure
{
    public class AsyncLocker
    {
        private event Action Unlocked;

        private bool _isLocked;

        private bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value) return;
                _isLocked = value;
                if (!value) Unlocked?.Invoke();
            }
        }

        public void Await(Action action)
        {
            if (IsLocked)
            {
                Unlocked += OnUnlocked;
            }
            else
            {
                IsLocked = true;
                action?.Invoke();
                IsLocked = false;
            }

            void OnUnlocked()
            {
                Unlocked -= OnUnlocked;
                action?.Invoke();
            }
        }
    }
}
