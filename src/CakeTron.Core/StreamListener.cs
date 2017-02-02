using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Domain;
using CakeTron.Core.Events;
using Newtonsoft.Json;

namespace CakeTron.Core
{
    public sealed class StreamListener
    {
        private readonly ILog _log;

        public StreamListener(ILog log)
        {
            _log = log;
        }

        public async Task Listen(StreamListenerContext context, Action<IEvent> callback)
        {
            var url = new Uri($"https://stream.gitter.im/v1/rooms/{context.Room.Id}/chatMessages");

            try
            {
                _log.Information("Listening for messages in {0}...", context.Room.Name);
                var response = await context.HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, context.Token);
                var stream = await response.Content.ReadAsStreamAsync();

                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        // Throw if cancellation was requested.
                        context.Token.ThrowIfCancellationRequested();

                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        var message = JsonConvert.DeserializeObject<GitterMessage>(line);
                        if (message.FromUser.Id == context.Bot.Id)
                        {
                            continue;
                        }

                        callback(new MessageEvent
                        {
                            Bot = context.Bot,
                            Room = context.Room,
                            Message = message
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _log.Information("Listener for {0} was cancelled.", context.Room.Name);
            }
            catch (Exception ex)
            {
                _log.Error("Error in {0}: {1}", context.Room.Name, ex.Message);
            }
        }
    }
}
