using System;
using Akka.Actor;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef consoleWriter;

        public ValidationActor(IActorRef consoleWriter)
        {
            this.consoleWriter = consoleWriter;
        }
        
        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (string.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input, as previously
                // received input was blank
                consoleWriter.Tell(new Messages.NullInputError("No input received."));
            }
            else
            {
                var isValid = IsValid(msg);

                var outGoingMsg = isValid
                    ? (object)new Messages.InputSuccess("Thank you! Message was valid.")
                    : new Messages.ValidationError("Invalid: input had odd number of characters.");

                consoleWriter.Tell(outGoingMsg);
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private static bool IsValid(string message)
        {
            var valid = message.Length % 2 == 0;
            return valid;
        }
    }
}