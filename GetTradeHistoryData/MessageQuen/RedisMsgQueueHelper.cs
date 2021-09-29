﻿using FreeRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    ///每小时,4小时，24小时 的成交量,爆仓，对应的交易对，区分永续和交割
    //


        /// <summary>
        /// 封装Redis消息队列
        /// </summary>
        public class RedisMsgQueueHelper : IDisposable
        {
            /// <summary>
            /// Redis客户端
            /// </summary>
            public  RedisClient redisClient { get; set; }

            public   RedisMsgQueueHelper(string redisHost)
            {
                redisClient = FreeRedisHelper.CreateInstance("");
             }

            /// <summary>
            /// 入队
            /// </summary>
            /// <param name="qKeys">入队key</param>
            /// <param name="qMsg">入队消息</param>
            /// <returns></returns>
            public static long EnQueue(string qKey, string qMsg)
            {
                var redisClients = FreeRedisHelper.CreateInstance("");

                //1、编码字符串
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(qMsg);

                //2、Redis消息队列入队
                long count = redisClients.LPush(qKey, bytes);

                return count;
            }

            /// <summary>
            /// 出队(非阻塞) === 拉
            /// </summary>
            /// <param name="qKey">出队key</param>
            /// <returns></returns>
            public static string DeQueue(string qKey)
            {
            var redisClients = FreeRedisHelper.CreateInstance("");
            //1、redis消息出队
            byte[] bytes = redisClients.RPop<byte[]>(qKey);
                string qMsg = null;
                //2、字节转string
                if (bytes == null)
                {
                    Console.WriteLine($"{qKey}队列中数据为空");
                }
                else
                {
                    qMsg = System.Text.Encoding.UTF8.GetString(bytes);
                }

                return qMsg;
            }

            /// <summary>
            /// 出队(阻塞) === 推
            /// </summary>
            /// <param name="qKey">出队key</param>
            /// <param name="timespan">阻塞超时时间</param>
            /// <returns></returns>
            public static string DeQueueBlock(string qKey, int timespan)
            {
            var redisClients = FreeRedisHelper.CreateInstance("");
            // 1、Redis消息出队
            string qMsg = redisClients.BRPop(qKey, timespan);

            return qMsg;
            }

            /// <summary>
            /// 获取队列数量
            /// </summary>
            /// <param name="qKey">队列key</param>
            /// <returns></returns>
            public long GetQueueCount(string qKey)
            {
                return redisClient.LLen(qKey);
            }

            /// <summary>
            /// 关闭Redis
            /// </summary>
            public void Dispose()
            {
                redisClient.Dispose();
            }
        }
    
}
