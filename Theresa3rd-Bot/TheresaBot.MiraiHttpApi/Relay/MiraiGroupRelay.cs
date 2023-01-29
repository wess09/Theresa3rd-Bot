﻿using Mirai.CSharp.HttpApi.Models.EventArgs;
using TheresaBot.Main.Relay;

namespace TheresaBot.MiraiHttpApi.Relay
{
    public class MiraiGroupRelay : GroupRelay
    {
        public IGroupMessageEventArgs Args { get; set; }

        public MiraiGroupRelay(IGroupMessageEventArgs args, int msgId, string message, long groupId, long memberId) : base(msgId, message, groupId, memberId)
        {
            this.Args = args;
        }

    }
}
