using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Redis.mvc.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDistributedCache _distributedCashe;
        private readonly IDatabase _db;

        public HomeController(ILogger<HomeController> logger, 
            IConnectionMultiplexer redis,
            IDistributedCache distributedCashe)
        {
            _logger = logger;
            _redis = redis;
            _distributedCashe = distributedCashe;
            _db = _redis.GetDatabase();
        }

        public IActionResult Index()
        {
            //Get, GetAsync: Accepts a string key and retrieves a cached item as a byte[] array if found in the cache.
            //Set, SetAsync: Adds an item(as byte[] array) to the cache using a string key.
            //Refresh, RefreshAsync: Refreshes an item in the cache based on its key, resetting its sliding expiration timeout(if any).
            //Remove, RemoveAsync: Removes a cache item based on its string key.
            var value = _distributedCashe.Get("name-key");
            if (value == null)
            {
                var obj = new Dictionary<string, string>
                {
                    ["FirstName"] = "Nick",
                    ["LastName"] = "Carter",
                };
                var str = JsonConvert.SerializeObject(obj);
                byte[] encoded = Encoding.UTF8.GetBytes(str);
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _distributedCashe.Set("name-key", encoded, options);
                return View(obj);
            }
            else
            {
                var str = Encoding.UTF8.GetString(value);
                var obj = JsonConvert.DeserializeObject<Dictionary<string,string>>(str);
                return View(obj);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
