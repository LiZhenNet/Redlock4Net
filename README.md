# Redlock4Net

Redlock Redis-based distributed locks implementation in .Net 
See More：[Distributed locks with Redis](http://redis.io/topics/distlock)

# Dependencies

StackExchange.Redis[GitHub](https://github.com/StackExchange/StackExchange.Redis) [Nuget](https://www.nuget.org/packages/StackExchange.Redis/)

# Usage
```
ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("127.0.0.0:6379,allowAdmin=true");
var redLockFactory = new RedLockFactory(connection);
using (var redLock = redLockFactory.GetLock("lock", TimeSpan.FromMinutes(10), 2, TimeSpan.FromSeconds(2)))
{
    Console.WriteLine("[A] Try to get lock");
    if (redLock.IsAcquired)
    {
        Console.WriteLine("[A] Geted lock");
        Thread.Sleep(10000);
    }
    else
    {
        Console.WriteLine("[A] UnAcquired");
    }
}
```
