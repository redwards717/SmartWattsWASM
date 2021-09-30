using SmartWatts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatts.Client
{
    public class AppState
    {
        public User LoggedInUser { get; set; }
        public event Action OnChange;
        public void SetUser(User user)
        {
            LoggedInUser = user;
            NotifyStateChanged();
        }

        public void ClearUser()
        {
            LoggedInUser = null;
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }



    }
}
