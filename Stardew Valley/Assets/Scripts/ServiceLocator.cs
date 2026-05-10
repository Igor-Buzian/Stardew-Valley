// File: Scripts/Core/ServiceLocator.cs
// Purpose: Lightweight service locator so systems find each other without
//          hard Inspector references that break prefab portability.
// WHY: Full DI frameworks (Zenject, VContainer) are great but overkill for a
//      prototype. ServiceLocator gives us decoupling with zero dependencies.
//      Systems register themselves on Awake; consumers resolve on Start.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmSim.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>Register a service instance (call from Awake).</summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Overwriting existing registration for {type.Name}");
            }
            _services[type] = service;
        }

        /// <summary>Retrieve a service (call from Start or later).</summary>
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return service as T;

            Debug.LogError($"[ServiceLocator] Service {typeof(T).Name} not registered!");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = obj as T;
                return service != null;
            }
            service = null;
            return false;
        }

        /// <summary>Call on scene unload to prevent stale references.</summary>
        public static void Clear() => _services.Clear();
    }
}