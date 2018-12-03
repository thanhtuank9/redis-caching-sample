using RedisCaching.Core;
using RedisCaching.Service.ViewModels;
using System.Collections.Generic;

namespace RedisCaching.Service.Services
{
    public class PropertyCaching: RedisBaseCaching
    {
        public PropertyCaching()
        {

        }

        /// <summary>
        /// Get Properties with out caching
        /// </summary>
        /// <returns></returns>
        public List<PropertyItemModel> GetPropertiesWithoutCaching()
        {
            //Without caching
            return SamplePropertyData.GetProperties();
        }

        /// <summary>
        /// The way 1: with in caching
        /// </summary>
        /// <returns></returns>
        public List<PropertyItemModel> GetCachingProperties1()
        {
            return base.GetDataWithCaching(RedisKeys.PropertyDataList, SamplePropertyData.GetProperties);
        }

        /// <summary>
        /// The way 2: with in caching
        /// </summary>
        /// <returns></returns>
        public List<PropertyItemModel> GetCachingProperties2()
        {
            var obj = base.GetCachingData<List<PropertyItemModel>>(RedisKeys.PropertyDataList);
            if (obj == null)
            {
                obj = SamplePropertyData.GetProperties();
                base.SetDataToCache(RedisKeys.PropertyDataList,obj);
            }

            return obj;
        }

        public void ClearCache()
        {
            base.RemoveCacheByKey(RedisKeys.PropertyDataList);
        }
        
    }
}
