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
			Crawl_BaiduSearch();
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
	}
}