﻿using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Invoker;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class CommandHelper
    {
        public static async Task ReplyProcessingMessageAsync(this GroupCommand command, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return;
            await command.ReplyGroupTemplateWithAtAsync(template);
        }

        public static async Task<BaseResult[]> ReplyGroupSetuAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = true)
        {
            BaseResult[] results = await command.ReplyAndRevokeAsync(setuContent, revokeInterval, sendImgBehind, isAt);
            if (results.Any(o => o.IsFailed) && BotConfig.PixivConfig.ImgResend != ResendType.None) //发送失败后重发
            {
                await Task.Delay(1000);
                SetuContent resendContent = setuContent.ToResendContent(BotConfig.PixivConfig.ImgResend);
                results = await command.ReplyAndRevokeAsync(resendContent, revokeInterval, sendImgBehind, isAt);
            }
            return results;
        }

        public static async Task<BaseResult> ReplyGroupSetuAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval, bool isAt = true)
        {
            BaseResult results = await command.ReplyAndRevokeAsync(setuContents, revokeInterval, isAt);
            if (results.IsFailed && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = setuContents.ToResendContent(BotConfig.PixivConfig.ImgResend);
                results = await command.ReplyAndRevokeAsync(resendContents, revokeInterval, isAt);
            }
            return results;
        }

        public static async Task<BaseResult> ReplyAndRevokeAsync(this GroupCommand command, List<BaseContent> contentList, int revokeInterval, bool isAt = false)
        {
            BaseResult results = await command.ReplyGroupMessageAsync(contentList, isAt);
            Task revokeTask = command.RevokeGroupMessageAsync(results.MsgId, command.GroupId, revokeInterval);
            return results;
        }

        private static async Task<BaseResult[]> ReplyAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false)
        {
            List<BaseResult> results = new List<BaseResult>();
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();

            if (sendImgBehind)
            {
                BaseResult workMsgResult = await command.ReplyGroupMessageAsync(msgContents, isAt);
                await Task.Delay(1000);
                BaseResult imgMsgResult = await command.ReplyGroupMessageAsync(imgContents, false);
                results.Add(workMsgResult);
                results.Add(imgMsgResult);
            }
            else
            {
                List<BaseContent> contentList = msgContents.Concat(imgContents).ToList();
                BaseResult msgResult = await command.ReplyGroupMessageAsync(contentList, isAt);
                results.Add(msgResult);
            }

            if (revokeInterval > 0)
            {
                long[] msgIds = results.Select(o => o.MsgId).ToArray();
                Task revokeTask = command.RevokeGroupMessageAsync(msgIds, command.GroupId, revokeInterval);
            }

            return results.ToArray();
        }

        private static async Task<BaseResult> ReplyAndRevokeAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval, bool isAt = false)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent().SetDefaultImage();
            BaseResult result = await command.ReplyGroupMessageAsync(contentList, isAt);
            Task revokeTask = command.RevokeGroupMessageAsync(result.MsgId, command.GroupId, revokeInterval);
            return result;
        }

        public static async Task<BaseResult> SendTempSetuAsync(this GroupCommand command, List<SetuContent> setuContents)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent().SetDefaultImage();
            return await command.SendTempMessageAsync(contentList);
        }

        public static async Task<BaseResult[]> SendTempSetuAsync(this GroupCommand command, SetuContent setuContent, bool sendImgBehind)
        {
            List<BaseResult> results = new List<BaseResult>();
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();

            if (sendImgBehind)
            {
                results.Add(await command.SendTempMessageAsync(msgContents));
                await Task.Delay(1000);
                results.Add(await command.SendTempMessageAsync(imgContents));
            }
            else
            {
                List<BaseContent> contentList = msgContents.Concat(imgContents).ToList();
                results.Add(await command.SendTempMessageAsync(contentList));
            }

            return results.ToArray();
        }

        public static async Task RevokeGroupMessageAsync(this GroupCommand command, long[] msgIds, long groupId, int revokeInterval = 0)
        {
            if (revokeInterval <= 0) return;
            foreach (long msgId in msgIds)
            {
                if (msgId == 0) continue;
                Task revokeTask = command.RevokeGroupMessageAsync(msgId, groupId, revokeInterval);
                await Task.Delay(1000);
            }
        }

        public static async Task RevokeGroupMessageAsync(this GroupCommand command, long msgId, long groupId, int revokeInterval = 0)
        {
            try
            {
                if (msgId == 0) return;
                if (revokeInterval <= 0) return;
                await Task.Delay(revokeInterval * 1000);
                await command.RevokeGroupMessageAsync(msgId, groupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        public static string GetSimilarGroupCommandStrs(string keyword)
        {
            List<string> groupCommands = GetSimilarGroupCommands(keyword);
            if (groupCommands.Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (string command in groupCommands)
            {
                if (builder.Length > 0) builder.Append('/');
                builder.Append($"{BotConfig.GeneralConfig.DefaultPrefix}{command}");
            }
            return builder.ToString();
        }

        public static List<string> GetSimilarGroupCommands(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new();
            string splitString = keyword.ToLower().Replace(" ", string.Empty).Trim();
            List<char> charList = splitString.ToList().Distinct().ToList();
            List<string> similarCommands = new List<string>();
            List<string> groupCommands = GetGroupCommands();
            foreach (char c in charList)
            {
                foreach (string command in groupCommands)
                {
                    if (command.Contains(c)) similarCommands.Add(command);
                }
            }
            return similarCommands.Distinct().ToList();
        }

        public static List<string> GetGroupCommands()
        {
            List<string> returnList = new List<string>();
            foreach (var item in HandlerInvokers.GroupCommands)
            {
                returnList.AddRange(item.Commands);
            }
            return returnList.Distinct().ToList();
        }



    }
}
