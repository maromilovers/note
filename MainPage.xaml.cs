using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using TouchTracking;
using Xamarin.Forms;

namespace note
{
    public partial class MainPage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 50,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        List<string> lstHiragana = new List<string>();
        List<string> lstKatakana = new List<string>();
        List<string> lstAlpha = new List<string>();

        bool flgHand = false;
        bool flgClear = false;

        enum playMode
        {
            hiragana = 0,
            katakana = 1,
            alpha = 2,
            other = 3
        }

        playMode pMode = playMode.hiragana;

        public MainPage()
        {
            InitializeComponent();

            createList();

            dispExample(true);

            setMode();
        }

        // 画面サイズ取得
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            edText.WidthRequest = width;
            edText.FontSize = width;
            pnlColor.WidthRequest = width;
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            flgHand = false;

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        flgHand = true;

                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();

                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }

        SKPoint ConvertToPixel(TouchTracking.TouchTrackingPoint pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;

            if (flgClear)
            {
                canvas.Clear();
                flgClear = false;
            }

            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

            if (flgHand)
            { 
                SKImage img = args.Surface.Snapshot(); // 画像取得
                //SKPoint point = new SKPoint((float)(canvasView.AnchorX), (float)(canvasView.AnchorY));                
                //canvas.DrawImage(img, point, null);
                SKRect rect = new SKRect((float)canvasView.X, (float)canvasView.Y, (float)canvasView.CanvasSize.Width, (float)canvasView.CanvasSize.Height);
                canvas.DrawImage(img, rect , null);

                inProgressPaths.Clear();
                completedPaths.Clear();
            }
        }

        // 付箋アプリ用
        //private void ibInputType_Clicked(object sender, System.EventArgs e)
        //{
        //    if (flgHand)
        //    {
        //        ibInputType.BackgroundColor = System.Drawing.Color.Aqua;
        //        edText.IsEnabled = true;
        //        flgHand = false;
        //    }
        //    else
        //    {
        //        ibInputType.BackgroundColor = System.Drawing.Color.BlueViolet;
        //        edText.IsEnabled = false;
        //        flgHand = true;
        //    }
        //}

        // Canvasクリア
        private void ibInputType_Clicked(object sender, System.EventArgs e)
        {
            clearCanvas();

        }

        private void clearCanvas()
        {
            inProgressPaths.Clear();
            completedPaths.Clear();

            flgClear = true;

            canvasView.InvalidateSurface();

        }

        #region " 色変更 "

        private void ibRed_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Red;
        }

        private void ibBlack_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Black;
        }

        private void ibOrange_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Orange;
        }

        private void ibYellow_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Yellow;
        }

        private void ibGreen_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Green;
        }

        private void ibBlue_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Blue;
        }

        private void ibIndigo_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Indigo;
        }

        private void ibPurple_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.Purple;
        }

        private void ibWhite_Clicked(object sender, System.EventArgs e)
        {
            paint.Color = SKColors.White;
        }

        #endregion

        // 手本リスト作成
        private void createList()
        {
            string str = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
            foreach (char c in str)
            {
                lstHiragana.Add(c.ToString());
            }

            str = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン";
            foreach (char c in str)
            {
                lstKatakana.Add(c.ToString());
            }

            str = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ";
            foreach (char c in str)
            {
                lstAlpha.Add(c.ToString());
            }
        }

        // 手本表示
        private void dispExample(bool flgInit = false)
        {
            clearCanvas();

            List<string> lst = new List<string>();

            if (pMode == playMode.hiragana) lst = lstHiragana;
            if (pMode == playMode.katakana) lst = lstKatakana;
            if (pMode == playMode.alpha) lst = lstAlpha;

            if (swPaint.IsToggled)
            {
                Random rnd = new Random();

                bool flg = false;

                while (!flg)
                {
                    var txt = lst[rnd.Next(0, lst.Count)];
                    if (edText.Text != txt)
                    {
                        edText.Text = txt;
                        flg = true;
                    }
                }
            }
            else
            {
                if (flgInit)
                {
                    edText.Text = lst[0];
                }
                else
                {
                    edText.Text = lst.IndexOf(edText.Text) == lst.Count - 1 ? lst[0] : lst[lst.IndexOf(edText.Text) + 1]; 
                }
            }
        }

        private void ibNext_Clicked(object sender, EventArgs e)
        {
            dispExample();
        }

        private void ibHiragana_Clicked(object sender, EventArgs e)
        {
            pMode = playMode.hiragana;
            setMode();
            dispExample(true);
        }

        private void ibKatakana_Clicked(object sender, EventArgs e)
        {
            pMode = playMode.katakana;
            setMode();
            dispExample(true);
        }

        private void ibAlpha_Clicked(object sender, EventArgs e)
        {
            pMode = playMode.alpha;
            setMode();
            dispExample(true);
        }

        private void setMode()
        {
            switch (pMode)
            {
                case playMode.hiragana:
                    ibHiragana.IsEnabled = false;
                    ibKatakana.IsEnabled = true;
                    ibAlpha.IsEnabled = true;
                    break;
                case playMode.katakana:
                    ibHiragana.IsEnabled = true;
                    ibKatakana.IsEnabled = false;
                    ibAlpha.IsEnabled = true;
                    break;
                case playMode.alpha:
                    ibHiragana.IsEnabled = true;
                    ibKatakana.IsEnabled = true;
                    ibAlpha.IsEnabled = false;
                    break;
                default:
                    ibHiragana.IsEnabled = true;
                    ibKatakana.IsEnabled = true;
                    ibAlpha.IsEnabled = true;
                    break;
            }
        }
    }
}
