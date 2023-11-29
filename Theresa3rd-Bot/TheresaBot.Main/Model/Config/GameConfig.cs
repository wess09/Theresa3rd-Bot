﻿namespace TheresaBot.Main.Model.Config
{
    public record GameConfig : BasePluginConfig
    {
        public List<string> JoinCommands { get; set; } = new();

        public List<string> StopCommands { get; set; } = new();

        public UndercoverConfig Undercover { get; set; }

        public override GameConfig FormatConfig()
        {
            if (JoinCommands is null) JoinCommands = new();
            if (StopCommands is null) StopCommands = new();
            Undercover?.FormatConfig();
            return this;
        }
    }

    public record UndercoverConfig : BaseGameConfig
    {
        public List<string> CreateCommands { get; set; } = new();

        public List<string> AddWordCommands { get; set; } = new();

        public List<string> SendWordCommands { get; set; } = new();

        public bool SendIdentity { get; set; }

        public int FirstRoundNonVoting { get; set; } = 3;

        public decimal MaxSimilarity { get; set; }

        public int MatchSeconds { get; set; } = 120;

        public int PrepareSeconds { get; set; } = 1;

        public int SpeakingSeconds { get; set; } = 120;

        public int VotingSeconds { get; set; } = 60;

        public int MuteSeconds { get; set; } = 0;

        public string RuleMsg { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (CreateCommands is null) CreateCommands = new();
            if (AddWordCommands is null) AddWordCommands = new();
            if (SendWordCommands is null) SendWordCommands = new();
            if (MatchSeconds < 30) MatchSeconds = 30;
            if (PrepareSeconds < 5) PrepareSeconds = 5;
            if (SpeakingSeconds < 10) SpeakingSeconds = 10;
            if (VotingSeconds < 10) VotingSeconds = 10;
            if (MuteSeconds < 0) MuteSeconds = 0;
            return this;
        }
    }


}
