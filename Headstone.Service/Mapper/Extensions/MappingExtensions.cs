using Headstone.Common.Mapper;
using Headstone.Models.Events;
using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Mapper.Extensions
{
    internal static class MappingExtensions
    {
        #region [ Utilities ]

        /// <summary>
        /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object
        /// </summary>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        private static TDestination Map<TDestination>(this object source)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// </summary>
        /// <typeparam name="TSource">Source object type</typeparam>
        /// <typeparam name="TDestination">Destination object type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>Mapped destination object, same instance as the passed destination object</returns>
        private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            //use AutoMapper for mapping objects
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #endregion

        #region [ Evet-Entity mapping ]

        public static TEntity ToEntity<TEntity>(this BaseEvent eventBase) where TEntity : Entity
        {
            if (eventBase == null)
                throw new ArgumentNullException(nameof(eventBase));

            return eventBase.Map<TEntity>();
        }


        public static TEntity ToEntity<TEntity, TEvent>(this TEvent eventBase, TEntity entity)
            where TEntity : Entity where TEvent : BaseEvent
        {
            if (eventBase == null)
                throw new ArgumentNullException(nameof(eventBase));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return eventBase.MapTo(entity);
        }

        #endregion
    }

}

