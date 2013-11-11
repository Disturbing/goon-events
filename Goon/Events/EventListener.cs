using System;

namespace Toolbox.Events
{
    /// <summary>
    /// Attribute flag on Private methods to register the method as a listener for a particular event
    /// 
    /// @author DisTurBinG
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListener : Attribute
    {

    }
}
