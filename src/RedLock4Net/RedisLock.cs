using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StackExchange.Redis;

namespace RedLock4Net
{
    public class RedLock : IDisposable
    {
        private const string UnlockScript =
        @"if redis.call('get', KEYS[1]) == ARGV[1] then
				return redis.call('del', KEYS[1])
			else
				return 0
			end";

        private readonly List<ConnectionMultiplexer> _redisConnections;
        private readonly int _redisDatabase;
        private readonly RedisKey _lockKey;
        private readonly TimeSpan _expiryTime;
        public RedLock(IList<ConnectionMultiplexer> redisConnections, int redisDatabase, RedisKey lockKey, TimeSpan expiryTime)
        {
            _redisConnections = redisConnections.ToList();
            _redisDatabase = redisDatabase;
            _lockKey = lockKey;
            _expiryTime = expiryTime;
        }

        public bool IsAcquired { get; private set; }

        public void Lock(int retryCount, TimeSpan retryDelay)
        {
            IsAcquired = Retry(retryCount, retryDelay, () =>
            {
                int quorum = (_redisConnections.Count / 2) + 1;
                int n = 0;
                _redisConnections.ForEach(connection =>
                {
                    if (SetNX(connection))
                        n++;
                });
                if (n >= quorum)
                {
                    return true;
                }
                else
                {
                    Unlock();
                }
                return false;
            });
        }

        private bool SetNX(ConnectionMultiplexer connection)
        {
            try
            {
                return connection.GetDatabase(_redisDatabase).StringSet(_lockKey, "RedLock4Net", _expiryTime, When.NotExists);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool Retry(int retryCount, TimeSpan retryDelay, Func<bool> action)
        {
            int currentCount = 0;
            while (currentCount++ < retryCount)
            {
                if (action()) return true;
                Thread.Sleep((int)retryDelay.TotalMilliseconds);
            }
            return false;
        }

        private void Unlock()
        {
            RedisKey[] key = { _lockKey };
            _redisConnections.ForEach(connection =>
            {
                connection.GetDatabase(_redisDatabase).ScriptEvaluate(UnlockScript, key);

            });
        }

        public void Dispose()
        {
            Unlock();
        }
    }
}
