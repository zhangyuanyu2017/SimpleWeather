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
    interface Interface_DraggableListAdapter
    {
        int mMobileCellPosition { get; set; }

        void SwapItems(int from, int to);

        void setImage(bool draggable);

        List<string> getCityList();
    }
}