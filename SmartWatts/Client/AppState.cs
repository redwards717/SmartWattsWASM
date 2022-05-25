namespace SmartWatts.Client
{
    public class AppState
    {
        public User LoggedInUser { get; set; }
        public List<Activity> UsersActivities { get; set; }
        public bool Loading { get; set; }
        public string LoadingMsg { get; set; } = "Loading...";
        public event Action OnChange;
        public void SetUser(User user)
        {
            LoggedInUser = user;
            NotifyStateChanged();
        }

        public void SetUsersActivities(List<Activity> activities)
        {
            UsersActivities = activities;
            NotifyStateChanged();
        }

        public void LoaderOn(string msg)
        {
            Loading = true;
            LoadingMsg = msg;
            NotifyStateChanged();
        }

        public void SetLoadingMsg(string msg)
        {
            LoadingMsg = msg;
            NotifyStateChanged();
        }
        public void LoaderOff()
        {
            Loading = false;
            LoadingMsg = null;
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
