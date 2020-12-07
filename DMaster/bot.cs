using System;
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

namespace DMaster
{
    public class bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            var config = new DiscordConfiguration
            {
                Token = "NzU0Nzg0ODE2NjQxMzQzNjMw.X15yIw.Q-9QMX6vKExczxjZZAP7jdlIZEI",
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
                StringPrefixes = new string[] {"$"}, 
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
