﻿using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Handler;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Invoker
{
    public static class HandlerInvokers
    {
        public readonly static List<CommandHandler<GroupCommand>> GroupCommands = new()
        {
            //菜单
            new(BotConfig.MenuConfig?.Commands, CommandType.Menu, new(async (botCommand,session) =>
            {
                await new MenuHandler(session).sendMenuAsync(botCommand);
                return true;
            })),

            //拉黑成员
            new(BotConfig.ManageConfig?.DisableMemberCommands, CommandType.BanMember, new(async (botCommand,session) =>
            {
                BanWordHandler handler = new BanWordHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //解禁成员
            new(BotConfig.ManageConfig?.EnableMemberCommands, CommandType.BanMember, new(async (botCommand,session) =>
            {
                BanWordHandler handler = new BanWordHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableMemberAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.AddCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv关注画师列表
            new(BotConfig.SubscribeConfig?.PixivUser?.SyncCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeFollowUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订pixiv画师
            new(BotConfig.SubscribeConfig?.PixivUser?.RmCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivUser) == false) return false;
                await handler.cancleSubscribeUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.AddCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                await handler.subscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订pixiv标签
            new(BotConfig.SubscribeConfig?.PixivTag?.RmCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.PixivTag) == false) return false;
                await handler.cancleSubscribeTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //订阅米游社用户
            new(BotConfig.SubscribeConfig?.Mihoyo?.AddCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                MYSHandler handler = new MYSHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.Mihoyo) == false) return false;
                await handler.subscribeMYSUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //退订米游社用户
            new(BotConfig.SubscribeConfig?.Mihoyo?.RmCommands, CommandType.Subscribe, new(async (botCommand,session) =>
            {
                MYSHandler handler = new MYSHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                if (await handler.CheckSubscribeEnableAsync(botCommand,BotConfig.SubscribeConfig?.Mihoyo) == false) return false;
                await handler.cancleSubscribeMysUserAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //禁止色图标签
            new(BotConfig.ManageConfig?.DisableTagCommands, CommandType.BanSetuTag, new(async (botCommand,session) =>
            {
                BanWordHandler handler = new BanWordHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.disableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //解禁色图标签
            new(BotConfig.ManageConfig?.EnableTagCommands, CommandType.BanSetuTag, new(async (botCommand,session) =>
            {
                BanWordHandler handler = new BanWordHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.enableSetuTagAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Pixiv
            new(BotConfig.SetuConfig?.Pixiv?.Commands, CommandType.Setu, new(async (botCommand,session) =>
            {
                PixivHandler handler = new PixivHandler(session);
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Pixiv) == false) return false;
                if (await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.pixivSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Lolicon
            new(BotConfig.SetuConfig?.Lolicon?.Commands, CommandType.Setu, new(async (botCommand,session) =>
            {
                LoliconHandler handler=new LoliconHandler(session);
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Lolicon) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.loliconSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Lolisuki
            new(BotConfig.SetuConfig?.Lolisuki?.Commands, CommandType.Setu, new(async (botCommand,session) =>
            {
                LolisukiHandler handler=new LolisukiHandler(session);
                if (await handler.CheckSetuEnableAsync(botCommand,BotConfig.SetuConfig?.Lolisuki) == false) return false;
                if (await handler.CheckMemberSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckGroupSetuCoolingAsync(botCommand)) return false;
                if (await handler.CheckSetuUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                CoolingCache.SetGroupSetuCooling(botCommand.GroupId, botCommand.MemberId);
                await handler.lolisukiSearchAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //Saucenao
            new(BotConfig.SaucenaoConfig?.Commands, CommandType.Saucenao, new(async (botCommand,session) =>
            {
                SaucenaoHandler handler=new SaucenaoHandler(session);
                if (await handler.CheckSaucenaoEnableAsync(botCommand) == false) return false;
                if (BotConfig.SaucenaoConfig.PullOrigin && await handler.CheckPixivCookieAvailableAsync(botCommand) == false) return false;
                if (await handler.CheckMemberSaucenaoCoolingAsync(botCommand)) return false;
                if (await handler.CheckSaucenaoUseUpAsync(botCommand)) return false;
                if (await handler.CheckHandingAsync(botCommand)) return false;
                await handler.searchResult(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //version
            new(new() { "版本", "version" }, CommandType.Version, new(async (botCommand,session) =>
            {
                await botCommand.ReplyGroupMessageWithAtAsync($"Theresa3rd-Bot：Version：{BotConfig.BotVersion}");
                return false;
            }))
        };

        public readonly static List<CommandHandler<FriendCommand>> FriendCommands = new()
        {
            //PixivCookie
            new(BotConfig.ManageConfig?.PixivCookieCommands, CommandType.SetCookie, new(async (botCommand,session) =>
            {
                CookieHandler handler=new CookieHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdatePixivCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            })),

            //SaucenaoCookie
            new(BotConfig.ManageConfig?.SaucenaoCookieCommands, CommandType.BanSetuTag, new(async (botCommand,session) =>
            {
                CookieHandler handler=new CookieHandler(session);
                if (await handler.CheckSuperManagersAsync(botCommand) == false) return false;
                await handler.UpdateSaucenaoCookieAsync(botCommand);
                await handler.addRecord(botCommand);
                return true;
            }))
        };



    }
}
