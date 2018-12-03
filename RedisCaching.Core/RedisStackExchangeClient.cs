using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace RedisCaching.Core
{
    public class RedisStackExchangeClient
    {
        private static readonly string ServerIp = ConfigurationSettings.AppSettings["RedisServerIP"];
        private static readonly string ServerPort = ConfigurationSettings.AppSettings["RedisServerPort"];
        private static readonly string RedisPassword = ConfigurationSettings.AppSettings["RedisPassword"];

        private const int IoTimeOut = 50000;
        private const int SyncTimeout = 50000;
        private static readonly object Log4 = new object();
        private const string shaLuaGroupLock = "";
        private static SocketManager _socketManager;

        private ConnectionMultiplexer _connection;
        private static volatile RedisStackExchangeClient _instance;

        public static readonly object SyncLock = new object();
        public static readonly object SyncConnectionLock = new object();

        public static RedisStackExchangeClient Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisStackExchangeClient();
                        }
                    }
                }

                return _instance;
            }
        }

        private RedisStackExchangeClient()
        {
            _socketManager = new SocketManager(GetType().Name);
            _connection = GetNewConnection();
        }

        private static ConnectionMultiplexer GetNewConnection()
        {
            var config = ConfigurationOptions.Parse(ServerIp + ":" + ServerPort);
            config.KeepAlive = 5;
            config.SyncTimeout = SyncTimeout;
            config.AbortOnConnectFail = false;
            config.AllowAdmin = true;
            config.ConnectTimeout = IoTimeOut;
            config.SocketManager = _socketManager;
            config.Password = RedisPassword;
            //var connection = ConnectionMultiplexer.Connect(config/*, logger*/);
            var connection = ConnectionMultiplexer.ConnectAsync(config);
            var muxer = connection.Result;
            return muxer;
        }

        public ConnectionMultiplexer GetConnection
        {
            get
            {
                lock (SyncConnectionLock)
                {
                    if (_connection == null)
                        _connection = GetNewConnection();
                    if (!_connection.IsConnected)
                        _connection = GetNewConnection();

                    if (_connection.IsConnected)
                        return _connection;
                    return _connection;
                }
            }
        }

        public static IDatabase CurrentConnection
        {
            get
            {
                var connection = RedisStackExchangeClient.Current.GetConnection.GetDatabase(2);
                return connection;
            }
        }

        public static void DeleteKeys(int dbid, string _pattern)
        {
            var server = RedisStackExchangeClient.Current.GetConnection.GetServer(ServerIp + ":" + ServerPort);
            var redisClient = RedisStackExchangeClient.Current.GetConnection.GetDatabase(dbid);
            lock (Log4)
            {
                var listKey = server.Keys(dbid, _pattern);
                var redisKeys = listKey as RedisKey[] ?? listKey.ToArray();
                if (listKey != null && redisKeys.Any())
                {
                    foreach (var key in redisKeys)
                    {
                        redisClient.KeyDelete(key);
                    }
                }
            }
        }

        public static List<string> GetKeys(int dbid, string _pattern)
        {
            var server = RedisStackExchangeClient.Current.GetConnection.GetServer(ServerIp + ":" + ServerPort);
            var redisClient = RedisStackExchangeClient.Current.GetConnection.GetDatabase(dbid);
            lock (Log4)
            {
                var listKey = server.Keys(dbid, _pattern);
                var redisKeys = listKey as RedisKey[] ?? listKey.ToArray();
                if (redisKeys.Count() > 0)
                {
                    return redisKeys.Select(x => x.ToString()).ToList();
                }
                return null;
            }
        }

    }

    public class RedisClient
    {
        public static int DbId = 1;
        public static long ExpiresTime = 3000000;
        public static object Log1 = new object();
        public static object Log2 = new object();
        public static object Log3 = new object();
        public static object LogSetConnection = new object();
        public static object LogGetConnection = new object();

        public static byte[] ProtoBufSerialize(Object item)
        {
            lock (Log1)
            {
                if (item != null)
                {
                    try
                    {
                        var ms = new MemoryStream();
                        Serializer.Serialize(ms, item);
                        var rt = ms.ToArray();
                        return rt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to serialize object", ex);
                    }
                }
                else
                {
                    throw new Exception("Object serialize is null");
                }
            }
        }

        public static T ProtoBufDeserialize<T>(byte[] byteArray)
        {
            lock (Log2)
            {
                if (byteArray != null && byteArray.Length > 0)
                {
                    try
                    {
                        var ms = new MemoryStream(byteArray);
                        return Serializer.Deserialize<T>(ms);
                    }
                    catch (Exception ex)
                    {
                        return default(T);
                    }
                }
                else
                {
                    //throw new Exception("Object Deserialize is null or empty");
                    return default(T);
                }
            }
        }

        public static List<T> ByteArrayToListObject<T>(byte[][] multiDataList)
        {
            lock (Log3)
            {
                if (multiDataList == null)
                    return new List<T>();

                var results = new List<T>();
                foreach (var multiData in multiDataList)
                {
                    results.Add(ProtoBufDeserialize<T>(multiData));
                }
                return results;
            }
        }

        public static byte[] StringGet(string key, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogGetConnection)
                {
                    var byteData = redisClient.GetDatabase(dbid).StringGet(key);
                    return byteData;
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static long LongGet(string key, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogGetConnection)
                {
                    var byteData = redisClient.GetDatabase(dbid).StringGet(key);
                    return (long)byteData;
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static void StringSet(string key, long value, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    redisClient.GetDatabase(dbid).StringSet(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static void StringSet(string key, string value, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    redisClient.GetDatabase(dbid).StringSet(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static void StringSet(string key, byte[] data, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    redisClient.GetDatabase(dbid).StringSet(key, data);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static void StringSet(string key, byte[] data, long expirySeconds, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    var expire = DateTime.Now.AddSeconds(expirySeconds);
                    redisClient.GetDatabase(dbid).StringSet(key, data, expire.Subtract(DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        public static void StringIncrement(string key, long value, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    redisClient.GetDatabase(dbid).StringIncrement(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }
        /// <summary>
        /// Delete cache by regex pattern
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="_pattern"></param>
        public static void DeleteKeys(string _pattern, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;

                lock (LogSetConnection)
                {
                    RedisStackExchangeClient.DeleteKeys(dbid, _pattern);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        /// <summary>
        /// Delete cache key
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="key"></param>
        public static void DeleteKey(string key, int dbid = -1)
        {
            try
            {
                if (dbid == -1) dbid = DbId;
                var redisClient = RedisStackExchangeClient.Current.GetConnection;
                lock (LogSetConnection)
                {
                    redisClient.GetDatabase(dbid).KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

        /// <summary>
        /// Delete cache by regex pattern in Default DbId (0)
        /// </summary>
        /// <param name="_pattern"></param>
        public static List<string> GetKeyByPattern(string _pattern, int dbid)
        {
            try
            {
                lock (LogSetConnection)
                {
                    return RedisStackExchangeClient.GetKeys(dbid, _pattern);
                }
            }
            catch (Exception ex)
            {
                throw new RedisException();
            }
        }

    }
}
