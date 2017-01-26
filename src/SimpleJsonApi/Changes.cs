using System;
using System.Collections.Generic;

namespace SimpleJsonApi
{
    public sealed class Changes<TResource> : IChanges
    {
        private readonly List<Action<TResource>> _changes = new List<Action<TResource>>();

        internal Changes()
        {
        }

        public void ApplyTo(TResource resource)
        {
            _changes.ForEach(x => x(resource));
        }

        internal void AddChange(Action<TResource> change)
        {
            _changes.Add(change);
        }

        void IChanges.ApplyTo(object resource)
        {
            if (!(resource is TResource)) throw new ArgumentException("Invalid resource type");
            ApplyTo((TResource)resource);
        }
    }
}
