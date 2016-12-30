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
using Android.Graphics;

namespace SimpleWeather
{
    class Class_Forecast
    {
        public float temp { get; set; }
        public string weather { get; set; }
        public string icon { get; set; }
        public string time { get; set; }
    }
}