using System.Windows;

namespace Sample_1
{

    //[Step.1] Define class with static costructor you want to be invoked once assembly that owns that type is loaded.
    class Setup
    {

        /// <inheritdoc />
        static Setup()
        {
            //[Step.2] In static constructor do what ever you need.
            MessageBox.Show("Setup initialized");
        }
    }
}
