using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldGrains
{
    [ImplicitStreamSubscription("RANDOMDATA")]
    public class ParrotGrain : Grain, IParrotGrain
    {
        private readonly ILogger<ParrotGrain> _logger;
        private readonly IPersistentState<ParrotState> _profile;

        public ParrotGrain([PersistentState("profile", "ParrotStorage")] IPersistentState<ParrotState> profile, ILogger<ParrotGrain> logger)
        {
            _logger = logger;
            _profile = profile;
        }

        public override async Task OnActivateAsync()
        {
            //Create a GUID based on our GUID as a grain
            var guid = this.GetPrimaryKey();
            //Get one of the providers which we defined in config
            var streamProvider = GetStreamProvider("Parrot");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");
            //Set our OnNext method to the lambda which simply prints the data, 
            //this doesn't make new subscriptions because we are using implicit subscriptions via [ImplicitStreamSubscription].
            //await stream.SubscribeAsync(async (message, token) => Console.WriteLine(message));

            await stream.SubscribeAsync(
                async (message, t) =>
                {
                    _logger.LogInformation($"got an OnNext with message={message.GetType().Name}, value={message} token={t}");
                    //return HandleAllTouchConsoleRequest(message);

                    await SaveState(message);
                },
                        (e) =>
                        {
                            _logger.LogError(e, "failed");
                            return Task.CompletedTask;
                        },
                        () =>
                        {
                            _logger.LogWarning("completed");
                            return Task.CompletedTask;
                        }
                   );
        }

        public async Task SaveState(int message)
        {
            var evenWas = _profile.State.Even;
            var evenNow = (message % 2 == 0);

            if (evenNow != evenWas)
            {
                if (evenNow)
                {
                    _profile.State.Even = true;
                    _profile.State.Name = "even";
                    _profile.State.DateOfBirth = new DateTime(2002, 2, 2);
                    _logger.LogInformation("Parrot change from FALSE to TRUE");
                }
                else
                {
                    _profile.State.Even = false;
                    _profile.State.Name = "odd";
                    _profile.State.DateOfBirth = new DateTime(2001, 1, 1);
                    _logger.LogInformation("Parrot change from TRUE to FALSE");
                }
                await _profile.WriteStateAsync();
            }
        }
    }
}
