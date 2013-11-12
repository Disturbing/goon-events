namespace Toolbox.Events.UnityEventListeners
{
    /// <summary>
    /// Unity's Monobehaviour#OnApplicationFocus event.
    /// 
    /// @author DisTurBinG
    /// </summary>
    public struct UnityApplicationFocusEvent : Event
    {
        public bool IsFocused;
    }
}
