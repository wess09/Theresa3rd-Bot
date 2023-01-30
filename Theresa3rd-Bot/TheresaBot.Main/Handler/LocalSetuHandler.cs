﻿using TheresaBot.Main.Business;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Config;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.LocalSetu;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class LocalSetuHandler : SetuHandler
    {
        private LocalSetuBusiness localSetuBusiness;

        public LocalSetuHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            localSetuBusiness = new LocalSetuBusiness();
        }

        public async Task sendTimingSetuAsync(TimingSetuTimer timingSetuTimer, long groupId)
        {
            string localPath = timingSetuTimer.LocalPath;
            if (string.IsNullOrWhiteSpace(localPath)) throw new Exception("未配置LocalPath");
            List<LocalSetuInfo> setuInfos = localSetuBusiness.loadRandom(localPath, timingSetuTimer.Quantity, timingSetuTimer.FromOneDir);
            if (setuInfos is null || setuInfos.Count == 0) throw new Exception("未能在LocalPath中读取任何涩图");
            string tags = timingSetuTimer.FromOneDir ? setuInfos[0].DirInfo.Name : "";
            await sendTimingSetuMessage(timingSetuTimer, tags, groupId);
            await Task.Delay(2000);
            foreach (LocalSetuInfo setuInfo in setuInfos)
            {
                await sendSetuInfoAsync(timingSetuTimer, setuInfo, groupId);
                await Task.Delay(1000);
            }
        }

        private async Task sendSetuInfoAsync(TimingSetuTimer timingSetuTimer, LocalSetuInfo setuInfo, long groupId)
        {
            try
            {
                List<BaseContent> workMsgs = new List<BaseContent>();
                string template = getSetuInfo(setuInfo, timingSetuTimer.LocalTemplate);
                if (string.IsNullOrWhiteSpace(template) == false) workMsgs.Add(new PlainContent(template));
                List<FileInfo> setuFiles = new List<FileInfo>() { setuInfo.FileInfo };
                await Session.SendGroupSetuAsync(workMsgs, setuFiles, groupId, true);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "定时涩图发送失败");
                Reporter.SendError(ex, "定时涩图发送失败");
            }
        }

        private string getSetuInfo(LocalSetuInfo setuInfo, string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            template = template.Replace("{FileName}", setuInfo.FileInfo.Name);
            template = template.Replace("{FilePath}", $"{setuInfo.DirInfo.Name}/{setuInfo.FileInfo.Name}");
            template = template.Replace("{SizeMB}", MathHelper.getMbWithByte(setuInfo.FileInfo.Length).ToString());
            return template;
        }





    }
}
