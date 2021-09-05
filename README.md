# 예약 확인 윈도우 프로그램_WPF
메이커스페이스의 장비 예약 확인을 위한 매니저 용 프로그램 제작. (개발 중인 메이커스페이스 어플과 연동)

## 개요
메이커스페이스에는 여러가지 시설과 장비들이 있습니다. 이들은 모두 개방되어 있으며 외부인의 경우 전화 예약과 함께 유료로 이용 가능합니다. 하지만 매니저의 입장에서 항상 외부인에게서 전화로 예약을 받다보니 많이 번거로웠습니다. 그래서 메이커스페이스 용 앱과 예약 확인을 위한 프로그램을 개발하기로 했습니다. 앱은 React-Native 로 개발 중에 있고 예약 확인 프로그램은 WPF라고 하는 프레임워크를 사용할 것 입니다.

WPF(Windows Presentation Foundation)는 데스크톱 클라이언트 애플리케이션을 만드는 UI 프레임워크입니다. WPF 개발 플랫폼은 애플리케이션 모델, 리소스, 컨트롤, 그래픽, 레이아웃, 데이터 바인딩, 문서 및 보안을 포함하여 다양한 애플리케이션 개발 기능 세트를 지원합니다. 언어는 C#을 사용하며 크게 UI를 만드는 xaml 부분과 기능 구현의 xaml.cs 부분으로 나뉘어집니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_1.png?raw=true'>

## 개발 과정
이번 예약 확인을 위한 프로그램에 필요한 기능은 다음과 같습니다.

1. 예약 시 예약 정보를 알림음과 함께 자동으로 불러오기
2. 버튼을 통해 수동 새로고침
3. 각 장비에 대한 예약 정보 출력
4. 장비 이용 후에 예약 초기화 하기

#### 기술
* WPF
  * 윈도우 프로그래밍을 위한 프레임워크
* C#
  * WPF 프로그래밍을 위한 언어
* Firebase
  * 예약 정보를 불러오기 위한 DB
* Google Spread Sheet
  * 예약 기록을 위한 구글 스프레드 시트 연동


### 1. C#에서의 파이어베이스와 스프레드 시트 연동 테스트
제가 지금까지 이용했던 구글의 여러가지 기능들은 자바스크립트나 파이썬으로 이용하였습니다. WPF는 C#으로 동작하기 때문에 지금까지 사용했던 구글의 기능들을 활용하기 위해서 공부가 필요했습니다. 

WPF는 .NET을 기반으로하는 프레임워크입니다. 다행히 파이어베이스의 경우 ,NET에서 활용할 수 있도록 C# 라이브러리가 존재했습니다. Nuget 관리자에서 FireSharp 라이브러리를 검색하여 다운로드 하고 불러오면 몇가지 함수와 코드로 파이어베이스를 조작할 수 있습니다.

이를 위해 테스트용으로 간단한 WPF 프로그램을 제작했습니다. 출입등록 버튼, 외부인 이용, 행사 이용의 3가지 버튼이 있는데 출입등록 버튼을 누르면 로그인 화면 으로 이동합니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_2.png?raw=true'>

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_3.png?raw=true'

Check 버튼을 누르면 현재 파이어베이스에 저장된 정보를 가져올 것 입니다. 또, 이름과 체온을 입력하고 Submit 버튼을 누르면 파이어베이스로 정보가 이동하고 스프레드 시트에 저장되도록 할 것 입니다.

먼저 파이어베이스의 경우 다운받은 FireSharp 라이브러리를 불러와야합니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_6.png?raw=true'>

~~~c#
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
~~~

그리고 파이어베이스의 주소와 패스워드를 선언해주고 페이지 생성자에 설정해줍니다.

~~~c#
private const string BasePath = "https://campbase-cfe26-default-rtdb.firebaseio.com/";
private const string FirebaseSecret = "파이어베이스 패스워드"
private static FirebaseClient _client;

public enterPage()
{
	InitializeComponent();

	IFirebaseConfig config = new FirebaseConfig
	{
		AuthSecret = FirebaseSecret,
    	BasePath = BasePath
	};

	_client = new FirebaseClient(config);
}
~~~

이제 파이어베이스 출력을 보기 위해  check 버튼 이벤트 함수를 보겠습니다.

~~~c#
private async void ID_CheckButton(object sender, RoutedEventArgs e)
{
	FirebaseResponse response = await _client.GetAsync("tmp/tmpName");
	String value = response.ResultAs<String>();
	MessageBox.Show(value);
}
~~~

ID_CheckButton은 Check 버튼을 눌렀을 때 실행되는 이벤트 함수로 파이어베이스의 tmp/tmpName 위치의 데이터를 value라는 변수에 저장하는 함수입니다. 불러온 값은 경고창으로 띄워줍니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_11.png?raw=true'>

다음은 파이어베이스 입력입니다. ID와 체온에 정보를 입력하고 submit 버튼을 누르면 파이어베이스의 정보가 업데이트됩니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_4.png?raw=true'>

~~~c#
private async void ID_SubmitButton(object sender, RoutedEventArgs e)
{
	//입력한 id와 체온을 저장하는 변수
	string id;
	string temp;
	id = ID.Text;
	temp = Temp.Text;
	
	//internal 클래스에서 선언한 파이어베이스 입력을 위한 변수정의
	var todoName = new Todo
	{
		tmpName = id,
	};

	var todoTemp = new Todo
	{
		tmpTemp = temp,
	};
	
	//파이어베이스 해당 위치에 입력
	var value1 = await _client.UpdateAsync("tmp", todoName);
	var value2 = await _client.UpdateAsync("tmpTemp", todoTemp);
	
	//json 키파일 경로와 시트의 id
    var gsh = new GoogleSheetsHelper("kiosksheet-05dfcd0a5ab2.json", "1EJaMVK8ciH1ZtXDXr7chBD1iRZ0r1nrD0QBzhiWxMFI");
	
	//열과 셀에 대한 생성자
    var row1 = new GoogleSheetRow();
    var row2 = new GoogleSheetRow();
	
	//첫번째 열의 3개 셀의 값
    var cell1 = new GoogleSheetCell() { CellValue = "날짜", };
    var cell2 = new GoogleSheetCell() { CellValue = "이름", };
    var cell3 = new GoogleSheetCell() { CellValue = "체온", };
	
	//두번째 열의 3개 셀의 값
    var cell4 = new GoogleSheetCell() { CellValue = DateTime.Now.ToString("yyyy-MM-dd") };
    var cell5 = new GoogleSheetCell() { CellValue = id };
    var cell6 = new GoogleSheetCell() { CellValue = temp };

    row1.Cells.AddRange(new List<GoogleSheetCell>() { cell1, cell2, cell3 });
    var row = new List<GoogleSheetRow>() { row1 };
    gsh.AddCells(new GoogleSheetParameters() { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = 1 }, row);
	
	//정보가 누적될 시트의 열 시작부분과 열의 개수 등 설정
    var gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 2, RangeRowEnd = 1000, FirstRowIsHeaders = true, SheetName = "Sheet1" };
    
    //작성된 열의 개수 가져오기
    int count = gsh.GetDataRow(gsp);
	
    row2.Cells.AddRange(new List<GoogleSheetCell>() { cell4, cell5, cell6 });
    var rows = new List<GoogleSheetRow>() { row2 };
    //정보가 들어올때마다 count+1의 열에 입력(누적)
    gsh.AddCells(new GoogleSheetParameters() { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = count + 1 }, rows);
	
	//모든 작업이 끝난 후 첫화면으로 돌아가기
    Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
    NavigationService.Navigate(uri);
}

internal class Todo
{
    public string tmpName { get; set; }
    public string tmpTemp { get; set; }
}
~~~

submit 버튼의 이벤트 함수는 조금 깁니다. 파이어베이스에 입력과 동시에 스프레드 시트에도 입력이 되어야하기 때문입니다. 먼저 파이어베이스 부분을 보면 internal 클래스로 설정한 이름과 체온 변수에 제가 입력한 id, 체온을 넣어줍니다. 그 후 await 연산자를 이용해서 입력 작업을 마칠때까지 메소드를 중단하도록 합니다. 이렇게 하면 다음과 같이 파이어베이스의 위치에 정보가 업데이트 됩니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_9.png?raw=true'>

다음은 구글 스프레드 시트와 연동입니다. 파이어베이스와는 다르게 구글 스프레드 시트는 조금 복잡한 과정이 필요합니다. 

https://www.hardworkingnerd.com/how-to-read-and-write-to-google-sheets-with-c/
저는 위의 코드를 참고했습니다.

먼저 GoogleApi를 Nuget관리자에서 다운로드 받아 불러옵니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_7.png?raw=true'>

~~~c#
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
~~~

이제 구글 클라우드 플랫폼(GCP)로 이동해야합니다. GCP에서 Google Sheets API 를 찾아서 사용 버튼을 누르고 사용자 인증 정보 만들기 라는 버튼을 눌러 API 설정을 해줍니다. 이때, Key 타입을 Json으로 해주어야 하는데 다운로드한 Json key 파일의 경로와 만들어둔 스프레드 시트의 주소를 vsh 변수의 GoogleSheetsHelper arguments로 넣어줍니다. 그리고 열과 셀을 생성자로 생성해주고 값을 설정해줍니다. 두번째 열 부터는 들어오는 정보를 받아 입력합니다. 입력이 끝나면 메인 화면으로 돌아갑니다.

Submit 버튼을 누르면 다음과 같이 시트에 정보가 입력되는 것을 볼 수 있습니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_5.png?raw=true'>

C#에서 스프레드 시트를 활용하는데에 자료조사가 굉장히 어려웠습니다. 제가 사용한 코드도 굉장히 많은 함수들을 엮어서 제가 목적에 맞게 수정한 코드 입니다. 모든 소스코드는 제 깃허브에서 찾아 볼 수 있습니다.


### 2. 테스트한 결과를 이용해 프로그램 제작
가장 중요한 기능의 테스트는 끝났기 때문에 이제 프로그램을 제작합니다. 위와 같은 방법이나 버튼이 여러개 있는 방식입니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_8.png?raw=true'>

우선, 파이어베이스와 스프레드 시트 외에 필요한 기능이 새로고침 버튼과 자동으로 새로고침하는 기능입니다. 하지만 계속해서 파이어베이스의 정보를 읽고 있는 것은 자원의 낭비가 심하므로 저는 10분마다 함수를 재실행하는 방법을 이용했습니다. 이때, 함수는 새로고침 버튼과 같은 함수를 이용합니다.

~~~c#
DispatcherTimer timer = new DispatcherTimer();
    timer.Interval = TimeSpan.FromTicks(6000000000);   // 10분
    timer.Tick += new EventHandler(Button_Click);
    timer.Start();        
~~~

Dispatcher 라고 하는 타이머를 이용했습니다. 시간은 10분으로 설정하고 10분이 지나면 Butto_Click이라는 새로고침 버튼에 사용되는 함수를 자동으로 실행합니다. 

이제 예약에 대한 알림 기능의 구현이 필요합니다.
~~~c#
Boolean[] ReservationNumber = new Boolean[] { false, false, false, false, false, false, false, false, false };
~~~

총 9대의 3d프린터와 레이저커터가 있으므로 9개의 Boolean 타입을 저장하는 배열을 만들어 각 장비의 버튼마다 동작하도록 했습니다. 10분 마다 혹은 새로고침을 누를 때 마다 각 버튼들이 자신들의 예약 정보가 담긴 파이어베이스 위치에서 정보를 불러오는데 이때 예약 정보가 있으면 자신의 위치의 배열을 true로 바꾸고 그 정보를 프로그램에서 출력해줍니다.

~~~c#
string alert = "";
	//새로운 예약 정보가 1개 이상이라면
	if (count > 0)
    {
    	//예약된 장비의 번호를 alert에 더해준다 
        for (int i = 0; i < 9; i++)
        {
        	if (ReservationNumber[i] == true)
        {
        	alert += (i + 1) + " ";
        }                  
    }
    alert += "번 새로 예약되었습니다.";
    //알림음 실행
    MediaPlayer media = new MediaPlayer();
    media.Open(new Uri("C:/Alert.mp3", UriKind.Absolute));
    media.Play();
    //예약된 장비 번호 출력
    MessageBox.Show(alert);
}
~~~

true로 바뀐 배열의 정보를 확인하여 true 값인 장비들의 번호들을 알림음과 함께 경고창으로 보여줍니다. 음악 재생은 C#의 MediaPlayer를 사용했습니다.

<img src='https://github.com/HwanGonJang/HwanGonJang.github.io/blob/master/Pictures/wk_10.png?raw=true'>

## 결과
이렇게 WPF라는 조금은 생소한 툴을 통해 프로그램을 제작해보았습니다. 생소하긴 헀지만 의외로 xaml을 이용한 UI 제작도 직관적이고 C# 개발도 효율적이었던 것 같습니다. 또, 이번 개발에도 역시 구글의 기능들을 사용했는데 개발 실력이 늘면 늘수록 구글의 대단함을 느끼는 것 같습니다. 아직 장비들에 대한 결과로 장비의 사용상태 정보만 불러오지만 앱 제작이 완료되면 다시 수정할 것입니다. 감사합니다.

