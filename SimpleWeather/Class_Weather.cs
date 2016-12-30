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
    class Class_Weather
    {
        public float temperature { get; set; }
        public string weather { get; set; }
        public string weather_description { get; set; }
        public string weather_icon { get; set; }
        public int wind_degree { get; set; }
        public int wind_speed { get; set; }
        public int visibility { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int clouds { get; set; }
    }
}