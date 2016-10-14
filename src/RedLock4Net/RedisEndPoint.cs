using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RedLock4Net
{
    public class RedisEndPoint
    {
        public RedisEndPoint()
        {
            EndPoints = new List<EndPoint>();
        }

        public EndPoint EndPoint
        {
            get
            {
                return EndPoints.FirstOrDefault();
            }
            set
            {
                EndPoints = new List<EndPoint> { value };
            }
        }
        public IList<EndPoint> EndPoints { get; private set; }
        public bool Ssl { get; set; }
        public string Password { get; set; }
        public int? ConnectionTimeout { get; set; }
        public int? ConfigCheckSeconds { get; set; }
    }

}
