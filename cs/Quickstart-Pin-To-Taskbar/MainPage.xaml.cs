using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quickstart_Pin_To_Taskbar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ShowTaskbarTipIfNeeded()
        {
            try
            {
                // You should  check if you've already shown this tip before,
                // and if so, don't show the tip to the user again.
                if (ApplicationData.Current.LocalSettings.Values.Any(i => i.Key.Equals("ShownTaskbarTip")))
                {
                    // But for purposes of this Quickstart, we'll always show the tip,
                    // so we've commented out the return statement.
                    //return;
                }

                // Store that you've shown this tip, so you don't show it again
                ApplicationData.Current.LocalSettings.Values["ShownTaskbarTip"] = true;

                // If Start screen manager API's aren't present
                if (!ApiInformation.IsTypePresent("Windows.UI.Shell.TaskbarManager"))
                {
                    ShowMessage("TaskbarManager API isn't present on this build.");
                    return;
                }

                // Get the taskbar manager
                var taskbarManager = TaskbarManager.GetDefault();

                // If Taskbar doesn't allow pinning, don't show the tip
                if (!taskbarManager.IsPinningAllowed)
                {
                    ShowMessage("Taskbar doesn't allow pinning (or Taskbar isn't supported on this device family).");
                    return;
                }

                // If already pinned, don't show the tip
                if (await taskbarManager.IsCurrentAppPinnedAsync())
                {
                    ShowMessage("This app is already pinned to Taskbar!");
                    return;
                }

                // Otherwise, show the tip
                FlyoutPinTip.ShowAt(ButtonShowTip);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }

        private async void ButtonPinToTaskbar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pin your app
                bool didPin = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();

                if (didPin)
                {
                    ShowMessage("Success! App was pinned!");
                }
                else
                {
                    ShowMessage("App was NOT pinned, did you click no on the dialog?");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }

        private void ButtonShowTip_Click(object sender, RoutedEventArgs e)
        {
            ShowTaskbarTipIfNeeded();
        }

        private async void ShowMessage(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }
    }
}
