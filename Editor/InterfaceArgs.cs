using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableInterface.Editor
{
    public struct InterfaceArgs
    {
        public readonly Type ObjectType;
        public readonly Type InterfaceType;

        public InterfaceArgs(Type objectType, Type interfaceType)
        {
            Debug.Assert(typeof(Object).IsAssignableFrom(objectType), $"{nameof(objectType)} needs to be of Type {typeof(Object)}.");
            Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} needs to be an interface.");

            ObjectType = objectType;
            InterfaceType = interfaceType;
        }
    }
}