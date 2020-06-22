// Это скрипт для отрисовки меню паузы и других иконок в процессе игры
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class PauseMenu : MonoBehaviour 
{
//---------------------------------------------------------------------------------------------------Текстуры меню паузы----------------------------------------------------------------------------------------------------------
	public Texture2D BlackOut;									// Затенение
	public Sprite [] RightProfileWindows = new Sprite[4];		// Варианты рамки для правого игрока в зависимости от счёта
	public Sprite [] LeftProfileWindows = new Sprite[4];		// Варианты рамки для левого игрока в зависимости от счёта
	public Texture2D FrameForAvatar;							// Рамка для аватара
	public Texture2D Conformation;								// Окно подтверждения
	public Texture2D BoxPauseMenu;								// Окно паузы
	public Texture2D WindowInventory;							// Текстура для окна инвентаря
	public Texture2D OverviewWindowObject;						// Текстура окошка обзора 3D предмета в меню объекта
	public Texture2D BoxScore;									// Окно очков показываемое при забитии гола
	public Texture2D Results;									// Окно показываемое в конце раунда со статистикой
	public Texture2D PauseBoxScore;								// Окно очков показываемое при вызове паузы
	public Texture2D [] WindowLevelSettings = new Texture2D[2];	// Окно настройки уровня
	public Texture2D ActiveSquareButton;						// Текстура активной кнопки выбора скина
	public Texture2D TexSelImprButtonDown;						// Текстура активнной кнопки выбора улучшения
	public Texture2D PrizesWindow;								// Текстура окна для призов
	public Texture2D QuestionIcon;								// Иконка знака вопроса для выбора приза в конце уровня
	public Texture2D NoPrize;									// Иконка указывающая что в этой ячейке приза нет
	public Texture2D PrizeReceived;								// Иконка указывающая что данный приз уже был получен
	public Texture2D ShadingObject;								// Текстура затемнения для полученных призов
	public Texture2D [] TexsPrizes;								// Здесь будут храниться текстуры для отображения призов
	string GUIAdress = "Texs&Mats/GUIElements/SingleElements/";	// Основной адрес папки с загружаемыми гуи текстурами
//-----------------------------------------------------------------------------------------------Общие данные для инвентаря--------------------------------------------------------------------------------------------------------
//	string[] LvlToolbar	= new string[] {"1Lvl","2Lvl","3Lvl","4Lvl","5Lvl"};	// Массив строк для уровней товаров магазина
//	int ActiveLvl = 0;															// Активный уровень в инвентаре
//	int ActiveCell = 0;															// Активная ячейка в инвентаре
//	byte ActiveSkin = 0;														// Выделенная ячейка материала в инвентаре
//	byte ActiveImpr = 10;														// Выделенное ячейка улучшения в инвентаре
//	Rect[] PosIconSkins = new Rect[10];											// Позиции иконок покупки скинов(Материалов)
//	Rect[] PosIconImprs = new Rect[10];											// Позиции иконок покупки улучшений
//	List<Texture2D>Skins=new List<Texture2D>();									// Изображения для кнопок материалов выбранного объекта
//	List<Texture2D>Imprvs=new List<Texture2D>();								// Изображения для кнопок улучшений выбранного объекта
//	Texture2D[] Texs = new Texture2D[25];										// Изображения ячеек просматриваемой категории инвентаря
//----------------------------------------------------------------------------------Данные для инвентаря 1 игрока-----------------------------------------------------------------------------------------------------------------
//	Texture2D[] Buts = new Texture2D[125];				// Массив текстур для раздела биты инвентаря 1 игрока
//	Texture2D[] Washers = new Texture2D[125];			// Массив текстур для раздела шайбы инвентаря 1 игрока
//	Texture2D[] Tables = new Texture2D[125];			// Массив текстур для раздела столы инвентаря 1 игрока
//---------------------------------------------------------------------------------Переменные для окна результатов----------------------------------------------------------------------------------------------------------------
	public Texture2D[] GoalArrow = new Texture2D[2];	// Массив иконки стрелок для обозначения кому был забит гол
	public string[] RoundTimeMin = new string[5];		// Массив сколько длилься отдельно каждый раунд (Минуты)
	public string[] RoundTimeSec = new string[5];		// Массив сколько длилься отдельно каждый раунд (Секунды)
	public string[] FullTimeMin = new string[5];		// Массив Время каждого раунда плюс все пердыдущие (Минуты)
	public string[] FullTimeSec = new string[5];		// Массив Время каждого раунда плюс все пердыдущие (Секунды)
	public byte[] LPSs	= new byte[5];					// (LeftPlayerScores) Очки (голы) левого игрока за каждый раунд
	public byte[] RPSs = new byte[5];					// (RightPlayerScores) Очки (голы) правого игрока за каждый раунд
	public int[] LPRC = new int[5];						// ( LeftPlayerRoundCredits ) массив кредитов левого игрока заработанных за каждый раунд
	public int[] RPRC = new int[5];						// ( RightPlayerRoundCredits ) массив кредитов правого игрока заработанных за каждый раунд
	public int LPFC;									// ( LeftPlayerFinalCredits ) количество кредитов левого игрока заработанных за весь уровень
	public int RPFC;									// ( RightPlayerFinalCredits ) количество кредитов правого игрока заработанных за весь уровень				
	public bool[] RightGoal = new bool[5];				// Массив состояний кому был забит гол в каждом раунде правому игроку или левому
	public byte Rounds;									// Количестов отыгранных раундов
	public int BatExp = 0;								// Количество набранного в этом раунде опыта биты
	public int PuckExp = 0;								// Количество набранного в этом раунде опыта шайбы
	public int SecondBatExp = 0;						// Опыт биты для второго игрока или компьютера
	public int SecondPuckExp = 0;						// Опыт шайбы второго игрока или компьютера
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public GameManager GM;						// Переменная для скрипта GameManager		
	public StoreAndInventory SAI;				// Переменная для скрипта StoreAndInventory
	public PauseMenuPlus PMP;					// В этой переменной лежит скрипт PauseMenu Plus
	StoreContent SC;							// Переменная для скрипта "Контент магазина"
	Keeper Kep;                                 // (Keeper) Переменная для скрипта "Хранитель"
    public byte ScorePlayer1 = 0;				// Переменная "Очки игрока 1" нужна для того чтобы подсчитывать голы забитые "игроку 2"
	public byte ScorePlayer2 = 0;				// Переменная "Очки игрока 2" нужна для того чтобы подсчитывать голы забитые "игроку 1"
	public byte LPS = 0;						// (Left Player Score) Очки (Голы) левого игрока забитые правому игроку
	public byte RPS = 0;						// (Right Player Score) Очки (Голы) правого игрока забитые левому игроку
	public string LPN, RPN;						// (Left Player Name) Имя левого игрока, // (Right Player Name) Имя правого игрока						
	public Texture2D LeftAv, RightAv;			// Аватары для игрока с права и игрока с лева
//	public Texture2D LBB, RBB;					// (Left Background Band, Right Background Band) Задний фон полосы улучшений левого и правого игрока для текущего активного улучшения
	public UserCursor UC;						// Переменная для скрипта "Пользовательский курсор"			
	public GUISkin GameSkin;					// Скин для всех гуи элементов
	public GameMenuWins Window;					// Переменная перечисление указывающая какое сейчас показываеться окно
	public GUIStyle LabelScoreStyle;			// Маленькое окно очков
	public GUIStyle ScoreGoalStyle;				// ??????????????????????
	public GUIStyle Neon;						// Стиль неоновых цифр отображающих реальное время
	public byte Scoregoal;						// Переменная хранящая кому был забит последний гол
	public bool Loading = false;				// эта переменная говорит произошли ли все события старта уровня
	public Canvas PlayersInform = null;			// (Game Element) переменная для канваса содержащего в себе элементы информации игроков - (голы, энергия улучшения)
    GameObject IO;								// (IndestructibleObject) Переменная для игрового объекта
	public byte Min = 0, Sec = 0;				// Минуты и секунды теущего уровня
	string Rhour, Rminute, Rsecond;				// Реальные час минута и секунда
	string Minute, Second;						// Минуты и секунды теущего раунда переведённые в строку
	string RealTime;							// Эта переменная отображает реальное время "В компьютере"
	string RoundTime;							// Эта переменная отображает время текущего раунда
	float OriginalHight = 1080; 				// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	float RatioH;								// Сюда заноситься результат деления оригинальной высоты экрана на текущую
	public bool PrizeSelection = true;			// Эта переменная говорит можно ли выбрать приз или приз уже был выбран
	public bool LevelSettingsEnabled = true;	// Эта переменная говорит активны ли кнопки в меню настройки уровня
	public bool StartEnabled = false;			// Эта переменная говорит активна ли кнопка старта уровня на двоих
	public bool TimeMoves = true;				// Эта переменная говорит остановленны ли игровые часы
	public bool ShowPrizes = false;				// Эта переменная говорит показывать ли призы или нет в зависимости от победы или проигрыша игрока
	public bool NextLevelTransition = false;	// Эта переменная говорит разрешён ли проход на следующий уровень
	public Rect[] PrizesRects = new Rect[6]; 	// Массив прямоугольников для призов отрисовывающихся в конце игры
	float CursorDelay;							// Здесь будет храниться время задержки курсора над открытой ячейкой
	byte SelectedPrizeCell = 0;					// Номер ячейки c открытым призом в этом раунде
	Rect[] Rects = new Rect[2];					// Это массив прямоугольников информации об игроках пересчитанных для данного экрана
	Text RT,LT;									// UI компоненты текста первого и второго игрока
	bool ShowInformLabel = false;				// Эта переменная говорит можно ли отрисовывать информационную метку над ячейкой
	char[] StatesPrizes = new char[6];			// Массив типов призов (объект "O"), (материал для стола "M") (улучшение для стола "I") (скайбокс "S") 
	short[] NombersPrizes = new short[6];		// Массив номеров объектов которые лежат в своих массивах взятые из 4 массивов ObjPrize, TableNomForMat, TableNomForImpr, SkyPrize
	byte[] NombersMats = new byte[6];			// Массив номеров материалов для каждой призовой ячейки (Только если эта ячейка содержит в себе материал)
	byte[] NombersImprs = new byte[6];			// Массив номеров улучшений для каждой призовой ячейки (Только если эта ячейка содержит в себе улучшение)
	ObjectScript BatObjScr;						// Сдесь будет лежать скрипт "ObjectScript" клона выбранной для игры биты первого игрока
	ObjectScript PuckObjScr;					// Сдесь будет лежать скрипт "ObjectScript" клона выбранной для игры шайбы первого игрока
	ObjectScript InformObjScr;					// Сдесь будет лежать скрипт "ObjectScript" объекта над которым задержалься курсор
	ImprovementScript InformImprScr;			// Сдесь будет лежать скрипт "ImprovementScript" улучшения выбранного объекта в магазине
	string InformMatName;						// Сдесь будет лежать название материала над которым завис курсор
	string InformImprName;						// Сдесь будет лежать название улучшения над которым завис курсор
	string InformImprType;						// Сдесь будет лежать название типа улучшения над которым завис курсор
	float InformImprMass;						// Сдесь будет лежать масса улучшения над которым завис курсор


	void OnEnable()										// Этот метод срабатывает до старта сцены при включении объекта
	{
		GM.StartSecondStep += _StartSecondStep_;		// Подписываем метод _StartSecondStep_ на событие StartSecondStep
	}


	void Start()
	{	
		RatioH = Screen.height / OriginalHight;			// Заносим в ScreenBalansHight результат деления высот экранов
		IO = GameObject.Find("IndestructibleObject");	// Находим в сцене скрипт StoreContent и ложим в переменную SC
		Kep = IO.GetComponent<Keeper>();				// Ложим в переменную Kep скрипт Keeper
		GM.StartGameEvent += StartGame;					// Подписываем метод "Старт игры" на событие StartGameEvent
		GM.GoalEvent += Goal;							// Подписываем метод "Гол" на событие GoalEvent
		GM.LastGoalEvent += LastGoal;					// Подписываем метод LastGoal на событие LastGoalEvent
		SAI = IO.GetComponent<StoreAndInventory>();		// Получаем от объекта IndestructibleObject скрипт StoreAndInventory и ложим в переменную SAI
		SC = IO.GetComponent<StoreContent>();			// Получаем от объекта IndestructibleObject скрипт StoreContent и ложим в переменную SC
		LoadTexsFromProject();							// Заполняем публичные переменные текстурами из проекта
		CalculateRects();								// Расчитываем прямоугольники для призов
	//	Sprite Testing = Sprite.Create(Kep.FPAA[0], new Rect(0, 0, 120, 120), new Vector2(0, 0));
	//	PlayersInform.transform.FindChild("Test").GetComponent<Image>().sprite = Testing;
	}


	void LoadTexsFromProject()							// Этот метод заполняет все указанные публичные переменные указанными текстурами
	{
		BlackOut = Resources.Load(GUIAdress + "Blackout") as Texture2D;								// Загружаем текстуру затенения
		for(byte a=0; a<RightProfileWindows.Length; a++) 
		{
			Sprite Timed = Sprite.Create(Resources.Load(GUIAdress + "RightFramesProfiles/RightFrameProfile_" + a) as Texture2D, new Rect(0, 0, 450, 145), new Vector2(0, 0));	// Создаём из текстуры спрайт для правого игрока
			RightProfileWindows[a] = Timed;															// Загружаем созданный спрайт в очередную ячейку массива правого игрока
		}
		for(byte a=0; a<LeftProfileWindows.Length; a++)
		{
			Sprite Timed = Sprite.Create(Resources.Load(GUIAdress + "LeftFramesProfiles/LeftFrameProfile_" + a) as Texture2D, new Rect(0, 0, 450, 145), new Vector2(0, 0));		// Создаём из текстуры спрайт для левого игрока
			LeftProfileWindows[a] = Timed;															// Загружаем созданный спрайт в очередную ячейку массива левого игрока	
		}

		FrameForAvatar = Resources.Load(GUIAdress + "FrameForAvatar") as Texture2D;					// Загружаем текстуру окошка аватара
		Conformation = Resources.Load(GUIAdress + "Confirmation") as Texture2D;						// Загружаем текстуру окошка подтверждения
		WindowInventory = Resources.Load(GUIAdress + "WindowInventory") as Texture2D;				// Загружаем текстуру инвентаря
		OverviewWindowObject = Resources.Load(GUIAdress + "OverviewWindowObject") as Texture2D;		// Загружаем текстуру окна обзора объекта
	}


	void _StartSecondStep_()							// Этот метод "Начать второй шаг" вызываеться когда первый шаг закончиться
	{
		Loading = true;									// Указываем что мы вызвали следующие за коронтиной FillObjTex методы (Их загрузка завершена)
	}


	void StartGame()									// Этот метод вызываеться при нажатии кнопки старт
	{
		LPS = 0; RPS = 0;								// Обнуляем RPS и LPS на случай если раунд играеться уже не превый раз
		InvokeRepeating("StopWatch", 0, 1);			// Запускаем секундомер
		Sprite RA = Sprite.Create(RightAv, new Rect(0,0, 120, 120), new Vector2(0,0));			// Преобразуем аватарку правого игрока из Texture2D в Sprite
		Sprite LA = Sprite.Create(LeftAv, new Rect(0,0, 120, 120), new Vector2(0,0));			// Преобразуем аватарку левого игрока из Texture2D в Sprite
		PlayersInform.transform.Find("RightAvatar").GetComponent<Image>().sprite = RA;		// Присваиваем аватарку правого игрока UI элементу отображающему аватарку правого игрока
		PlayersInform.transform.Find("LeftAvatar").GetComponent<Image>().sprite = LA;		// Присваиваем аватарку левого игрока UI элементу отображающему аватарку правого игрока
		RT = PlayersInform.transform.Find("RightName").GetComponent<Text>();				// Сохраняем переменную UI текст правого игрока в переменную RT
		LT = PlayersInform.transform.Find("LeftName").GetComponent<Text>();				// Сохраняем переменную UI текст левого игрока в переменную LT
		PlayersInform.transform.Find("RightFrameProfile").GetComponent<Image>().sprite = RightProfileWindows[LPS];		// Меняем информационное окно правого игрока на окно соответствующее его счёту
		PlayersInform.transform.Find("LeftFrameProfile").GetComponent<Image>().sprite = LeftProfileWindows[RPS];		// Меняем информационное окно левого игрока на окно соответствующее его счёту
		RT.text = RPN;																			// Присваиваем UI тексту правого игрока имя правого игрока
		LT.text = LPN;																			// Присваиваем UI тексту левого игрока имя левого игрока
		LT.fontSize = RT.fontSize = 20; 														// Ставим размер текста переменным LT и RT снова 20 на случай если раунд играеться уже не превый раз чтобы при старте он не уменьшился ещё больше
		if(RT.text.Length > 15) RT.fontSize -= (RT.text.Length - 15);							// Если длинна текста правого игрока больше 15 символов то уменьшаем размер на количество длинны букв превышающих это число
		if(LT.text.Length > 15) LT.fontSize -= (LT.text.Length - 15);							// Если длинна текста левого игрока больше 15 символов то уменьшаем размер на количество длинны букв превышающих это число

		BatObjScr = GM.Player1.GetComponent<ObjectScript>();									// Берём из биты первого игрока скрипт ObjectScript и ложим его в переменную BatObjScr
		PuckObjScr = GM.Puck.GetComponent<ObjectScript>();										// Берём из шайбы перовго игрока скрипт ObjectScript и ложим его в переменную PuckObjScr


		PlayersInform.enabled = true;															// Включаем отображение канваса
	}


	void Goal()											// Этот метод вызываеться событием CallGoalEvent
	{
		PlayersInform.transform.Find("RightFrameProfile").GetComponent<Image>().sprite = RightProfileWindows[LPS];		// Меняем информационное окно правого игрока на окно соответствующее его счёту
		PlayersInform.transform.Find("LeftFrameProfile").GetComponent<Image>().sprite = LeftProfileWindows[RPS];		// Меняем информационное окно левого игрока на окно соответствующее его счёту
	}


	void LastGoal()										// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		PlayersInform.transform.Find("RightFrameProfile").GetComponent<Image>().sprite = RightProfileWindows[LPS];		// Меняем информационное окно правого игрока на окно соответствующее его счёту
		PlayersInform.transform.Find("LeftFrameProfile").GetComponent<Image>().sprite = LeftProfileWindows[RPS];		// Меняем информационное окно левого игрока на окно соответствующее его счёту
		StartCoroutine(AfterLastGoal());				// Вызываем коронтину AfterGoal	
	}


	IEnumerator AfterLastGoal()									// Этот метод вызываеться после выполнения метода LastGoal
	{
		yield return new WaitForSeconds(1);						// Ждём 1 секунду
		Window = GameMenuWins.Finish;							// Выводим окно финиш
		PlayersInform.GetComponent<Canvas>().enabled = false;	// Делаем невидимым канвас
		CancelInvoke();											// Выключаем секундомер
		string Minute = Second = "0";							// Ставим минутам и секундам переведённым в строку текст 0
	}
		

	void Update()
	{
		Clock();			// Вызываем метод обновляющий реальное время
		EscapeButton ();	// Вызываем метод который просчитывает что произойдёт по нажатии клавиши Escape
		if(SceneManager.GetActiveScene().buildIndex > 1 && Window == GameMenuWins.Finish)									// Иначе если мы отрисовываем окно Финиша в режиме прохождения
		{
			FinishCursorDelayTimer();																						// Вызываем метод узнающий находиться ли курсор над только что открытым призом
		}
	}

	
	void Clock()			// Этот метод обновляет реальное время на каждый кадр
	{
		Rhour = DateTime.Now.Hour.ToString("<color='#00fc00'>00</color>");
		Rminute = DateTime.Now.Minute.ToString("<color='#00fc00'>00</color>");
		Rsecond = DateTime.Now.Second.ToString("<color='#00fc00'>00</color>");
		RealTime = Rhour + "<color='#00fc00'> : </color>" + Rminute + "<color='#00fc00'> : </color>" + Rsecond;
	}


	void StopWatch()		// Этот метод Вызываеться раз в секунду и отсчитывает время от начала до конца раунда
	{
		if(TimeMoves)		// Если переменная TimeMoves равна правда
			Sec ++;			// Плюсуем одну секунду
		if(Sec > 59)		// Если значение переменной секунда больше 59
		{
			Sec = 0;		// То обнуляем значение переменной секунда
			Min ++ ;		// И плюсуем переменной минута
		}
		Minute = Min.ToString("<color='#00fc00'><size=40>00</size></color>");
		Second = Sec.ToString("<color='#00fc00'><size=40>00</size></color>");
		RoundTime = Minute + "<color='#00fc00'><size=50> : </size></color>" + Second;
	}
	

	void EscapeButton ()						// Этот метод высчитывает что произойдёт по нажатии на кнопку Escape
	{
		if(Input.GetKeyDown(KeyCode.Escape))	// Если была нажата клавиша "Escape"
		{
			switch(Window)
			{
			case GameMenuWins.Inventory:		// Если окно было равно 1 "Inventory"
				SAI.CameraClone.GetComponent<Camera>().targetTexture = null;			// Удаляем рендер текстуру с камеры для камеры рендер текстуру
				GameObject.DestroyImmediate(SAI.CameraClone);							// Удаляем со сцены клон префаба фотографа
				GameObject.DestroyImmediate(SAI.VideoObjClone);							// Удаляем со сцены объект который мы обозревали
				SAI.LastProf = 10;														// Ставим LastProfile 10 чтобы при повторном входе в магазин заного срабатывал метод FillActiveCellInf
				GM.CalculatePosObjects();												// То на случай если была изменена бита или шайба пересчитываем стартовые позиции биты и шайбы
				Window = GameMenuWins.Start;	// То мы переходим в окно 0 "Start"
				break;
			case GameMenuWins.Game:				// Если окно было равно 2 "Game"
				Window = GameMenuWins.Pause;	// То мы переходим в окно 6 "Pause"
				break;
			case GameMenuWins.Pause:			// Если окно было равно 3 "Pause"
				Window = GameMenuWins.Game;		// То мы переходим в окно 5 "Game"
				break;
			}
		}
	}


	void OnGUI()
	{
		GUI.depth = 2;						// Устанавливаем дальность гуи от камеры на второй слой
		GUI.skin = GameSkin;				// Устанавливаем игровой скин

		if(Window == GameMenuWins.Start)	// Это окно настройки уровня показываеться каждый раз при старте любого уровня
		{
			if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)			// И если это один из уровеней прохождения
			{
				LevelSettings();			// Вызываем метод отрисовки настройки уровня
			}
			else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)		// Иначе если это уровень игры на двоих
			{
				LevelSettingsForTwo();		// Вызываем метод отрисовки настройки уровня на двоих
			}
		}

		if(Window == GameMenuWins.Inventory)	// Если окно равно инвентарь
		{
			SAI.Inventory();					// Вызываем метод отрисовки инвентаря
		}
			
		if(Window == GameMenuWins.Game)		// Окно 0 работает когда идёт игра
		{
			Time.timeScale = 1;				// Устанавливаем скорость течения времени на нормально
			Cursor.visible = false;			// Делаем стандартный Windows курсор невидимым

			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.Label(new Rect(-100, 0, 200, 52), "", "TimerBox");												// Рисуем рамку для часов
			GUI.Label(new Rect(-65, -2, 20, 20), RoundTime, Neon);												// Рисуем часы (Время раунда)

			// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего левого края экрана
	//		GUI.matrix = Matrix4x4.TRS(new Vector3(0, Screen.height,0),Quaternion.identity ,new Vector3(RatioH, RatioH, 1));
	//		GUI.Label(new Rect(0, -145, 450, 145), LeftProfileWindows[RPS]);									// Отрисовываем окно информации левого игрока и голы забитые ему правым игроком
	//		GUI.DrawTexture(new Rect(11, -131, 120, 120), LeftAv);												// Рисуем портрет игрока с лева
	//		GUI.Label(new Rect(165, -38, 270, 30), LPN, "FirstPlayerName");										// Пишем ник игрока с лева

			// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего правого края экрана
	//		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width, Screen.height,0),Quaternion.identity, new Vector3(RatioH, RatioH, 1));
	//		GUI.Label(new Rect(-450, -145, 450, 145), RightProfileWindows[LPS]);								// Отрисовываем окно информации правого игрока и голы забитые ему левым игроком
	//		GUI.DrawTexture(new Rect(-131, -131, 120, 120), RightAv);											// Рисуем портрет игрока с права
	//		GUI.Label(new Rect(-435, -38, 270, 30), RPN, "SecondPlayerName");									// Пишем ник игрока с права
		}

		if(Window == GameMenuWins.Pause) // Окно это окно паузы
		{
			// немного ниже после каждого гола меньшими цифрами отрисовываеться время раунда.

			Time.timeScale = 0;			// Останавливаем время
			UC.SetCursorVisible = true;	// Делаем пользовательский курсор видимым

			// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего левого края экрана
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, Screen.height,0),Quaternion.identity,new Vector3(RatioH, RatioH, 1));
			GUI.DrawTexture(new Rect(10, -140, 128, 128), FrameForAvatar);					// Рисуем Портрет игрока 1
			
			// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего правого края экрана
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width, Screen.height,0),Quaternion.identity,new Vector3(RatioH, RatioH, 1));
			GUI.DrawTexture(new Rect(-140, -140, 128, 128), FrameForAvatar);				// Рисуем Портрет игрока 1

			GUI.matrix = Matrix4x4.identity;												// Ставим матрицу по умолчанию 
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BlackOut);			// Рисуем окно затемнения

			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.DrawTexture(new Rect(-250, 45, 500, 100),PauseBoxScore);					// Рисуем окно очков паузы
	//		GUI.Label(new Rect(-190, 96, 0, 0), "Общий счёт", ScoreStyle);					// Рисуем текст "общий счёт"
	//		GUI.Label(new Rect(120, 94, 0, 0), "-", ScoreStyle);							// Рисуем тире счёта игроков в меню паузы
	//		GUI.Label(new Rect(90, 97, 0, 0), ""+ScorePlayer1, ScoreStyle);					// Рисуем очки первого игрока в меню паузы
	//		GUI.Label(new Rect(140, 97, 0, 0),"" +ScorePlayer2, ScoreStyle);				// Рисуем очки второго игрока в меню паузы
			GUI.DrawTexture(new Rect(-200, 270, 400, 500), BoxPauseMenu);					// Рисуем окно меню паузы					
			if(GUI.Button(new Rect(-93, 345, 186, 42), "Продолжить"))		// Если была нажата кнопка "Продолжить"
			{
				Window = GameMenuWins.Game;									// Вызываем окно игры
			}
			if(GUI.Button(new Rect(-93, 415, 186, 42), "В главное меню"))	// Если была нажата кнопка "В главное меню"
			{
				// Сделать второй скрипт плюс как в главном меню и вместо вызова ещё одного окна будем отрисовывать окно подверждения поверх
				// Window = 2;												// Вызываем окно подтверждения
			}
			if(GUI.Button(new Rect(-93, 695, 186, 42), "Выход из игры"))
			{
				// Сделать второй скрипт плюс как в главном меню и вместо вызова ещё одного окна будем отрисовывать окно подверждения поверх
				// Window = 3;												// Вызываем окно подтверждения
			}
			GUI.Label(new Rect(-10,780,20,20), RealTime, Neon);				// Рисуем часы (Реальное время)
		}
					

	//	if(Window == 0)	// Окно 2 это окно подтверждения выхода в главное меню			
	//	{
	//		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TexArry[0]);			// Рисуем окно затемнения
	//
	//		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
	//		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
	//		GUI.DrawTexture(new Rect(-238, -100, 476, 140), TexArry[3]);											// Рисуем окошко подтверждения
	//		GUI.Label(new Rect(0, -75, 0, 0), "<color='#00fc00'>Выйти в главное меню?</color>", "ConfurmationText");// Рисуем текст подтверждения			
	//		if(GUI.Button(new Rect(-204, 10, 186, 42), "Да"))									// Рисуем кнопку "Да"
	//		{
	//			Application.LoadLevel(0);
	//		}
	//
	//		if(GUI.Button(new Rect(18, 10, 186, 42), "Нет"))									// Рисуем кнопку "Нет"
	//		{
	//			Window = 1;
	//		}
	//	}

	//	if(Window == 3)	// Окно 3 это окно подтверждения выхода из игры
	//	{
	//		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TexArry[0]);							// Рисуем окно затемнения
	//
	//		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
	//		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
	//		GUI.DrawTexture(new Rect(-238, -100, 476, 140), TexArry[3]);										// Рисуем окошко подтверждения
	//		GUI.Label(new Rect(0, -75, 0, 0), "<color='#00fc00'>Выйти из игры?</color>", "ConfurmationText");	// Рисуем текст подтверждения
	//		if(GUI.Button(new Rect(-204, 10, 186, 42), "Да"))													// Рисуем кнопку "Да"
	//		{
	//			Application.Quit();
	//		}
	//		
	//		if(GUI.Button(new Rect(18, 10, 186, 42), "Нет"))													// Рисуем кнопку "Нет"
	//		{
	//			Window = 1;
	//		}
	//	}


		if(Window == GameMenuWins.Finish)// Это окно показываеться когда матч закончен, выводиться общий счёт и предлогаеться выбрать действие.
		{
			Cursor.visible = true;																// Включаем курсор
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.BeginGroup(new Rect(-340, -430, 680, 900), Results);							// Начинаем группу и рисуем окно результатов
			GUI.Button(new Rect(10, 70, 128, 128), LeftAv, "Avatar");							// Рисуем портрет игрока с лева
			GUI.Button(new Rect(541, 70, 128, 128), RightAv, "Avatar");							// Рисуем портрет игрока с лева
			GUI.Label(new Rect(12, 195, 270, 30), LPN, "FirstPlayerName");						// Пишем ник игрока с лева
			GUI.Label(new Rect(395, 195, 270, 30), RPN, "SecondPlayerName");					// Пишем ник игрока с права
			GUI.Label(new Rect(310, 185, 0, 0),"" + LPS, "ScoreStyle");							// Пишем голы игрока с лева
			GUI.Label(new Rect(370, 185, 0, 0),"" + RPS, "ScoreStyle");							// Пишем голы игрока с права
			GUI.Label(new Rect(339, 181, 0, 0), "-", "ScoreStyle");								// Рисуем тире в счёте игроков
			GUI.Label(new Rect(29, 762, 100, 20), "Раунд");										// Рисуем слово Раунд
			GUI.Label(new Rect(135, 762, 100, 20), "Счёт");										// Рисуем слово Раунд
			GUI.Label(new Rect(272, 762, 200, 20), "Время раунда");								// Рисуем заголовок "Время раунда"
			GUI.Label(new Rect(513, 762, 200, 20), "Общее время");								// Рисуем заголовок "Общее время"

			GUI.Label(new Rect(326, 70, 30, 30), FullTimeMin[Rounds-1] + " : " + FullTimeSec[Rounds-1], "CreditsResults");				// Рисуем общее время
			GUI.Label(new Rect(220, 70, 30, 30), LPFC + "", "CreditsResults");					// Финальное количество кредитов левого игрока
			GUI.Label(new Rect(430, 70, 30, 30), RPFC + "", "CreditsResults");					// Финальное количество кредитов правого игрока

			if(Kep.RightSide[Kep.ActiveProfile])												// Если игрок с права это человек
			{
				GUI.Label(new Rect(392, 110, 30, 30), BatExp + "", "NomberXP");					// Выводим опыт набранный битой за этот уровень битой (с права)
				GUI.Label(new Rect(392, 130, 30, 30), PuckExp + "", "NomberXP");				// Выводим опыт набранный шайбой за этот уровень шайбой (с права)
				GUI.Label(new Rect(430, 110, 70, 30), "EXP Bat", "RightXp");					// Выводим буквы означающие что эта строка означает опыт биты (с права)
				GUI.Label(new Rect(430, 130, 70, 30), "EXP Puck", "RightXP");					// Выводим буквы означающие что эта строка означает опыт шайбы (с права)

				GUI.Label(new Rect(255, 110, 30, 30), SecondBatExp + "", "NomberXP");			// Выводим опыт набранный битой за этот уровень битой (с лева)
				GUI.Label(new Rect(255, 130, 30, 30), SecondPuckExp + "", "NomberXP");			// Выводим опыт набранный шайбой за этот уровень шайбой (с лева)
				GUI.Label(new Rect(178, 110, 70, 30), "Bat EXP", "LeftXp");						// Выводим буквы означающие что эта строка означает опыт биты (с лева)	
				GUI.Label(new Rect(178, 130, 70, 30), "Puck EXP", "LeftXp");					// Выводим буквы означающие что эта строка означает опыт шайбы (с лева)
			}
			else if(!Kep.RightSide[Kep.ActiveProfile])											// Если игрок с лева это человек
			{
				GUI.Label(new Rect(255, 110, 30, 30), BatExp + "", "NomberXP");					// Выводим опыт набранный битой за этот уровень (с лева)
				GUI.Label(new Rect(255, 130, 30, 30), PuckExp + "", "NomberXP");				// Выводим опыт набранный шайбой за этот уровень (с лева)
				GUI.Label(new Rect(178, 110, 70, 30), "EXP Bat", "LeftXP");						// Выводим буквы означающие что эта строка означает опыт биты (с лева)
				GUI.Label(new Rect(178, 130, 70, 30), "EXP Puck", "LeftXP");					// Выводим буквы означающие что эта строка означает опыт шайбы (с лева)

				GUI.Label(new Rect(392, 110, 30, 30), SecondBatExp + "", "NomberXP");			// Выводим опыт набранный битой за этот уровень битой (с права)
				GUI.Label(new Rect(392, 130, 30, 30), SecondPuckExp + "", "NomberXP");			// Выводим опыт набранный шайбой за этот уровень шайбой (с права)
				GUI.Label(new Rect(430, 110, 70, 30), "Bat EXP", "RightXP");					// Выводим буквы означающие что эта строка означает опыт биты (с права)
				GUI.Label(new Rect(430, 130, 70, 30), "Puck EXP", "RightXP");					// Выводим буквы означающие что эта строка означает опыт шайбы (с права)
			}

			GUI.Label(new Rect(125, 270, 30, 30), LPRC[0] + "", "CreditsResults");				// Рисуем кредиты левого игрока заработанные за 1 раунд
			GUI.Label(new Rect(125, 375, 30, 30), LPRC[1] + "", "CreditsResults");				// Рисуем кредиты левого игрока заработанные за 2 раунд
			GUI.Label(new Rect(125, 480, 30, 30), LPRC[2] + "", "CreditsResults");				// Рисуем кредиты левого игрока заработанные за 3 раунд

			GUI.Label(new Rect(520, 270, 30, 30), RPRC[0] + "", "CreditsResults");				// Рисуем кредиты правого игрока заработанные за 1 раунд
			GUI.Label(new Rect(520, 375, 30, 30), RPRC[1] + "", "CreditsResults");				// Рисуем кредиты правого игрока заработанные за 2 раунд
			GUI.Label(new Rect(520, 480, 30, 30), RPRC[2] + "", "CreditsResults");				// Рисуем кредиты правого игрока заработанные за 3 раунд

			GUI.Label(new Rect(45, 785, 100, 20), "1");											// Рисуем номер раунда 1
			GUI.Label(new Rect(45, 808, 100, 20), "2");											// Рисуем номер раунда 2
			GUI.Label(new Rect(45, 831, 100, 20), "3");											// Рисуем номер раунда 3

			GUI.Label(new Rect(137, 785, 100, 20), LPSs[0] + " - " + RPSs[0]);					// Рисуем счёт в 1 раунде
			GUI.Label(new Rect(137, 808, 100, 20), LPSs[1] + " - " + RPSs[1]);					// Рисуем счёт в 2 раунде
			GUI.Label(new Rect(137, 831, 100, 20), LPSs[2] + " - " + RPSs[2]);					// Рисуем счёт в 3 раунде

			GUI.Label(new Rect(301, 785, 50, 20), RoundTimeMin[0] + " : " + RoundTimeSec[0]);	// Выводим время 1 раунда
			GUI.Label(new Rect(301, 808, 50, 20), RoundTimeMin[1] + " : " + RoundTimeSec[1]);	// Выводим время 2 раунда
			GUI.Label(new Rect(301, 831, 50, 20), RoundTimeMin[2] + " : " + RoundTimeSec[2]);	// Выводим время 3 раунда

			GUI.Label(new Rect(538, 785, 50, 20), FullTimeMin[0] + " : " + FullTimeSec[0]);		// Выводим общее время к концу 1 раунда
			GUI.Label(new Rect(538, 808, 50, 20), FullTimeMin[1] + " : " + FullTimeSec[1]);		// Выводим общее время к концу 2 раунда
			GUI.Label(new Rect(538, 831, 50, 20), FullTimeMin[2] + " : " + FullTimeSec[2]);		// Выводим общее время к концу 3 раунда


			if(RightGoal[0])											// Если в 1 раунде был забит гол правому игроку
			{
				GUI.Label(new Rect(290, 237, 94, 94), GoalArrow[0]);	// То рисуем стрелку в право							
			}
			else 														// Иначе если был забит гол левому игроку
			{
				GUI.Label(new Rect(290, 237, 94, 94), GoalArrow[1]);	// То рисуем стрелку в лево
			}

			if(RightGoal[1])											// Если во 2 раунде был забит гол правому игроку
			{
				GUI.Label(new Rect(290, 343, 94, 94), GoalArrow[0]);	// То рисуем стрелку в право							
			}
			else 														// Иначе если был забит гол левому игроку
			{
				GUI.Label(new Rect(290, 343, 94, 94), GoalArrow[1]);	// То рисуем стрелку в лево
			}

			if(RightGoal[2])											// Если в 3 раунде был забит гол правому игроку
			{
				GUI.Label(new Rect(290, 449, 94, 94), GoalArrow[0]);	// То рисуем стрелку в право							
			}
			else 														// Иначе если был забит гол левому игроку
			{
				GUI.Label(new Rect(290, 449, 94, 94), GoalArrow[1]);	// То рисуем стрелку в лево
			}

			if(Rounds > 3)	// Если количество раундов больше 3
			{
				GUI.Label(new Rect(125, 585, 30, 30), LPRC[3] + "", "CreditsResults");				// Рисуем кредиты левого игрока заработанные за 4 раунд
				GUI.Label(new Rect(520, 585, 30, 30), RPRC[3] + "", "CreditsResults");				// Рисуем кредиты правого игрока заработанные за 4 раунд
				GUI.Label(new Rect(45, 854, 100, 20), "4");											// Рисуем номер раунда 4
				GUI.Label(new Rect(137, 854, 100, 20), LPSs[3] + " - " + RPSs[3]);					// Рисуем счёт в 4 раунде
				GUI.Label(new Rect(301, 854, 50, 20), RoundTimeMin[3] + " : " + RoundTimeSec[3]);	// Выводим время 4 раунда
				GUI.Label(new Rect(538, 854, 50, 20), FullTimeMin[3] + " : " + FullTimeSec[3]);		// Выводим общее время к концу 4 раунда

				if(RightGoal[3])											// Если в 4 раунде был забит гол правому игроку
				{
					GUI.Label(new Rect(290, 555, 94, 94), GoalArrow[0]);	// То рисуем стрелку в право							
				}
				else 														// Иначе если был забит гол левому игроку
				{
					GUI.Label(new Rect(290, 555, 94, 94), GoalArrow[1]);	// То рисуем стрелку в лево
				}
			}

			if(Rounds > 4) // Если количество раундов больше 4
			{
				GUI.Label(new Rect(125, 690, 30, 30), LPRC[4] + "", "CreditsResults");				// Рисуем кредиты левого игрока заработанные за 5 раунд
				GUI.Label(new Rect(520, 690, 30, 30), RPRC[4] + "", "CreditsResults");				// Рисуем кредиты правого игрока заработанные за 5 раунд
				GUI.Label(new Rect(45, 877, 100, 20), "5");											// Рисуем номер раунда 5
				GUI.Label(new Rect(137, 877, 100, 20), LPSs[4] + " - " + RPSs[4]);					// Рисуем счёт в 5 раунде
				GUI.Label(new Rect(301, 877, 50, 20), RoundTimeMin[4] + " : " + RoundTimeSec[4]);	// Выводим время 5 раунда
				GUI.Label(new Rect(538, 877, 50, 20), FullTimeMin[4] + " : " + FullTimeSec[4]);		// Выводим общее время к концу 5 раунда

				if(RightGoal[4])											// Если в 5 раунде был забит гол правому игроку
				{
					GUI.Label(new Rect(290, 661, 94, 94), GoalArrow[0]);	// То рисуем стрелку в право							
				}
				else 														// Иначе если был забит гол левому игроку
				{
					GUI.Label(new Rect(290, 661, 94, 94), GoalArrow[1]);	// То рисуем стрелку в лево
				}
			}
			if(GUI.Button(new Rect(16, 15, 186, 42), "Сыграть ещё раз"))	// По нажатию кнопки сыграть ещё раз
			{
				if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)			// И если это один из уровеней прохождения
					UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); 	// Загружаем тот же уровень
				else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)		// Иначе если это уровень игры на двоих
					GM.RepeatLevel();																	// Вызываем метод который обнуляет все данные на уровне и запускает стартовое окно
			}
			
			if(GUI.Button(new Rect(246, 15, 186, 42), "В главное меню"))	// По нажатию кнопки "В главное меню"
			{
				GameObject.Destroy(GameObject.Find("IndestructibleObject"));// Находим и уничтожаем на сцене объект IndestructbleObject
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);		// Загружаем главное меню
			}

			GUI.enabled = NextLevelTransition;	// Определяем состояние активности кнопки "Следующий уровень" по переменной NextLevelTransition

			if(GUI.Button(new Rect(476, 15, 186, 42), "Следующий уровень"))	// Если нажата кнопка следующий уровень
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex +1); // Загружаем следующий уровень
			}
			GUI.EndGroup();

			GUI.enabled = true;		// Включаем следующие гуи элементы

			if(ShowPrizes)			// Если игрок выиграл мы показываем ему призы иначе мы не будем вызывать метод RenderPrizes
			{
				RenderPrizes();		// Отрисовываем призы вызвав метод отрисовка призов
			}

			if(ShowInformLabel)		// Если переменная "Показывать информационное окно правда"
			{
				RenderInformLabel();	// Вызываем метод показывающий информационное окно
			}
		}
	}


	void CalculateRects()													// Этот метод расчитывает прямоугольники для призов
	{
		int Left = 10;			 											// Позиция с лева

		for(byte a=0; a<PrizesRects.Length; a++)							// Продолжаем цикл пока не назначим позиции всему массиву прямоугольников для призов
		{
			PrizesRects[a] = new Rect(Left, 9, 84, 84);						// Расчитываем очередной прямоугольник
			Left += 92;														// Прибавляем позиции следующего прямоугольника в лево 92 пикселя
		}
	}


	void RenderPrizes()														// Этот метод отрисовывает кнопки текстур призов и реагирует на их нажатия
	{
	//	Rect CurrentRectangle = new Rect(10, 9, 84, 84);					// Здесь будет отображаться позиция текущего прямоугольника

		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));

		GUI.BeginGroup(new Rect(-281, 0, 562, 100), PrizesWindow);			// Начинаем группу и отрисовываем окно для призов
		for(byte a=0; a<6; a++)												// Продолжаем цикл 6 раз
		{
			if(a<TexsPrizes.Length)											// Если мы не вышли за пределы массива TexPrizes
			{
				if(TexsPrizes[a].name == "QuestionIcon")					// Если у картинки имя "QuestionIcon"
				{ 
					if(PrizeSelection)															// Если переменная "Выбор приза" правда
					{
						if(GUI.Button(PrizesRects[a], TexsPrizes[a], "PrizeSelectionButton")) 	// Отрисовываем кнопку выбора приза и при нажатии на кнопку 
						{
							SelectedPrizeCell = a;												// Записываем номер открытой ячейки для этого раунда
							FillPrizesStates();													// Вызываем метод который заполняет состояние приза для каждой кнопки
							TexsPrizes[a] =	Kep.GiveMePrize(a);									// Мы вызываем метод GiveMePrize
						//	PrizeSelection = false;												// А также ставим переменной PrizeSelection состояние ложь чтобы остальные призы нельзя было выбрать
						}
					}
					else 																		// Иначе если переменная "Выбор приза" ложь
					{
						GUI.Button(PrizesRects[a], TexsPrizes[a], "PrizeSelectionButton");		// Орисовываем кнопку выбора приза
						GUI.Label(PrizesRects[a], SAI.ShadingObject);							// Отрисовываем маску
					}
				}
				else 														// Иначе если мы отрисовываем открытый объект
				{											
					GUI.DrawTexture(PrizesRects[a], NoPrize);				// Отрисовываем иконку для приза
					GUI.DrawTexture(new Rect(PrizesRects[a].x +2, PrizesRects[a].y + 3, PrizesRects[a].width -4, PrizesRects[a].height -6), SAI.ObjsBackgrounds[0]);	// Отрисовываем задний фон имеющегося приза
					GUI.DrawTexture(new Rect(PrizesRects[a].x +2, PrizesRects[a].y + 3, PrizesRects[a].width -4 , PrizesRects[a].height -6), TexsPrizes[a]);			// Отрисовываем картинку имеющегося приза
				}
			}
			else 															// Иначе если мы вышли за пределы массив TexPrizes
			{
				GUI.Label(new Rect(PrizesRects[a]),NoPrize);				// То отрисовываем иконку несуществующего приза
			}						
		}
		GUI.EndGroup();														// Заканчиваем группу
	}


	void LevelSettings()																			// Этот метод отрисовывает окна которые должны отрисовываться при старте
	{
		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
		GUI.BeginGroup(new Rect(-153, 20, 306, 366), WindowLevelSettings[0]);						// Начинаем группу и отрисовываем окно настройки уровня

		GUI.enabled = false;																		// Отключаем отклик следующих гуи на команды
		GUI.Button(new Rect(199, 15, 92, 40), "Профиль", "ButtonProfileSelection");					// Отрисовываем кнопку выбора профиля
		GUI.enabled = true;																			// Включаем отклик следующих гуи на команды

		if(GUI.Button(new Rect(107, 15, 92, 40), "Старт","StartButton"))							// Рисуем кнопку старт уровня
		{
			GM.CallStartGame();																		// Вызываем метод вызывающий событие старта игры
			Window = GameMenuWins.Game;																// Делаем активным окно игры
		}
		if(Loading)																					// Если переменная Loading стала правда
		{	
			if(Kep.RightSide[Kep.ActiveProfile])														// Если переменная RightSide для активного профиля правда 
			{
				if(GUI.Button(new Rect(15, 15, 92, 40), "", "ButtonInvertRight"))						// Отрисовываем кнопку смены стороны для активного профиля и если она была нажата
				{
					Kep.RightSide[Kep.ActiveProfile] = false;											// Ставим состояние переменной RightSide ложь
					GM.RestartGameObjects();															// Вызываем метод перезагрузки объектов
				}
			}
			else 																						// Иначе если переменная RightSide для активного профиля ложь 
			{
				if(GUI.Button(new Rect(15, 15, 92, 40), "", "ButtonInvertLeft"))						// Отрисовываем кнопку смены стороны для активного профиля и если она была нажата
				{
					Kep.RightSide[Kep.ActiveProfile] = true;											// Ставим состояние переменной RightSide ложь
					GM.RestartGameObjects();															// Вызываем метод перезагрузки объектов
				}
			}

			if(GUI.Button(new Rect(15, 75, 128, 128),  Kep.FPAA[0],"ButtonElementSelection"))			// Если кнопка выбора биты была нажата (отрисовываем на ней задний фон)
			{
				SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина	
				SAI.ActiveCategory = 0;																// Ставим номер активной категории инвентаря "Биты"
				Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
			}					//15
		//	GUI.Label(new Rect(50, 75, 120, 120), Kep.FPAA[0], "ActiveObjectTexture");				// Отрисовываем фотографию объекта поверх кнопки
		//	Kep.Test = Kep.FPAA;

			if(GUI.Button(new Rect(163, 75, 128, 128), Kep.FPAA[1],"ButtonElementSelection"))		// Если кнопка выбора шайбы была нажата (отрисовываем на ней задний фон)
			{
				SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
				SAI.ActiveCategory = 1;																// Ставим номер активной категории инвентаря "Шайбы"
				Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
			}
		//	GUI.Label(new Rect(163, 75, 120, 120), Kep.FPAA[1], "ActiveObjectTexture");				// Отрисовываем фотографию объекта поверх кнопки

			if(GUI.Button(new Rect(15, 226, 128, 128), Kep.NotExist,"ButtonElementSelection"))		// Если кнопка выбора стола была нажата (отрисовываем на ней задний фон)
			{
				
			}
				
			if(GUI.Button(new Rect(163, 226, 128, 128), Kep.NotExist,"ButtonElementSelection"))		// Если кнопка выбора скайбокса была нажата
			{
				
			}
		}
		GUI.EndGroup();															// Заканчиваем группу
	}


	void LevelSettingsForTwo()													// Этот метод отрисовывает окна которые должны отрисовываться при старте
	{
		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
		GUI.BeginGroup(new Rect( -153, -450, 306, 390), WindowLevelSettings[Kep.NomberPlayer - 1]);		// Начинаем группу и отрисовываем окно настройки уровня
		GUI.enabled = LevelSettingsEnabled;																// Определяем активны ли следующие гуи элементы или нет
		GUI.enabled = StartEnabled;																		// Определяем активна ли кнопка старта уровня
		if(GUI.Button(new Rect(107, 39, 92, 40), "Старт","StartButton"))								// Рисуем кнопку старт уровня
		{
			GM.CallStartGame();																			// Вызываем метод вызывающий событие старта игры
			Window = GameMenuWins.Game;																	// Делаем активным окно игры
		}

		GUI.enabled = true;																				// Определяем остальные кнопки как активные

		if(GUI.Button(new Rect(0, 0, 153, 24), "Игрок 1","PlayerSelectionSettingsButton"))				// Если была нажата кнопка настроек первого игрока
		{
			Kep.NomberPlayer = 1;																		// Выбираем окно настроек первого игрока
			Kep.ViewedProfile = Kep.ActiveProfile;														// Номер просматриваемого профиля берём у 1 игрока
			GM.Currentkep = Kep;																		// Присваиваем текущей базе данных глобальную базу данных
		}
		if(GUI.Button(new Rect(154, 0, 153, 24), "Игрок 2","PlayerSelectionSettingsButton"))			// Если была нажата кнопка настроек второго игрока
		{
			Kep.NomberPlayer = 2;																		// Выбираем окно настроек второго игрока
			Kep.ViewedProfile = Kep.SecondActiveProfile;												// Номер просматриваемого профиля берём у второго игрока
			if(Kep.ActiveProfile == Kep.SecondActiveProfile) GM.Currentkep = GM.LocalKep;				// Если оба игрока используют один профиль то мы присваиваем текущей базе данных локальную базу данных		
			else GM.Currentkep = Kep;																	// Иначе если игроки используют разные профили Присваиваем текущей базе данных глобальную базу данных
		}
		if(Loading)																						// Если переменная Loading стала правда
		{	
			if(Kep.RightSide[Kep.ActiveProfile])														// Если переменная RightSide для активного профиля правда 
			{
				if(GUI.Button(new Rect(15, 39, 92, 40), "", "ButtonInvertRight"))						// Отрисовываем кнопку смены стороны для активного профиля и если она была нажата
				{
					Kep.RightSide[Kep.ActiveProfile] = false;											// Ставим состояние переменной RightSide ложь
					GM.RestartGameObjects();															// Вызываем метод перезагрузки объектов
				}
			}
			else 																						// Иначе если переменная RightSide для активного профиля ложь 
			{
				if(GUI.Button(new Rect(15, 39, 92, 40), "", "ButtonInvertLeft"))						// Отрисовываем кнопку смены стороны для активного профиля и если она была нажата
				{
					Kep.RightSide[Kep.ActiveProfile] = true;											// Ставим состояние переменной RightSide ложь
					GM.RestartGameObjects();															// Вызываем метод перезагрузки объектов
				}
			}
			if(Kep.NomberPlayer == 1)																	// Если мы выбираем объекты для первого игрока
			{
				GUI.enabled = false;																	// Отключаем отклик следующих гуи на команды
				GUI.Button(new Rect(199, 39, 92, 40), "Профиль", "ButtonProfileSelActivator");			// Отрисовываем кнопку выбора профиля
				GUI.enabled = true;																		// Включаем отклик следующих гуи на команды

				if(GUI.Button(new Rect(15, 99, 128, 128), Kep.FPAA[0],"ButtonElementSelection"))		// Если кнопка выбора биты была нажата (отрисовываем на ней задний фон)
				{
					SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина	
					SAI.ActiveCategory = 0;																// Ставим номер активной категории инвентаря "Биты"
					Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
				}
			//	GUI.Label(new Rect(15, 99, 120, 120), Kep.FPAA[0], "ActiveObjectTexture");				// Отрисовываем фотографию объекта поверх кнопки

				if(GUI.Button(new Rect(163, 99, 128, 128), Kep.FPAA[1],"ButtonElementSelection"))		// Если кнопка выбора шайбы была нажата (отрисовываем на ней задний фон)
				{
					SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
					SAI.ActiveCategory = 1;																// Ставим номер активной категории инвентаря "Шайбы"
					Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
				}
			//	GUI.Label(new Rect(163, 99, 120, 120), Kep.FPAA[1], "ActiveObjectTexture");				// Отрисовываем фотографию объекта поверх кнопки

				if(GUI.Button(new Rect(15, 247, 128, 128), Kep.FPAA[2],"ButtonElementSelection"))		// Если кнопка выбора стола была нажата (отрисовываем на ней задний фон)
				{
					if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer") 	// Если это сцена игрок против игрока
					{
						SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
						SAI.ActiveCategory = 2;																// Ставим номер активной категории инвентаря "Столы"
						Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
					}
				}
			//	GUI.Label(new Rect(15, 247, 120, 120), Kep.FPAA[2], "ActiveObjectTexture");					// Отрисовываем фотографию объекта поверх кнопки

				if(GUI.Button(new Rect(163, 247, 128, 128), Kep.FPAA[3],"ButtonElementSelection"))			// Если кнопка выбора скайбокса была нажата
				{
					if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer") 	// Если это сцена игрок против игрока
					{
						SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
						SAI.ActiveCategory = 3;																// Ставим номер активной категории инвентаря "Скайбоксы"
						Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
					}
				}
			}
			else if(Kep.NomberPlayer == 2) 																	// Иначе если мы выбираем объекты для второго игрока
			{
				if(GUI.Button(new Rect(199, 39, 92, 40), "Профиль", "ButtonProfileSelActivator"))			// Отрисовываем кнопку выбора профиля и если она нажата
				{
					PMP.SecondProfileSelection = true;														// Указываем что пора отрисовывать окно для выбора профиля второго игрока
					LevelSettingsEnabled = false;															// Ставим что кнопки окна настройки уровня неактивны
				}
				if(Kep.SecondActiveProfile == 10)															// Если активный профиль второго игрока отсутствует
				{
					GUI.Button(new Rect(15, 99, 128, 128), "", "ButtonElementSelection");
					GUI.Button(new Rect(15, 99, 128, 128), "Выберите профиль для второго игрока", "ButtonElementText");

					GUI.Button(new Rect(163, 99, 128, 128), "", "ButtonElementSelection");
					GUI.Button(new Rect(163, 99, 128, 128), "Выберите профиль для второго игрока", "ButtonElementText");

					GUI.Button(new Rect(15, 247, 128, 128), "", "ButtonElementSelection");
					GUI.Button(new Rect(15, 247, 128, 128), "Выберите профиль для второго игрока", "ButtonElementText");

					GUI.Button(new Rect(163, 247, 128, 128), "", "ButtonElementSelection");
					GUI.Button(new Rect(163, 247, 128, 128), "Выберите профиль для второго игрока", "ButtonElementText");
				}
				else 																							// Иначе если у нас есть активный профиль для второго игрока
				{
					if(GUI.Button(new Rect(15, 99, 128, 128), GM.Currentkep.SPAA[0],"ButtonElementSelection"))	// Если кнопка выбора биты была нажата
					{
						SAI.PreparationForTheOpeningStore();													// Вызываем метод подготовки к открытию магазина	
						SAI.ActiveCategory = 0;																	// Ставим номер активной категории инвентаря "Биты"
						Window = GameMenuWins.Inventory;														// Вызываем окно инвентаря
					}
				//	GUI.Label(new Rect(15, 99, 120, 120), GM.Currentkep.SPAA[0], "ActiveObjectTexture");		// Отрисовываем фотографию объекта поверх кнопки

					if(GUI.Button(new Rect(163, 99, 128, 128),  GM.Currentkep.SPAA[1],"ButtonElementSelection"))// Если кнопка выбора шайбы была нажата
					{
						SAI.PreparationForTheOpeningStore();													// Вызываем метод подготовки к открытию магазина
						SAI.ActiveCategory = 1;																	// Ставим номер активной категории инвентаря "Шайбы"
						Window = GameMenuWins.Inventory;														// Вызываем окно инвентаря
					}
				//	GUI.Label(new Rect(163, 99, 120, 120),  GM.Currentkep.SPAA[1], "ActiveObjectTexture");		// Отрисовываем фотографию объекта поверх кнопки

					if(GUI.Button(new Rect(15, 247, 128, 128), GM.Currentkep.SPAA[2],"ButtonElementSelection"))	// Если кнопка выбора стола была нажата
					{
						if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer") 	// Если это сцена игрок против игрока
						{
							SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
							SAI.ActiveCategory = 2;																// Ставим номер активной категории инвентаря "Столы"
							Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
						}
					}
				//	GUI.Label(new Rect(15, 247, 120, 120), GM.Currentkep.SPAA[2], "ActiveObjectTexture");		// Отрисовываем фотографию объекта поверх кнопки

					if(GUI.Button(new Rect(163, 247, 128, 128), GM.Currentkep.SPAA[3],"ButtonElementSelection"))// Если кнопка выбора скайбокса была нажата
					{
						if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer") 	// Если это сцена игрок против игрока
						{
							SAI.PreparationForTheOpeningStore();												// Вызываем метод подготовки к открытию магазина
							SAI.ActiveCategory = 3;																// Ставим номер активной категории инвентаря "Скайбоксы"
							Window = GameMenuWins.Inventory;													// Вызываем окно инвентаря
						}
					}
				}
			}
		}
		GUI.enabled = true;																						// Включаем следующие гуи элементы
		GUI.EndGroup();																							// Заканчиваем группу
	}
		

	// Этот метод отрисовывает кнопки материалов либо улучшений и возвращает номер выбранного объекта
	byte RenderGroup(Rect[] Rects, byte Selected, List<Texture2D> Textures, Texture2D Tex, GUIStyle Style)
	{
		for(byte a=0; a<Textures.Count ;a++)						// Продолжаем цикл пока у нас есть текстуры для отрисовки
		{
			if(a == Selected)										// Если в этом цикле мы наткнулись активную выбранную кнопку
			{
				GUI.DrawTexture(Rects[a], Tex);						// Отрисовываем псевдокнопку из бокса
				GUI.DrawTexture(new Rect(Rects[a].position.x + 1, Rects[a].position.y + 1, 40, 40), Textures[a]);// Отрисовываем контент псевдокнопки
			}
			else if(GUI.Button(Rects[a], Textures[a], Style))		// Если одна из отрисовываемых кнопок была нажата 
			{
				Selected = a;										// Присваиваем переменной Selected номер кнопки
			}
		}
		if(Textures.Count <= 0)
		{
			GUI.Label(new Rect(568, 440, 200, 200), "<Color=black>Улучшения для этого объекта отсутствуют</Color>");
		}
		return Selected;
	}


	void FinishCursorDelayTimer()									// Этот метод определяет не находиться ли курсор над только что открытым объектом (Призом)в окошке финиша, если да то сколько
	{
		bool Coincidence = false;									// Переменная говорящая обнаружилось ли совпадение позиции курсора и открытой ячейки

		Vector2 CursorPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);	// Настоящая позиция курсора
		CursorPos = new Vector2(((CursorPos.x - Screen.width/2) / RatioH), ((CursorPos.y) / RatioH));	// Учитываем то что курсор ищет квадрат изменённый матрицей (Отсчёт координат от центра по ширине и с верху по высоте)
		CursorPos = new Vector2(CursorPos.x +281,  CursorPos.y);										// Учитываем что курсор ищет квадрат в группе 

		if(ShowPrizes && PrizeSelection == false)					// Если игрок выиграл и для него показываються призы, а также если был открыт какойто приз		
		{															// То мы можем начать процедуру проверки курсора над прямоугольником и отсчёта времени нахождения над ним
			for(byte a=0; a<PrizesRects.Length; a++)				// Определяем над какой ячейкой по счёту находиться курсор и находиться ли
			{
				if(PrizesRects[a].Contains(CursorPos) && SelectedPrizeCell == a)	// Если курсор находиться над прямоугольником объекта этого цикла и если он совпадает с открытой ячейкой.
				{
					Coincidence = true;								// Ставим переменной совпадение правда 
				}
			}
		}
			
		if(Coincidence)												// Если стрелка находиться над курсором
		{
			CursorDelay += Time.deltaTime;							// Прибавляем ко времени задержки время отрисовки текущего кадра

			if(CursorDelay > 1) 									// Если курсор удерживаеться на этой клетке более 1х секунды
			{
				if(StatesPrizes[SelectedPrizeCell] == 'O')			// И если мы работаем с объектом
				{
					InformObjScr = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>();
				}
				else if(StatesPrizes[SelectedPrizeCell] == 'M')		// Иначе если мы работаем с материалом стола
				{
					InformMatName = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>().NamesMaterials[NombersPrizes[SelectedPrizeCell]];
				}
				else if(StatesPrizes[SelectedPrizeCell] == 'I')		// Иначе емли мы работаем с улучшением стола
				{
					// Получаем из выбранного объекта номер улушения над которым завис курсор
					InformImprScr = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>().Improvements[NombersPrizes[SelectedPrizeCell]].GetComponent<ImprovementScript>();
					// Ложим имя улучшения над которым завис курсор в переменную InformMatName	
					InformImprName = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>().NamesImprovements[NombersPrizes[SelectedPrizeCell]];
					// Ложим имя типа улучшения над которым завис курсор в переменную InformImprType
					InformImprType = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>().NamesTypesImprovements[NombersPrizes[SelectedPrizeCell]];
					// Ложим массу типа улучшения над которым завис курсор в переменную InformImprType
					InformImprMass = SC.ObjectsStore[NombersPrizes[SelectedPrizeCell]].GetComponent<ObjectScript>().Improvements[NombersPrizes[SelectedPrizeCell]].GetComponent<ImprovementScript>().Mass;

					/*
					if(InformImprScr.TypeImprovement == TypeImpr.Backlight_B || InformImprScr.TypeImprovement ==  TypeImpr.Illuminator_B)						// Если тип улучшения Подсветка, Осветитель		
					{
						Color32[] ColorMass = new Color32[400];		// Создаём масив цветов для текстуры цвета улучшения
						for(short a=0; a<ColorMass.Length; a++)		// Продолжаем цикл пока не заполним массив цветов
						{
							ColorMass[a] = InformImprScr.GetComponentInChildren<Light>().color;		// Заполняем массив ColorMass цветом из источника освещения
						}
						ColorImprTexture = new Texture2D(20, 20);	// Создаём текстуру отображающую цвет освещения
						ColorImprTexture.SetPixels32(ColorMass);	// Заполняем текстуру цветом из массива цветов ColorMass
						ColorImprTexture.Apply(false);				// Применяем текстуру
					}
					else  											// Иначе если тип улучшения Ускоритель, Замедлитель, Притягатель, Отталкиватель
					{

					}
					*/
				}
				else if(StatesPrizes[SelectedPrizeCell] == 'S')		// Иначе если мы работаем со скайбоксом
				{
					Debug.Log("Тип ячейки скайбокс");
				}
				ShowInformLabel = true;								// Ставим переменной (Показывать информационную метку) состояние правда
			}
		}
		else 													// Иначе если стрелка в даный момент не находиться над курсором
		{
			CursorDelay = 0;									// Сбрасываем накопившееся время задержки на ноль		
			ShowInformLabel = false;							// Ставим переменной (Показывать информационную метку) состояние ложь
		}
	}


	void FillPrizesStates()										// Этот метод заполняет массив состояния ячеек и массив номеров объектов ячеек в массивах которых лежат эти объекты
	{
		// Для начала вычесляем сколько циклов нам надо пройти - сколько нам нужно заполнить ячеек для этого мы обращаемся к скрипту GameManager и опрашиваем переменные сначала GM.ObjPrize , GM.TableNomForMat GM.TableNomForImpr.Length,
		// и в конце skyprize Мы сочитаем в месте длинну всех массивов и получаем сколько циклов нам нужно пройти, далее нам нужно в каждом цикле опираясь на последовательность этих массивов присваивать буквы массиву StatesPrizes

		byte NumberPrizes = (byte)(GM.ObjPrize.Length + GM.TableNomForMat.Length + GM.TableNomForImpr.Length + GM.SkyPrize.Length);			// Вычисляем общее количество призов и заносим это число в переменную NumberPrizes


		for(byte a=0; a<NumberPrizes; a++)									// Продолжаем цикл пока не переберём все призы
		{
			if(a < GM.ObjPrize.Length)										// Если номер цикла меньше чем длинна массива objprize
			{
				StatesPrizes[a] = 'O';										// Значит обозначаем тип этой ячейки как "Объект"
				NombersPrizes[a] = GM.ObjPrize[a];							// Извлекаем из массива ObjPrize номер соответствующего элемента и помещаем его в NombersPrizes
			}
			else if(a < (GM.ObjPrize.Length + GM.TableNomForMat.Length))	// Иначе если номер цикла больше чем длинна массива objprize но меньше чем совокупная длинна массивов ObjPrize и TableNomForMat
			{
				StatesPrizes[a] = 'M';											// Значит обозначаем тип этой ячейки как "Материал для стола"
				NombersPrizes[a] = GM.TableNomForMat[(a - GM.ObjPrize.Length)];	// Извлекаем из массива TableNomForMat номер соответствующего элемента и помещаем его в NombersPrizes
				NombersMats[a] = GM.TableMatPrize[(a - GM.ObjPrize.Length)];	// Извлекаем из массива TableMatPrize номер соответствующего элемента материала и помещаем его в NombersPrizes
			}
			else if(a < (GM.ObjPrize.Length + GM.TableNomForMat.Length + GM.TableNomForImpr.Length))		// Иначе если номер цикла больше чем длинна массивов objprize и TableNomForMat но меньше чем длинна objprize TableNomForMat и TableNomForImpr
			{
				StatesPrizes[a] = 'I';										// Значит обозначаем тип этой ячейки как "Улучшение для стола"
				NombersPrizes[a] = GM.TableNomForImpr[(a - GM.ObjPrize.Length - GM.TableNomForMat.Length)];	// Извлекаем из массива TableNomForImpr номер соответствующего элемента и помещаем его в NombersPrizes
				NombersImprs[a] = GM.TableImprPrize[(a - GM.ObjPrize.Length - GM.TableNomForMat.Length)];	// Извлекаем из массива TableImpPrize номер соответствубщего элемента улучшения и помещаем его в NombersPrizes
			}
			else 															// Иначе если номер нажатой ячейки равен или больше длинны трёх вышеуказанных массивов
			{
				StatesPrizes[a] = 'S';										// Значит обозначаем тип этой ячейки как "Скайбокс"
				NombersPrizes[a] = GM.SkyPrize[(a - GM.ObjPrize.Length - GM.TableNomForMat.Length - GM.TableNomForImpr.Length)];	// Извлекаем из массив SkyPrize номер соответствующего элемента и помещаем его в NombersPrizes
			}
		}
	}


	void RenderInformLabel()									// Этот метод подобен методу RenderInformWindow однако приспособлен для нужд показывания призов
	{
		short LabelWidth = 244;									// Ширина группы информационного окошка
		short LabellHeight = 150;								// Высота группы информационного окошка

		Vector2 CursorPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);	// Настоящая позиция курсора
		CursorPos = new Vector2(((CursorPos.x - Screen.width/2) / RatioH), ((CursorPos.y) / RatioH));	// Учитываем то что курсор ищет квадрат изменённый матрицей (Отсчёт координат от центра по ширине и с верху по высоте)

		GUI.BeginGroup(new Rect(CursorPos.x, CursorPos.y, LabelWidth, LabellHeight));

		if(StatesPrizes[SelectedPrizeCell] == 'O')				// Если мы работаем с объектами
		{
			GUI.DrawTexture(new Rect(0, 0, LabelWidth, LabellHeight), SAI.InformWindow);		// Отрисовываем информационное окно	
			GUI.Label(new Rect(0, 2, 230, 16), InformObjScr.ObjectName, "InformTextHeader"); 	// Отрисвываем название объекта

			if(InformObjScr.tag == "Bat")														// И если это бита
			{
				GUI.Label(new Rect(5, 20, 150, 15), "Вес биты");									// Отрисовываем слова "Вес биты"	
				GUI.Label(new Rect(5, 40, 150, 15), "Сила биты");									// Отрисовываем слова "Сила биты"
				GUI.Label(new Rect(5, 60, 150, 15), "Удельная мощность");							// Отрисовываем слова "Удельная мощность"
				GUI.Label(new Rect(5, 80, 150, 15), "Диаметр биты");								// Отрисовываем слова "Диаметр биты"
				GUI.Label(new Rect(5, 100, 150, 15), "Высота биты");								// Отрисовываем слова "Высота биты"

				if(InformObjScr.Mass == BatObjScr.Mass)													// Если приз бита над которой завис курсор равна по массе выбранной для игры игроком
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "WhiteInformText");	// Отрисовываем вес биты без улучшения белыми цифрами
				else if(InformObjScr.Mass > BatObjScr.Mass)												// Иначе если приз бита над которой завис курсор больше по массе выбранной для игры игроком
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "GreenInformText");	// Отрисовываем вес биты без улучшения зелёными цифрами
				else if(InformObjScr.Mass < BatObjScr.Mass)												// Иначе если приз бита над которой завис курсор меньше по массе выбранной для игры игроком
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "RedInformText");		// Отрисовываем вес биты без улучшения красными цифрами

				if(InformObjScr.Force == BatObjScr.Force)												// Если приз бита над которой завис курсор равна по силе выбранной для игры игроком
					GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "WhiteInformText");	// Отрисовываем силу биты без улучшения белыми цифрами
				else if(InformObjScr.Force > BatObjScr.Force)											// Если приз бита над которой завис курсор больше по силе выбранной для игры игроком
					GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "GreenInformText");	// Отрисовываем силу биты без улучшения зелёными цифрами
				else if(InformObjScr.Force < BatObjScr.Force)											// Если бита над которым завис курсор меньше по силе выделенного в магазине
					GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "RedInformText");	// Отрисовываем силу биты без улучшения красными цифрами

				if(InformObjScr.Force/InformObjScr.Mass == BatObjScr.Force/BatObjScr.Mass)				// Если удельная мощность биты над которой завис курсор равна удельной мощности выбранной для игры игроком
					GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "WhiteInformText");	// Отрисовываем удельную мощность биты белыми цифрами
				else if(InformObjScr.Force/InformObjScr.Mass > BatObjScr.Force/BatObjScr.Mass)			// Если удельная мощность биты над которой завис курсор больше удельной мощности выбранной для игры игроком
					GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "GreenInformText");	// Отрисовываем удельную мощность биты зелёными цифрами
				else if(InformObjScr.Force/InformObjScr.Mass < BatObjScr.Force/BatObjScr.Mass)			// Если удельная мощность биты над которой завис курсор меньше удельной мощности выбранной для игры игроком
					GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "RedInformText");	// Отрисовываем удельную мощность биты красными цифрами

				GUI.Label(new Rect(155, 80, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100, "WhiteInformText");	// Отрисовываем диаметр предмета белыми цифрами
				GUI.Label(new Rect(155, 100, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100, "WhiteInformText");	// Отрисовываем высоту предмета белыми цифрами

				GUI.Label(new Rect(199, 20, 5, 15), "/");														// Отрисовываем 1 знак "косая черта"
				GUI.Label(new Rect(199, 40, 5, 15), "/");														// Отрисовываем 2 знак "косая черта"
				GUI.Label(new Rect(199, 60, 5, 15), "/");														// Отрисовываем 3 знак "косая черта"
				GUI.Label(new Rect(199, 80, 5, 15), "/");														// Отрисовываем 4 знак "косая черта"
				GUI.Label(new Rect(199, 100, 5, 15), "/");														// Отрисовываем 5 знак "косая черта"

				GUI.Label(new Rect(206, 20, 40, 15 ), "" + BatObjScr.Mass);										// Отрисовываем массу сравниваемого объекта белыми цифрами
				GUI.Label(new Rect(206, 40, 40, 15 ), "" + BatObjScr.Force);															// Отрисовываем силу сравниваемого объекта белыми цифрами
				GUI.Label(new Rect(206, 60, 40, 15 ), "" + (Mathf.Round(BatObjScr.Force/BatObjScr.Mass*100)/100));						// Отрисовываем удельную мощность сравниваемого объекта белым цветом
				GUI.Label(new Rect(206, 80, 40, 15 ), "" + Mathf.Round(BatObjScr.GetComponent<MeshRenderer>().bounds.size.x*100)/100);	// Отрисовываем диаметр биты сравниваемого объекта белыми цифрами
				GUI.Label(new Rect(206, 100, 40, 15 ), "" + Mathf.Round(BatObjScr.GetComponent<MeshRenderer>().bounds.size.y*100)/100);	// Отрисовываем высоту биты сравниваемого объекта белыми цифрами
			}
			else if(InformObjScr.tag == "Puck")							// Или если это шайба
			{
				GUI.Label(new Rect(5, 20, 150, 15), "Вес шайбы");		// Отрисовываем слова "Вес шайбы"	
				GUI.Label(new Rect(5, 40, 150, 15), "Диаметр шайбы");	// Отрисовываем слова "Диаметр шайбы"
				GUI.Label(new Rect(5, 60, 150, 15), "Высота шайбы");	// Отрисовываем слова "Высота шайбы"

				if(InformObjScr.Mass == PuckObjScr.Mass)				// Если предмет над которым завис курсор равен по массе выбранной для игры игроком
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "WhiteInformText");	// Отрисовываем вес шайбы без улучшения белыми цифрами
				else if(InformObjScr.Mass > PuckObjScr.Mass)			// Иначе если предмет над которым завис курсор больше по массе выбранной для игры игроком
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "GreenInformText");	// Отрисовываем вес шайбы без улучшения зелёными цифрами
				else if(InformObjScr.Mass < PuckObjScr.Mass)			// Иначе если предмет над которым завис курсор меньше по массе выделенного в магазине
					GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "RedInformText");		// Отрисовываем вес шайбы без улучшения красными цифрами

				GUI.Label(new Rect(155, 40, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100, "WhiteInformText");	// Отрисовываем диаметр предмета белыми цифрами
				GUI.Label(new Rect(155, 60, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100, "WhiteInformText");	// Отрисовываем высоту предмета белыми цифрами

				GUI.Label(new Rect(199, 20, 5, 15), "/");												// Отрисовываем 1 знак "косая черта"
				GUI.Label(new Rect(199, 40, 5, 15), "/");												// Отрисовываем 2 знак "косая черта"
				GUI.Label(new Rect(199, 60, 5, 15), "/");												// Отрисовываем 3 знак "косая черта"

				GUI.Label(new Rect(206, 20, 30, 15 ), "" + PuckObjScr.Mass);							// Отрисовываем массу сравниваемого объекта белыми цифрами
				GUI.Label(new Rect(206, 40, 30, 15 ), "" + Mathf.Round(PuckObjScr.GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.x*100)/100);// Отрисовываем диаметр биты сравниваемого объекта белыми цифрами
				GUI.Label(new Rect(206, 60, 30, 15 ), "" + Mathf.Round(PuckObjScr.GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.y*100)/100);// Отрисовываем высоту биты сравниваемого объекта белыми цифрами
			}
			else if(InformObjScr.tag == "Table")				// Или если это стол
			{
				Debug.Log("Мы открыли стол");
			}
		}
		else if(StatesPrizes[SelectedPrizeCell] == 'M')			// Если мы работаем с материалом стола
		{
			
		}
		else if(StatesPrizes[SelectedPrizeCell] == 'I')			// Иначе емли мы работаем с улучшением стола
		{
			
		}
		else if(StatesPrizes[SelectedPrizeCell] == 'S')			// Иначе если мы работаем со скайбоксом
		{
			
		}

		GUI.EndGroup();
	}


	void OnDisable()											// Вызываем этот метод перед уничтожением объекта
	{
		GM.StartGameEvent -= StartGame;							// Отписываем метод "Старт игры" от события StartGameEvent
		GM.StartSecondStep -= _StartSecondStep_;				// Отписываем метод _StartSecondStep_ от события StartSecondStep
		GM.GoalEvent -= Goal;									// Отписываем метод "Гол" от события GoalEvent
		GM.LastGoalEvent -= LastGoal;							// Отписываем метод LastGoal на отбытия LastGoalEvent
	}
}

