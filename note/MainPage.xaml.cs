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

        // 塗りつぶし線の設定
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
        List<string> lstOther = new List<string>();

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

        bool flgRand = false;

        string strInput = "";

        int cntList = 0;

        public MainPage()
        {
            InitializeComponent();

            // 設定値読込
            readSetting();

            // 文字リスト作成
            createList();

            // お手本表示
            dispExample(true);

            // 設定画面
            setMode();
        }

        // 画面サイズ取得
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            edText.WidthRequest = width;
            edText.FontSize = width - 20;
            pnlColor.WidthRequest = width;
            grdDetail.WidthRequest = width - 40;
            grdDetail.HeightRequest = height - 150;
            imgDetail.WidthRequest = width - 40;
            imgDetail.HeightRequest = height - 150;
            layoutMain.WidthRequest = width;
            layoutMain.HeightRequest = height;
        }

        // タッチ動作
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

        // 設定読込
        private void readSetting()
        {
            if (Application.Current.Properties.ContainsKey("disp"))
            {
                switch(int.Parse(Application.Current.Properties["disp"].ToString()))
                {
                    case 0:
                        pMode = playMode.hiragana;
                        break;
                    case 1:
                        pMode = playMode.katakana;
                        break;
                    case 2:
                        pMode = playMode.alpha;
                        break;
                    case 3:
                        pMode = playMode.other;
                        break;
                    default:
                        pMode = playMode.hiragana;
                        break;
                }
            }

            if (Application.Current.Properties.ContainsKey("mode"))
            {
                flgRand = bool.Parse(Application.Current.Properties["mode"].ToString());
            }

            if (Application.Current.Properties.ContainsKey("input"))
            {
                strInput = Application.Current.Properties["input"].ToString();
            }

            txtInput.Text = strInput;

            setMode();
            setRandom();
        }

        // 手本リスト作成
        private void createList()
        {
            lstHiragana.Clear();
            lstKatakana.Clear();
            lstAlpha.Clear();
            lstOther.Clear();

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

            str = strInput == "" ? "　": strInput;
            foreach (char c in str)
            {
                lstOther.Add(c.ToString());
            }
        }

        // 手本表示
        private void dispExample(bool flgInit = false)
        {
            clearCanvas();

            if (flgInit) cntList = 0;

            List<string> lst = new List<string>();

            if (pMode == playMode.hiragana) lst = lstHiragana;
            if (pMode == playMode.katakana) lst = lstKatakana;
            if (pMode == playMode.alpha) lst = lstAlpha;
            if (pMode == playMode.other) lst = lstOther;

            if (flgRand)
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
                    edText.Text = lst[cntList];
                }
                else
                {
                    if (cntList >= lst.Count)
                    {
                        cntList = 0;
                    }
                    edText.Text = lst[cntList];

                    //edText.Text = lst.IndexOf(edText.Text) == lst.Count - 1 ? lst[0] : lst[lst.IndexOf(edText.Text) + 1]; 
                }

                cntList = cntList + 1;
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

        private void ibInput_Clicked(object sender, EventArgs e)
        {
            pMode = playMode.other;
            setMode();
            dispExample(true);
        }

        // 設定保存
        private void ibOk_Clicked(object sender, EventArgs e)
        {
            grdDetail.IsVisible = false;
            grdMain.IsEnabled = true;
            strInput = txtInput.Text;

            createList();

            Application.Current.Properties["mode"] = flgRand;
            Application.Current.Properties["disp"] = pMode;
            Application.Current.Properties["input"] = strInput;

            // 画面再描写
            dispExample(true);

        }

        private void setMode()
        {
            switch (pMode)
            {
                case playMode.hiragana:
                    ibHiragana.BackgroundColor = Color.SkyBlue;
                    ibKatakana.BackgroundColor = Color.White;
                    ibAlpha.BackgroundColor = Color.White;
                    ibInput.BackgroundColor = Color.White;
                    txtInput.IsVisible = false;
                    break;
                case playMode.katakana:
                    ibHiragana.BackgroundColor = Color.White;
                    ibKatakana.BackgroundColor = Color.SkyBlue;
                    ibAlpha.BackgroundColor = Color.White;
                    ibInput.BackgroundColor = Color.White;
                    txtInput.IsVisible = false;
                    break;
                case playMode.alpha:
                    ibHiragana.BackgroundColor = Color.White;
                    ibKatakana.BackgroundColor = Color.White;
                    ibAlpha.BackgroundColor = Color.SkyBlue;
                    ibInput.BackgroundColor = Color.White;
                    txtInput.IsVisible = false;
                    break;
                case playMode.other:
                    ibHiragana.BackgroundColor = Color.White;
                    ibKatakana.BackgroundColor = Color.White;
                    ibAlpha.BackgroundColor = Color.White;
                    ibInput.BackgroundColor = Color.SkyBlue;
                    txtInput.IsVisible = true;
                    break;
                default:
                    ibHiragana.BackgroundColor = Color.SkyBlue;
                    ibKatakana.BackgroundColor = Color.White;
                    ibAlpha.BackgroundColor = Color.White;
                    ibInput.BackgroundColor = Color.White;
                    txtInput.IsVisible = false;
                    break;
            }
        }

        // 設定表示
        private void ibDetail_Clicked(object sender, EventArgs e)
        {
            grdMain.IsEnabled = false;
            grdDetail.IsVisible = true;
        }

        private void ibJun_Clicked(object sender, EventArgs e)
        {
            flgRand = false;
            setRandom();
        }

        private void ibRand_Clicked(object sender, EventArgs e)
        {
            flgRand = true;
            setRandom();
        }

        private void setRandom()
        {
            if (flgRand)
            {
                ibRand.BackgroundColor = Color.SkyBlue;
                ibJun.BackgroundColor = Color.White;
            }
            else
            {
                ibRand.BackgroundColor = Color.White;
                ibJun.BackgroundColor = Color.SkyBlue;
            }     
        }

        private void ibSave_Clicked(object sender, EventArgs e)
        {


        }
    }
}
