using Accelerider.Windows.ServerInteraction;
using Prism.Events;

namespace Accelerider.Windows
{
    internal class MainWindowLoadingEvent : PubSubEvent<bool> { }

    internal class SignUpSuccessEvent : PubSubEvent<SignUpArgs> { }
}
