using System;
using System.Threading;
using StackExchange.Redis;

namespace RedLock4Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("10.9.210.197:6379,allowAdmin=true");
            var redLockFactory = new RedLockFactory(connection);
            using (var redLock = redLockFactory.GetLock("lock", TimeSpan.FromMinutes(50), 2, TimeSpan.FromSeconds(2)))
            {
                Console.WriteLine("[A] Try to get lock");
                if (redLock.IsAcquired)
                {
                    Console.WriteLine("[A]  Geted lock");
                    Thread.Sleep(10000);
                }
                else
                {
                    Console.WriteLine("[A] UnAcquired");
                }
            }
            Console.WriteLine("[A]  End");
            Console.ReadLine();
        }
    }
}
