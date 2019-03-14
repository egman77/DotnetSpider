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
				"SELECT * FROM jd.jd_sku", new[] { "Sku" },
				"http://chat1.jd.com/api/checkChat?my=list&pidList={0}&callback=json"));
			//https://search.jd.com/Search?keyword=%E5%8D%B8%E5%A6%86%E6%B0%B4&enc=utf-8&spm=a.0.0&pvid=cbacf833896844babc3f51f659d5a4af
			//https://chat1.jd.com/api/checkChat?pid={0}&returnCharset=utf-8
			//pid= J_AD_1145779
			//https://chat1.jd.com/api/checkChat?pid=J_AD_1145779&returnCharset=utf-8
			AddEntityType<Item>();
		}

		[Schema("jd", "jd_sku", TableNamePostfix.Monday)]
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
