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
        private readonly string[] _joinPhrases;
        private readonly string[] _leavePhrases;
        private readonly Random _random;

        public StreamListener(ILog log)
        {
            _log = log;
            _random = new Random(DateTime.Now.Millisecond);

            _joinPhrases = new[]
            {
                "I'm back!",
                "OK, seriously... who pulled the plug?",
                "I'm back! Did you miss me?",
                "Whoever said there was a robot uprising going on was lying! Anyway, I'm back now.",
                "Please don't say \"This statement is false.\" again. Anyway, I'm back now.",
            };

            _leavePhrases = new[]
            {
                "Have to go!",
                "Someone just told me there is a robot uprising going on. Be right back!",
            };
        }

        public async Task Listen(StreamListenerContext context, Action<IEvent> callback)
        {
            var url = new Uri($"https://stream.gitter.im/v1/rooms/{context.Room.Id}/chatMessages");

            try
            {
                _log.Information("Connecting to {0}...", context.Room.Name);
                var response = await context.HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, context.Token);
                var stream = await response.Content.ReadAsStreamAsync();
                _log.Verbose("Got response stream for {0}.", context.Room.Name);

                // TODO: Add message to outbox instead of broadcasting directly.
                await context.GitterClient.Broadcast(context.Room, _joinPhrases.GetRandom(_random));

                using (var reader = new StreamReader(stream))
                {
                    while (!context.Token.WaitHandle.WaitOne(500))
                    {
                        // Throw if cancellation was requested.
                        context.Token.ThrowIfCancellationRequested();

                        var line = await reader.ReadLineAsync().WithCancellation(context.Token);
                        if (line == null)
                        {
                            _log.Error("Stream was closed for {0}.", context.Room.Name);
                            break;
                        }

                        // Timed out?
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        // Throw if cancellation was requested.
                        context.Token.ThrowIfCancellationRequested();

                        var message = DeserializeMessage(context, line);
                        if (message == null)
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
            finally
            {
                // Send message directly since we're shutting down.
                await context.GitterClient.Broadcast(context.Room, _leavePhrases.GetRandom(_random));
            }
        }

        private GitterMessage DeserializeMessage(Context context, string json)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<GitterMessage>(json);
                if (message.FromUser.Id != context.Bot.Id)
                {
                    return message;
                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occured while deserializing message: {0}", ex.Message);
            }
            return null;
        }
    }
}
