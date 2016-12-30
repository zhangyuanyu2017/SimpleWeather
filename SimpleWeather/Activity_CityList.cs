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
using Android.Graphics.Drawables;
using static Android.App.Usage.UsageEvents;
using Android.Preferences;
using Android.Support.Design.Widget;

namespace SimpleWeather
{
    [Activity(Label = "Activity_CityList")]
    public class Activity_CityList : Activity
    {
        private const int ADD = 1023;

        View_DraggableList list;
        private PopupWindow window;
        RelativeLayout lin_ok, lin_bar;
        List<string> origin;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetStatusBarColor(Color.ParseColor("#DCDCDC"));

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_city_list);
            // Create your application here

            list = FindViewById<View_DraggableList>(Resource.Id.draggable_list_citylist);

            List<string> city = new List<string>(Class_CityList.getCity_User());
            list.Adapter = new DraggableListAdapter(this, city);
            list.SetDraggable(false);

            lin_ok = FindViewById<RelativeLayout>(Resource.Id.lin_citylist_ok);
            lin_bar = FindViewById<RelativeLayout>(Resource.Id.lin_citylist_bar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab_city_list);
            fab.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(Activity_AddCity));
                this.StartActivityForResult(intent,ADD);
                
            };

            ImageView imageview_edit = FindViewById<ImageView>(Resource.Id.imageview_citylist_edit);
            imageview_edit.Click += delegate
            {
                list.SetDraggable(true);
                lin_ok.Visibility = ViewStates.Visible;
                lin_bar.Visibility = ViewStates.Gone;
                origin = new List<string>(Class_CityList.getCity_User());
                Toast.MakeText(this, "Long Press to Change Order", ToastLength.Short).Show();
                //showPopwindow();
            };

            ImageView imageview_back = FindViewById<ImageView>(Resource.Id.imageview_citylist_back);
            imageview_back.Click += delegate
            {
                if (Class_CityList.City_User.Count == 0)
                {
                    Intent intent = new Intent(this, typeof(Activity_AddCity));
                    StartActivity(intent);
                }
                else
                {
                    SetResult(Result.Ok);
                    Finish();
                }
            };

            Button button_ok = FindViewById<Button>(Resource.Id.button_citylist_ok);
            button_ok.Click += delegate
            {
                list.SetDraggable(false);
                origin = new List<string>(list.getCityList());
                Class_CityList.setCity_User(origin);
                lin_ok.Visibility = ViewStates.Gone;
                lin_bar.Visibility = ViewStates.Visible;

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutInt("Size", origin.Count());
                for (int i = 1; i <= origin.Count(); i++)
                {
                    editor.PutString("City_" + i, origin[i-1]);
                    editor.PutInt("CityId_" + i, Class_CityList.City_All[i - 1].id);
                }
                editor.Apply();
            };


            Button button_cancel = FindViewById<Button>(Resource.Id.button_citylist_cancel);
            button_cancel.Click += delegate
            {
                list.Adapter = new DraggableListAdapter(this, origin);
                list.SetDraggable(false);
                lin_ok.Visibility = ViewStates.Gone;
                lin_bar.Visibility = ViewStates.Visible;
            };
        }

        private void showPopwindow()
        {
            // 利用layoutInflater获得View
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.popwindow_citylist, null);

            // 下面是两种方法得到宽度和高度 getWindow().getDecorView().getWidth()
            window = new PopupWindow(view, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);

            // 设置popWindow弹出窗体可点击，这句话必须添加，并且是true
            window.Focusable = true;


            // 实例化一个ColorDrawable颜色为半透明
            Color c = new Color(176, 00, 00, 00);
            ColorDrawable dw = new ColorDrawable(c);
            //window.SetBackgroundDrawable(dw);
            window.OutsideTouchable = true;

            window.SetTouchInterceptor(new MyTouchListener());

            // 设置popWindow的显示和消失动画
            window.AnimationStyle = (Resource.Style.mypopwindow_anim_style);
            // 在底部显示
            window.ShowAtLocation(this.FindViewById(Resource.Id.imageview_citylist_edit), GravityFlags.Top, 0, 0);

            // 这里检验popWindow里的button是否可以点击
            Button button_ok = view.FindViewById<Button>(Resource.Id.button_popwindow_citylist_ok);
            button_ok.Click += delegate
            {
                list.SetDraggable(false);
                Class_CityList.setCity_User(list.getCityList());
                window.Dismiss();
            };


            Button button_cancel = view.FindViewById<Button>(Resource.Id.button_popwindow_citylist_cancel);
            button_cancel.Click += delegate
            {
                List<string> city = Class_CityList.getCity_User();
                list.Adapter = new DraggableListAdapter(this, city);
                list.SetDraggable(false);
                window.Dismiss();
            };

        }

        public class MyTouchListener: Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                
                if(e.Action ==MotionEventActions.Outside ){  return true;  }  
                else return false;
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
                        SetResult(Result.Ok);
                        Finish();
                        //List<string> city = new List<string>(Class_CityList.getCity_User());
                        //list.Adapter = new DraggableListAdapter(this, city);
                        //list.SetDraggable(false);
                        break;
                }

            }
        }
    }
}