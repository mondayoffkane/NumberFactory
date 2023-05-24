using UnityEngine;
public class Managers : MonoBehaviour
{
    ///<summary>내부적으로 사용되는 Managers 변수</summary>
    static Managers _instance;
    ///<summary>내부적으로 사용되는 Managers Property</summary>
    static Managers Instance { get { Init(); return _instance; } }

    [SerializeField]
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    [HideInInspector]
    GameManager _game;
      
    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    public static GameManager Game => Instance._game;
    public static UI_GameScene GameUI;



    ///<summary>가장 처음 매니저 만들때 한번 Init</summary>
    static void Init()
    {
        if (_instance == null && Application.isPlaying)
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            UnityEngine.Input.multiTouchEnabled = false;

            GameObject go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            GameObject SceneTrasition = Managers.Resource.Instantiate("SceneTrasition");
            SceneTrasition.transform.SetParent(_instance.transform);
            Scene._sceneTrasitionAni = SceneTrasition.GetComponent<Animator>();

            _instance._sound.Init();
            _instance._data.Init();
            _instance._pool.Init();
            _instance._scene.Init();
            _instance._resource.Init();

            _instance._game = go.AddComponent<GameManager>();
        }
    }

    public static void GameInit()
    {
        Instance._game.Init();
    }

    ///<summary>새로운 씬으로 갈때마다 클리어</summary>
    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        Resource.Clear();
        Game.Clear();
    }



    //============Check Internet============ 지워도 됨!
    //[HideInInspector]
    //bool isInternetOn = true;
    //[HideInInspector]
    //UI_PopupInternet popup;
    //[HideInInspector]
    //float _oriTimeScale = 1;
    //private void Update()
    //{
    //    if (isInternetOn && Application.internetReachability == NetworkReachability.NotReachable)
    //    {
    //        isInternetOn = false;
    //        popup = Managers.UI.ShowPopupUI<UI_PopupInternet>();
    //        _oriTimeScale = Time.timeScale;
    //        Time.timeScale = 0;
    //    }
    //    else if (!isInternetOn && Application.internetReachability != NetworkReachability.NotReachable)
    //    {
    //        isInternetOn = true;
    //        if (popup != null)
    //            Managers.UI.ClosePopupUI(popup);
    //        Time.timeScale = _oriTimeScale;
    //    }
    //}
    //============Check Internet============ 지워도 됨!

    static readonly string[] CurrencyUnits = new string[] { "", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", "ca", "cb", "cc", "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn", "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", };


    public static string ToCurrencyString(double number, int _num = 0)
    {
        string zero = "0";

        if (-1d < number && number < 1d)
        {
            return zero;
        }

        if (double.IsInfinity(number))
        {
            return "Infinity";
        }

        //  부호 출력 문자열
        string significant = (number < 0) ? "-" : string.Empty;

        //  보여줄 숫자
        string showNumber = string.Empty;

        //  단위 문자열
        string unityString = string.Empty;

        //  패턴을 단순화 시키기 위해 무조건 지수 표현식으로 변경한 후 처리
        string[] partsSplit = number.ToString("E").Split('+');

        //  예외
        if (partsSplit.Length < 2)
        {
            return zero;
        }

        //  지수 (자릿수 표현)
        if (!int.TryParse(partsSplit[1], out int exponent))
        {
            //Debug.LogWarningFormat("Failed - ToCurrentString({0}) : partSplit[1] = {1}", number, partsSplit[1]);
            return zero;
        }

        //  몫은 문자열 인덱스
        int quotient = exponent / 3;

        //  나머지는 정수부 자릿수 계산에 사용(10의 거듭제곱을 사용)
        int remainder = exponent % 3;

        //  1A 미만은 그냥 표현
        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(number).ToString();
        }
        else
        {
            //  10의 거듭제곱을 구해서 자릿수 표현값을 만들어 준다.
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * System.Math.Pow(10, remainder);
            switch (_num)
            {
                case 0:
                    showNumber = temp.ToString("F1").Replace(".0", "");
                    break;

                case 1:
                    if (remainder == 2)
                    {
                        showNumber = temp.ToString("F0").Replace(".0", "");
                    }
                    else
                    {
                        showNumber = temp.ToString("F1").Replace(".0", "");
                    }

                    break;

                case 2: //  소수 둘째자리까지만 출력한다.
                    showNumber = temp.ToString("F2").Replace(".0", "");
                    break;

                case 3:
                    showNumber = temp.ToString("F0").Replace(".0", "");
                    break;
            }


        }

        unityString = CurrencyUnits[quotient];

        return string.Format("{0}{1}{2}", significant, showNumber, unityString);
    }


}
