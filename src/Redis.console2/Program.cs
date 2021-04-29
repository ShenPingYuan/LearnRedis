using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace Redis.console2
{
    class Program
    {
        static void Main(string[] args)
        {
            using IRedisClient client = new RedisClient("127.0.0.1", 6379);
            #region Set集合
            client.FlushDb();
            //Set集合自动去重
            var libai = new User
            {
                ID = "001",
                Name = "李白",
                Age = "24"
            };
            client.AddItemToSet("set0", JsonConvert.SerializeObject(libai));
            libai = new User
            {
                ID = "001",
                Name = "李白",
                Age = "25"
            };
            client.AddItemToSet("set0", JsonConvert.SerializeObject(libai));
            client.AddItemToSet("set0", JsonConvert.SerializeObject(libai));
            Console.WriteLine(client.GetSetCount("set0"));
            //批量操作添加
            client.AddRangeToSet("set1", new List<string> { "0", "1", "2", "3" });
            //获取Set中的元素
            var sets = client.GetAllItemsFromSet("set1");
            Console.WriteLine("获取set中的元素......");
            foreach (var item in sets)
            {
                Console.WriteLine(item);
            }
            //从set中随机获取一个值
            Console.WriteLine("随机获取set中的一个值......");
            Console.WriteLine(client.GetRandomItemFromSet("set1"));
            //随机删除集合中的元素，并返回删除的值
            Console.WriteLine("随机删除set中的一个值并返回删除的值......");
            var count = client.GetSetCount("set1");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(client.PopItemFromSet("set1"));
            }
            //删除set集合中的某个值
            client.AddRangeToSet("set1", new List<string> { "0", "1", "2", "3" });
            client.RemoveItemFromSet("set1", "1");
            sets = client.GetAllItemsFromSet("set1");
            Console.WriteLine("剩下的值.....");
            foreach (var item in sets)
            {
                Console.WriteLine(item);
            }
            //从原来的集合移除放到新的集合中去
            client.MoveBetweenSets("set1", "set2", "3");
            #region 交叉并补
            client.AddRangeToSet("a", new List<string> { "1", "2", "3", "4" });
            client.AddRangeToSet("b", new List<string> { "4", "3", "5", "6" });
            var jiaoList = client.GetIntersectFromSets("a", "b");
            Console.WriteLine("交集");
            foreach (var item in jiaoList)
            {
                Console.WriteLine(item);
            }
            var bingList = client.GetUnionFromSets("a", "b");
            Console.WriteLine("并集");
            foreach (var item in bingList)
            {
                Console.WriteLine(item);
            }
            #endregion
            #endregion
            #region  ZSet集合
            //zset 自动去重，而且多了一个权重，或者分数的字段，自动排序
            client.FlushDb();
            Console.Clear();
            client.AddItemToSortedSet("zsetid", "a");//不给分数赋值，默认是最大的
            client.AddItemToSortedSet("zsetid", "b",100);
            client.AddItemToSortedSet("zsetid", "c");
            client.AddItemToSortedSet("zsetid", "b");
            client.AddItemToSortedSet("zsetid", "d",120);
            //get all items order by asc
            var zsetList = client.GetAllItemsFromSortedSet("zsetid");
            Console.WriteLine("the zsetList(asc)");
            foreach (var item in zsetList)
            {
                Console.WriteLine(item);
            }
            //get all items order by desc
            Console.WriteLine("zsetList(desc)");
            var descZsetList = client.GetAllItemsFromSortedSetDesc("zsetid");
            foreach (var item in descZsetList)
            {
                Console.WriteLine(item);
            }
            //get items by index(Desc)
            var zsets = client.GetRangeFromSortedSetDesc("zsetid", 0, 1);
            foreach (var item in zsets)
            {
                Console.WriteLine(item);
            }
            //get items by index(Asc)
            zsets = client.GetRangeFromSortedSet("zsetid", 0, 1);
            foreach (var item in zsets)
            {
                Console.WriteLine(item);
            }
            //get the items with scores
            var zsets1 = client.GetAllWithScoresFromSortedSet("zsetid");
            foreach (var item in zsets1)
            {
                Console.WriteLine(item);
            }
            client.AddItemToSortedSet("zsetid1", "a",80);//不给分数赋值，默认是最大的
            client.AddItemToSortedSet("zsetid1", "b", 100);
            client.AddItemToSortedSet("zsetid1", "c",234);
            client.AddItemToSortedSet("zsetid1", "b",543);
            client.AddItemToSortedSet("zsetid1", "d", 120);
            client.AddItemToSortedSet("zsetid1", "e", 120);

            client.AddItemToSortedSet("zsetid2", "b", 543);
            client.AddItemToSortedSet("zsetid2", "d", 120);
            client.AddItemToSortedSet("zsetid2", "f", 120);
            //将多个集合中的交集放入一个新的集合中
            var dics = client.StoreIntersectFromSortedSets("newzsetid", "zsetid", "zsetid1", "zsetid2");
            #endregion
        }
    }
    class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public override string ToString()
        {
            return "ID:" + ID + "\nName:" + Name + "\nAge:" + Age;
        }
    }
}
