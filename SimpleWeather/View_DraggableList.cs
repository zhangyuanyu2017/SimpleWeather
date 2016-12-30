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
using Android.Util;
using Android.Graphics;
using Android.Animation;
using Android.Graphics.Drawables;

namespace SimpleWeather
{
    class View_DraggableList : ListView, ITypeEvaluator, GestureDetector.IOnGestureListener
    {
        //bool _reorderingEnabled = true;

        //public bool ReorderingEnabled
        //{
        //    get
        //    {
        //        return _reorderingEnabled;
        //    }
        //    set
        //    {
        //        if (!value)
        //        {
        //            ItemLongClick -= HandleItemLongClick;
        //        }
        //        else
        //        {
        //            ItemLongClick += HandleItemLongClick;
        //        }
        //        _reorderingEnabled = value;
        //    }
        //}

        const int LINE_THICKNESS = 15;
        const int INVALID_ID = -1;
        const int INVALID_POINTER_ID = -1;

        int mLastEventY = -1;
        int mDownY = -1;
        int mDownX = -1;
        int mTotalOffset = 0;
        int mActivePointerId = INVALID_POINTER_ID;

        bool mCellIsMobile = false;
        public bool Draggable { get; set; } = true;

        long mAboveItemId = INVALID_ID;
        long mMobileItemId = INVALID_ID;
        long mBelowItemId = INVALID_ID;

        View mobileView;
        Rect mHoverCellCurrentBounds;
        Rect mHoverCellOriginalBounds;
        BitmapDrawable mHoverCell;
        GestureDetector dectector;

        public View_DraggableList(Context context) : base(context)
        {
            init(context);
        }

        public View_DraggableList(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            init(context);
        }

        public View_DraggableList(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context);
        }

        public void init(Context context)
        {
            dectector = new GestureDetector(this);
            ItemLongClick += HandleItemLongClick;
        }

        #region Handlers


        void HandleItemLongClick(object sender, ItemLongClickEventArgs e)
        {
        }


        void HandleHoverAnimatorUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
        {
            Invalidate();
        }

        void HandleHoverAnimationStart(object sender, EventArgs e)
        {
            Enabled = false;
        }

        void HandleHoverAnimationEnd(object sender, EventArgs e)
        {
            mAboveItemId = INVALID_ID;
            mMobileItemId = INVALID_ID;
            mBelowItemId = INVALID_ID;
            mHoverCell = null;
            Enabled = true;
            Invalidate();

            mobileView.Visibility = ViewStates.Visible;
        }

        #endregion

        #region IOnGestureListener Implementation

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
            if (!Draggable) return;
            mTotalOffset = 0;

            int position = PointToPosition(mDownX, mDownY);

            if (position < 0 || !LongClickable)
                return;

            int itemNum = position - FirstVisiblePosition;

            View selectedView = GetChildAt(itemNum);
            mMobileItemId = Adapter.GetItemId(position); 
            mHoverCell = GetAndAddHoverView(selectedView);
            selectedView.Visibility = ViewStates.Invisible; 

            mCellIsMobile = true;

            UpdateNeighborViewsForID(mMobileItemId);
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        #endregion

        #region Bitmap Drawable Creation

        BitmapDrawable GetAndAddHoverView(View v)
        {

            int w = v.Width;
            int h = v.Height;
            int top = v.Top;
            int left = v.Left;

            Bitmap b = GetBitmapWithBorder(v);

            BitmapDrawable drawable = new BitmapDrawable(Resources, b);

            mHoverCellOriginalBounds = new Rect(left, top, left + w, top + h);
            mHoverCellCurrentBounds = new Rect(mHoverCellOriginalBounds);

            drawable.SetBounds(left, top, left + w, top + h);

            return drawable;
        }

        static Bitmap GetBitmapWithBorder(View v)
        {
            Bitmap bitmap = GetBitmapFromView(v);
            Canvas can = new Canvas(bitmap);

            Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

            Paint paint = new Paint();
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = LINE_THICKNESS;
            paint.Color = Color.Red;

            Paint alphaPaint = new Paint();
            alphaPaint.Alpha = 2;

            can.DrawBitmap(bitmap, 0, 0, alphaPaint);
            //can.DrawRect(rect, paint);

            return bitmap;
        }

        static Bitmap GetBitmapFromView(View v)
        {
            try
            {
                Bitmap bitmap = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                v.Draw(canvas);
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default(Bitmap);

        }

        #endregion

        void UpdateNeighborViewsForID(long itemID)
        {
            int position = GetPositionForID(itemID);
            mAboveItemId = Adapter.GetItemId(position - 1);
            mBelowItemId = Adapter.GetItemId(position + 1);

        }

        public View GetViewForID(long itemID)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                View v = GetChildAt(i);
                int position = FirstVisiblePosition + i;
                long id = Adapter.GetItemId(position);
                if (id == itemID)
                {
                    return v;
                }
            }
            return null;
        }

        public int GetPositionForID(long itemID)
        {
            View v = GetViewForID(itemID);
            if (v == null)
            {
                return -1;
            }
            else
            {
                return GetPositionForView(v);
            }
        }


        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            if (mHoverCell != null)
            {
                mHoverCell.Draw(canvas);
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!Draggable) return base.OnTouchEvent(e);
            try
            {
                dectector.OnTouchEvent(e);
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        mDownX = (int)e.GetX();
                        mDownY = (int)e.GetY();
                        mActivePointerId = e.GetPointerId(0);
                        break;
                    case MotionEventActions.Move:
                        if (mActivePointerId == INVALID_POINTER_ID)
                            break;

                        int pointerIndex = e.FindPointerIndex(mActivePointerId);
                        mLastEventY = (int)e.GetY(pointerIndex);
                        int deltaY = mLastEventY - mDownY;


                        if (mCellIsMobile)
                        { // Responsible for moving the bitmap drawable to the touch location
                            Enabled = false;

                            mHoverCellCurrentBounds.OffsetTo(mHoverCellOriginalBounds.Left,
                                mHoverCellOriginalBounds.Top + deltaY + mTotalOffset);
                            mHoverCell.SetBounds(mHoverCellCurrentBounds.Left, mHoverCellCurrentBounds.Top, mHoverCellCurrentBounds.Right, mHoverCellCurrentBounds.Bottom);
                            Invalidate();
                            HandleCellSwitch();
                        }
                        break;
                    case MotionEventActions.Up:
                        TouchEventsEnded();
                        break;
                    case MotionEventActions.Cancel:
                        TouchEventsCancelled();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error ", ex.Message);
                //Console.WriteLine("Error ", ex.StackTrace);
            }

            return base.OnTouchEvent(e);
        }


        void HandleCellSwitch()
        {
            try
            {
                int deltaY = mLastEventY - mDownY;
                int deltaYTotal = mHoverCellOriginalBounds.Top + mTotalOffset + deltaY;

                View belowView = GetViewForID(mBelowItemId);
                View mobileView = GetViewForID(mMobileItemId);
                View aboveView = GetViewForID(mAboveItemId);

                bool isBelow = (belowView != null) && (deltaYTotal > belowView.Top);
                bool isAbove = (aboveView != null) && (deltaYTotal < aboveView.Top);

                if (isBelow || isAbove)
                {
                    View switchView = isBelow ? belowView : aboveView; 

                    var diff = GetViewForID(mMobileItemId).Top - switchView.Top; 
                 
                    ObjectAnimator anim = ObjectAnimator.OfFloat(switchView, "TranslationY", switchView.TranslationY, switchView.TranslationY + diff);
                    anim.SetDuration(100);
                    anim.Start();

                    mMobileItemId = GetPositionForView(switchView);

                    UpdateNeighborViewsForID(mMobileItemId);

                    anim.AnimationEnd += (sender, e) => {
                        ((Interface_DraggableListAdapter)Adapter).SwapItems(GetPositionForView(mobileView), GetPositionForView(switchView));
                    };
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error ", ex.Message);
                //Console.WriteLine("Error ", ex.StackTrace);

            }

        }

        void TouchEventsEnded()
        {
            mobileView = GetViewForID(mMobileItemId);
            if (mCellIsMobile)
            {
                mCellIsMobile = false;
                mActivePointerId = INVALID_POINTER_ID;
                ((DraggableListAdapter)Adapter).mMobileCellPosition = int.MinValue;

                mHoverCellCurrentBounds.OffsetTo(mHoverCellOriginalBounds.Left, mobileView.Top);

                ObjectAnimator hoverViewAnimator = ObjectAnimator.OfObject(mHoverCell, "Bounds", this, mHoverCellCurrentBounds);
                hoverViewAnimator.Update += HandleHoverAnimatorUpdate;
                hoverViewAnimator.AnimationStart += HandleHoverAnimationStart;
                hoverViewAnimator.AnimationEnd += HandleHoverAnimationEnd;
                hoverViewAnimator.Start();
            }
            else
            {
                TouchEventsCancelled();
            }
        }

        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
        {
            var startValueRect = startValue as Rect;
            var endValueRect = endValue as Rect;

            return new Rect(Interpolate(startValueRect.Left, endValueRect.Left, fraction),
                Interpolate(startValueRect.Top, endValueRect.Top, fraction),
                Interpolate(startValueRect.Right, endValueRect.Right, fraction),
                Interpolate(startValueRect.Bottom, endValueRect.Bottom, fraction));
        }

        public int Interpolate(int start, int end, float fraction)
        {
            return (int)(start + fraction * (end - start));
        }

        void TouchEventsCancelled()
        {
            mobileView = GetViewForID(mMobileItemId);
            if (mCellIsMobile)
            {
                mAboveItemId = INVALID_ID;
                mMobileItemId = INVALID_ID;
                mBelowItemId = INVALID_ID;
                mHoverCell = null;
                Invalidate();
            }

            if (mobileView != null)
                mobileView.Visibility = ViewStates.Visible;

            Enabled = true;
            mCellIsMobile = false;
            mActivePointerId = INVALID_POINTER_ID;
        }

        public void SetDraggable(bool d)
        {
            Draggable = d;
            ((Interface_DraggableListAdapter)Adapter).setImage(d);
        }

        public List<string> getCityList()
        {
            return ((Interface_DraggableListAdapter)Adapter).getCityList();
        }
    }
}
