using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetSpider.Downloader;

namespace DotnetSpider.Core.Processor.RequestExtractor
{
	public class XPathRequestExtractor : IRequestExtractor
	{
		private readonly IEnumerable<string> _xPaths;

		public XPathRequestExtractor(params string[] xPaths)
		{
			if (xPaths == null || xPaths.Length == 0) throw new SpiderException($"{nameof(xPaths)} should not be empty.");
			_xPaths = xPaths;
		}

		public XPathRequestExtractor(IEnumerable<string> xPaths) : this(xPaths.ToArray())
		{
		}

		public IEnumerable<Request> Extract(Page page)
		{
			var urls = new List<string>();
			foreach (var xpath in _xPaths)
			{
				var links = page.Selectable().XPath(xpath).Links().GetValues();
				foreach (var link in links)
				{
					//hdh@2019/3/12  url合法性过滤 (硬编码)
					if (CheckIsUrlFormat(link))
					{

#if !NETSTANDARD
						urls.Add(System.Web.HttpUtility.HtmlDecode(System.Web.HttpUtility.UrlDecode(link)));
#else
						urls.Add(System.Net.WebUtility.HtmlDecode(System.Net.WebUtility.UrlDecode(link)));
#endif
					}
				}
			}

			return urls.Select(url => new Request(url, page.CopyProperties()));
		}

		internal bool ContainsXpath(string xpath)
		{
			return _xPaths.Contains(xpath);
		}


		/// <summary>
		/// 检测串值是否为合法的网址格式
		/// </summary>
		/// <param name="strValue">要检测的String值</param>
		/// <returns>成功返回true 失败返回false</returns>
		public static bool CheckIsUrlFormat(string strValue)
		{
			return CheckIsFormat(@"htt(p|ps)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", strValue);
		}
		/// <summary>
		/// 检测串值是否为合法的格式
		/// </summary>
		/// <param name="strRegex">正则表达式</param>
		/// <param name="strValue">要检测的String值</param>
		/// <returns>成功返回true 失败返回false</returns>
		public static bool CheckIsFormat(string strRegex, string strValue)
		{
			if (strValue != null && strValue.Trim() != "")
			{
				Regex re = new Regex(strRegex);
				if (re.IsMatch(strValue))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;

		}
	}

}
