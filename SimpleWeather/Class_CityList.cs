using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Text;
using Android.Widget;
using Java.Lang;
using System.IO;

namespace SimpleWeather
{
    class Class_CityList
    {
        public static List<Class_CityInfo> City_User { get; set; }

        public static List<Class_CityInfo> City_All { get; set; }

        public static void init()
        {
            City_User = new List<Class_CityInfo>();
            City_All = new List<Class_CityInfo>();
        }

        public static List<string> getCity_User()
        {
            List<string> User = new List<string>();
            for(int i = 0; i < City_User.Count; i++)
            {
                User.Add(City_User[i].name);
            }
            return User;
        }

        public static void setCity_User(List<string> list)
        {
            List<Class_CityInfo> temp = new List<Class_CityInfo>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < City_User.Count; j++)
                {
                    if (list[i].Equals(City_User[j].name))
                    {
                        Class_CityInfo item_city = new Class_CityInfo(City_User[j]);
                        temp.Add(item_city);
                        break;
                    }
                }
            }
            City_User = temp;
        }


    }
}