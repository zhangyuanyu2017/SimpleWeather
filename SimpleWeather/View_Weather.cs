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
using Java.Lang;

namespace SimpleWeather
{
    class View_Weather : View 
    {

        private Color bgColor = Color.Rgb(Integer.ParseInt("4d", 16), Integer.ParseInt("af", 16), Integer.ParseInt("ea", 16));

        private Color singleColumnFillColor = Color.Rgb(Integer.ParseInt("e7", 16),
                Integer.ParseInt("e7", 16), Integer.ParseInt("e9", 16));

        private Color doubleColumnFillColor = Color.Rgb(Integer.ParseInt("4d", 16),
                Integer.ParseInt("af", 16), Integer.ParseInt("ea", 16));

        private Color fillDownColor = Color.Rgb(Integer.ParseInt("45", 16), Integer.ParseInt("64", 16), Integer.ParseInt("bf", 16));

        private Color xyLineColor = Color.Rgb(Integer.ParseInt("a9", 16),
                Integer.ParseInt("d8", 16), Integer.ParseInt("f5", 16));

        private Color chartLineColor = Color.White;

        private Color shadowLineColor = Color.Rgb(Integer.ParseInt("1a", 16),
                Integer.ParseInt("49", 16), Integer.ParseInt("84", 16));

        private string yUnit = "";

        private bool isDrawY = false;

        private bool isDrawX = true;

        private bool isDrawInsideX = false;

        private bool isDrawInsedeY = false;

        private bool isFillDown = false;

        private bool isFillUp = false;

        private bool isAppendX = false;

        private bool isDemo = false;

        private bool isSetYTitle = false;

        private int ScreenX;

        private int ScreenY;

        private int paddingTop = 30;

        private int paddingLeft = 70;

        private int paddingRight = 30;

        private int paddingDown = 50;

        private int numberOfX = 6;

        private int numberOfY = 5;

        private int appendXLength = 10;

        private float maxNumber = 0;

        private float minNumber = 0;

        private List<List<float>> pointList;

        private List<string> bitmapList;

        private List<Color> lineColorList;

        private List<string> titleXList;

        private List<string> titleYList;

        public View_Weather(Context context) : base(context)
        {
            demo();
        }

        public View_Weather(Context context, IAttributeSet attrs) : base(context, attrs, 0) 
        {
            demo();
        }

        private void demo()
        {
            if (!isDemo)
            {
                return;
            }
            pointList = new List<List<float>>();
            titleXList = new List<string>();
            lineColorList = new List<Color>();
            lineColorList.Add(Color.White);
            lineColorList.Add(Color.Green);
            lineColorList.Add(Color.Yellow);
            
            for (int i = 0; i < 3; i++)
            {
                List<float> pointInList = new List<float>();
                for (int j = 0; j < 6; j++)
                {
                    Random r = new Random();
                    float z = r.Next(1) * 100;
                    pointInList.Add(z);
                    titleXList.Add("12." + (i + 1) + "1");
                }
                pointList.Add(pointInList);
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int measuredHeight = measureHeight(heightMeasureSpec);

            int measuredWidth = measureWidth(widthMeasureSpec);

            SetMeasuredDimension(measuredWidth, measuredHeight);

            ScreenX = measuredWidth;

            ScreenY = measuredHeight;
        }

        private int measureHeight(int measureSpec)
        {

            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            int result = 300;
            if (specMode == MeasureSpecMode.AtMost)
            {

                result = specSize;
            }
            else if (specMode == MeasureSpecMode.Exactly)
            {

                result = specSize;
            }

            return result;
        }

        private int measureWidth(int measureSpec)
        {
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            int result = 3000;
            //IWindowManager wm = Context.GetSystemService(Context.WindowService);

            if (specMode == MeasureSpecMode.AtMost)
            {
                result = specSize;
            }

            else if (specMode == MeasureSpecMode.Exactly)
            {

                result = specSize;
            }
            Console.WriteLine(specSize);
            return result;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            maxNumber = -1000;
            minNumber = 1000;
            List<Point> listX = initNumberOfX();
            List<Point> listY = initNumberOfY();
            canvas.DrawColor(bgColor);// ±³¾°É«
            fillColor(listX, canvas);

            
            Paint paint = new Paint();
            paint.Color = xyLineColor;
            if (isDrawX)
            {
                int appendX = 0;
                if (isAppendX)
                {
                    appendX = appendXLength;
                }
                canvas.DrawLine(paddingLeft - appendX, paddingTop + listY[0].Y, listY[0].X + paddingLeft, paddingTop + listY[0].Y, paint);
            }
            if (isDrawY)
            {
                canvas.DrawLine(listX[0].X, paddingTop, listX[0].X, listX[0].Y + paddingTop, paint);
            }

            if (isDrawInsedeY)
            {
                for (int i = 0; i < listX.Count(); i++) 
                {
                    Point point = listX[i];
                    if (!isDrawX)
                    {
                        isDrawX = !isDrawX;
                        continue;
                    }
                    canvas.DrawLine(point.X, paddingTop, point.X, point.Y + paddingTop, paint);
                }
            }
            if (isDrawInsideX)
            {
                for (int i = 0; i < listY.Count(); i++)
                {
                    Point point = listY[i];
                    if (!isDrawY)
                    {
                        isDrawY = !isDrawY;
                        continue;
                    }
                    int appendX = 0;
                    if (isAppendX)
                    {
                        appendX = appendXLength;
                    }
                    canvas.DrawLine(paddingLeft - appendX, paddingTop + point.Y, point.X + paddingLeft,
                            paddingTop + point.Y, paint);
                }
            }

            setYTitle(listY, canvas);

            List<List<Point>> positionList = countListPosition(listX);
            drawFill(canvas, positionList);
            drawChart(canvas, positionList);
            drawCicle(canvas, positionList);

            setXTitle(listX, canvas);
        }

        private List<Point> initNumberOfX()
        {
            int num = (ScreenX - paddingLeft - paddingRight) / (numberOfX - 1);
            List<Point> list = new List<Point>();
            for (int i = 0; i < numberOfX; i++)
            {
                Point point = new Point();
                point.Y = ScreenY - paddingDown - paddingTop;
                point.X = paddingLeft + num * i;
                list.Add(point);
            }
            return list;
        }

        private List<Point> initNumberOfY()
        {
            int num = (ScreenY - paddingDown - paddingTop) / (numberOfY - 1);
            List<Point> list = new List<Point>();
            for (int i = 0; i < numberOfY; i++)
            {
                Point point = new Point();
                point.X = ScreenX - paddingLeft - paddingRight;
                point.Y = ScreenY - paddingDown - paddingTop - num * i;
                list.Add(point);
            }
            return list;
        }

        private void fillColor(List<Point> listX, Canvas canvas)
        {
            Paint paint = new Paint();
            paint.SetStyle(Paint.Style.Fill);
            for (int i = 0; i < numberOfX - 1; i++)
            {
                if (i % 2 == 0)
                {
                    paint.Color = singleColumnFillColor;
                    paint.Alpha=102;
                }
                else
                {
                    paint.Color = doubleColumnFillColor;
                    paint.Alpha=255;
                }
                canvas.DrawRect(listX[i].X, paddingTop, listX[i + 1].X, ScreenY - paddingDown, paint);
            }
        }

        private void setYTitle(List<Point> listY, Canvas canvas)
        {
            Paint paint = new Paint();
            paint.Color = Color.White;
            if (pointList == null)
            {
                titleYList = new List<string>();
                for (int i = 1; i <= numberOfY; i++)
                {
                    titleYList.Add((100 / i)+"");
                }
            }//For the test
            else
            {
                for (int i = 0; i < pointList.Count(); i++)
                {
                    for (int j = 0; j < pointList[i].Count(); j++)
                    {
                        if ((pointList[i])[j] > maxNumber)
                        {
                            maxNumber = pointList[i][j];
                        }
                        if ((pointList[i][j] < minNumber))
                        {
                            minNumber = pointList[i][j];
                        }
                    }
                }
                if (maxNumber >= 0) maxNumber = maxNumber + maxNumber / 3 + 1;
                else maxNumber = maxNumber - maxNumber / 3 + 1;
                if (minNumber >= 0) minNumber = minNumber - minNumber / 3 - 1;
                else minNumber = minNumber + minNumber / 3 - 1;
                titleYList = new List<string>();
                for (int i = 0; i < numberOfY; i++)
                {
                    titleYList.Add(((int)(minNumber + i * ((maxNumber-minNumber) / (numberOfY - 1))))+"");
                }
            }
            for (int i = 0; i < numberOfY; i++)
            {
                int appendX = 0;
                if (isAppendX)
                {
                    appendX = appendXLength;
                }
                if (i != 0)
                {
                    if (isSetYTitle) canvas.DrawText(titleYList[i], paddingLeft - appendX - paddingLeft / 3,
                         paddingTop + listY[i].Y, paint);
                }
                else
                {
                    if (isSetYTitle) canvas.DrawText(titleYList[i] + yUnit, paddingLeft - appendX - paddingLeft / 3,
                          paddingTop + listY[i].Y, paint);
                }
            }
        }

        private List<List<Point>> countListPosition(List<Point> listX)
        {
            List<List<Point>> positionList = new List<List<Point>>();
            if (pointList == null)
            {
                pointList = new List<List<float>>();
                List<float> pointInList = new List<float>();
                for (int i = 0; i < numberOfX; i++)
                {
                    pointInList.Add(0f);
                }
                pointList.Add(pointInList);
            }//for the test
            for (int i = 0; i < pointList.Count(); i++)
            {
                List<Point> positionInList = new List<Point>();
                for (int j = 0; j < pointList[i].Count(); j++)
                {
                    Point point = new Point();
                    float z = pointList[i][j];
                    point.X = listX[j].X;
                    point.Y = listX[j].Y + paddingTop
                            - (int)((listX[j].Y) * (float)(z - minNumber) / ((maxNumber - minNumber)));
                    positionInList.Add(point);
                }
                positionList.Add(positionInList);
            }
            return positionList;
        }

        private void drawFill(Canvas canvas, List<List<Point>> positionList)
        {
            if (!isFillDown)
            {
                return;
            }
            Paint paint = new Paint();
            paint.AntiAlias = true;
            paint.Color = fillDownColor;
            paint.Alpha = 76;
            for (int i = 0; i < positionList.Count(); i++)
            {
                Path path = new Path();
                path.MoveTo(paddingLeft, ScreenY - paddingDown);
                for (int j = 0; j < positionList[i].Count(); j++)
                {
                    path.LineTo(positionList[i][j].X, positionList[i][j].Y);
                }
                path.LineTo(ScreenX - paddingRight, ScreenY - paddingDown);
                path.Close();
                canvas.DrawPath(path, paint);
            }
        }

        private void drawChart(Canvas canvas, List<List<Point>> positionList)
        {
            Paint paint = new Paint();
            paint.AntiAlias = (true);
            paint.Color = chartLineColor;
            paint.StrokeWidth = 3;// Width of line
            Paint shadowPaint = new Paint();
            shadowPaint.AntiAlias = true;
            shadowPaint.Color = shadowLineColor;
            shadowPaint.StrokeWidth = 1;// 
            shadowPaint.Alpha = 178;
            for (int i = 0; i < positionList.Count(); i++)
            {
                if (lineColorList != null && lineColorList[i] != null)
                {
                    paint.Color = (lineColorList[i]);
                }
                for (int j = 0; j < positionList[i].Count() - 1; j++)
                {
                    canvas.DrawLine(positionList[i][j].X, positionList[i][j].Y + 2,
                            positionList[i][j+1].X, positionList[i][j+1].Y + 2,
                            shadowPaint);
                    canvas.DrawLine(positionList[i][j].X, positionList[i][j].Y,
                            positionList[i][j+1].X, positionList[i][j+1].Y, paint);
                }
            }
        }

        private void drawCicle(Canvas canvas, List<List<Point>> positionList)
        {
            Paint paint = new Paint();
            paint.AntiAlias = true;
            paint.Color = Color.Green;
            List<Bitmap> Lists = new List<Bitmap>();
            Paint TextPaint = new Paint();
            TextPaint.Color = Color.White;
            TextPaint.TextSize = 25;
            // Bitmap bitmap = BitmapFactory.decodeResource(getResources(),
            // R.drawable.comm_chart_point);
            int resouceId = 0;
            for (int i = 0; i < positionList.Count(); i++)
            {
                // canvas.drawCircle(positionList.get(i).x, positionList.get(i).y,
                // 7, paint);
                for (int j = 0; j < positionList[i].Count; j++)
                {
                    if (bitmapList != null && bitmapList[i] != null)
                    {
                        if (bitmapList[j].Equals("01d")) resouceId = Resource.Drawable.Drawable_sunny_d;
                        if (bitmapList[j].Equals("01n")) resouceId = Resource.Drawable.Drawable_sunny_n;
                        if (bitmapList[j].Equals("02d")) resouceId = Resource.Drawable.Drawable_b_d;
                        if (bitmapList[j].Equals("02n")) resouceId = Resource.Drawable.Drawable_b_n;
                        if (bitmapList[j].Equals("03d")) resouceId = Resource.Drawable.Drawable_c_d;
                        if (bitmapList[j].Equals("03d")) resouceId = Resource.Drawable.Drawable_c_n;
                        if (bitmapList[j].Equals("04n")) resouceId = Resource.Drawable.Drawable_d_d;
                        if (bitmapList[j].Equals("04n")) resouceId = Resource.Drawable.Drawable_d_n;
                        if (bitmapList[j].Equals("09d")) resouceId = Resource.Drawable.Drawable_e_d;
                        if (bitmapList[j].Equals("09n")) resouceId = Resource.Drawable.Drawable_e_n;
                        if (bitmapList[j].Equals("10d")) resouceId = Resource.Drawable.Drawable_f_d;
                        if (bitmapList[j].Equals("10n")) resouceId = Resource.Drawable.Drawable_f_n;
                        if (bitmapList[j].Equals("11d")) resouceId = Resource.Drawable.Drawable_g_d;
                        if (bitmapList[j].Equals("11n")) resouceId = Resource.Drawable.Drawable_g_n;
                        if (bitmapList[j].Equals("13d")) resouceId = Resource.Drawable.Drawable_h_d;
                        if (bitmapList[j].Equals("13n")) resouceId = Resource.Drawable.Drawable_h_n;
                        if (bitmapList[j].Equals("50d")) resouceId = Resource.Drawable.Drawable_i_d;
                        if (bitmapList[j].Equals("50n")) resouceId = Resource.Drawable.Drawable_i_n;
                        Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resouceId);
                        Lists.Add(bitmap);
                    }
                    else
                    {
                        resouceId = Resource.Drawable.Drawable_Chart_Point;
                        Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resouceId);
                        Lists.Add(bitmap);
                        //resouceId = Resource.Drawable.Drawable_sunny_d;
                    }
                }
                
                for (int j = 0; j < positionList[i].Count(); j++)
                {
                    canvas.DrawBitmap(Lists[j], positionList[i][j].X + 0.5f - Lists[j].Width
                            / 2, positionList[i][j].Y + 0.5f - Lists[j].Height / 2, paint);
                    canvas.DrawText(pointList[i][j] + "¡ã", positionList[i][j].X + 0.5f,
                        positionList[i][j].Y - Lists[j].Height / 2, TextPaint);
                }
            }
        }

        private void setXTitle(List<Point> listX, Canvas canvas)
        {
            Paint paint = new Paint();
            paint.Color = Color.White;
            if (titleXList == null)
            {
                titleXList = new List<string>();
                for (int i = 1; i <= numberOfX; i++)
                {
                    titleXList.Add("title" + i);
                }
            }
            for (int i = 0; i < numberOfX; i++)
            {
                canvas.Save();
                canvas.Rotate(30, listX[i].X, listX[i].Y + paddingTop + paddingDown / 2);
                canvas.DrawText(titleXList[i], listX[i].X,
                        listX[i].Y + paddingTop + paddingDown / 2, paint);
                canvas.Restore();
            }
        }

        public bool getDrawY()
        {
            return isDrawY;
        }

        public void setDrawY(bool isDrawY)
        {
            this.isDrawY = isDrawY;
        }

        public bool getDrawX()
        {
            return isDrawX;
        }

        public void setDrawX(bool isDrawX)
        {
            this.isDrawX = isDrawX;
        }

        public bool getFillDown()
        {
            return isFillDown;
        }

        public void setFillDown(bool isFillDown)
        {
            this.isFillDown = isFillDown;
        }

        public bool getFillUp()
        {
            return isFillUp;
        }

        public void setFillUp(bool isFillUp)
        {
            this.isFillUp = isFillUp;
        }

        public int getScreenX()
        {
            return ScreenX;
        }

        public void setScreenX(int screenX)
        {
            ScreenX = screenX;
        }

        public int getScreenY()
        {
            return ScreenY;
        }

        public void setScreenY(int screenY)
        {
            ScreenY = screenY;
        }

        public int getNumberOfX()
        {
            return numberOfX;
        }

        public void setNumberOfX(int numberOfX)
        {
            this.numberOfX = numberOfX;
        }

        public int getNumberOfY()
        {
            return numberOfY;
        }

        public void setNumberOfY(int numberOfY)
        {
            this.numberOfY = numberOfY;
        }

        public bool getDrawInsideX()
        {
            return isDrawInsideX;
        }

        public void setDrawInsideX(bool isDrawInsideX)
        {
            this.isDrawInsideX = isDrawInsideX;
        }

        public bool getDrawInsedeY()
        {
            return isDrawInsedeY;
        }

        public void setDrawInsedeY(bool isDrawInsedeY)
        {
            this.isDrawInsedeY = isDrawInsedeY;
        }

        public bool getAppendX()
        {
            return isAppendX;
        }

        public void setAppendX(bool isAppendX)
        {
            this.isAppendX = isAppendX;
        }

        public int getPaddingTop()
        {
            return paddingTop;
        }

        public void setPaddingTop(int paddingTop)
        {
            this.paddingTop = paddingTop;
        }

        public int getPaddingLeft()
        {
            return paddingLeft;
        }

        public void setPaddingLeft(int paddingLeft)
        {
            this.paddingLeft = paddingLeft;
        }

        public int getPaddingRight()
        {
            return paddingRight;
        }

        public void setPaddingRight(int paddingRight)
        {
            this.paddingRight = paddingRight;
        }

        public int getPaddingDown()
        {
            return paddingDown;
        }

        public void setPaddingDown(int paddingDown)
        {
            this.paddingDown = paddingDown;
        }

        public int getAppendXLength()
        {
            return appendXLength;
        }

        public void setAppendXLength(int appendXLength)
        {
            this.appendXLength = appendXLength;
        }

        public float getMaxNumber()
        {
            return maxNumber;
        }

        public void setMaxNumber(float maxNumber)
        {
            this.maxNumber = maxNumber;
        }

        public List<string> getTitleXList()
        {
            return titleXList;
        }

        public void setTitleXList(List<string> titleXList)
        {
            this.titleXList = titleXList;
        }

        public List<string> getTitleYList()
        {
            return titleYList;
        }

        public void setTitleYList(List<string> titleYList)
        {
            this.titleYList = titleYList;
        }

        public int getBgColor()
        {
            return bgColor;
        }

        public void setBgColor(Color bgColor)
        {
            this.bgColor = bgColor;
        }

        public int getSingleColumnFillColor()
        {
            return singleColumnFillColor;
        }

        public void setSingleColumnFillColor(Color singleColumnFillColor)
        {
            this.singleColumnFillColor = singleColumnFillColor;
        }

        public int getDoubleColumnFillColor()
        {
            return doubleColumnFillColor;
        }

        public void setDoubleColumnFillColor(Color doubleColumnFillColor)
        {
            this.doubleColumnFillColor = doubleColumnFillColor;
        }

        public int getFillDownColor()
        {
            return fillDownColor;
        }

        public void setFillDownColor(Color fillDownColor)
        {
            this.fillDownColor = fillDownColor;
        }

        public int getXyLineColor()
        {
            return xyLineColor;
        }

        public void setXyLineColor(Color xyLineColor)
        {
            this.xyLineColor = xyLineColor;
        }

        public int getShadowLineColor()
        {
            return shadowLineColor;
        }

        public void setShadowLineColor(Color shadowLineColor)
        {
            this.shadowLineColor = shadowLineColor;
        }

        public int getChartLineColor()
        {
            return chartLineColor;
        }

        public void setChartLineColor(Color chartLineColor)
        {
            this.chartLineColor = chartLineColor;
        }

        public string getyUnit()
        {
            return yUnit;
        }

        public void setyUnit(string yUnit)
        {
            this.yUnit = yUnit;
        }

        public List<List<float>> getPointList()
        {
            return pointList;
        }

        public void setPointList(List<List<float>> pointList)
        {
            this.pointList = pointList;
        }

        public List<string> getBitmapList()
        {
            return bitmapList;
        }

        public void setBitmapList(List<string> bitmapList)
        {
            this.bitmapList = bitmapList;
        }

        public List<Color> getLineColorList()
        {
            return lineColorList;
        }

        public void setLineColorList(List<Color> lineColorList)
        {
            this.lineColorList = lineColorList;
        }

        public void setIsSetYTitle(bool b)
        {
            this.isSetYTitle = b;
        }

        public bool getIsSetYTitle()
        {
            return this.isSetYTitle;
        }
    }
}