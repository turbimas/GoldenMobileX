﻿using System;
using GoldenMobileX;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Android.Preferences;
using Android.Provider;
using System.Collections.Generic;

namespace GoldenMobileX.Droid
{
    [Activity(Label = "GoldenMobileX", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            this.RequestedOrientation = ScreenOrientation.Portrait;

            /*
                        MyHomeImpleWatcher myHomeImpleWatcher = new MyHomeImpleWatcher();

                        HomeWatcher mHomeWatcher = new HomeWatcher(this);
                        mHomeWatcher.setOnHomePressedListener(myHomeImpleWatcher);
                        mHomeWatcher.startWatch();

                        */
 

        }


 

        private int currentState = 0;
        protected override void OnPause()
        {
            base.OnPause();
            // uygulama arka plana atıldığında yapılacak işlemler
            SaveAppState();
        }

        protected override void OnResume()
        {
            base.OnResume();
            // uygulama yeniden ön planda çalıştırıldığında yapılacak işlemler
            LoadAppState();
        }

        private void SaveAppState()
        {
            // uygulama durumunu kaydetmek için kullanılacak kodlar
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("CurrentState", currentState);
            editor.Apply();
        }

        private void LoadAppState()
        {
            // uygulama durumunu geri yüklemek için kullanılacak kodlar
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            currentState = prefs.GetInt("CurrentState", 0);
        }



        /*
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        class MyHomeImpleWatcher : OnHomePressedListener
        {
            public void onHomeLongPressed()
            {
                System.Diagnostics.Debug.WriteLine("onHomePressed is pressed......");
            }
            public void onHomePressed()
            {
                System.Diagnostics.Debug.WriteLine("onHomeLongPressed is pressed......");
            }
        }
        */
    }
}



