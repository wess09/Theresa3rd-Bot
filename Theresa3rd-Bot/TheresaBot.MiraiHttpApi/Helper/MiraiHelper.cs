﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Event;

namespace TheresaBot.MiraiHttpApi.Helper
{
    public static class MiraiHelper
    {
        public static IServiceProvider Services;

        public static IServiceScope Scope;

        public static IMiraiHttpSession Session;

        public static async Task ConnectMirai()
        {
            try
            {
                LogHelper.Info("尝试连接到mirai-console...");
                Services = new ServiceCollection().AddMiraiBaseFramework()
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework()
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>()
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddHandler<GroupMemberJoinedEvent>()
                                                               .AddHandler<DisconnectedEvent>()
                                                               .AddClient<MiraiHttpSession>()
                                                               .Services
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = MiraiConfig.MiraiHost;
                                                                   options.Port = MiraiConfig.MiraiPort;
                                                                   options.AuthKey = MiraiConfig.MiraiAuthKey;
                                                                   options.SuppressAwaitMessageInvoker = true;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
                Scope = Services.CreateAsyncScope();
                Services = Scope.ServiceProvider;
                Session = Services.GetRequiredService<IMiraiHttpSession>();
                await Session.ConnectAsync(MiraiConfig.MiraiBotQQ);
                LogHelper.Info("已成功连接到mirai-console...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "连接到mirai-console失败");
                throw;
            }
        }

        /// <summary>
        /// 加载MiraiHttpApi配置
        /// </summary>
        public static void LoadMiraiConfig(IConfiguration configuration)
        {
            MiraiConfig.ConnectionString = configuration["Database:ConnectionString"];
            MiraiConfig.MiraiHost = configuration["Mirai:host"];
            MiraiConfig.MiraiPort = Convert.ToInt32(configuration["Mirai:port"]);
            MiraiConfig.MiraiAuthKey = configuration["Mirai:authKey"];
            MiraiConfig.MiraiBotQQ = Convert.ToInt64(configuration["Mirai:botQQ"]);
        }

        /// <summary>
        /// 获取机器人信息
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBotProfileAsync()
        {
            try
            {
                IBotProfile profile = await MiraiHelper.Session.GetBotProfileAsync();
                MiraiConfig.MiraiBotName = profile?.Nickname ?? "Bot";
                LogHelper.Info($"Bot名片获取完毕，QQNumber={Session.QQNumber}，Nickname={profile?.Nickname ?? ""}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Bot名片获取失败");
            }
        }

        public static async Task SendStartUpMessageAsync()
        {
            await Task.Delay(3000);
            List<IChatMessage> msgList = new List<IChatMessage>();
            StringBuilder msgBuilder=new StringBuilder();
            msgBuilder.AppendLine($"欢迎使用【Theresa3rd-Bot {BotConfig.BotVersion}】");
            msgBuilder.AppendLine($"群聊发送【#菜单】可以查看指令");
            msgBuilder.AppendLine($"部署或者使用教程请访问");
            msgBuilder.Append($"{BotConfig.BotHomepage}");
            IChatMessage welcomeMessage = new PlainMessage(msgBuilder.ToString());
            foreach (var memberId in BotConfig.PermissionsConfig.SuperManagers)
            {
                try
                {
                    await Session.SendFriendMessageAsync(memberId, welcomeMessage);
                }
                catch (Exception)
                {
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        public static async Task ReplyRelevantCommandsAsync(string instruction, long groupId, long memberId)
        {
            try
            {
                String similarCommands = CommandHelper.GetSimilarGroupCommandStrs(instruction);
                if (string.IsNullOrWhiteSpace(similarCommands)) return;
                List<IChatMessage> msgList = new List<IChatMessage>();
                msgList.Add(new AtMessage(memberId));
                msgList.Add(new PlainMessage($"不存在的指令，你想要输入的指令是不是【{similarCommands}】?"));
                await Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, long groupId, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, groupId, memberId) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, string command, long groupId, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.StartsWith(command) == false) return null;
            return new(handler, session, args, instruction, command, groupId, memberId);
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, memberId) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, string command, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.StartsWith(command) == false) return null;
            return new(handler, session, args, instruction, command, memberId);
        }

        /// <summary>
        /// 获取消息id
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetMessageId(this IGroupMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        /// <summary>
        /// 获取消息id
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetMessageId(this IFriendMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        /// <summary>
        /// 将通用消息转为Mirai消息
        /// </summary>
        /// <param name="chatContents"></param>
        /// <returns></returns>
        public static async Task<IChatMessage[]> ToMiraiMessageAsync(this List<BaseContent> chatContents)
        {
            List<IChatMessage> chatList = new List<IChatMessage>();
            foreach (BaseContent content in chatContents)
            {
                IChatMessage chatMessage = await content.ToMiraiMessageAsync();
                if (chatMessage is not null) chatList.Add(chatMessage);
            }
            return chatList.ToArray();
        }

        /// <summary>
        /// 将通用消息转为Mirai消息
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public static async Task<IChatMessage> ToMiraiMessageAsync(this BaseContent chatContent)
        {
            if (chatContent is PlainContent plainContent)
            {
                return string.IsNullOrEmpty(plainContent.Content) ? null : new PlainMessage(plainContent.Content);
            }
            if (chatContent is LocalImageContent localImageContent)
            {
                return await UploadPictureAsync(localImageContent);
            }
            if (chatContent is WebImageContent webImageContent)
            {
                return new ImageMessage(null, webImageContent.Url, null);
            }
            return null;
        }

        /// <summary>
        /// 上传图片,返回mirai图片消息
        /// </summary>
        /// <param name="setuFiles"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static async Task<List<IChatMessage>> UploadPictureAsync(this List<FileInfo> setuFiles, UploadTarget target)
        {
            List<IChatMessage> imgMsgs = new List<IChatMessage>();
            foreach (FileInfo setuFile in setuFiles)
            {
                if(setuFile is not null)
                {
                    imgMsgs.Add((IChatMessage)await Session.UploadPictureAsync(target, setuFile.FullName));
                    continue;
                }
                FileInfo errorImg = FilePath.GetDownErrorImg();
                if(errorImg is not null)
                {
                    imgMsgs.Add((IChatMessage)await Session.UploadPictureAsync(target, errorImg.FullName));
                }
            }
            return imgMsgs;
        }

        /// <summary>
        /// 上传图片,返回mirai图片消息
        /// </summary>
        /// <param name="imageContent"></param>
        /// <returns></returns>
        private static async Task<IChatMessage> UploadPictureAsync(LocalImageContent imageContent)
        {
            if (imageContent?.FileInfo == null) return null;
            return imageContent.SendTarget switch
            {
                SendTarget.Group => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Group, imageContent.FileInfo.FullName),
                SendTarget.Friend => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Friend, imageContent.FileInfo.FullName),
                SendTarget.Temp => (IImageMessage)await Session.UploadPictureAsync(UploadTarget.Temp, imageContent.FileInfo.FullName),
                _ => null
            };
        }

    }
}