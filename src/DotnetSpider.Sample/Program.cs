using DotnetSpider.Sample.docs;
#if NETCOREAPP
using System.Text;

#else
using System.Threading;
#endif

namespace DotnetSpider.Sample
{
	static class Program
	{
		static void Main(string[] args)
		{
#if NETCOREAPP
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#else
			ThreadPool.SetMinThreads(256, 256);
#endif

			//HttpClientDownloader downloader = new HttpClientDownloader();
			//var response = downloader.Download(new Request("http://www.163.com")
			//{
			//	Method = HttpMethod.Post,
			//	Content = JsonConvert.SerializeObject(new { a = "bb" }),
			//	ContentType = "application/json"
			//});

			//抓百度搜索的练习
			//EntityModelSpider.Run();

			//抓百度搜索的练习
			//Crawl_BaiduSearch();

			//Crawl_BaiduSearch2();

			//新浪新闻
			//Crawl_SinaSearch();

			//抓csdn 新闻 自增请求
			//Crawl_csdnAutoIncrement();

			//抓某网站的内容,传入cookie内容
			//Crawl_xxCookie();

			//抓cnblogs整站
			//Crawl_cnblogsWholeStie();

			//抓携程数据
			//Crawl_ctripCity();

			Crawl_youku();
		}


		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_BaiduSearch()
		{
			//抓百度搜索的练习
			var spider = new TestSpider();
			spider.Run();

		}

		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_BaiduSearch2()
		{
			////抓百度搜索的练习
			//OrmSpider.Run();


			
		}


		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_SinaSearch()
		{
			//抓新浪新闻的练习
			AfterDownloadCompleteHandlerSpider.Run();
			

		}

		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_csdnAutoIncrement()
		{
			//抓csdn 新闻 自增请求 (按页请求)
			AutoIncrementTargetRequestExtractorSpider.Run();


		}

		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_xxCookie()
		{
			//抓csdn 新闻 自增请求 (按页请求)
			var spider=new CookiesSpider();
			spider.Run();


		}

		/// <summary>
		/// <c>MyTest</c> is a method in the <c>Program</c>
		/// </summary>
		private static void Crawl_cnblogsWholeStie()
		{
			//抓cnblogs整站
			CrawlerWholeSiteSpider.Run();
		}

		private static void Crawl_ctripCity()
		{
			//抓携程
			var spider = new CtripCitySpider();
			spider.Run();
		}

		private static void Crawl_youku()
		{
			//采样优酷
			CustmizeProcessorAndPipelineSpider.Run();

		}

		
	}
}