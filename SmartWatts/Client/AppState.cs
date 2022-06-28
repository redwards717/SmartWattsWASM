namespace SmartWatts.Client
{
    public class AppState
    {
        public User LoggedInUser { get; set; }
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
            LoggedInUser.Activities = activities;
            NotifyStateChanged();
        }

        public void AddUsersActivities(List<Activity> activities)
        {
            LoggedInUser.Activities.AddRange(activities);
            NotifyStateChanged();
        }

        public void AddUsersActivities(Activity activity)
        {
            LoggedInUser.Activities.Add(activity);
            NotifyStateChanged();
        }

        public void LoaderOn(string msg)
        {
            Loading = true;
            LoadingMsg = msg;
            NotifyStateChanged();
        }

        public void SetLoadingMsg(string msg, bool overwrite = true)
        {
            LoadingMsg = overwrite ? msg : LoadingMsg + "\n" + msg;
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
