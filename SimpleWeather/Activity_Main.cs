using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.View;
using System.Collections.Generic;

namespace SimpleWeather
{
    [Activity(Label = "SimpleWeather")]
    public class Activity_Main : AppCompatActivity
    {
        ViewPager viewpager_main;
        Android.Support.V4.App.FragmentManager fm;
        List<Android.Support.V4.App.Fragment> fragmentList;
        List<Class_CityInfo> city_Data;

        private const int ADD = 1089;
        private const int MODIFY = 1090;

        protected override void OnCreate(Bundle bundle)
        {
            this.SetTheme(Android.Resource.Style.ThemeNoTitleBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.layout_main);
            // Set our view from the "main" layout resource
            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);
            //SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab_main);
            fab.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(Activity_AddCity));
                StartActivityForResult(intent,ADD);
            };


            initFragment();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        private void initFragment()
        {
            viewpager_main = FindViewById<ViewPager>(Resource.Id.viewpager_main);
            fm = SupportFragmentManager;
            fragmentList = new List<Android.Support.V4.App.Fragment>();
            city_Data = new List<Class_CityInfo>();

            for (int i = 0; i < Class_CityList.City_User.Count; i++)
            {
                Fragment_City cityfragment = new Fragment_City();
                Bundle b = new Bundle();
                b.PutString("CityName", Class_CityList.City_User[i].name);
                b.PutInt("CityId", Class_CityList.City_User[i].id);
                cityfragment.Arguments = b;
                fragmentList.Add(cityfragment);

                Class_CityInfo item = new Class_CityInfo();
                item.name = Class_CityList.City_User[i].name;
                item.id = Class_CityList.City_User[i].id;
                item.city = cityfragment;

                city_Data.Add(item);
            }

            viewpager_main.Adapter = new Adapter_PagerStateAdaper(this, fm, fragmentList);
            viewpager_main.CurrentItem = 0;
            viewpager_main.OffscreenPageLimit = 2;
        }

       // public override bool OnCreateOptionsMenu(IMenu menu)
       // {
       //     MenuInflater.Inflate(Resource.Menu.menu_main, menu);
       //     return true;
       // }

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Resource.Id.menu_main_modify:
        //            showPopwindow();
        //            return true;
        //    }
        //    return base.OnOptionsItemSelected(item);
        //}

        private void refreshFragment(int mode)
        {
            List<Class_CityInfo> temp = new List<Class_CityInfo>();
            fragmentList.Clear();

            for(int i = 0; i < Class_CityList.City_User.Count; i++)
            {
                bool find = false;
                for(int j = 0; j < city_Data.Count; j++)
                {
                    if (city_Data[j].name.Equals(Class_CityList.City_User[i].name)){
                        Class_CityInfo item_city = new Class_CityInfo(city_Data[j]);
                        fragmentList.Add(item_city.city);
                        temp.Add(item_city);
                        find = true;
                        break;
                    }
                }
                if (find == false)
                {
                    Fragment_City cityfragment = new Fragment_City();
                    Bundle b = new Bundle();
                    b.PutString("CityName", Class_CityList.City_User[i].name);
                    b.PutInt("CityId", Class_CityList.City_User[i].id);
                    cityfragment.Arguments = b;
                    fragmentList.Add(cityfragment);

                    Class_CityInfo item = new Class_CityInfo();
                    item.name = Class_CityList.City_User[i].name;
                    item.id = Class_CityList.City_User[i].id;
                    item.city = cityfragment;

                    temp.Add(item);
                }
                city_Data = new List<Class_CityInfo>(temp);

                if (mode == ADD)
                {
                    viewpager_main.Adapter = new Adapter_PagerStateAdaper(this, SupportFragmentManager, fragmentList);
                    viewpager_main.CurrentItem = fragmentList.Count-1;
                }

                if (mode == MODIFY)
                {
                    viewpager_main.Adapter = new Adapter_PagerStateAdaper(this, SupportFragmentManager, fragmentList);
                    viewpager_main.CurrentItem = fragmentList.Count - 1;

                }
            }

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case ADD:
                        refreshFragment(ADD);
                        break;

                    case MODIFY:
                        refreshFragment(MODIFY);
                        break;

                }

            }
        }
    }
}
    


