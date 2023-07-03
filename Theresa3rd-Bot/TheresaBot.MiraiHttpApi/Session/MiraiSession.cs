﻿using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Result;

namespace TheresaBot.MiraiHttpApi.Session
{
    public class MiraiSession : BaseSession
    {
        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, string message)
        {
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return MiraiResult.Undo;
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync(UploadTarget.Group);
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Group);
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAtAll) msgList.Add(new AtAllMessage());
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAtAll) msgList.Add(new AtAllMessage());
            if (atMembers is not null)
            {
                foreach (long memberId in atMembers) msgList.Add(new AtMessage(memberId));
            }
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            if (contentLists.Length == 0) return MiraiResult.Undo;
            List<IForwardMessageNode> nodeList = new List<IForwardMessageNode>();
            foreach (var contentList in contentLists)
            {
                nodeList.Add(new ForwardMessageNode(MiraiConfig.BotName, MiraiConfig.BotQQ, DateTime.Now, await contentList.ToMiraiMessageAsync(UploadTarget.Group)));
            }
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, new ForwardMessage(nodeList.ToArray()));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, string message)
        {
            var msgId = await MiraiHelper.Session.SendFriendMessageAsync(memberId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return MiraiResult.Undo;
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync(UploadTarget.Group);
            var msgId = await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Group);
            var msgId = await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
            return new MiraiResult(msgId);
        }

    }
}
