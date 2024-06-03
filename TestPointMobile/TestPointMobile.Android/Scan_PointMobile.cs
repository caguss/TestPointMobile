using Android.Content;
using TestPointMobile.Droid;
using System;
using Xamarin.Forms;
using Android.OS;



[assembly: Dependency(typeof(Scan_PointMobile))]
namespace TestPointMobile.Droid {
    public class Scan_PointMobile : IScanner_PointMobile {

        // action scanner Settings
        private const String SCANNER_ACTION_SETTING_CHANGE = "com.android.server.scannerservice.settingchange";
        // Action Scanner Setting Parameter and Value
        private const String SCANNER_ACTION_PARAMETER = "android.intent.action.SCANNER_PARAMETER";
        // action Scanner Enable or Disable
        private const String SCANNER_ACTION_ENABLE = "com.android.server.scannerservice.m3onoff";
        // Extra about scanner enable or Disable
        private const String SCANNER_EXTRA_ENABLE = "scanneronoff";

        // Action start decoding
        private const String SCANNER_ACTION_START = "android.intent.action.M3SCANNER_BUTTON_DOWN";
        // action Stop decoding
        private const String SCANNER_ACTION_CANCEL = "android.intent.action.M3SCANNER_BUTTON_UP";

        // Action to receive decoding result and setting parameter result
        private const String SCANNER_ACTION_BARCODE = "com.android.server.scannerservice.broadcast";
        // Extra about decoding result
        public const String SCANNER_EXTRA_BARCODE_DATA = "m3scannerdata";
        // Extra about code type
        public const String SCANNER_EXTRA_BARCODE_CODE_TYPE = "m3scanner_code_type";

        private static ScanResultReceiver _scanReceiver = new ScanResultReceiver();
        public void RegisterReceiver()
        {
            // for getting decoding result and setParam, getParam result.
            System.Diagnostics.Debug.WriteLine(String.Format("RegisterReceiver"));
            IntentFilter filter = new IntentFilter();
            filter.AddAction(SCANNER_ACTION_BARCODE);
            Android.App.Application.Context.RegisterReceiver(_scanReceiver, filter);
        }

        public void UnregisterReceiver()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("UnregisterReceiver"));
            Android.App.Application.Context.UnregisterReceiver(_scanReceiver);
        }


        public class ScanResultReceiver : BroadcastReceiver {
            private String barcode;
            private String type;
            private App scanApp = new App();

            public override void OnReceive(Context context, Intent intent) {

                Android.Util.Log.Info("ScanReceiver", "onReceiver: " + intent.Action);
                if (intent.Action.Equals(SCANNER_ACTION_BARCODE))
                {
                    barcode = intent.GetStringExtra(SCANNER_EXTRA_BARCODE_DATA);
                    if (barcode != null)
                    {
                        // Send Barcode Data
                        type = intent.GetStringExtra(SCANNER_EXTRA_BARCODE_CODE_TYPE);
                        MessagingCenter.Send<App, string>(scanApp, "barcode", barcode);
                        System.Diagnostics.Debug.WriteLine(String.Format("OnReceive barcode: " + barcode + " type: " + type));
                    }
                    else
                    {
                        // Send Parameter data
                        int nParam = intent.GetIntExtra("symbology", -1);
                        int nValue = intent.GetIntExtra("value", -1);
                        MessagingCenter.Send<App, int>(scanApp, "value", nValue);
                        System.Diagnostics.Debug.WriteLine(String.Format("OnReceive param: " + nParam + " value: " + nValue));
                    }
                }
            }
        }
    }
}
