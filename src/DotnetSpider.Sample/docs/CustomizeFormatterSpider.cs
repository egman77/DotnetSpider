using DotnetSpider.Core;
using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using DotnetSpider.Extraction;
using DotnetSpider.Extraction.Model.Attribute;
using DotnetSpider.Extraction.Model.Formatter;
using System.Collections.Generic;

namespace DotnetSpider.Sample.docs
{
	/// <summary>
	/// 在爬网结束后,检查数据库的数据, 清空每一个"网站url"值
	/// After spider complete, check the data in mysql, website are null in every row.
	/// </summary>
	public class CustomizeFormatterSpider
	{
		public static void Run()
		{
			BaiduSearchSpider spider = new BaiduSearchSpider();
			spider.Run();
		}

		[TaskName("baidu_search")]
		class BaiduSearchSpider : EntitySpider
		{
			protected override void OnInit(params string[] arguments)
			{
				EmptySleepTime = 1000;
				var word = "可乐|雪碧";
				AddRequest(string.Format("http://news.baidu.com/ns?word={0}&tn=news&from=news&cl=2&pn=0&rn=20&ct=1", word), new Dictionary<string, dynamic> { { "Keyword", word } });
				AddEntityType<Result>();
				AddPipeline(new ConsoleEntityPipeline());
			}

			[Schema("baidu", "baidu_search_customizeforamtter")]
			[Entity(Expression = ".//div[@class='result']", Type = SelectorType.XPath)]
			class Result : BaseEntity
			{
				/// <summary>
				/// 关键字
				/// </summary>
				[Column]
				[Field(Expression = "Keyword", Type = SelectorType.Enviroment)]
				public string Keyword { get; set; }

				/// <summary>
				/// 标题
				/// </summary>
				[Column]
				[Field(Expression = ".//h3[@class='c-title']/a")]
				[ReplaceFormatter(NewValue = "", OldValue = "<em>")]
				[ReplaceFormatter(NewValue = "", OldValue = "</em>")]
				public string Title { get; set; }

				/// <summary>
				/// 网址
				/// </summary>
				[Column]
				[Field(Expression = ".//h3[@class='c-title']/a/@href")]
				public string Url { get; set; }

				/// <summary>
				/// 网站地址
				/// </summary>
				[Column]
				[Field(Expression = ".//div/p[@class='c-author']/text()")]
				[NullFormatter]
				public string Website { get; set; }
			}
		}
	}

	public class NullFormatter : Formatter
	{
		protected override void CheckArguments()
		{
		}

		protected override object FormatValue(object value)
		{
			return "";
		}
	}
}
