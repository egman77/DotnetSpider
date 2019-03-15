using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using DotnetSpider.Extraction;
using DotnetSpider.Extraction.Model;
using DotnetSpider.Extraction.Model.Attribute;
using DotnetSpider.Extraction.Model.Formatter;

namespace DotnetSpider.Sample.docs
{
	public class OneForAllSpider
	{
		public static void Run()
		{
			Spider spider = new Spider();
			spider.Run();
		}

		class Spider : EntitySpider
		{
			protected override void OnInit(params string[] arguments)
			{
				AddRequests("http://www.jd.com/allSort.aspx");
				AddEntityType<Category>();
				//后面两个实体,在此实例中,url无法匹配得上
				AddEntityType<TmpProduct>();
				AddEntityType<JdProduct>();
				AddPipeline(new ConsoleEntityPipeline());
			}

			/// <summary>
			/// 类型
			/// </summary>
			[Entity(Expression = ".//div[@class='items']//a")]
			class Category : IBaseEntity
			{
				/// <summary>
				/// 类型名
				/// </summary>
				[Field(Expression = ".")]
				public string CategoryName { get; set; }

				/// <summary>
				/// 类型地址
				/// </summary>
				[Next(Extras = new[] { "CategoryName" })]
				[RegexAppendFormatter(Pattern = "http://list.jd.com/list.html\\?cat=[0-9]+", AppendValue = "&page=1&trans=1&JL=6_0_0")]
				[Field(Expression = "./@href")]
				public string Url { get; set; }
			}

			/// <summary>
			/// 临时商品
			/// </summary>
			[Entity(Expression = "//li[@class='gl-item']/div[contains(@class,'j-sku-item')]")]
			[Target(XPaths = new[] { "//span[@class=\"p-num\"]" }, Patterns = new[] { @"&page=[0-9]+&" })]
			class TmpProduct : IBaseEntity
			{
				/// <summary>
				/// 类型名
				/// </summary>
				[Field(Expression = "CategoryName", Type = SelectorType.Enviroment)]
				public string CategoryName { get; set; }

				/// <summary>
				/// 类型名地址
				/// </summary>
				[Next(Extras = new[] { "CategoryName", "Sku", "Name", "Url" })]
				[Field(Expression = "./div[@class='p-name']/a[1]/@href")]
				public string Url { get; set; }

				/// <summary>
				/// 名称
				/// </summary>
				[Field(Expression = ".//div[@class='p-name']/a/em")]
				public string Name { get; set; }

				/// <summary>
				/// 最小商品标识
				/// </summary>
				[Field(Expression = "./@data-sku")]
				public string Sku { get; set; }
			}

			/// <summary>
			/// 京东商品
			/// </summary>
			[Target(XPaths = new[] { "//span[@class=\"p-num\"]" }, Patterns = new[] { @"&page=[0-9]+&" })]
			[Schema("jd", "jd_product")]
			class JdProduct : IBaseEntity
			{
				/// <summary>
				/// 名称
				/// </summary>
				[Column(Length = 100)]
				[Field(Expression = "Name", Type = SelectorType.Enviroment)]
				public string Name { get; set; }

				/// <summary>
				/// 最小商品标识
				/// </summary>
				[Column(Length = 100)]
				[Field(Expression = "Sku", Type = SelectorType.Enviroment)]
				[Index("SKU")]
				[Unique("SKU")]
				public string Sku { get; set; }

				/// <summary>
				/// 地址
				/// </summary>
				[Column()]
				[Field(Expression = "Url", Type = SelectorType.Enviroment)]
				public string Url { get; set; }

				/// <summary>
				/// 类型名
				/// </summary>
				[Column(Length = 100)]
				[Field(Expression = "CategoryName", Type = SelectorType.Enviroment)]
				public string CategoryName { get; set; }

				/// <summary>
				/// 商铺名
				/// </summary>
				[Column(Length = 100)]
				[Field(Expression = ".//a[@class='name']")]
				public string ShopName { get; set; }

				/// <summary>
				/// logo
				/// </summary>
				[StringFormatter(Format = "http:{0}")]
				[Download]
				[Field(Expression = "//*[@class='brand-logo']/a[1]/img[1]/@src")]
				public string Logo { get; set; }
			}
		}
	}
}