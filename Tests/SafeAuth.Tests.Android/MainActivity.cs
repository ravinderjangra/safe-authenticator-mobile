﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Android.App;
using Android.Content.PM;
using Android.OS;
using NUnit.Runner;
using NUnit.Runner.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SafeAuth.Tests.Android
{
    [Activity(
        Name = "com.safe.auth.tests.MainActivity",
        Label = "NUnit",
        Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Holo.Light",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);

            // This will load all tests within the current project
            // var nunit = new NUnit.Runner.App();
            var nunit = new App
            {
                Options = new TestOptions
                {
                    // If you want to add tests in another assembly
                    // nunit.AddTestAssembly(typeof(MyTests).Assembly);

                    // Available options for testing
                    // nunit.Options = new TestOptions

                    // If True, the tests will run automatically when the app starts
                    // otherwise you must run them manually.
                    AutoRun = true,

                    // If True, the application will terminate automatically after running the tests.
                    // TerminateAfterExecution = true,

                    // Information about the tcp listener host and port.
                    // For now, send result as XML to the listening server.
                    TcpWriterParameters = new TcpWriterInfo("10.0.2.2", 10500),

                    // Creates a NUnit Xml result file on the host file system using PCLStorage library.
                    // CreateXmlResultFile = true,

                    // Choose a different path for the xml result file
                    // ResultFilePath = Path.Combine(Environment.ExternalStorageDirectory.Path, Environment.DirectoryDownloads, "Nunit", "AndroidResults.xml")
                }
            };
            LoadApplication(nunit);
        }
    }
}
