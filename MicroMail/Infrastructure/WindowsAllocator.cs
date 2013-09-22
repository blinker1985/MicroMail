using System;
using System.Collections.Generic;
using System.Windows;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Windows;
using Ninject.Syntax;
using Ninject;

namespace MicroMail.Infrastructure
{
    class WindowsAllocator
    {
        private readonly EventBus _eventBus;

        private readonly Dictionary<Type, List<Type>> _windowsTypeMap = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<object, List<Type>> _windowsObjectMap = new Dictionary<object, List<Type>>();
        private readonly IResolutionRoot _injector;
        private readonly Dictionary<string, ISingularWindow> _singularWindowsMap = new Dictionary<string, ISingularWindow>();

        public WindowsAllocator(EventBus bus, IResolutionRoot injector)
        {
            _eventBus = bus;
            _injector = injector;
        }

        public void Register<TWindow, TEvent>() where TEvent : class 
        {
            var key = typeof (TEvent);
            var windows = GetWindowsByType(key);
            if (windows == null)
            {
                _windowsTypeMap[key] = windows = new List<Type>();
            }

            windows.Add(typeof(TWindow));

            _eventBus.Subscribe<TEvent>(ShowWindow);
        }

        public void Register<TWindow>(object triggerEvent)
        {
            var windows = GetWindowsByObject(triggerEvent);
            if (windows == null)
            {
                _windowsObjectMap[triggerEvent] = windows = new List<Type>();
            }

            windows.Add(typeof(TWindow));

            _eventBus.Subscribe(triggerEvent, ShowWindow);
        }

        public void Unregister<TWindow, TEvent>() where TEvent : class
        {
            var key = typeof(TEvent);
            var windows = GetWindowsByType(key);
            if (windows != null)
            {
                windows.RemoveAll(m => typeof (TWindow) == m);
            }

            _eventBus.Unsubscribe<TEvent>(ShowWindow);
        }

        public void Unregister<TWindow>(object triggerEvent)
        {
            var windows = GetWindowsByObject(triggerEvent);
            if (windows != null)
            {
                windows.RemoveAll(m => typeof (TWindow) == m);
            }

            _eventBus.Unsubscribe(triggerEvent, ShowWindow);
        }

        private void ShowWindow(object triggerEvent)
        {
            var windowTypes = GetWindowsByObject(triggerEvent) ?? GetWindowsByType(triggerEvent.GetType());

            if (windowTypes == null) return;

            foreach (var windowType in windowTypes)
            {
                CreateWindow(windowType, triggerEvent);
            }
        }

        private void CreateWindow(Type windowType, object passedEvent)
        {
            var window = _injector.Get(windowType) as Window;
            var eventWindow = window as IEventWindow;

            if (eventWindow != null)
            {
                eventWindow.ReceiveEvent(passedEvent);
            }

            var singularWindow = window as ISingularWindow;

            if (singularWindow != null)
            {
                if (_singularWindowsMap.ContainsKey(singularWindow.SingularId))
                {
                    var openedWindow = _singularWindowsMap[singularWindow.SingularId] as Window;
                    if (openedWindow != null)
                    {
                        if (openedWindow.WindowState == WindowState.Minimized)
                        {
                            openedWindow.WindowState = WindowState.Normal;
                        }
                        openedWindow.Activate();
                    }
                    return;
                }

                window.Closed += WindowOnClosed;
                _singularWindowsMap.Add(singularWindow.SingularId, singularWindow);
            }

            if (window != null)
            {
                window.Show();
            }
        }

        private void RemoveWindow(Window window)
        {
            if (window == null) return;

            window.Closed -= WindowOnClosed;

            var singularWindow = window as ISingularWindow;

            if (singularWindow != null)
            {
                _singularWindowsMap.Remove(singularWindow.SingularId);
            }
        }

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            RemoveWindow(sender as Window);
        }

        private List<Type> GetWindowsByType(Type eventType)
        {
            return _windowsTypeMap.ContainsKey(eventType) ? _windowsTypeMap[eventType] : null;
        }

        private List<Type> GetWindowsByObject(object eventType)
        {
            return _windowsObjectMap.ContainsKey(eventType) ? _windowsObjectMap[eventType] : null;
        }
    }
}
