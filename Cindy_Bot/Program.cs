using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace CindyBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);
            
            var token = Environment.GetEnvironmentVariable("DiscordToken");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.MessageUpdated += MessageUpdated;

            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected");
                return Task.CompletedTask;
            };

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage pMessage)
        {
            if (pMessage.Content == "!ping")
            {
                await pMessage.Channel.SendMessageAsync("Pong!");
            }
            else if (pMessage.Content == "!Hello")
            {
                await pMessage.Channel.SendMessageAsync($"Hello {pMessage.Author.Username}!");
            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> pBefore, SocketMessage pAfter, ISocketMessageChannel pChannel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of 'after'.
            var lMessage = await pBefore.GetOrDownloadAsync();
            Console.WriteLine($"{lMessage} -> {pAfter} on Channel {pChannel}");
        }

        public string GetChannelTopic(ulong pChannelID)
        {
            var lChannel = _client.GetChannel(pChannelID) as SocketTextChannel;
            return lChannel?.Topic;
        }

        public SocketGuildUser GetGuildOwner(SocketChannel pChannel)
        {
            var lGuild = (pChannel as SocketGuildChannel)?.Guild;
            return lGuild.Owner;
        }
    }
}
