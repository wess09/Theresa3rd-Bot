﻿using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;

namespace TheresaBot.Main.Helper
{
    public static class EnumHelper
    {
        public static Dictionary<int, string> PixivSyncOptions => new()
        {
            {(int)PixivSyncType.Merge, "合并（只添加不存在的订阅，保留在不关注列表中的订阅）"},
            {(int)PixivSyncType.Overwrite, "覆盖（移除所有原有的订阅，并将关注列表重新添加到订阅中）"},
        };

        public static Dictionary<int, string> GroupPushOptions => new()
        {
            {(int)GroupPushType.AllGroup, "所有拥有订阅权限的群"},
            {(int)GroupPushType.CurrentGroup, "当前群"},
        };

        public static Dictionary<int, string> TagMatchOptions => new()
        {
            {(int)TagMatchType.Contain, "部分一致"},
            {(int)TagMatchType.FullMatch, "全词匹配"},
            {(int)TagMatchType.Regular, "正则匹配"}
        };

        public static Dictionary<int, string> ResendOptions => new()
        {
            {(int)ResendType.None, "不重发"},
            {(int)ResendType.WithoutImg, "不带图片重发"},
            {(int)ResendType.Rotate180, "旋转180度重发"},
            {(int)ResendType.Blur, "高斯模糊后重发"}
        };

        public static Dictionary<int, string> PixivRandomOptions => new()
        {
            {(int)PixivRandomType.RandomTag, "随机指定标签中的作品"},
            {(int)PixivRandomType.RandomSubscribe, "随机订阅中的作品"},
            {(int)PixivRandomType.RandomFollow, "随机关注画师的作品"},
            {(int)PixivRandomType.RandomBookmark, "随机收藏中的作品"},
        };

        public static string GetTypeName(this TimingSetuSourceType sourceType)
        {
            return sourceType switch
            {
                TimingSetuSourceType.Local => "本地图库",
                TimingSetuSourceType.Lolicon => "Lolicon Api",
                TimingSetuSourceType.Lolisuki => "Lolisuki Api",
                _ => string.Empty
            };
        }

        public static List<OptionVo> ToOptionList(this Dictionary<int, string> dic)
        {
            return dic.Select(o => new OptionVo(o.Key, o.Value)).ToList();
        }

    }
}
