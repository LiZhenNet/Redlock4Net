using System;
using System.Net;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;


namespace RedLock4Net
{
    public class RedLockFactory 
    {
        private readonly int _connectionTimeout;
        private readonly int _configCheckSeconds;
        private readonly int _redisDatabase;
        private readonly IList<ConnectionMultiplexer> _redisConnections;

        public RedLockFactory(params ConnectionMultiplexer[] connection):this(100,10,0,connection)
        {
        }

        public RedLockFactory(int connectionTimeout = 100, int configCheckSeconds= 10, int redisDatabase = 0, params ConnectionMultiplexer[] redisConnections)
        {
            _configCheckSeconds = configCheckSeconds;
            _redisDatabase = redisDatabase;
            _connectionTimeout = connectionTimeout;
            this._redisConnections = redisConnections.ToList();
        }
        public RedLockFactory(int connectionTimeout = 100, int configCheckSeconds=10, int redisDatabase=0, params EndPoint[] endPoints)
        {
            _connectionTimeout = connectionTimeout;
            _configCheckSeconds = configCheckSeconds;
            _redisDatabase = redisDatabase;
            this._redisConnections = endPoints.Select(endPoint => new RedisEndPoint
            {
                EndPoint = endPoint
            }).Select(redisEndPoint => new ConfigurationOptions()
            {
                AbortOnConnectFail = false,
                ConnectTimeout = redisEndPoint.ConnectionTimeout ?? _connectionTimeout,
                Ssl = redisEndPoint.Ssl,
                Password = redisEndPoint.Password,
                ConfigCheckSeconds = redisEndPoint.ConfigCheckSeconds ?? _configCheckSeconds
            }).Select(config => ConnectionMultiplexer.Connect(config)).ToList();
        }
        public RedLockFactory(int connectionTimeout = 100, int configCheckSeconds=10, int redisDatabase=0, params RedisEndPoint[] redisEndPoints)
        {
            _connectionTimeout = connectionTimeout;
            _configCheckSeconds = configCheckSeconds;
            _redisDatabase = redisDatabase;
            this._redisConnections = redisEndPoints.Select(redisEndPoint => new ConfigurationOptions()
            {
                AbortOnConnectFail = false,
                ConnectTimeout = redisEndPoint.ConnectionTimeout ?? _connectionTimeout,
                Ssl = redisEndPoint.Ssl,
                Password = redisEndPoint.Password,
                ConfigCheckSeconds = redisEndPoint.ConfigCheckSeconds ?? _configCheckSeconds
            }).Select(config => ConnectionMultiplexer.Connect(config)).ToList();
        }

        public RedLock GetLock(RedisKey lockkey, TimeSpan expiryTime,int retryCount,TimeSpan retryDelay)
        {
            RedLock redLock = new RedLock(_redisConnections,_redisDatabase,lockkey, expiryTime);
            redLock.Lock(retryCount,retryDelay);
            return redLock;
        }
    }
}
