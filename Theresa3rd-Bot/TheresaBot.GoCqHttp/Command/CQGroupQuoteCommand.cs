﻿using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupQuoteCommand : GroupQuoteCommand
    {
        private ICqActionSession Session;
        private CqGroupMessagePostContext Args;
        private CQGroupCommand BaseCommand;
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public CQGroupQuoteCommand(CommandHandler<GroupQuoteCommand> invoker, ICqActionSession session, CqGroupMessagePostContext args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.MessageId, instruction, command, groupId, memberId)
        {
            Args = args;
            Session = session;
            BaseCommand = new(session, args, invoker.CommandType, instruction, command, groupId, memberId);
        }

        public override List<string> GetImageUrls() => BaseCommand.GetImageUrls();

        public override long GetQuoteMessageId() => BaseCommand.GetQuoteMessageId();

        public override Task<BaseResult> ReplyGroupMessageAsync(string message, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(message, isAt);

        public override Task<BaseResult> ReplyGroupMessageAsync(List<BaseContent> contentList, bool isAt = false) => BaseCommand.ReplyGroupMessageAsync(contentList, isAt);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(string plainMsg) => BaseCommand.ReplyGroupMessageWithAtAsync(plainMsg);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(params BaseContent[] contentArr) => BaseCommand.ReplyGroupMessageWithAtAsync(contentArr);

        public override Task<BaseResult> ReplyGroupMessageWithAtAsync(List<BaseContent> contentList) => BaseCommand.ReplyGroupMessageWithAtAsync(contentList);

        public override Task<BaseResult> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "") => BaseCommand.ReplyGroupTemplateWithAtAsync(template, defaultmsg);

        public override Task RevokeGroupMessageAsync(long msgId, long groupId) => BaseCommand.RevokeGroupMessageAsync(msgId, groupId);

        public override Task<BaseResult> SendTempMessageAsync(List<BaseContent> contentList) => BaseCommand.SendTempMessageAsync(contentList);

    }
}
