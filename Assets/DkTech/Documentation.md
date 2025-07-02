# Service Support Unity
# Made by KMT



## Getting started

B1: Export các file sau:
- Folder: Assets/DkTech
- Assets/csc.rsp
- Assets/Plugins/Android/proguard-user.txt
B2: Import vào project mới
B3: Import các SDK bên thứ 3 cần thiết 
B4: Chọn Update3rdPartyIntergrations trên tab Tools trên thanh task bar để thư viện nhận SDK
B5: Kéo Object Assets/DkTech/Prefabs/Services/Services.prefab vào scene đầu tiên và nhập các thông tin cần thiết
- Add component LoadingUI trong Loading scene thêm callback khi loading xong, nhập onresume id nếu cần
- Thêm các sự kiện AdsUtilities.OnWinLevel(), AdsUtilities.OnLoseLevel(),AdsUtilities.OnPlayLevel() vào các vị trí tương ứng trong game;

## Setting Remote Config
- Set network cho mỗi định dạng quảng cáo
    key: BANNER/INTERS/REWARD/AOA value: number 
    * 0: Không có ads,1: Admob,2:Applovin,3: Ironsource

- Set thời gian tối thiểu giữa 2 lần show Inters
    key: TIME_FOR_INTER value: number
    * thời gian tính băng giây(s)

- Set cách đếm level
    TYPE_COUNT_LEVEL: number
    * 0: đếm lúc win, 1: đếm lúc win/lose, 2: đếm lúc play 1 level

- Set level show lại CMP nếu không được accept
    LEVEL_CMP: chuỗi ký tự
    * là các level được nối với nhau bởi dấu ",": VD:"1,5,10"

- Set level bắt đầu show Banner Ads
    LEVEL_START_BANNER value: number

- Set level bắt đầu show Banner Collapsible
    LEVEL_START_COLLAPS value; number

- Set khoảng cách level load lại Banner Collapsible 
    DISTANCE_BANNER vulue: number

- Set level bắt đầu show Inters Ads
    LEVEL_START_INTERS value: number

- Set thời gian tối thiểu giữa 2 lần show Inters (tính bằng level) 
    DISTANCE_INTERS value: number

- set check no internet
    CHECK_INTERNET value: true/false


## Show Banner

AdsUtilities.instance.ShowBanner(bool isShow)

isShow=true: show banner
isShow=false: hide banner

AdsUtilities.instance.CheckReloadBanner();

## Show Inters
- check remote config show inters
AdsUtilities.instance.CheckShowInters(() =>
        {
            sau khi tắt inters
        });
        
- show inters không check remote cònig
AdsUtilities.instance.ShowInterstitial(() =>
        {
            sau khi tắt inters
        });


## Show Reward

AdsUtilities.instance.ShowRewarded((earned) =>
        {
            if(earned)
            {
                nhận được phần thưởng
            }
            else
            {
                không nhận được phần thưởng
            }
        });


## Track Event Firebase

FirebaseManager.TrackEvent(string eventName);