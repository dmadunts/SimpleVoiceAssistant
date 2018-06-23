using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Util;

namespace VoiceAssistant.Services
{
    [Service]
    public class FlashlightService : Service
    {
        private const int REQUEST_FLASH_PERMISSION = 50;
        private CameraManager cameraManager;
        private string cameraId;
        private static readonly string LOG_TAG = "FlashlightService";

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            bool hasFlash = PackageManager.HasSystemFeature(PackageManager.FeatureCameraFlash);

            cameraManager = (CameraManager)this.GetSystemService(Context.CameraService);
            cameraId = cameraManager.GetCameraIdList()[0];

            string state = intent.GetStringExtra("state");
            if (state == "off")
            {
                FlashLightOff();
            }
            else FlashLightOn();

            return StartCommandResult.NotSticky;
        }

        private void FlashLightOn()
        {
            cameraManager.SetTorchMode(cameraId, true);
        }

        private void FlashLightOff()
        {
            cameraManager.SetTorchMode(cameraId, false);
        }

        public override void OnCreate()
        {
            Log.Debug(LOG_TAG, "OnCreate");
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Log.Debug(LOG_TAG, "OnDestroy");
            base.OnDestroy();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        
        private void Flashlight(bool state)
        {
            try
            {
                cameraManager.SetTorchMode(cameraId, state);
            }
            catch (Exception ex)
            {
                Log.Debug(LOG_TAG, "Flashlight error: " + ex.ToString());
            }
        }
    }
}