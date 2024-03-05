using Epsiloner.Helpers;
using System;
using Epsiloner;

namespace Sample_1
{
    public partial class App
    {
        /// <inheritdoc />
        public App()
        {
            //[Step.4] Run InitializeTypesFromAttribute in current AppDomain.
            //Note: this code can be invoked multiple times, will not generate any exceptions and static consatructors will be executed only once.
            AppDomain.CurrentDomain.InitializeTypesFromAttribute();

        }

        private void DoNothing(int number) { }
    }
}
