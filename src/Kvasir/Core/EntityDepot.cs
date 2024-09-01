using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Core {
    /// <summary>
    ///   A collection of Entities, indexable by <see cref="Type"/>.
    /// </summary>
    internal sealed class EntityDepot {
        /// <summary>
        ///   Get all the (untyped) Entities of a particular <see cref="Type"/> that have been stored.
        /// </summary>
        /// <param name="type">
        ///   [GET] The <see cref="Type"/> of Entity.
        /// </param>
        /// <seealso cref="GetEntities{TEntity}"/>
        public IEnumerable<object> this[Type type] {
            get {
                Debug.Assert(type is not null);
                entities_.TryAdd(type, new HashSet<object>(ReferenceEqualityComparer.Instance));
                return entities_[type];
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="EntityDepot"/> with no stored Entities.
        /// </summary>
        public EntityDepot() {
            entities_ = new Dictionary<Type, HashSet<object>>();
        }

        /// <summary>
        ///   Get all the (typed) Entities of a particular <see cref="Type"/> that have been stored.
        /// </summary>
        /// <seealso cref="this[Type]"/>
        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : class {
            return this[typeof(TEntity)].Cast<TEntity>();
        }

        /// <summary>
        ///   Store a new Entity.
        /// </summary>
        /// <param name="entity">
        ///   The Entity to store.
        /// </param>
        public void StoreEntity(object entity) {
            Debug.Assert(entity is not null);
            Debug.Assert(entity.GetType().IsClass);

            (this[entity.GetType()] as HashSet<object>)!.Add(entity);
        }


        private readonly Dictionary<Type, HashSet<object>> entities_;
    }
}
