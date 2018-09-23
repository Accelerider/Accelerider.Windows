using System;

namespace Accelerider.Windows.Infrastructure
{
    public class AsyncLocker
    {
        private event Action Unlock;

        private bool _isLocked;

        private bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value) return;
                _isLocked = value;
                if (!value) Unlock?.Invoke();
            }
        }

        public void Await(Action action)
        {
            if (IsLocked)
            {
                Unlock += OnUnlock;
            }
            else
            {
                IsLocked = true;
                action?.Invoke();
                IsLocked = false;
            }

            void OnUnlock()
            {
                Unlock -= OnUnlock;
                action?.Invoke();
            }
        }
    }
}
