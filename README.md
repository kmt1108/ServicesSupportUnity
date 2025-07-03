# ServicesSupportUnity 
Made by KMT

## Getting started
    - Download file .unitypackage và import vào project
    - Chọn các SDK bên thứ 3 cần
    - Mở tab DKTech>Integration Manager> Chọn các tính năng mà Project đang sử dụng> Update Define Symbols
    - Set up
        - Kéo prefab Assets/DkTech/Prefabs/ServicesManager.prefab vào scene đầu tiên và nhập các thông tin cần thiết
        - Nếu project sử dụng Adjust SDK, Kéo Object Assets/Adjust/Prefab/Adjust.prefab vào scene đầu tiên, set Enviroment thành Production và nhập SDK Key
        - Add component LoadingUI trong Loading scene thêm callback khi loading xong
        - Nhập các thông tin cần thiết cho LoadingUI.
## 1. Set RemoteConfig trên Firebase cho mỗi vị trí quảng cáo (Dành cho GD)
    - key: Tên quảng cáo (VD: BANNER_HOME,INTER_HOME,...)
    - value: Thông tin quảng cáo (string) - Mạng quảng cáo,ID quảng cáo 1,ID quảng cáo 2,Vị trí quảng cáo(cho Banner, Native Overlays,AppOpen),Collapsible(cho Banner);
#### Mạng quảng cáo: 
    0: None - Tắt quảng cáo ở vị trí này
    1: Quảng cáo Admob
    2: Quảng cáo Applovin
    3: Quảng cáo Ironsource
#### ID quảng cáo 1 & 2: 
    - Nhâp ID cho Monetize cung cấp (Không nhập hoặc nhập sai có thể gây lỗi)
    - Nếu load fail ID1 thư viện sẽ tự động load lại bằng ID2 cứ thế luân phiên cho đến khi load thành công
#### Vị trí quảng cáo (Banner, Native Overlays,AppOpen): 
    - Đối với Banner và Native Overlay Ad
        0: Bottom
        1: Top
        2: Center
    - Đối với App Open Ad
        0: AppOpen (Show khi mở game)
        1: OnResume (Show khi quay lại game)
#### Collapsible (Banner): 
    True: Banner Collaps
    False: Banner thường
### VD: 
    key:"BANNER_HOME"; value:"1,ca-app-pub-3940256099942544/9214589741,,0,True" - Banner Collaps Admob ở dưới màn hình
    key:"BANNER_HOME"; value:"1,ca-app-pub-3940256099942544/9214589741," - Banner Admob vị trí và collapsible set mặc định trong code
    key:"AOA1"; value:"1,ca-app-pub-3940256099942544/9214589741,0," - AppOpen Admob show lúc mở game
    key:"INTER_HOME"; value:"2,ca-app-pub-3940256099942544/9214589741" - Inter Applovin
## 2. Set config trên Editor cho mỗi vị trí quảng cáo (Dành cho Dev)
#### Mở Avertisement Setting tại DKTech>Advertisement Settings
    - Ads Order Priority: Danh sách load ad ưu tiên được load trước ở màn loading
    - Ads Order Normal: Danh sách load ad không ưu tiên được load sau danh sách Priority và sau màn loading
    - Is Test Mode: Bắt buộc show test ad (chỉ cho Admob và Ironsource)
    - No Ads: tắt tất cả quảng cáo ngoại trừ Rewarded Ad
    - Free Rewarded: tự động nhận rewarded không cần xem quảng cáo
    - Waiting In Background Show Onresume: back ra khỏi game 1 thời gian mới cho show Onresume
    - Check Test Devices: Check xem có trả về quảng cáo test khôngAdsUtilities.IsTestDevice
#### Config của 1 vị trí quảng cáo bao gồm    
    - Ad Name (Tên quảng cáo - Dùng để gọi khi show ad)
    - Ad RC Key: Key RemoteConfig dùng để lấy giá trị từ firebase, key này trùng với key do GD set
    - Ad Network: Chọn mạng quảng cáo
    - Ad ID,Ad ID2: Nhâp ID cho Monetize cung cấp (Không nhập hoặc nhập sai có thể gây lỗi), nếu load fail ID1 thư viện sẽ tự động load lại bằng ID2 cứ thế luân phiên cho đến khi load thành công
    - Auto Reload: Nếu quảng cáo show đi show lại nhiều lần thì tick vào ô này
    - Ad Position(Banner, Native Overlay Ad, AppOpen): Set vị trí quảng cáo
    - IsCollapsible(Banner)
    - Sau khi nhập xong quảng cáo nhấn Update Ads Key để lưu Ad Name vào file AdsKeys.cs thuận tiện cho việc gọi show
## 3. Set up Firebase setting (Dành cho Dev)
#### Mở Firebase Setting tại DKTech>Firebase Settings
    - Get Token Firebase: Chỉ sử dụng khi debug. Khi chạy game sẽ lưu token firebase vào clipboard của bàn phím
    - Config Time Waiting Inter: config thời gian giữa 2 lần show Intersitital Ad
    - Config Check Internet: cho phép kiểm tra máy có kết nối mạng không
    - Config Check Update: cho phép kiểm tra phiên bản mới trên store và update
    - List Request: Danh sách config được set mặc định. mất mạng hoặc lỗi firebase sẽ lấy giá trị ở đâu
#### Các thuộc tính của 1 Remote config
    - Name: Tên để gọi khi lấy remote config
    - Key: là key remote config set trên firebase
    - Value: giá trị mặc định (Vì kiểu nhập là String nên cần cẩn thận lỗi FormatException)
    - Sau khi nhập xong Remote Config nhấn Update RemoteKey để lưu Name vào file RemoteKeys.cs thuận tiện cho việc lấy Data
#### Các hàm get giá trị remote config:
    string[] GetRemoteConfigStringArrayValue(string key)
    int[] GetRemoteConfigIntArrayValue(string key)
    double GetRemoteConfigDoubleValue(string key)
    string GetRemoteConfigStringValue(string key)
    bool GetRemoteConfigBooleanValue(string key)
    int GetRemoteConfigIntValue(string key)
## Các hàm show quảng cáo
#### Show Banner
    public static void BannerAdManager.ShowBanner(string AdName);
    public static void BannerAdManager.HideBanner(string AdName);
#### Show Inters
    - Show Inter
    public static void ShowInterstitial(string adName, Action actShowed = null, Action<bool> actClosed = null)
    - Show inter không cần chờ delay
    public static void ShowInterstitial(string adName, Action actShowed = null, Action<bool> actClosed = null)
    Example:
    InterstitialAdManager.ShowInterstitial("INTER_ADD",
            actShowed: () =>
            {
                Debug.Log("inter show success!");
            }
            , actClosed: (showed) =>
            {
                Debug.Log("inter closed: " + (showed ? "show success" : "show fail"));
            });
## Show Reward
    public static void ShowRewarded(string adName, Action<bool> action)
    Example
    RewardedAdManager.ShowRewarded(rewardName.text, (earned) =>
            {
                if (earned)
                {
                    Debug.Log("earned reward");
                }
                else
                {
                    Debug.Log("reward fail");
                }
            });
## Show Native
    public static void ShowNative(string name, NativeAdContent content, Action actShowed = null, Action<bool> actClosed = null)
    public static void AdsUtilities.Native.HideNative(AdName);
    Example
    NativeAdManager.ShowNative(nativeName.text, nativeAdContent,
            actShowed: () =>
            {
                Debug.Log("native show success!");
            },
            actClosed: (showed) =>
            {
                Debug.Log("native closed: " + (showed ? "show success" : "show fail"));
            });

## Track Event Firebase
FirebaseManager.TrackEvent(string eventName);

