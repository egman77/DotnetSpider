using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using DotnetSpider.Extraction.Model.Attribute;
using System;
using DotnetSpider.Core.Processor.Filter;

namespace DotnetSpider.Sample.docs
{
	public class MultiEntityModelSpider
	{
		public static void Run()
		{
			CnblogsSpider spider = new CnblogsSpider();
			spider.Run();
		}

		private class CnblogsSpider : EntitySpider
		{
			protected override void OnInit(params string[] arguments)
			{
				Identity = ("cnblogs_" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));
				AddRequests("http://www.cnblogs.com");
				AddRequests("https://www.cnblogs.com/news/");  //注意,这里请求的是https
				AddPipeline(new ConsoleEntityPipeline());

				//这里的过滤是针对 request.Url进行筛选!
				AddEntityType<News>().Filter = new PatternFilter("^https://www\\.cnblogs\\.com/news/?$", "www\\.cnblogs\\.com/news/\\d+");
				AddEntityType<BlogSumary>().Filter = new PatternFilter(@"^http://www\.cnblogs\.com/?$", @"http://www\.cnblogs\.com/sitehome/p/\d+");
				//AddEntityType<BlogSumary>();
				//AddEntityType<News>();
			}


			/// <summary>
			/// 新闻类
			/// </summary>
			[Entity(Expression = "//div[@class='post_item']")]
			class News : BaseEntity
			{
				/// <summary>
				/// 名称
				/// </summary>
				[Field(Expression = ".//a[@class='titlelnk']")]
				public string Name { get; set; }

				/// <summary>
				/// 作者
				/// </summary>
				[Field(Expression = ".//div[@class='post_item_foot']/a[1]")]
				public string Author { get; set; }

				/// <summary>
				/// 发布时间
				/// </summary>
				[Field(Expression = ".//div[@class='post_item_foot']/text()")]
				public string PublishTime { get; set; }

				/// <summary>
				/// 网址
				/// </summary>
				[Field(Expression = ".//a[@class='titlelnk']/@href")]
				public string Url { get; set; }
			}
			
			/// <summary>
			/// 博客类
			/// </summary>
			[Entity(Expression = "//div[@class='post_item']")]
			class BlogSumary : BaseEntity
			{
				/// <summary>
				/// 名称
				/// </summary>
				[Field(Expression = ".//a[@class='titlelnk']")]
				public string Name { get; set; }

				/// <summary>
				/// 作者
				/// </summary>
				[Field(Expression = ".//div[@class='post_item_foot']/a[1]")]
				public string Author { get; set; }

				/// <summary>
				/// 发布时间
				/// </summary>
				[Field(Expression = ".//div[@class='post_item_foot']/text()")]
				public string PublishTime { get; set; }

				/// <summary>
				/// 网址
				/// </summary>
				[Field(Expression = ".//a[@class='titlelnk']/@href")]
				public string Url { get; set; }
			}
		}
	}
}
