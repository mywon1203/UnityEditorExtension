#nullable enable
namespace KGGEEP
{
    using System;

    using UnityEngine;

    [Serializable]
    public class TypeSerializableReference
    {
        [SerializeField]
        private string assemblyQualifiedName;
        
        public TypeSerializableReference(Type type)
        {
            this.Type = type;
            this.assemblyQualifiedName = type?.AssemblyQualifiedName ?? string.Empty;
        }

        public Type? Type
        {
            get
            {
                if (string.IsNullOrEmpty(this.assemblyQualifiedName))
                {
                    return null;
                }

                try
                {
                    return Type.GetType(this.assemblyQualifiedName);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                this.assemblyQualifiedName = value?.AssemblyQualifiedName ?? string.Empty;
            }
        }

        public bool IsValid => this.Type != null;
        public string Name => this.Type?.FullName ?? this.assemblyQualifiedName;

        public static implicit operator Type(TypeSerializableReference typeRef) => typeRef?.Type!;
        public static implicit operator TypeSerializableReference(Type type) => new(type);

        public override int GetHashCode() => this.Type?.GetHashCode() ?? 0;
        public override string ToString() => this.Name;
    }
}
