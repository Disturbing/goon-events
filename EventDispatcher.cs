using System;
using System.Collections.Generic;
using System.Reflection;

namespace Goon.Events
{
    /// <summary>
    /// Dispatches custom events to methods flagged with EventListener attribute.
    /// Must register list of event types in constructor, which implement interface Event.
    /// 
    /// @author DisTurBinG
    /// </summary>
    public class EventDispatcher
    {
        private readonly Dictionary<Type, List<EventListenerContainer>> eventListenerContainerMap = new Dictionary<Type, List<EventListenerContainer>>();

        private readonly HashSet<object> registeredEventListeners = new HashSet<object>(); 

        private struct EventListenerContainer
        {
            internal readonly object Instance;
            internal readonly MethodInfo EventListenerMethod;

            internal EventListenerContainer(object instance, MethodInfo eventListenerMethod)
            {
                Instance = instance;
                EventListenerMethod = eventListenerMethod;
            }
        }

        public EventDispatcher(params Type[] managedEvents)
        {
            foreach(Type managedEvent in managedEvents)
            {
                if (managedEvent.GetInterface(typeof(Event).FullName) != null)
                {
                    eventListenerContainerMap[managedEvent] = null;
                }
                else
                {
                    throw new Exception("All Managed Events Must Be Type EventListener! Passed in invalid type: " + managedEvent.FullName);
                }
            }
        }

        public bool AddEventListener(object eventListenerInstance)
        {
            bool successfullyAdded = false;

            if (!registeredEventListeners.Contains(eventListenerInstance))
            {
                successfullyAdded = AttemptToRegisterEventListener(eventListenerInstance);
            }

            return successfullyAdded;
        }

        private bool AttemptToRegisterEventListener(object eventListenerInstance)
        {
            bool registeredEventListener = false;

            foreach (MethodInfo methodInfo in eventListenerInstance.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (Attribute.GetCustomAttribute(methodInfo, typeof (EventListener)) != null)
                {
                    registeredEventListener = RegisterMethodToEvent(eventListenerInstance, methodInfo);
                }
            }

            return registeredEventListener;
        }

        private bool RegisterMethodToEvent(object eventListenerInstance, MethodInfo methodInfo)
        {
            bool registeredMethod = false;

            if (methodInfo.GetParameters().Length == 1)
            {
                Type eventType = methodInfo.GetParameters()[0].ParameterType;
                List<EventListenerContainer> eventListeners;
                registeredMethod = eventListenerContainerMap.TryGetValue(eventType, out eventListeners);

                if (registeredMethod)
                {
                    if (eventListeners == null)
                    {
                        eventListeners = eventListenerContainerMap[eventType] = new List<EventListenerContainer>();
                    }

                    registeredEventListeners.Add(eventListenerInstance);
                    eventListeners.Add(new EventListenerContainer(eventListenerInstance, methodInfo));
                }
            }
            else
            {
                throw new Exception(string.Format("Flagged EventListener {0}#{1} has invalid MethodParameters",
                                             eventListenerInstance.GetType().FullName,
                                             methodInfo.Name));
            }

            return registeredMethod;
        }

        public bool RemoveEventListener(object eventListenerInstance)
        {
            bool removedEventListener = registeredEventListeners.Remove(eventListenerInstance);

            if (removedEventListener)
            {
                foreach (List<EventListenerContainer> eventListenerInstances in eventListenerContainerMap.Values)
                {
                    eventListenerInstances.RemoveAll(e => e.Instance == eventListenerInstance);
                }
            }
          
            return removedEventListener;
        }

        public void BroadcastEvent(Event eventToBroadcast)
        {
            List<EventListenerContainer> eventListeners = eventListenerContainerMap[eventToBroadcast.GetType()];

            if (eventListeners != null)
            {
                foreach (EventListenerContainer eventListener in eventListeners)
                {
                    eventListener.EventListenerMethod.Invoke(eventListener.Instance, new object[] {eventToBroadcast} );
                }
            }
        }
    }
}
