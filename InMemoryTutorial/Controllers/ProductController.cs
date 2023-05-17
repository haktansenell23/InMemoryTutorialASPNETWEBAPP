using InMemoryTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace InMemoryTutorial.Controllers
{
    //Absolute Expiration => Cache kesin bir zaman atar 
    //Sliding Expiration ==> Belli bir sürede erişlmezse datayı siler, varsa ömrünü arttır
    // Absolute ve Sliding beraber kullanılabilir, ama sliding arttırmı toplamada absolute expriation 'ı geçemez 

    public class ProductController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache; 
        }
        public IActionResult Index()
        {
            //key olup olmadığını tespit etme
            if (string.IsNullOrEmpty(_memoryCache.Get<string>("1")))
            {

            }
            // Datanın olup olmadığına bak out ile varsa zamancache 'e at
            if (_memoryCache.TryGetValue("zaman", out string zamancache)) ;
           

            //Expiration VE priority ayarlamaları
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            //options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High;
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key}-->{value}==>sebeb:{reason}");
            });
            _memoryCache.Set<String>("1", DateTime.Now.ToString(), options);

            Product product = new Product { Id = 1, Name = "Kalem", Price = 100 };

            MemoryCacheEntryOptions options1 = new MemoryCacheEntryOptions();
            options1.AbsoluteExpiration = DateTime.Now.AddSeconds(30);

            _memoryCache.Set<Product>("product:1", product);

            Product product1 = _memoryCache.Get<Product>("product:1");

            return View();
        }
        [HttpGet]
        public IActionResult ShowDate()
        {


           // //Memoryden sil
           //_memoryCache.Remove("1");



            ////Varsa getir yoksa oluştur
            //_memoryCache.GetOrCreate("1", x =>
            //{

            //    return DateTime.Now.ToString();
            //}

            //);
            //Data getirme
            ViewBag.Date = _memoryCache.Get<string>("1");
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Reason = callback;
             _memoryCache.TryGetValue("product:1", out Product product);
            Product pro = product;
            ViewBag.Product = pro;
            return View();
        }


    }
}
