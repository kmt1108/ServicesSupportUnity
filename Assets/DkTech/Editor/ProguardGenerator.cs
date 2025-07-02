using System.IO;
using UnityEditor;

public static class ProguardGenerator
{
    public static string proguard = @"############################
# Adjust SDK
############################
-keep class com.adjust.sdk.** { *; }

############################
# Google Play Services / Ads / Identifier
############################
-keep class com.google.android.gms.** { *; }
-dontwarn com.google.android.gms.**

-keep class com.google.android.gms.common.ConnectionResult {
    int SUCCESS;
}

-keep class com.google.android.gms.ads.identifier.AdvertisingIdClient {
    com.google.android.gms.ads.identifier.AdvertisingIdClient$Info getAdvertisingIdInfo(android.content.Context);
}
-keep class com.google.android.gms.ads.identifier.AdvertisingIdClient$Info {
    java.lang.String getId();
    boolean isLimitAdTrackingEnabled();
}
-dontwarn com.google.android.gms.ads.identifier.**

############################
# Google UMP (User Messaging Platform)
############################
-keep public class com.google.android.ump.** { public *; }

############################
# Google Play Core
############################
-keep class com.google.android.play.core.** { *; }

############################
# Install Referrer
############################
-keep public class com.android.installreferrer.** { *; }

############################
# Unity Ads & Unity Services
############################
-keep class com.unity3d.ads.** { *; }
-keep class com.unity3d.services.** { *; }
-dontwarn com.unity3d.services.**
-dontwarn com.ironsource.adapters.unityads.**

############################
# IronSource SDK
############################
-keep class com.ironsource.adapters.** { *; }
-keep class com.ironsource.unity.androidbridge.** { *; }
-keepclassmembers class com.ironsource.sdk.controller.IronSourceWebView$JSInterface {
    public *;
}
-dontwarn com.ironsource.**

############################
# InMobi SDK
############################
-keep class com.inmobi.** { *; }
-dontwarn com.inmobi.**
-keep public class com.google.ads.mediation.inmobi.InMobiConsent { *; }

############################
# Vungle SDK
############################
-keep class com.vungle.warren.** { *; }
-keep class com.vungle.ads.VunglePrivacySettings { *; }
-dontwarn com.vungle.warren.error.VungleError$ErrorCode

############################
# Facebook SDK
############################
-keep class com.facebook.** { *; }
-dontwarn com.facebook.**
-keep class com.facebook.internal.FetchedAppSettingsManager { *; }
-keep class com.facebook.unity.FB { *; }

############################
# AppLovin SDK
############################
-keep public class com.applovin.** { public protected *; }
-keep public class com.applovin.impl.**.*Impl { public protected *; }
-keepclassmembers class com.applovin.sdk.AppLovinSdkSettings { private java.util.Map localSettings; }
-keep class com.applovin.mediation.adapters.** { *; }
-keep class com.applovin.mediation.adapter.** { *; }

############################
# MBridge (MobVista)
############################
-keep class com.mbridge.** { *; }
-keep interface com.mbridge.** { *; }
-keep class **.R$* { public static final int mbridge*; }
-keep public class com.mbridge.* extends androidx.** { *; }
-dontwarn com.mbridge.**

############################
# Moat SDK
############################
-keep class com.moat.** { *; }
-dontwarn com.moat.**

############################
# Firebase SDK
############################
-keep class com.google.firebase.** { *; }
-dontwarn com.google.firebase.**

-keepattributes *Annotation*
-keepattributes SourceFile,LineNumberTable

############################
# Gson
############################
-keep class * implements com.google.gson.JsonSerializer
-keep class * implements com.google.gson.JsonDeserializer
-keep class * implements com.google.gson.TypeAdapterFactory
-dontwarn sun.misc.**

############################
# Retrofit + Okio
############################
-dontwarn retrofit2.Platform$Java8
-dontwarn okio.**
-dontwarn org.codehaus.mojo.animal_sniffer.IgnoreJRERequirement

############################
# WebView Javascript Interface
############################
-keepattributes JavascriptInterface
-keep class android.webkit.JavascriptInterface { *; }
-keepclassmembers class * {
    @android.webkit.JavascriptInterface <methods>;
}
-keep class android.webkit.WebViewClient
-keep class * extends android.webkit.WebViewClient
-keepclassmembers class * extends android.webkit.WebViewClient {
    <methods>;
}

############################
# AndroidX / Support
############################
-keep class android.support.v4.** { *; }
-keep class androidx.viewpager.widget.PagerAdapter { *; }
-keep class androidx.viewpager.widget.ViewPager$OnPageChangeListener { *; }
-keep class androidx.fragment.app.Fragment { *; }
-keep class androidx.core.content.FileProvider { *; }
-keep class androidx.core.app.NotificationCompat { *; }
-keep class androidx.appcompat.widget.AppCompatImageView { *; }
-keep class androidx.recyclerview.** { *; }
-keep interface androidx.annotation.** { *; }

############################
# Parcelable Support
############################
-keepclassmembers class * implements android.os.Parcelable {
    public static final android.os.Parcelable$Creator *;
}

############################
# Shaded / Firebase Legacy / Khác
############################
-keepnames class com.shaded.fasterxml.jackson.** { *; }
-keepnames class org.shaded.apache.** { *; }
-dontwarn org.w3c.dom.**
-dontwarn org.joda.time.**
-dontwarn org.shaded.apache.commons.logging.impl.**

############################
# Network Plugin DKTech
############################
-keep class com.dktech.networkplugin.NetworkPlugin { *; }
";

    public static void GenerateProguardFile()
    {
        string filePath = "Assets/Plugins/Android/proguard-user.txt";

        // Tạo thư mục nếu chưa có
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Nội dung file (bạn có thể thay đổi tùy ý)
        File.WriteAllText(filePath, proguard);

        // Refresh để Unity nhận diện file
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("Đã tạo file proguard-user.pro tại: " + filePath);
    }
}
