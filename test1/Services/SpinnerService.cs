using System;

namespace test1.Services
{
    public class SpinnerService
    {
        public bool IsShowing { get; private set; }

        // Event with current visibility state
        public event Action<bool>? OnChange;

        public void Show() => Toggle(true);
        public void Hide() => Toggle(false);

        public void Toggle(bool show)
        {
            if (IsShowing == show) return;
            IsShowing = show;
            OnChange?.Invoke(IsShowing);
        }
    }
}