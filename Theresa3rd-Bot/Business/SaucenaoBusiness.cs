﻿using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Exceptions;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.Saucenao;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class SaucenaoBusiness
    {
        public async Task<SaucenaoResult> getSaucenaoResultAsync(string imgHttpUrl)
        {
            DateTime startTime = DateTime.Now;
            string saucenaoHtml = await SearchResultAsync(imgHttpUrl);

            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument document = await htmlParser.ParseDocumentAsync(saucenaoHtml);
            IEnumerable<IElement> domList = document.All.Where(m => m.ClassList.Contains("result"));

            List<SaucenaoItem> itemList = new List<SaucenaoItem>();
            if (domList == null || domList.Count() == 0) return new SaucenaoResult(itemList, startTime, 0);
            foreach (IElement resultElement in domList)
            {
                IHtmlCollection<IElement> contentList = resultElement.GetElementsByClassName("resulttablecontent");
                if (contentList == null || contentList.Length == 0) continue;

                IHtmlCollection<IElement> linkifyList = contentList.First().GetElementsByTagName("a");
                if (linkifyList == null || linkifyList.Length == 0) continue;

                decimal similarity = 0;
                IHtmlCollection<IElement> similarityList = resultElement.GetElementsByClassName("resultsimilarityinfo");
                string similarityStr = similarityList != null && similarityList.Length > 0 ? similarityList.First()?.InnerHtml : "00.00";
                if (string.IsNullOrWhiteSpace(similarityStr)) similarityStr = "00.00";
                similarityStr = similarityStr.Replace("%", "");
                decimal.TryParse(similarityStr, out similarity);
                if (similarity > 0 && similarity < BotConfig.SaucenaoConfig.MinSimilarity) continue;

                foreach (IElement linkifyElement in linkifyList)
                {
                    SaucenaoItem saucenaoItem = getSaucenaoItem(linkifyElement, similarity);
                    if (saucenaoItem == null) continue;
                    if (itemList.Where(o => o.SourceUrl == saucenaoItem.SourceUrl).Any()) continue;
                    itemList.Add(saucenaoItem);
                }
            }
            return new SaucenaoResult(itemList, startTime, domList.Count());
        }


        public SaucenaoItem getSaucenaoItem(IElement linkElement, decimal similarity)
        {
            string href = linkElement.GetAttribute("href");
            if (string.IsNullOrWhiteSpace(href)) return null;

            href = href.Trim();
            string hrefLower = href.ToLower();
            Dictionary<string, string> paramDic = StringHelper.splitHttpUrl(href);

            //https://www.pixiv.net/member_illust.php?mode=medium&illust_id=73572009
            if (hrefLower.Contains("www.pixiv.net/member_illust"))
            {
                string illustId = paramDic["illust_id"].Trim();
                return new SaucenaoItem(SaucenaoSourceType.Pixiv, href, illustId, similarity);
            }

            //https://www.pixiv.net/fanbox/creator/705370
            if (hrefLower.Contains("www.pixiv.net/fanbox"))
            {
                return new SaucenaoItem(SaucenaoSourceType.FanBox, href, "", similarity);
            }

            //https://yande.re/post/show/523988
            if (hrefLower.Contains("yande.re"))
            {
                return new SaucenaoItem(SaucenaoSourceType.Yande, href, "", similarity);
            }

            //https://twitter.com/i/web/status/1007548268048416769
            if (hrefLower.Contains("twitter.com"))
            {
                return new SaucenaoItem(SaucenaoSourceType.Twitter, href, "", similarity);
            }

            //https://danbooru.donmai.us/post/show/3438512
            if (hrefLower.Contains("danbooru.donmai.us"))
            {
                return new SaucenaoItem(SaucenaoSourceType.Danbooru, href, "", similarity);
            }

            //https://gelbooru.com/index.php?page=post&s=view&id=4639560
            if (hrefLower.Contains("gelbooru.com"))
            {
                return new SaucenaoItem(SaucenaoSourceType.Gelbooru, href, "", similarity);
            }

            //https://konachan.com/post/show/279886
            if (hrefLower.Contains("konachan.com"))
            {
                return new SaucenaoItem(SaucenaoSourceType.Konachan, href, "", similarity);
            }

            //https://anime-pictures.net/pictures/view_post/602645
            if (hrefLower.Contains("anime-pictures.net"))
            {
                return new SaucenaoItem(SaucenaoSourceType.AnimePictures, href, "", similarity);
            }

            //https://anidb.net/anime/4427
            //https://bcy.net/illust/detail/120185/1519032
            //https://mangadex.org/chapter/07c706d9-e575-4498-b504-dd85014c555b
            //https://www.mangaupdates.com/series.html?id=13582
            //https://myanimelist.net/manga/5255/
            //https://www.imdb.com/title/tt2402101/

            return null;
        }

        public async Task<SaucenaoItem> getBestMatchAsync(SaucenaoResult saucenaoResult)
        {
            if (saucenaoResult.Items.Count == 0) return null;
            List<SaucenaoItem> sortList = sortSaucenaoItem(saucenaoResult.Items);
            for (int i = 0; i < sortList.Count; i++)
            {
                try
                {
                    SaucenaoItem saucenaoItem = sortList[i];
                    if (saucenaoItem.SourceType == SaucenaoSourceType.Pixiv)
                    {
                        PixivWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(saucenaoItem.SourceId);
                        if (pixivWorkInfo == null || pixivWorkInfo.error == true) continue;
                        if (pixivWorkInfo.body.IsImproper()) continue;
                        saucenaoItem.PixivWorkInfo = pixivWorkInfo;
                        return saucenaoItem;
                    }
                    if (saucenaoItem.SourceType == SaucenaoSourceType.Twitter)
                    {
                        //ToDo
                        return saucenaoItem;
                    }
                    if (saucenaoItem.SourceType == SaucenaoSourceType.FanBox)
                    {
                        //ToDo
                        return saucenaoItem;
                    }
                    return saucenaoItem;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "尝试获取原图信息时异常", false);
                }
            }
            return null;
        }

        /// <summary>
        /// 对搜索结果进行优先级排序
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private List<SaucenaoItem> sortSaucenaoItem(List<SaucenaoItem> itemList)
        {
            if (itemList == null) return new List<SaucenaoItem>();
            List<SaucenaoItem> sortList = new List<SaucenaoItem>();
            sortList.AddRange(itemList.Where(o => o.SourceType == SaucenaoSourceType.Pixiv && o.Similarity >= 80).OrderByDescending(o => o.Similarity).ToList());
            sortList.AddRange(itemList);
            return sortList.Distinct().ToList();
        }

        /// <summary>
        /// 请求saucenao,获取返回结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        /// <exception cref="BaseException"></exception>
        private async static Task<string> SearchResultAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>() { { "url", imgHttpUrl } };
            HttpResponseMessage response = await HttpHelper.PostFormForHtml(HttpUrl.SaucenaoUrl, paramDic);
            await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string contentString = await response.GetContentStringAsync();
                throw new BaseException($"saucenao返回Code：{(int)response.StatusCode}，Content：{contentString.cutString(200)}");
            }
            return await response.Content.ReadAsStringAsync();
        }

    }
}
