using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Core
{
    public class ModelStore
    {
        // Dictionary to store models based on their type
        private static Dictionary<Type, object> _models = new Dictionary<Type, object>();

        // Method to register a model
        public static void Register<T>(T model) where T : class
        {
            Type type = typeof(T);
            if (!_models.ContainsKey(type))
            {
                _models.Add(type, model);
                Debug.Log($"<color=cyan>{model}</color> is added to Collection!");
            }
            else
            {
                throw new Exception($"Model of type {type} is already registered.");
            }
        }

        public static T Get<T>() where T : class, new()
        {
            Type type = typeof(T);

            // Attempt to retrieve the model
            if (_models.TryGetValue(type, out var model))
            {
                return model as T;
            }

            // If not found, create and register a new model            
            T newModel = new T();
            Register(newModel);
            return newModel;
        }

        // Generic method to retrieve or create a model
        public static T GetOrCreate<T>() where T : class, new()
        {
            // Attempt to retrieve the model
            T model = Get<T>();

            // If not found, create and register a new one
            if (model == null)
            {
                model = new T();
                Register(model);
            }

            return model;
        }
        

        // Method to clear all models (useful for resetting during scenes or game restart)
        public static void Clear()
        {
            _models.Clear();
        }
    }
}