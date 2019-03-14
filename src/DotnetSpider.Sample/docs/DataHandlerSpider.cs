using DotnetSpider.Core;
using DotnetSpider.Core.Processor;
using DotnetSpider.Downloader;
using DotnetSpider.Downloader.AfterDownloadCompleteHandlers;
using DotnetSpider.Extension;
using DotnetSpider.Extension.Model;
using DotnetSpider.Extension.Pipeline;
using DotnetSpider.Extraction;
using DotnetSpider.Extraction.Model;
using DotnetSpider.Extraction.Model.Attribute;
using DotnetSpider.Extraction.Model.Formatter;
using System.Collections.Generic;

namespace DotnetSpider.Sample.docs
{
	/// <summary>
	/// 数据处理
	/// </summary>
	public class DataHandlerSpider
	{
		public static void Run()
		{
			Spider spider = new Spider();
			spider.Run();
		}

		private class MyDataHanlder : IDataHandler
		{
			public void Handle(ref dynamic data, Page page)
			{
				var price = float.Parse(data.price);
				var sold = int.Parse(data.sold);

				//价格区间在[100-5000]之间的,销售量小于1的,不处理
				if (price >= 100 && price < 5000)
				{
					if (sold <= 1)
					{
						if (!page.SkipTargetRequests)
						{
							//跳过当前页的处理
							page.SkipTargetRequests = true;
						}
					}
					else
					{
						return;
					}
				}//价格区间在[0-100]之间的,销售量小于5的,不处理
				else if (price < 100)
				{
					if (sold <= 5)
					{
						if (!page.SkipTargetRequests)
						{
							//跳过当前页处理
							page.SkipTargetRequests = true;
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					//销售量为0的不处理
					if (sold == 0)
					{
						if (!page.SkipTargetRequests)
						{
							//跳过当前页处理
							page.SkipTargetRequests = true;
						}
					}
					else
					{
						return;
					}
				}
			}
		}

		private class Spider : EntitySpider
		{
			private class MyBeforeProcessorHandler : IBeforeProcessorHandler
			{
				public void Handle(ref Page page)
				{
					var pager = page.Selectable().Select(Selectors.JsonPath("$.mods.pager.status")).GetValue();
					if (pager != "show")
					{
						page.SkipTargetRequests = true;
					}
				}
			}

			protected override void OnInit(params string[] arguments)
			{
				AddHeaders("s.taobao.com", new Dictionary<string, object> {
					{ "Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
					{ "Referer", "https://www.taobao.com/"},
					{ "Cache-Control","max-age=0" },
					{ "Upgrade-Insecure-Requests","1" },
					{"UserAgent","Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36" },
				
				});
				
				//淘宝要求登录,否则要传有效的cookies数据
				Downloader.AddCookies("t=58b5ddc7c130e81575fca2c15e85bc61; hng=CN%7Czh-CN%7CCNY%7C156; cna=7b3nFIxO1RsCAd7YEeVOurhL; thw=cn; tg=0; enc=eN2nc%2Bj0ZXM3C6Zxd1yRe7zDugMHgPr8N4yA5wuDLFwiKL5gI0ubDqYevNVhYJdl8mUUOts0btJxZUHq7BWrBg%3D%3D; x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0%26__ll%3D-1%26_ato%3D0; l=bBSBPDXHvwE4d3iGBOCZquI8LPb9vIRYouPRwCcXi_5hK1LsFo7OlPV_wep6Vj5R_T8B4-uriCv9-etk2; cookie2=1321d345208707942ca645704a3b085b; _tb_token_=fbb50bbf98b57; alitrackid=www.taobao.com; lastalitrackid=www.taobao.com; swfstore=186251; _m_h5_tk=5ea0d74100c210df07832abe5d79cd6e_1552118218067; _m_h5_tk_enc=edce944ad13aef5533d3d01f1915524a; _cc_=W5iHLLyFfA%3D%3D; whl=-1%260%260%261552123792252; mt=ci=0_0; v=0; JSESSIONID=AC9C4701634AEFBDACF16939C3D06036; isg=BGFhXgTnmbmVbzU8zL-X4cUScC27pnIS0iY0fcM2XWjHKoH8C17l0I9giB4J5W04",
				"s.taobao.com");
				//下载完成后处理
				Downloader.AddAfterDownloadCompleteHandler(new CutoutHandler("g_page_config = {", "g_srp_loadCss();", 16, 22));
				//Downloader.AddAfterDownloadCompleteHandler(new CutoutHandler("<div class=\"grid g-clearfix\">", "<div class=\"items\" id=\"J_itemlistCont\">", 0, 0));
				AddBeforeProcessor(new MyBeforeProcessorHandler());
				SkipTargetRequestsWhenResultIsEmpty = true;
				//AddRequest(new Request("https://s.taobao.com/search?q=妙可蓝多&imgfile=&js=1&stats_click=search_radio_all%3A1&ie=utf8&sort=sale-desc&s=0&tab=all", new Dictionary<string, dynamic> { { "bidwordstr", "妙可蓝多" } }));
				//淘宝以 &data-value=132 ,每页44条方式累加
				AddRequest(new Request("https://s.taobao.com/search?q=OA&imgfile=&js=1&stats_click=search_radio_all%3A1&ie=utf8&sort=sale-desc&s=0&tab=all", new Dictionary<string, dynamic> { { "bidwordstr", "妙可蓝多" } }));
				AddEntityType<TaobaoItem>(new MyDataHanlder()); //对采集的数据进行干预处理
				AddPipeline(new ConsoleEntityPipeline());
			}
		}

		[Entity(Expression = "$.mods.itemlist.data.auctions[*]", Type = SelectorType.JsonPath)]
		private class TaobaoItem : IBaseEntity
		{

			/// <summary>
			/// 标题
			/// </summary>
			[Field(Expression = "$.raw_title", Type = SelectorType.JsonPath)]
			[ReplaceFormatter(NewValue = "", OldValue = "付款")]
			public string Title { get; set; }

			/// <summary>
			/// 价格
			/// </summary>
			[Field(Expression = "$.view_price", Type = SelectorType.JsonPath)]
			public string price { get; set; }

			/// <summary>
			/// 类型
			/// </summary>
			[Field(Expression = "$.category", Type = SelectorType.JsonPath)]
			public string cat { get; set; }

			/// <summary>
			/// 付款
			/// </summary>
			[Field(Expression = "$.view_sales", Type = SelectorType.JsonPath)]
			[ReplaceFormatter(NewValue = "", OldValue = "付款")]
			[ReplaceFormatter(NewValue = "", OldValue = "收货")]
			[ReplaceFormatter(NewValue = "", OldValue = "人")]
			public string sold { get; set; }

			/// <summary>
			/// 标题编号
			/// </summary>
			[Field(Expression = "$.nid", Type = SelectorType.JsonPath)]
			public string item_id { get; set; }

			/// <summary>
			/// 所在地
			/// </summary>
			[Field(Expression = "$.item_loc", Type = SelectorType.JsonPath)]
			public string item_loc { get; set; }


			/// <summary>
			/// 用户编号
			/// </summary>
			[Field(Expression = "$.user_id", Type = SelectorType.JsonPath)]
			public string user_id { get; set; }


			/// <summary>
			/// 用户编号
			/// </summary>
			[Field(Expression = "$.nick", Type = SelectorType.JsonPath)]
			public string ShopName { get; set; }
		}
	}
}
