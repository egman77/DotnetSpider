using DotnetSpider.Core;
using DotnetSpider.Core.Infrastructure;
using DotnetSpider.Downloader.AfterDownloadCompleteHandlers;
using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using DotnetSpider.Extraction;
using DotnetSpider.Extraction.Model;
using DotnetSpider.Extraction.Model.Attribute;

namespace DotnetSpider.Sample.docs
{
	public class DbRequestBuilderSpider : EntitySpider
	{
		protected override void OnInit(params string[] arguments)
		{
			Downloader.AddAfterDownloadCompleteHandler(new CutoutHandler("json(", ");", 5, 0));
			AddPipeline(new ConsoleEntityPipeline());
			AddRequestBuilder(new DatabaseRequestBuilder(Database.MySql, Env.DataConnectionString,
				"SELECT * FROM test.jd_sku", new[] { "sku" },
				"http://chat1.jd.com/api/checkChat?my=list&pidList={0}&callback=json"));

			AddEntityType<Item>();
		}

		[Schema("test", "jd_sku", TableNamePostfix.Monday)]
		[Entity(Expression = "$.[*]", Type = SelectorType.JsonPath)]
		class Item : IBaseEntity
		{
			/// <summary>
			/// 最小库存单位
			/// </summary>
			[Column]
			[Unique]
			[Field(Expression = "$.pid", Type = SelectorType.JsonPath)]
			public string Sku { get; set; }

			/// <summary>
			/// 商店编号
			/// </summary>
			[Column]
			[Update]
			[Field(Expression = "$.shopId", Type = SelectorType.JsonPath)]
			public int ShopId { get; set; }
		}
	}
}
