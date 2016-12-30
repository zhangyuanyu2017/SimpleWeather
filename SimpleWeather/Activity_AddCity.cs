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
using Java.Lang;
using Android.Preferences;
using Android.Graphics;

namespace SimpleWeather
{
    [Activity(Label = "Activity_AddCity")]
    public class Activity_AddCity : Activity
    {
        SimpleAdapter adapter;

        List<Class_CityInfo> allData = Class_CityList.City_All;

        List<IDictionary<string, object>> mData = new List<IDictionary<string, object>>();

        EditText editTextSearch;
        ImageView imageViewDelete;
        ListView list;

        bool check = false;

        Handler handler = new Handler();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetStatusBarColor(Color.ParseColor("#DCDCDC"));
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_add_city);
            // Create your application here

            var image = FindViewById<ImageView>(Resource.Id.imageview_add_return);
            image.Click += delegate
            {
                SetResult(Result.Canceled);
                Finish();
            };

            setAdapter();

            setTextChange();
        }

        private void setAdapter()
        {
            list = FindViewById<ListView>(Resource.Id.listview_add_city);

            adapter = new SimpleAdapter(this, mData, Resource.Layout.item_listview_add_city, 
                new string[] { "add" }, new int[] { Resource.Id.textview_item_listview });
                
            list.Adapter = adapter;

            list.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                string select = (mData[e.Position])["add"].ToString();
                int id = int.Parse((mData[e.Position])["id"].ToString());

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                int num = prefs.GetInt("Size",0);
                num++;
                editor.PutInt("Size", num);
                editor.PutString("City_" + num, select);
                editor.PutInt("CityId_" + num, id);
                editor.Apply();

                List<Class_CityInfo> User = new List<Class_CityInfo>(Class_CityList.City_User);
                if (User.Count() == 0)
                {
                    Class_CityInfo item_city = new Class_CityInfo();
                    item_city.name = select;
                    item_city.id = id;
                    User.Add(item_city);
                    Class_CityList.City_User = User;

                    Console.WriteLine("In ActivityAdd, id = " + id);

                    Intent intent = new Intent(this, typeof(Activity_Main));
                    StartActivity(intent);
                }
                else
                {
                    Class_CityInfo item_city = new Class_CityInfo();
                    item_city.name = select;
                    item_city.id = id;
                    User.Add(item_city);
                    Class_CityList.City_User = User;

                    SetResult(Result.Ok);
                    Finish();
                }
                //editTextSearch = FindViewById<EditText>(Resource.Id.edittext_add_city);
                //editTextSearch.Text = select;
                //list.Visibility = ViewStates.Gone;
                check = true;
            };
        }

        private void setTextChange()
        {
            editTextSearch = FindViewById<EditText>(Resource.Id.edittext_add_city);
            //imageViewDelete = FindViewById<ImageView>(Resource.Id.imageview_delete_text);
            editTextSearch.AfterTextChanged += (s, e) =>
            {
                //if (editTextSearch.Text.Length > 0) imageViewDelete.Visibility = ViewStates.Visible;
                //else imageViewDelete.Visibility = ViewStates.Gone;
                handler.Post(new Runnable(() =>
                {
                    if (check == true)
                    {
                        check = false;
                    }
                    else
                    {
                        list.Visibility = ViewStates.Visible;
                        string data = editTextSearch.Text;
                        mData.Clear();
                        getSubData(data);
                        //  adapter.NotifyDataSetChanged();
                        adapter = new SimpleAdapter(this, mData, Resource.Layout.item_listview_add_city, 
                            new string[] { "add" }, new int[] { Resource.Id.textview_item_listview });
                        list.Adapter = adapter;
                    }
                }));
            };
        }

        private void getSubData(string data)
        {
            if (data.Equals("") || data == null) return;
            int length = allData.Count();
            int num = 0;
            for (int i = 0; i < length; i++)
            {
                if (num > 50) break;
                if (allData[i].name.Contains(data))
                {
                    IDictionary<string, object> item = new JavaDictionary<string, object>();
                    item.Add("add", allData[i].name);
                    item.Add("id", allData[i].id);
                    mData.Add(item);
                    num++;
                }
            }
        }
    }
}