﻿using TheresaBot.Main.Model.Type;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Mode
{
    public class PixivRankingMode
    {
        public PixivRankingType Type { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }

        public PixivRankingMode(PixivRankingType type, string code, string name)
        {
            this.Type = type;
            this.Code = code;
            this.Name = name;
        }

        public static readonly PixivRankingMode Daily = new(PixivRankingType.Daily, "daily", "日榜");
        public static readonly PixivRankingMode Daily_R18 = new(PixivRankingType.Daily, "daily_r18", "R18日榜");
        public static readonly PixivRankingMode DailyAI = new(PixivRankingType.DailyAI, "daily_ai", "AI日榜");
        public static readonly PixivRankingMode DailyAI_R18 = new(PixivRankingType.DailyAI, "daily_r18_ai", "R18AI日榜");
        public static readonly PixivRankingMode Male = new(PixivRankingType.Male, "male", "男性向日榜");
        public static readonly PixivRankingMode Male_R18 = new(PixivRankingType.Male,"male_r18", "R18男性向日榜");
        public static readonly PixivRankingMode Weekly = new(PixivRankingType.Weekly, "weekly", "周榜");
        public static readonly PixivRankingMode Weekly_R18 = new(PixivRankingType.Weekly, "weekly_r18", "R18周榜");
        public static readonly PixivRankingMode Monthly = new(PixivRankingType.Monthly, "monthly", "月榜");

    }
}