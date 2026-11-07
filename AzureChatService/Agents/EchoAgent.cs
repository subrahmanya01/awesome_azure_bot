using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;

namespace AzureChatService.Agents
{
    public class EchoAgent : AgentApplication
    {
        public EchoAgent(AgentApplicationOptions options) : base(options)
        {
            OnActivity( ActivityTypes.Message, OnMessageAsync, rank: RouteRank.Last );
        }

        private async Task OnMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
        {
            var streaming = turnContext.StreamingResponse;

            try
            {
                // Start the streaming response
                await streaming.QueueInformativeUpdateAsync( "Generating response...", cancellationToken );

                var message = turnContext.Activity.Text ?? "";

                await foreach ( var word in GetStreamingResponse( message, cancellationToken))
                {
                    streaming.QueueTextChunk(word);
                }
            }
            finally
            {
                await streaming.EndStreamAsync(cancellationToken);
            }
        }

        private async IAsyncEnumerable<string> GetStreamingResponse(string message, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var words = message.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (var word in words)
            {
                await Task.Delay(1000, cancellationToken);

                yield return word + " ";
            }
        }
    }
}