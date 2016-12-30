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
    public class DraggableListAdapter : BaseAdapter, Interface_DraggableListAdapter, View.IOnClickListener
    {
        public List<string> Items { get; set; }

        private bool draggable = true;

        public int mMobileCellPosition { get; set; }

        Activity context;

        public DraggableListAdapter(Activity context, List<string> items) : base()
        {
            Items = items;
            this.context = context;
            mMobileCellPosition = int.MinValue;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return Items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View cell = convertView;
            if (cell == null)
            {
                cell = context.LayoutInflater.Inflate(Resource.Layout.item_draggablelist_citylist, parent, false);//改变导入的layout？
                cell.SetMinimumHeight(150);
                //cell.SetBackgroundColor(Color.DarkViolet);
            }

            var text = cell.FindViewById<TextView>(Resource.Id.textview_item_city_list);
            if (text != null)
            {
                text.Text = Items[position];
            }
            var delete = cell.FindViewById<ImageView>(Resource.Id.imageview_item_delete_city_list);
            //var change = cell.FindViewById<ImageView>(Resource.Id.move);
            if (draggable)
            {
                delete.Visibility = ViewStates.Visible; //change.Visibility = ViewStates.Visible;
            }
            else
            {
                delete.Visibility = ViewStates.Gone; //change.Visibility = ViewStates.Gone;
            }
            delete.SetOnClickListener(this);
            //delete.Click += delegate{
            //    Console.WriteLine(position);
            //    
            //    NotifyDataSetChanged();
            //};

            cell.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
            cell.TranslationY = 0;

            return cell;
        }

        public void OnClick(View v)
        {
            var item = (v.Parent as View);
            var Text = item.FindViewById<TextView>(Resource.Id.textview_item_city_list).Text;
            Items.Remove(Text.ToString());
            NotifyDataSetChanged();
            Toast.MakeText(this.context, "删除Text:" + Text.ToString(), ToastLength.Short).Show();
        }

        public override int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public void SwapItems(int indexOne, int indexTwo)
        {
            var oldValue = Items[indexOne];
            Items[indexOne] = Items[indexTwo];
            Items[indexTwo] = oldValue;
            mMobileCellPosition = indexTwo;
            NotifyDataSetChanged();
        }

        public void setImage(bool draggable0)
        {
            draggable = draggable0;
            NotifyDataSetChanged();
        }

        public List<string> getCityList()
        {
            return Items;
        }
    }
}