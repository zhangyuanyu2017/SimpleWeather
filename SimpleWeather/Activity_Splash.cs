using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Preferences;
using Java.Lang;
using Android.Graphics;

namespace SimpleWeather
{
    [Activity(Label = "Simple Weather", MainLauncher = true, Icon = "@drawable/Drawable_Splash")]
    public class Activity_Splash : Activity
    {
        private const int GET_DATA_FINISH = 1024;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetStatusBarColor(Color.White);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_splash);
            // Create your application here
        }

        private void start()
        {
            if (Class_CityList.City_User.Count() == 0)
            {
                Intent intent = new Intent(this, typeof(Activity_AddCity));
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(Activity_Main));
                StartActivity(intent);
            }
        }

        class mHandler : Handler
        {
            public override void HandleMessage(Message message)
            {
                switch (message.What)
                {
                    case GET_DATA_FINISH:
                        break;
                }
            }
        }
        mHandler hand = new mHandler();

        protected override void OnStart()
        {
            base.OnStart();
            Class_CityList.init();

            getCityDataFromAssets();
            //getCityIdFromAssets();
            getCityUsers();
            hand.PostDelayed(new Runnable(() =>
            {
                start();
            }),500);

            Message msg = new Message();
            msg.What = GET_DATA_FINISH;
            hand.SendMessage(msg);
        }

        private void getCityDataFromAssets()
        {
            List<Class_CityInfo> City_Data = new List<Class_CityInfo>();
            StreamReader sr_name = new StreamReader(Assets.Open("city.txt"));
            StreamReader sr_id = new StreamReader(Assets.Open("city_id.txt"));
            string str_name = "";
            string str_id = "";
            int num = 0;
            while ((str_name = sr_name.ReadLine()) != null && ((str_id = sr_id.ReadLine()) != null)) 
            {
                Class_CityInfo item_city = new Class_CityInfo();
                item_city.id = int.Parse(str_id);
                item_city.name = str_name;
                num++;
                City_Data.Add(item_city);
            }
            Class_CityList.City_All = City_Data;
        }

        //private void getCityIdFromAssets()
        //{
        //    List<int> City_id = new List<int>();
        //    StreamReader sr = new StreamReader(Assets.Open("city_id.txt"));
        //    string str = "";
        //    int num = 0;
        //    while ((str = sr.ReadLine()) != null)
        //    {
        //        City_id.Add(int.Parse(str));
        //        num++;
        //    }
        //    Class_CityList.setCity_id(City_id);
        //}

        private void getCityUsers()
        {
            List<Class_CityInfo> City_User = new List<Class_CityInfo>();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            int num = prefs.GetInt("Size",0);
            for(int i = 0; i < num; i++)
            {
                int j = i + 1;
                Class_CityInfo item_city = new Class_CityInfo();
                item_city.name = prefs.GetString("City_" + j, "");
                item_city.id = prefs.GetInt("CityId_" + j, 0);
                City_User.Add(item_city);
            }
            Class_CityList.City_User = City_User;
        }
    }
}