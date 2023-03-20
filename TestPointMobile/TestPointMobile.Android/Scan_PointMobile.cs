using Android.App;
using Android.Content;
using TestPointMobile.Droid;
using System;
using Xamarin.Forms;
using Device.Sdk;
using Device.Common;
using System.Linq;
using Android.OS;

//using  device.common.DecodeResult;
//using device.common.DecodeStateCallback;
//using device.common.ScanConst;
//using device.sdk.ScanManager;



[assembly: Dependency(typeof(Scan_PointMobile))]
namespace TestPointMobile.Droid {
    public class Scan_PointMobile : IScanner_PointMobile {

        #region 바코드
        #region Broadcast Intents
        // 스캐너 활성화/비활성화
        private const String SCANNER_ENABLED_SCANNER = "device.common.ENABLED_SCANNER";
        //트리거를 통해 실행 가능/불가능 설정
        private const String SCANNER_ENABLED_TRIGGER = "device.common.ENABLED_TRIGGER";
        //화면내 스캔 버튼 활성화/비활성화
        private const String SCANNER_ENABLED_TOUCHSCAN = "device.common.ENABLED_TOUCHSCAN";
        //Touchscan 버튼 설정의 현재 상태 확인
        private const String SCANNER_READ_TOUCHSCAN_STATE = "device.common.READ_TOUCHSCAN_STATE";
        #endregion

        #region onReceive Intents
        //Barcode Decode 완료 후 실패/성공에 대한 결과값만 전달
        private const String SCANNER_USERMSG = "device.common.USERMSG";
        //Barcode Decode 완료 후 실패/성공에 대한 결과와 세부 데이터를 함께 전달
        private const String SCANNER_EVENT = "device.scanner.EVENT";
        //Touchscan 버튼의 활성화/비활성화 상태에 대한 인텐트
        private const String SCANNER_STATE_TOUCHSCAN = "device.common.SCANNER_STATE_TOUCHSCAN";
        #endregion
        #endregion

        private DecodeStateCallback mStateCallback;
        private static ScanManager mScanner = new ScanManager();
        private static DecodeResult mDecodeResult = new DecodeResult();
        private static ScanResultReceiver mScanResultReceiver = new ScanResultReceiver();
        private static Handler mHandler = new Handler();
        private int mBackupResultType = ScanConst.ResultType.DcdResultCopypaste;

        public void RegisterReceiver() {
            if (mScanner != null) {
                mScanner.ARegisterDecodeStateCallback(mStateCallback);
                mBackupResultType = mScanner.ADecodeGetResultType();
                mScanner.ADecodeSetResultType(ScanConst.ResultType.DcdResultUsermsg);
            }

            mScanResultReceiver = new ScanResultReceiver();
            // for getting decoding result and setParam, getParam result.
            IntentFilter filter = new IntentFilter();
            filter = new IntentFilter();
            //filter.AddAction(ScanConst.EnabledScannerAction);

            filter.AddAction(ScanConst.IntentUsermsg);
            filter.AddAction(ScanConst.IntentEvent);
            Android.App.Application.Context.RegisterReceiver(mScanResultReceiver, filter);

        }

        public void UnregisterReceiver() {
            Android.App.Application.Context.UnregisterReceiver(mScanResultReceiver);
        }



        [IntentFilter(new[] { "device.common.USERMSG", "device.scanner.EVENT" })]
        [BroadcastReceiver(Enabled = true)]
        public class ScanResultReceiver : BroadcastReceiver {
            private String type;
            private App scanApp = new App();

            public override void OnReceive(Context context, Intent intent) {
                if (mScanner != null) {
                    try {
                        if (ScanConst.IntentUsermsg.Equals(intent.Action)) {
                            mScanner.ADecodeGetResult(mDecodeResult.Recycle());

                            string data = string.Format("{0},{1}",  mDecodeResult.ToString(), mDecodeResult.SymName);
                            MessagingCenter.Send<App, string>(scanApp, "barcode", data);
                        }
                        else if (ScanConst.IntentEvent.Equals(intent.Action)) {
                            bool result = intent.GetBooleanExtra(ScanConst.ExtraEventDecodeResult, false);
                            int decodeBytesLength = intent.GetIntExtra(ScanConst.ExtraEventDecodeLength, 0);
                            byte[] decodeBytesValue = intent.GetByteArrayExtra(ScanConst.ExtraEventDecodeValue);
                            String decodeValue = new String(System.Text.Encoding.UTF8.GetString(decodeBytesValue).ToCharArray(), 0, decodeBytesLength);
                            //int decodeLength = decodeValue.Length;
                            String symbolName = intent.GetStringExtra(ScanConst.ExtraEventSymbolName);
                            //sbyte symbolId = intent.GetByteExtra(ScanConst.ExtraEventSymbolId, (sbyte)0);
                            //int symbolType = intent.GetIntExtra(ScanConst.ExtraEventSymbolType, 0);
                            //sbyte letter = intent.GetByteExtra(ScanConst.ExtraEventDecodeLetter, (sbyte)0);
                            //sbyte modifier = intent.GetByteExtra(ScanConst.ExtraEventDecodeModifier, (sbyte)0);
                            //int decodingTime = intent.GetIntExtra(ScanConst.ExtraEventDecodeTime, 0);


                            //"1. result: " + result);
                            //"2. bytes length: " + decodeBytesLength);
                            //"3. bytes value: " + decodeBytesValue);
                            //"4. decoding length: " + decodeLength);
                            //"5. decoding value: " + decodeValue);
                            //"6. symbol name: " + symbolName);
                            //"7. symbol id: " + symbolId);
                            //"8. symbol type: " + symbolType);
                            //"9. decoding letter: " + letter);
                            //"10.decoding modifier: " + modifier);
                            //"11.decoding time: " + decodingTime);

                            string data = string.Format("{0},{1}", symbolName, decodeValue);
                            MessagingCenter.Send<App, string>(scanApp, "barcode", data);
                        }
                    }
                    catch (Exception e) {
                        MessagingCenter.Send<App, string>(scanApp, "barcode", e.Message);
                    }
                }
            }
        }
    }
}
