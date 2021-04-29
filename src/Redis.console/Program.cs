using ServiceStack.Redis;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Redis.console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IRedisClient client = new RedisClient("127.0.0.1", 6379, null, 0))
            {

                //删除当前数据库中的所有Key
                client.FlushDb();
                string hashId = "stu";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("name", "Micahel Shen");
                dic.Add("age", "18");
                dic.Add("address", "anyue");
                #region 添加、获取字符串
                //新增Key,默认是做了序列化存储
                client.Set<string>("name", "clay");
                //读取key 一般不建议这个方法
                var value0 = client.GetValue("name");
                Console.WriteLine(JsonConvert.DeserializeObject<string>(value0));
                //读取二 推荐试用，帮我们作了反序列
                var value1 = client.Get<string>("name");
                Console.WriteLine(value1);
                Dictionary<string, string> disc = new Dictionary<string, string>();
                disc.Add("id", "001");
                disc.Add("name", "michaelshen");
                disc.Add("address", "anyuexian");
                #endregion
                #region 批量的写
                //批量的写
                client.SetAll(disc);

                var list = client.GetAll<string>(new string[] { "id", "name" });
                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }
                #endregion
                #region 设置过期时间
                client.FlushDb();
                client.Set<string>("name", "Michael Shen", TimeSpan.FromSeconds(10));
                client.Set<string>("key", "Shen", DateTime.Now.AddSeconds(10));
                //如果读不到key则返回一个"";
                #endregion
                #region 追加字符串
                client.FlushDb();
                //client.Set("name", "Michael Shen");
                client.AppendToValue("name", "Michael Shen");
                client.AppendToValue("name", " is SPY");
                Console.WriteLine(client.Get<string>("name"));
                #endregion
                #region 自增自减
                client.FlushDb();
                var a = client.Increment("count", 1);
                client.Increment("count", 100);
                //自减
                client.Decrement("count", 1);
                var b = client.Decrement("count", 5);
                Console.WriteLine(client.Get<int>("count"));
                #endregion
                #region Add和Set
                //Add会先判断数据库中是否存在对应的key，如果存在，再Add的话，会失败，并返回false，否则成功并返回true
                client.FlushDb();
                Console.WriteLine(client.Add<string>("name", "michael Shen"));//true
                Console.WriteLine(client.Add<string>("name", "michael Shen"));//false
                Console.WriteLine(client.Add<string>("name", "Shen"));//false
                //Set
                Console.WriteLine(client.Add<int>("age", 18));//true
                Console.WriteLine(client.Add<int>("age", 19));//true
                Console.WriteLine(client.Add<int>("age", 20));//true 
                #endregion
                #region 判断Redis中是否包含某一个Key
                Console.WriteLine(client.ContainsKey("name"));//true
                Console.WriteLine(client.ContainsKey("NoKey"));//true
                #endregion
                #region 获取类型
                client.FlushDb();
                client.Set<string>("name", "MichaelShen");
                var type = client.GetEntryType("name");
                Console.WriteLine(type);//String
                client.AddItemToList("list1", "asdga");
                Console.WriteLine(client.GetEntryType("list1"));//List
                #endregion
                #region Hash
                client.FlushDb();
                Console.WriteLine("Hash");
                client.SetEntryInHash(hashId, "name", "Michael Shen");
                client.SetEntryInHash(hashId, "age", "18");
                var value = client.GetValueFromHash(hashId, "name");
                Console.WriteLine(value);
                //批量操作
                Dictionary<string, string> stu1 = new Dictionary<string, string>();
                stu1.Add("name", "Michael Shen1");
                stu1.Add("Id", "2017060528");
                stu1.Add("age", "19");
                client.SetRangeInHash(hashId, stu1);
                var result = client.GetAllEntriesFromHash(hashId);
                foreach (var item in result)
                {
                    Console.WriteLine(item.Key + ":" + item.Value);
                }
                //如果hash集合中存在对应的key，则新增失败，否则新增成功
                var res = client.SetEntryInHashIfNotExists(hashId, "Nkey", "new value");//true
                var res1 = client.SetEntryInHashIfNotExists(hashId, "name", "new value");//false
                Console.WriteLine(res);
                Console.WriteLine(res1);
                #endregion
                #region 存用户信息
                client.FlushDb();
                Console.Clear();
                //用String去存储用户信息对象
                //用户信息，首先把对象做一个json序列化，然后存储再Redis的string里，
                //但是如果需要修改这个对象中的其中一个属性值的时候，先把这个对象json字符串读取出来反序列化，
                //然后修改属性值，修改完后再继续序列化再存储进去


                //用hash存储用户信息对象
                client.StoreAsHash<User>(new User { ID = "001", Age = "22", Name = "MichaelShen" });
                Console.WriteLine(client.GetFromHash<User>("001").ToString());
                #endregion
                #region Hash里面Key总数
                Console.Clear();
                client.FlushDb();
                client.SetRangeInHash(hashId, dic);
                Console.WriteLine(client.GetHashCount(hashId));
                #endregion
                #region 获取所有的Keys和Values
                Console.Clear();
                client.FlushDb();
                client.SetRangeInHash(hashId, dic);
                var keys = client.GetHashKeys(hashId);
                var values = client.GetHashValues(hashId);
                foreach (var item in keys)
                {
                    Console.WriteLine(item);
                }
                foreach (var item in values)
                {
                    Console.WriteLine(item);
                }
                #endregion
                #region 删除Hash里的key
                //接上
                Console.WriteLine(client.RemoveEntryFromHash(hashId, "age"));
                foreach (var item in client.GetHashKeys(hashId))
                {
                    Console.WriteLine(item);
                }
                client.SetEntryInHash(hashId, "age","22");
                #endregion
                #region 判断Hash里是否包含某个Key项
                Console.Clear();
                Console.WriteLine(client.HashContainsEntry(hashId,"name"));
                Console.WriteLine(client.HashContainsEntry(hashId,"Nokey"));
                #endregion
                #region 给Hash里的数字自增
                client.SetEntryInHash(hashId, "count", "1");
                client.IncrementValueInHash(hashId, "count", 1);
                Console.WriteLine(client.GetValueFromHash(hashId,"count"));
                #endregion
                #region List
                client.FlushDb();
                Console.Clear();
                {
                    var libai = new User
                    {
                        ID = "001",
                        Name = "李白",
                        Age = "22"
                    };
                    var guanyu = new User
                    {
                        Age = "23",
                        Name = "关于",
                        ID = "002"
                    };
                    client.AddItemToList("list0", JsonConvert.SerializeObject(libai));
                    client.AddItemToList("list0", JsonConvert.SerializeObject(guanyu));
                    #region 从前面、后面插入
                    //从前面插入
                    client.PrependItemToList("list0", JsonConvert.SerializeObject(new User { ID = "003", Age = "25", Name = "花木兰" }));
                    //从后面插入
                    client.PushItemToList("list0", JsonConvert.SerializeObject(new User { ID = "004", Age = "26", Name = "鲁班" }));
                    #endregion
                    #region List设置过期时间
                    client.ExpireEntryAt("list0", DateTime.Now.AddSeconds(10));
                    #endregion
                    #region 批量操作
                    //批量增加
                    client.AddRangeToList("list1", new List<string> { "fdasf", "sadfa", "fdsafsagdfs", "1354213" });
                    var results = client.GetRangeFromList("list1", 0, 2);
                    foreach (var item in results)
                    {
                        Console.WriteLine(item);
                    }
                    #endregion
                    #region 当数据结构操作
                    //当栈来操作(后进先出)
                    client.AddItemToList("listNum", "1");
                    client.AddItemToList("listNum", "2");
                    client.AddItemToList("listNum", "3");
                    client.AddItemToList("listNum", "4");
                    //从尾部删除并返回对应的值
                    Console.WriteLine(client.RemoveEndFromList("listNum"));//4
                    Console.WriteLine(client.RemoveEndFromList("listNum"));//3
                    Console.WriteLine(client.RemoveEndFromList("listNum"));//2
                    Console.WriteLine(client.RemoveEndFromList("listNum"));//1
                    //当队列来操作（先进先出）
                    client.AddItemToList("listNum", "1");
                    client.AddItemToList("listNum", "2");
                    client.AddItemToList("listNum", "3");
                    client.AddItemToList("listNum", "4");
                    //从头部删除并返回对应的值
                    Console.WriteLine(client.RemoveStartFromList("listNum"));
                    Console.WriteLine(client.RemoveStartFromList("listNum"));
                    Console.WriteLine(client.RemoveStartFromList("listNum"));
                    Console.WriteLine(client.RemoveStartFromList("listNum"));

                    //Pop和Push（栈数据结构的操作-先进后出）
                    client.PushItemToList("listStack", "1");
                    client.PushItemToList("listStack", "2");
                    client.PushItemToList("listStack", "3");
                    client.PushItemToList("listStack", "4");
                    Console.WriteLine(client.PopItemFromList("listStack"));
                    Console.WriteLine(client.PopItemFromList("listStack"));
                    Console.WriteLine(client.PopItemFromList("listStack"));
                    Console.WriteLine(client.PopItemFromList("listStack"));
                    //从一个List中pop出来添加到另一个List的头部中
                    client.PushItemToList("fromList", "1");
                    client.PushItemToList("fromList", "2");
                    client.PushItemToList("fromList", "3");
                    client.PushItemToList("fromList", "4");
                    Console.WriteLine(client.PopAndPushItemBetweenLists("fromList", "toList"));
                    Console.WriteLine(client.PopAndPushItemBetweenLists("fromList", "toList"));
                    Console.WriteLine(client.PopAndPushItemBetweenLists("fromList", "toList"));
                    #endregion
                    #region 其他
                    //获取key的过期时间
                    Console.WriteLine(client.GetTimeToLive("fromList"));
                    client.ExpireEntryAt("fromList", DateTime.Now.AddSeconds(60));
                    Console.WriteLine(client.GetTimeToLive("fromList"));
                    #endregion
                }
                #endregion
            }
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
    //string类型会有一定的空间浪费，官网推荐试用hash结构
}
