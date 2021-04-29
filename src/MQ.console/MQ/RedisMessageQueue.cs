using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.console.MQ
{
    /// <summary>
    /// redis message queue
    /// </summary>
    class RedisMessageQueue : IDisposable
    {
        public RedisClient redisClient { get; }
        public RedisMessageQueue(string redisHost)
        {
            redisClient = new RedisClient(redisHost);
        }
        /// <summary>
        /// 消息入队
        /// </summary>
        /// <param name="QKey"></param>
        /// <param name="QMessage"></param>
        /// <returns></returns>
        public long EnQueue(string QKey,string QMessage)
        {
            //1、编码字符串
            byte[] bytes = Encoding.UTF8.GetBytes(QMessage);

            //2、redis消息队列入队
            long count = redisClient.LPush(QKey, bytes);
            return count;
        }
        /// <summary>
        /// 出队（非阻塞）=== 拉
        /// </summary>
        /// <param name="QKey"></param>
        /// <returns></returns>
        public string DeQueue(string QKey)
        {
            //1、redis消息出队
            byte[] bytes = redisClient.RPop(QKey);
            string Qmessage = null;
            //2、字节转string
            if (bytes == null)
            {
                Console.WriteLine("队列中数据为空");
            }
            else
            {
                Qmessage = Encoding.UTF8.GetString(bytes);
            }
            return Qmessage;
        }
        /// <summary>
        /// 出队（阻塞）=== 推
        /// </summary>
        /// <param name="QKey"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public string BDeQueue(string QKey, TimeSpan? timespan)
        {
            //1、redis消息出队
            string Qmessage = redisClient.BlockingPopItemFromList(QKey, timespan);
            return Qmessage;
        }
        /// <summary>
        /// 获取队列数量
        /// </summary>
        /// <param name="QKey"></param>
        /// <returns></returns>
        public long GetQueueCount(string QKey)
        {
            return redisClient.GetListCount(QKey);
        }
        public void Dispose()
        {
            redisClient.Dispose();
        }
    }
}
