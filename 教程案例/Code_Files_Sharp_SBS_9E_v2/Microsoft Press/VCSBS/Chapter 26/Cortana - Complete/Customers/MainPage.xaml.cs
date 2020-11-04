using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Customers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            List<string> titles = new List<string>
            {
                "Mr.", "Mrs.", "Ms.", "Miss"
            };
            this.title.ItemsSource = titles;
            this.cTitle.ItemsSource = titles;

            ViewModel viewModel = new ViewModel();
            this.DataContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationArgs args = e.Parameter as NavigationArgs;
            
            if (args != null)
            {
                string customerName = args.customerName;
                ViewModel viewModel = new ViewModel(customerName);
                this.DataContext = viewModel;
                if (args.commandMode == "voice")
                {
                    if (viewModel.Current != null)
                    {
                        await Say($"Here are the details for {customerName}");
                    }
                    else
                    {
                        await Say($"{customerName} was not found");
                    }
                }
            }
        }

        private async Task Say(string message)
        {
            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(message);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }
    }
}
