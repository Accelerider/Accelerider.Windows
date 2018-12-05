using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public class AsyncLocker
    {
        private event Action Unlocked;

        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value) return;
                _isLocked = value;
                if (!value) Unlocked?.Invoke();
            }
        }

        public void Await(Action action, bool executeAfterUnlocked = true)
        {
            if (!IsLocked)
            {
                try
                {
                    IsLocked = true;
                    action?.Invoke();
                }
                finally
                {
                    IsLocked = false;
                }
            }
            else if (executeAfterUnlocked)
            {
                Unlocked += OnUnlocked;
            }

            void OnUnlocked()
            {
                Unlocked -= OnUnlocked;
                Await(action, executeAfterUnlocked);
            }
        }

        public async void Await(Func<Task> task, bool executeAfterUnlocked = true)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (!IsLocked)
            {
                try
                {
                    IsLocked = true;
                    await task();
                }
                finally
                {
                    IsLocked = false;
                }
            }
            else if (executeAfterUnlocked)
            {
                Unlocked += OnUnlocked;
            }

            void OnUnlocked()
            {
                Unlocked -= OnUnlocked;
                Await(task, executeAfterUnlocked);
            }
        }

        public async Task AwaitAsync(Func<Task> task, bool executeAfterUnlocked = true)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (!IsLocked)
            {
                try
                {
                    IsLocked = true;
                    await task();
                }
                finally
                {
                    IsLocked = false;
                }
            }
            else if (executeAfterUnlocked)
            {
                Unlocked += OnUnlocked;
            }

            async void OnUnlocked()
            {
                Unlocked -= OnUnlocked;
                await AwaitAsync(task, executeAfterUnlocked);
            }
        }
    }
}
