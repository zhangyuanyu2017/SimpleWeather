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

namespace SimpleWeather
{
    class Class_CityInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public Fragment_City city { get; set; }

        public Class_CityInfo()
        {
            city = null;
        }

        public Class_CityInfo(Class_CityInfo City)
        {
            id = City.id;
            name = City.name;
            city = City.city;
        }


        public static int Size { get; set; }
    }
}