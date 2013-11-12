namespace Goon.Events.UnityEventListeners
{
    /// <summary>
    /// Unity's Monobehaviour#OnApplicationPause event.
    /// 
    /// @author DisTurBinG
    /// </summary>
    public struct UnityApplicationPauseEvent : Event
    {
        public bool IsPaused;
    }
}
