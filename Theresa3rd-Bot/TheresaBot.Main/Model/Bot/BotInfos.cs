﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Bot
{
    public class BotInfos
    {
        public long QQ { get; init; }
        public string NickName { get; init; }
        public BotInfos(long qq, string nickName)
        {
            QQ = qq;
            NickName = nickName;
        }
    }
}
