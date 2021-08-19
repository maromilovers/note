using note;
using AddAdMobBannerSample.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

#pragma warning disable CS0612 // 型またはメンバーが旧型式です
[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
#pragma warning restore CS0612 // 型またはメンバーが旧型式です
namespace AddAdMobBannerSample.Droid.Renderers
{
    [System.Obsolete]
    public class AdMobBannerRenderer : ViewRenderer<AdMobBanner, Android.Gms.Ads.AdView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<AdMobBanner> e)
        {

            const string adUnitID = "ca-app-pub-4439948160128694/7631880145";
            //テスト
            //const string adUnitID = "ca-app-pub-3940256099942544/6300978111";

            base.OnElementChanged(e);

            if (Control == null)
            {
                var adMobBanner = new Android.Gms.Ads.AdView(Forms.Context);
                adMobBanner.AdSize = Android.Gms.Ads.AdSize.Banner;
                adMobBanner.AdUnitId = adUnitID;

                var requestbuilder = new Android.Gms.Ads.AdRequest.Builder();
                adMobBanner.LoadAd(requestbuilder.Build());

                SetNativeControl(adMobBanner);
            }
        }
    }
}