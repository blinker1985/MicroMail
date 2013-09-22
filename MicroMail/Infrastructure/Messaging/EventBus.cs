using System;
using System.Collections.Generic;

namespace MicroMail.Infrastructure.Messaging
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<IExecutable>> _subscribersTypeMap = new Dictionary<Type, List<IExecutable>>();
        private readonly Dictionary<object, List<IExecutable>> _subscribersObjectMap = new Dictionary<object, List<IExecutable>>();

        public void Subscribe<TMessage>(Action<TMessage> callbackAction) where TMessage : class 
        {
            if (callbackAction == null) return;

            var key = typeof (TMessage);
            var callbacks = GetCallbacksByType(key);

            if (callbacks == null)
            {
                _subscribersTypeMap[key] = callbacks = new List<IExecutable>();
            }

            callbacks.Add(new Callback<TMessage> { InnerAction = callbackAction });
        }

        public void Subscribe(object triggerEvent, Action<object> callbackAction)
        {
            if (callbackAction == null) return;

            var callbacks = GetCallbacksByKey(triggerEvent);

            if (callbacks == null)
            {
                _subscribersObjectMap[triggerEvent] = callbacks = new List<IExecutable>();
            }

            callbacks.Add(new Callback<object> { InnerAction = callbackAction });
        }

        public void Unsubscribe<TMessage>(Action<object> callbackAction) where TMessage : class
        {
            var callbacks = GetCallbacksByType(typeof(TMessage));

            if (callbacks != null)
            {
                callbacks.RemoveAll(m => m != null && ((Callback<TMessage>)m).InnerAction == callbackAction);
            }
        }

        public void Unsubscribe(object triggerEvent, Action<object> callbackAction)
        {
            var callbacks = GetCallbacksByKey(triggerEvent);

            if (callbacks != null)
            {
                callbacks.RemoveAll(m => m != null && ((Callback<object>)m).InnerAction == callbackAction);
            }
        }

        public void Trigger(object message)
        {
            var callbacks = GetCallbacksByKey(message) ?? GetCallbacksByType(message.GetType());

            if (callbacks == null) return;

            foreach (var callback in callbacks)
            {
                callback.Execute(message);
            }
        }

        private List<IExecutable> GetCallbacksByType(Type type)
        {
            return _subscribersTypeMap.ContainsKey(type) ? _subscribersTypeMap[type] : null;
        }

        private List<IExecutable> GetCallbacksByKey(object key)
        {
            return _subscribersObjectMap.ContainsKey(key) ? _subscribersObjectMap[key] : null;
        }
    }

}
