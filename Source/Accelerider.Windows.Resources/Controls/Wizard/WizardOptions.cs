using System;

namespace Accelerider.Windows.Resources.Controls.Wizard
{
    public class WizardOptions
    {
        /// <summary>
        /// Default wizard behavior: leave the current View and switch to the next View.
        /// </summary>
        public static readonly object Continue = new object();

        /// <summary>
        /// Cancel this navigation, which will cause the Dialog to stay in the current View.
        /// </summary>
        public static readonly object Cancel = new object();

        /// <summary>
        /// End this guide early, and close the current Dialog window. 
        /// </summary>
        public static readonly object Break = new object();

        public static object JumpTo<T>(T parameter)
        {
            throw new NotImplementedException();
            //return (typeof(IWizardable<T>), parameter);
        }
    }
}
