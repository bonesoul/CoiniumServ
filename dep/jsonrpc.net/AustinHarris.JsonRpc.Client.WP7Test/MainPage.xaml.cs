using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Reactive.Concurrency;

namespace AustinHarris.JsonRpc.Client.WP7Test
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        JsonResponse<object> response;
        Exception ex;
        bool done;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            JsonRpcClient client = new JsonRpcClient(new Uri("http://localhost:49718/json.rpc"));
            var request = client.Invoke<object>("internal.echo", "hi", Scheduler.ThreadPool);

            request.Subscribe(
                response =>
                {
                    this.response = response;
                },
                error =>
                {
                    ex = error;
                },
                () =>
                {
                    done = true;
                }
                );
        }
    }
}