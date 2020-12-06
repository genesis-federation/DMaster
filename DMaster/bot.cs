﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DMaster.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DMaster
{
    public class bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<Configjson>(json);

            var config = new DiscordConfiguration
            {
                Token = "NzU0Nzg0ODE2NjQxMzQzNjMw.X15yIw.FOZN9XETphtfkYr9olHMsVEEglQ",
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(15)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix}, 
                EnableDms = false,
                EnableMentionPrefix = true,

            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<DmCommands>();
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
