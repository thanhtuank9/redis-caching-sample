using System;
using System.Configuration;

namespace RedisCaching.Core
{
    public class RedisBaseCaching
    {
        /// <summary>
        /// Prefix for Redis cache name
        /// </summary>
        protected string _cacheNamePrefix = ConfigurationSettings.AppSettings["RedisCachePrefix"];

        /// <summary>
        /// Get data from cache server by cache name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramObj"></param>
        /// <param name="cacheKey"></param>
        /// <param name="invokeMethod"></param>
        /// <param name="timeout">Expire in second(s)</param>
        /// <returns></returns>
        public T GetDataWithCaching<T, T2>(T2 paramObj, string cacheKey, Func<T2, T> invokeMethod, long timeout = -1) where T : new()
        {
            T ret = default(T);
            try
            {
                if (timeout == -1) timeout = RedisClient.ExpiresTime;
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.StringGet(key);
                if (byteData != null)
                {
                    ret = RedisClient.ProtoBufDeserialize<T>(byteData);
                }
                else
                {
                    ret = invokeMethod(paramObj);
                    if (ret != null)
                    {
                        byteData = RedisClient.ProtoBufSerialize(ret);
                        RedisClient.StringSet(key, byteData, timeout);
                    }
                }
            }
            catch (RedisException rex)
            {
                ret = invokeMethod(paramObj);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// Get data from cache server by cache name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramObj"></param>
        /// <param name="cacheKey"></param>
        /// <param name="invokeMethod"></param>
        /// <param name="timeout">Expire in second(s)</param>
        /// <returns></returns>
        public long GetLongWithCaching<T>(T paramObj, string cacheKey, Func<T, long> invokeMethod, long timeout = -1)
        {
            long value = 0;

            try
            {
                if (timeout == -1) timeout = RedisClient.ExpiresTime;
                string key = _cacheNamePrefix + cacheKey;
                value = RedisClient.LongGet(key);

                if (value == 0)
                {
                    value = invokeMethod(paramObj);

                    if (value != 0)
                    {
                        RedisClient.StringSet(key, value);
                    }
                }
            }
            catch (RedisException rex)
            {
                value = invokeMethod(paramObj);
            }
            catch (Exception ex)
            { }

            return value;
        }

        /// <summary>
        /// Get data from cache server by cache name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="invokeMethod"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T GetDataWithCaching<T>(string cacheKey, Func<T> invokeMethod, long timeout = -1) where T : new()
        {
            T ret = default(T);
            try
            {
                if (timeout == -1) timeout = RedisClient.ExpiresTime;
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.StringGet(key);
                if (byteData != null)
                {
                    ret = RedisClient.ProtoBufDeserialize<T>(byteData);
                }
                else
                {
                    ret = invokeMethod();
                    if (ret != null)
                    {
                        byteData = RedisClient.ProtoBufSerialize(ret);
                        RedisClient.StringSet(key, byteData, timeout);
                    }
                }
            }
            catch (RedisException rex)
            {
                ret = invokeMethod();
            }
            catch (Exception ex) { }

            return ret;
        }

        /// <summary>
        /// Get data from cache server by cache name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public T GetCachingData<T>(string cacheKey) where T : new()
        {
            T ret = default(T);
            try
            {
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.StringGet(key);

                if (byteData != null)
                {
                    ret = RedisClient.ProtoBufDeserialize<T>(byteData);
                }
            }
            catch (Exception ex)
            { }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public string GetCachingString(string cacheKey)
        {
            string ret = null;

            try
            {
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.StringGet(key);

                if (byteData != null)
                {
                    ret = RedisClient.ProtoBufDeserialize<string>(byteData);
                }
            }
            catch (Exception ex)
            { }

            return ret;
        }

        /// <summary>
        /// Get data from cache server by cache name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public void SetDataToCache<T>(string cacheKey, T data, long timeout = -1) where T : new()
        {
            try
            {
                if (timeout == -1) timeout = RedisClient.ExpiresTime;
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.ProtoBufSerialize(data);
                RedisClient.StringSet(key, byteData, timeout);
            }
            catch (Exception ex)
            {

            }
        }

        public void AppendDataToCache<T>(string cacheKey, T data) where T : new()
        {
            try
            {
                string key      = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.ProtoBufSerialize(data);
                RedisClient.StringAppend(key, byteData);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        public void SetStringToCache(string cacheKey, string data, long timeout = -1)
        {
            try
            {
                if (timeout == -1) timeout = RedisClient.ExpiresTime;
                string key = _cacheNamePrefix + cacheKey;
                byte[] byteData = RedisClient.ProtoBufSerialize(data);
                RedisClient.StringSet(key, byteData, timeout);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="data"></param>
        public void SetLongToCache(string cacheKey, long data)
        {
            try
            {
                string key = _cacheNamePrefix + cacheKey;
                RedisClient.StringSet(key, data);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        public void SetStringIncrement(string cacheKey, int IncValues)
        {
            try
            {
                string key = _cacheNamePrefix + cacheKey;
                RedisClient.StringIncrement(key, IncValues);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Generate cache key by list param that used to get data from store proc
        /// </summary>
        /// <param name="paramObj"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        protected string GenerateCachingKey(object[] paramObj, string cacheKey)
        {
            string key = cacheKey;
            foreach (var item in paramObj)
            {
                key += "_" + item.ToString();
            }

            return key;
        }

        /// <summary>
        /// Delete cache by key with default dbid=0
        /// </summary>
        /// <param name="key"></param>
        public void RemoveCacheByKey(string key)
        {
            try
            {
                key = _cacheNamePrefix + key;
                RedisClient.DeleteKey(key);
            }
            catch (Exception ex)
            { }

        }

        /// <summary>
        /// Delete cache by pattern with default dbid=0
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveCacheByPattern(string pattern)
        {
            try
            {
                pattern = _cacheNamePrefix + pattern;
                RedisClient.DeleteKeys(pattern);
            }
            catch (Exception ex)
            { }

        }

        public void ClearAllCache()
        {
            try
            {
                RedisClient.DeleteKeys(_cacheNamePrefix + "*");
            }
            catch (Exception ex)
            { }
        }
    }
}
