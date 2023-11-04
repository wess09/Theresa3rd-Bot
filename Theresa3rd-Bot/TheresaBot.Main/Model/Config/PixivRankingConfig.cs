﻿using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Config
{
    public record PixivRankingConfig : BasePluginConfig
    {
        public string ProcessingMsg { get; set; }
        public string Template { get; set; }
        public int MaxScan { get; set; }
        public int PreviewInPage { get; set; }
        public PixivRankingSortType SortType { get; set; } = PixivRankingSortType.RankingRate;
        public int GroupCD { get; set; }
        public int CacheSeconds { get; set; }
        public int SendDetail { get; set; }
        public PixivRankingR18Item Daily { get; set; }
        public PixivRankingR18Item DailyAI { get; set; }
        public PixivRankingR18Item Male { get; set; }
        public PixivRankingR18Item Weekly { get; set; }
        public PixivRankingSafeItem Monthly { get; set; }
        public List<PixivRankingTimer> Subscribes { get; set; } = new();

        public override PixivRankingConfig FormatConfig()
        {
            if (MaxScan < 0) MaxScan = 0;
            if (MaxScan > 1000) MaxScan = 1000;
            if (PreviewInPage <= 0) PreviewInPage = 25;
            if (PreviewInPage > 50) PreviewInPage = 50;
            if (SendDetail < 0) SendDetail = 0;
            if (Subscribes is null) Subscribes = new();
            if (Daily is not null) Daily.FormatConfig();
            if (DailyAI is not null) DailyAI.FormatConfig();
            if (Male is not null) Male.FormatConfig();
            if (Weekly is not null) Weekly.FormatConfig();
            if (Monthly is not null) Monthly.FormatConfig();
            foreach (var item in Subscribes) item?.FormatConfig();
            return this;
        }
    }

    public record PixivRankingSafeItem : BaseConfig
    {
        public bool Enable { get; set; }
        public List<string> Commands { get; set; } = new();
        public int MinRatingCount { get; set; }
        public double MinRatingRate { get; set; }
        public int MinBookCount { get; set; }
        public double MinBookRate { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (MinRatingCount < 0) MinRatingCount = 0;
            if (MinRatingRate < 0) MinRatingRate = 0;
            if (MinBookCount < 0) MinBookCount = 0;
            if (MinBookRate < 0) MinBookRate = 0;
            if (Commands is null) Commands = new();
            return this;
        }
    }

    public record PixivRankingR18Item : PixivRankingSafeItem
    {
        public List<string> R18Commands { get; set; } = new();

        public override BaseConfig FormatConfig()
        {
            base.FormatConfig();
            if (R18Commands is null) R18Commands = new();
            return this;
        }
    }

    public record PixivRankingTimer : BaseConfig
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public List<long> Groups { get; set; } = new();
        public List<string> Contents { get; set; } = new();
        public string Cron { get; set; }
        public int SendDetail { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (SendDetail < 0) SendDetail = 0;
            if (Groups is null) Groups = new();
            if (Contents is null) Contents = new();
            return this;
        }
    }

}
