using Orleans;
using System;
using System.Threading.Tasks;

namespace HelloWorldGrains
{
    public class HelloWorldGrain : Grain, IHelloWorldGrain
    {
        IDisposable _timer;

        public override async Task OnActivateAsync()
        {
        }

        private async Task StartSending()
        {
            var streamProvider = GetStreamProvider("Parrot");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<int>(Guid.NewGuid(), "RANDOMDATA");

            _timer = RegisterTimer(s =>
            {
                return stream.OnNextAsync(new System.Random().Next());
            }, null, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000));
        }

        public async Task<string> SayHello(string greeting)
        {
            await StartSending();
            return "hiiiiii" + greeting;
        }
    }
}
