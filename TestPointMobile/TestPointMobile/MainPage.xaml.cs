using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestPointMobile {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }
        IScanner_PointMobile scan;
        bool isreading = false;
        protected async override void OnAppearing() {
            base.OnAppearing();

            try {
                scan = DependencyService.Get<IScanner_PointMobile>();
                scan.RegisterReceiver();

                MessagingCenter.Subscribe<App, string>(this, "barcode", (Action<App, string>)(async (sender, arg) => {
                    if (isreading) {
                        await OnScanAction(arg);
                    }

                }));
            }
            catch (Exception ex) {
                await DisplayAlert("오류", ex.Message, "확인");
            }


        }
        protected override void OnDisappearing() {

            scan.UnregisterReceiver();
            MessagingCenter.Unsubscribe<App, string>(this, "barcode");
            base.OnDisappearing();
        }


        private async Task OnScanAction(string arg) {
            isreading = true;


            isreading = false;
        }
    }
}
