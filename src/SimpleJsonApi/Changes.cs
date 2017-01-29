using System;
using System.Collections.Generic;

namespace SimpleJsonApi
{
    /// <summary>
    /// Class that can be used as a resource type wrapper for HttpPatch actions.<br />
    /// The changes object can then be used to apply patched changes to a resource instance.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    /// <example>
    /// <![CDATA[
    /// [HttpPatch]
    /// [Route("{id}"]
    /// public async Task<IHttpActionResult> PatchCar(Guid id, Changes<Car> changes)
    /// {
    ///     var car = await _repository.GetCar(id);
    ///     changes.ApplyTo(car);
    ///     await _repository.Save(car);
    ///     return Ok();
    /// }
    /// ]]>
    /// </example>
    public sealed class Changes<TResource> : IChanges
    {
        private readonly List<Action<TResource>> _changes = new List<Action<TResource>>();

        internal Changes()
        {
        }

        /// <summary>
        /// Method that applies the changes to given resource instance.
        /// </summary>
        /// <param name="resource">The resource instance to apply the changes to.</param>
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
