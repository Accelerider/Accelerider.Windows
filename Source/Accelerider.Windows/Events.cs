using Accelerider.Windows.Models;
using Prism.Events;

namespace Accelerider.Windows
{
    internal class MainWindowLoadingEvent : PubSubEvent<bool> { }

    internal class SignUpSuccessEvent : PubSubEvent<SignUpInfoBody> { }
}
