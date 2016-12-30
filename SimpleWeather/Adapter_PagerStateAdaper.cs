using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;
using Android.Support.V4.View;

namespace SimpleWeather
{
    class Adapter_PagerStateAdaper : FragmentStatePagerAdapter
    {
        private List<Fragment> fragmentList = new List<Fragment>();
        private Context context;

        public Adapter_PagerStateAdaper(Context context0,FragmentManager fm,List<Fragment> list) : base(fm)
        {
            fragmentList = list;
            context = context0;
            NotifyDataSetChanged();
        }

        public override int Count
        {
            get
            {
                return fragmentList.Count;
            }
        }

        public override Fragment GetItem(int position)
        {
            return fragmentList[position];
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PagerAdapter.PositionNone;
        }
    }
}