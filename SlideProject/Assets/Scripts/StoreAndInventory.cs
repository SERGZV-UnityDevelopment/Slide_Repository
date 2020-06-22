// Это скрипт для отрисовки Магазина и инвенатря. Который также содержит вспомогательные методы для магазина и инвентаря

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//UnityEngine.SceneManagement.SceneManager
public class StoreAndInventory : MonoBehaviour 
{
//---------------------------------------------------------------------------------------------------- Текстуры для магазина и инвентаря ------------------------------------------------------------------------------------------------------------------------

	public Texture2D WindowStore;							// Текстура для окна магазина
	public Texture2D OverviewWindowObject;					// Текстура окошка обзора 3D предмета в меню объекта
	public Texture2D ClosedEl;								// Стандартная текстура для закрытых объектов в магазине
	public Texture2D ClosedMatAndImpr;						// Стандартная текстура для закрытых материалов и улучшений в магазине
	public Texture2D NotExist;								// Стандартная текстура для несуществующих объектов в магазине
	public Texture2D ShadingObject;							// Текстура для затенения открытого объекта в магазине
	public Texture2D ShadingSquareButon;					// Текстура для затенения открытого материала в магазине
	public Texture2D ActiveShadeObject;						// Текстура помогающая выделить активный объект
	public Texture2D ActiveShadeSquareButton;				// Текстура помогающая выделить активный материал или улучшение
	public Texture2D ObjectWeight;							// Иконка веса объекта
	public Texture2D ObjectStrength;						// Иконка силы объекта
	public Texture2D InformWindow;							// Текстура для окошка подробной информации об объекте
	public Texture2D FrameForCredits;						// Рамка кредитов игрока активного профиля
	public Texture2D CreditsIcon;							// Иконка кредитов
//---------------------------------------------------------------------------------------------- Публичные переменные для магазина и инвентаря ------------------------------------------------------------------------------------------------------------------

	public StoreContent SC;									// Переменная для скрипта "Контент магазина"
	public MainMenu MM;										// Переменная для скрипта "Главное меню"
	public PauseMenu PM;									// Переменная для скрипта "Меню паузы"
	public GameManager GM;									// Переменная для скрипта "GameManager"
	public Keeper Kep;										// Переменная для скрипта "Хранитель" (Или глобальной базы данных)
	public GameObject Fotographer;							// Переменная фотограф сюда помещаеться префаб камеры с источником света
	public Material[] Skyboxes = new Material[5];			// Материалы скайбоксов для наложения на камеру для фотографии объекта свой скайбокс для каждого уровня
	public Texture2D[] ObjsBackgrounds = new Texture2D[5];	// Текстура скайбокса 120 X 120 используемая как задний фон для объектов магазина
	public Texture2D[] MiniBackgrounds = new Texture2D[5];	// Мини Текстура скайбокса 46 X 46 используемая как задний фон для материалов и улучшений магазина
	public Rect[] PosIconObj = new Rect[25];				// В этом массиве храняться позиции	прямоугольников для отрисовки иконок объектов магазина
	public Rect[] PosIconMask = new Rect[25];				// В этом массиве храняться позиции масок для покупок в магазине
	public Rect[] PosIconMats = new Rect[10];				// В этом массиве храняться позиции иконок покупки материалов
	public Rect[] PosIconImprs = new Rect[10];				// В этом массиве храняться позиции иконок покупки улучшений

//------------------------------------------------------------------------------------------------------------ Остальные переменные -----------------------------------------------------------------------------------------------------------------------------
	Rigidbody SelObj;										// Сдесь будет лежать Rigidbody клона просматриваемого объекта
	ObjectScript ObjScr;									// Сдесь будет лежать скрипт "ObjectScript" клона выбранного в магазине объекта
	ObjectScript InformObjScr;								// Сдесь будет лежать скрипт "ObjectScript" объекта над которым задержалься курсор
	ImprovementScript InformImprScr;						// Сдесь будет лежать скрипт "ImprovementScript" улучшения выбранного объекта в магазине
	string InformMatName;									// Сдесь будет лежать название материала над которым завис курсор
	string InformImprName;									// Сдесь будет лежать название улучшения над которым завис курсор
	string InformImprType;									// Сдесь будет лежать название типа улучшения над которым завис курсор
	float InformImprMass;									// Сдесь будет лежать масса улучшения над которым завис курсор
	Texture2D ColorImprTexture;								// Сдесь будет лежать текстура цвета света улучшения
	public GameObject CameraClone;							// Ссылка на клон камеры снимающей видео
	public GameObject VideoObjClone;						// Ссылка на клон снимаемого объекта
	GameObject VideoImprClone;								// Ссылка на клон улучшения снимаемого объекта
	short ActiveObjNomber = 0;								// Высчитанный номер объекта выбранного в магазине для массива ObjectsStore
	short ActiveSkyNomber = 0;								// Высчитанный номер скайбокса выбранного в магазине для массива SkyboxMats
	string[] CategoryToolbar = new string[] {"Биты","Шайбы","Столы","Система"}; // Массив строк для категорий товаров магазина
	string[] ShortedCategory = new string[] {"Биты","Шайбы"};					// Массив строк для укороченного выбоа категории
	string[] LvlToolbar	= new string[] {"1Lvl","2Lvl","3Lvl","4Lvl","5Lvl"};	// Массив строк для уровней товаров магазина
	Texture2D[] ObjsTexs = new Texture2D[25];				// В этом массиве храняться изображения объектов или скайбоксов просматриваемой категории и уровня
	List<Texture2D>Mats = new List<Texture2D>();			// В этом списке храняться изображения для кнопок материалов выбранного объекта
	List<Texture2D>Imprvs = new List<Texture2D>();			// В этом списке храняться изображения для кнопок улучшений выбранного объекта
    RenderTexture VideoTex;									// Эта переменная хранит в себе фотографию объекта за этот кадр для большого окна магазина
	char[] ObjsStates = new char[25];						// В этом массиве храняться состояния объектов или скайбоксов для отоброжаемых в данный момент 25 штук  (C - Закрыт),(O - Открыт),(B - Куплен)
	List<char> StateMats = new List<char>(0);				// В этом списке храняться состояния материалов для выделенной ячейки (C - Закрыт),(O - Открыт),(B - Куплен),(A - Активен)
	List<char> StateImprs = new List<char>(0);				// В этом списке храняться состояния улучшений для выделенной ячейки (C - Закрыт),(O - Открыт),(B - Куплен), (A - Активен)
	int[] PriceObjs = new int[25];							// В этом массиве хряняться цены объектов для просматриваемой категории и уровня
	int[] ExpObjs = new int[25];							// В этом массиве храниться опыт объектов для просматриваемой категории и уровня
	List<int> PriceMats = new List<int>(0);					// В этом списке храняться цены материалов для выделенного объекта в магазине
	List<int> PriceImpr = new List<int>(0);					// В этом списке храняться цены улучшений для выделенного объекта в магазине
	List<int> ExpMats = new List<int>(0);					// В этом списке храняться значения опыта необходимое для открытия каждого из материалов выбранного объекта в магазине
	List<int> ExpImprs = new List<int>(0);					// В этом списке храняться значения опыта необходимое для открытия каждого из улучшений выбранного объекта в магазине
	short ActiveCatObj;										// В этой переменной храниться номер активного объекта или скайбокса для отрисовываемой категории и уровня
	byte ActiveMat = 0;										// В этой переменной храниться номер активного материала для отрисовываемого объекта
	byte ActiveImpr = 10;									// В этой переменной храниться номер активного улучшения для отрисовываемого объекта
	public int ActiveCategory = 0;							// Активная категория в магазине
	int ActiveLvl = 0;										// Активный уровень в магазине
	public int SelectedCell = 0;							// Выбранная ячейка в магазине
	byte SelectedMat = 0;									// Выделенная ячейка материала в магазине
	byte SelectedImpr = 10;									// Выделенная ячейка улучшения активного предмета в магазине
	sbyte LastCat = -1;										// Категория в магазине которая просматривалась последний раз
	sbyte LastLevel = -1;									// Уровень в магазине который просматривалься прошлый раз
	sbyte LastCell = 0;										// Последняя выбранная ячека в магазине
	public sbyte LastProf = 0;								// Последний профиль выбранный в последний раз
	bool Button3DObjDown = false;							// Эта переменная говорит зажата ли кнопка 3D Объекта в магазине или нет
	bool FirstCall = true;									// Эта переменная говорит был лы это первый вызов метода RotationObject или нет метода после нажатия на кнопку обзора объекта в магазине
	float X=0, Z=0;											// Объявляем переменные XYZ Чтобы вращать объект в окне обзора 3D предмета
	float OriginalHeight = 1080; 							// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	float RatioH;											// Сюда заноситься результат деления оригинальной высоты экрана на текущую
	bool ShowInformLabel = false;							// Эта переменная говорит можно ли отрисовывать информационную метку над ячейкой
	Rect CheckedRect = new Rect(0,0,0,0);					// Прямоугольник в магазине на котором курсор был секунду назад
	Vector2 RecalculatedMousePos;							// Сдесь будет храниться пересчитанная позиция мыши для правильного определения ячеек в магазине
	float CursorDelay;										// Здесь будет храниться время задержки курсора над одной ячейкой
	char InformCellType;									// Тип клетки над которой завис курсор. Объект материал или улучшение (Объект 'O') (Материал 'M') (Улучшение 'i')
	char InformCellNom;										// Номер клетки над которой завис курсор
	byte Nom = 0;											// Номер обнаруженной ячейки в магазине


	void Start()
	{
		RatioH = Screen.height / OriginalHeight;			// Заносим в ScreenBalansHight результат деления описанный выше
		VideoTex = new RenderTexture(322, 322, 0, RenderTextureFormat.ARGB32);
		VideoTex.antiAliasing = 8;							// Ставим уровень сглаживания рендер текстуры на 8
	}


	void Update()
	{	
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 && MM.Window == MainMenuWins.Store)			// Если мы отрисовываем окно магазина		
		{
			RotationObject();																										// Вызываем метод который высчитывает вращение объекта мышкой в магазине
			RecalculatedMousePos = RecalculateMousePos();																			// Вызываем метод пересчёта кординат мыши для правильного определения ячеек курсором.
			CursorDelayTimer();																										// Вызываем метод узнающий находиться ли курсор над клеткой магазина и если да сколько времени
		}
		else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// Иначе если мы отрисовываем окно инвентаря
		{
			RotationObject();																										// Вызываем метод который высчитывает вращение объекта мышкой в магазине
			RecalculatedMousePos = RecalculateMousePos();																			// Вызываем метод пересчёта кординат мыши для правильного определения ячеек курсором.
			InventoryCursorDelayTimer();																							// Вызываем метод узнающий находиться ли курсор над клеткой инвентаря и если да сколько времени
		}													
	}


	public void PreparationForTheOpeningStore()																		    // Этот метод подготавливает всё перед открытием магазина
	{
		byte ObjEl = (byte)((ActiveCategory*125)+(ActiveLvl*25)+SelectedCell);										    // Определяем номер выбранного объекта в массиве ObjStore
		CameraClone = (GameObject)Instantiate(Fotographer, new Vector3(5, 100, -1.4f), Quaternion.identity); 		    // Помещаем на сцену клон префаба фотографа а ссылку на этот клон отправляем в CamClone
		CameraClone.GetComponent<Camera>().targetTexture = VideoTex;												    // Устанавливаем для камеры рендер текстуру
		CameraClone.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;									    // Устанавливаем для камеры настройку чтобы она видела скайбокс
	}


	public void Store()											// Этот метод вызываеться из GUI другого скрипта для отрисовки магазина
	{
		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
		GUI.BeginGroup(new Rect(-501, -401.5f, 1002, 803), WindowStore);												// Начинаем группу магазина
		GUI.DrawTexture(new Rect(770, 10, 170, 40), FrameForCredits);													// Отрисовываем рамку для кредитов
		GUI.DrawTexture(new Rect(772, 12, 36, 36), CreditsIcon);														// Отрисовываем иконку кредитов
		GUI.Label(new Rect(812, 16, 130, 30),  Kep.Credits[Kep.ActiveProfile].ToString("#,0"), "CreditsScore");			// Отрисовываем количество кредитов
		GUI.Label(new Rect(300, 62, 200, 25), "<Size=25><Color=black>Магазин</Color></Size>");							// Текст магазин
		GUI.Label(new Rect(750, 62, 300, 25), "<Color=black>Обзор предмета</Color>");									// Текст обзор предмета			
		GUI.Label(new Rect(788, 459, 200, 20), "<Color=black>Расцветки</Color>");										// Текст скины
		GUI.Label(new Rect(785, 620, 200, 20), "<Color=black>Улучшения</Color>");										// Текст улучшения
		ActiveCategory = GUI.Toolbar(new Rect(5, 89, 640, 35), ActiveCategory, CategoryToolbar, "ButtonCategory");		// Выбор раздела магазина
		ActiveLvl = GUI.Toolbar(new Rect(5, 123, 640, 35), ActiveLvl, LvlToolbar, "ButtonLevel"); 				  		// Выбор уровня предмета
		SelectedCell = RenderObjCells(PosIconObj, (byte)SelectedCell);													// Отрисовываем ячейки

		if(LastCat != ActiveCategory || LastLevel != ActiveLvl || LastProf != Kep.ActiveProfile)	// Если изменилась категория или выбранный уровень или активный профиль
		{
			LastProf = (sbyte)Kep.ActiveProfile;				// Присваиваем LastProf выбранный профиль
			LastCat = (sbyte)ActiveCategory;					// Присваиваем LastCat выбранную категорию
			LastLevel = (sbyte)ActiveLvl;						// Присваиваем LastLevel выбранный уровень
			FillActiveCategory();								// Заполняем все массивы относящиеся к активной категории
			FillActiveCellInf();								// Заполняем все массивы подробно описывающие выбранный объект
			LastCell = (sbyte)SelectedCell;						// Присваиваем LastCell выбранную ячейку
		}

		if(LastCell != SelectedCell)							// Если изменилась ячейка
		{
			LastCell = (sbyte)SelectedCell;						// Приравниваем LastCell выбранную ячейку
			FillActiveCellInf();								// Заполняем все массивы подробно описывающие выбранный объект
		}


		if(GUI.RepeatButton(new Rect(660, 87, 330, 330), OverviewWindowObject) && VideoObjClone != null && ObjsStates[LastCell] == 'B')	// Если мы зажали кнопку обзора 3D предмета объект существует и имеет состояние куплен
			Button3DObjDown = true;																							// То ставим состояние переменной Button3DObjDown правда
		if(Input.GetMouseButtonUp(0)) 																						// Иначе если мы отпустили левую кнопку мыши
			Button3DObjDown = false;																						// То ставим состояние переменной Button3DObjDown ложь

		if(ObjsStates[LastCell] == 'C')																						// Если просматриваемый объект закрыт
		{
			GUI.DrawTexture(new Rect(664,91, 322, 322), ClosedEl);															// Отрисовываем вместо окна текстуру замка
			if(ActiveCategory == 3)																							// Если выбранная категория, категория скайбоксов
			{
				GUI.Label(new Rect(727, 441, 200, 200), "Системы не имеют расцветок", "StoreInformText");					// Отрисовываем уведомление об этом
				GUI.Label(new Rect(727, 615, 200, 200), "Системы не имеют улучшений", "StoreInformText");					// Отрисовываем уведомление что системы не имеют улучшений
			}
			else 																											// Иначе если выбранная категория не категория скайбоксов
			{
				GUI.Label(new Rect(727, 441, 200, 200), "Для закрытых объектов расцветки не видны", "StoreInformText");		// Отрисовываем уведомление об этом						
				GUI.Label(new Rect(727, 615, 200, 200), "Для закрытых объектов улучшения не видны", "StoreInformText");		// Отрисовываем уведомление об этом
			}
		}
		else if(ObjsStates[LastCell] == 'O')																				// Иначе если просматриваемый объект открыт
		{
			GUI.DrawTexture(new Rect(664, 91, 322, 322), VideoTex);															// Отрисовываем Видео поток снимаемого объекта
			GUI.DrawTexture(new Rect(664, 91, 322, 322), ShadingObject);													// Отрисовываем Маску поверх окна обзора предмета
			if(ActiveCategory == 0 || ActiveCategory == 1)																	// Если мы отрисовываем категорию бит или шайб
			{
				GUI.DrawTexture(new Rect(660, 426, 32, 32), ObjectWeight);													// Отрисовываем иконку массы прдемета									
				GUI.Label(new Rect(692, 427, 32, 32), "" + SelObj.mass, "StoreInformText");									// Отрисовываем массу предмета
			}																				
			if(ActiveCategory == 0)																							// Если активная категория биты
			{
				GUI.DrawTexture(new Rect(748, 426, 32, 32), ObjectStrength);												// Отрисовываем иконку мощности предмета и его мощность
				GUI.Label(new Rect(783, 427, 32, 32), "" + ObjScr.Force, "StoreInformText");								// Отрисовываем силу биты
			}
			if(ActiveCategory == 3)																							// Если выбранная категория, категория скайбоксов
			{
				GUI.Label(new Rect(727, 441, 200, 200), "Системы не имеют расцветок", "StoreInformText");					// Отрисовываем уведомление что системы не исеют раскрасок
				GUI.Label(new Rect(727, 615, 200, 200), "Системы не имеют улучшений", "StoreInformText");					// Отрисовываем уведомление что системы не имеют улучшений
			}
			else 																											// Иначе если выбранная категория не категория скайбоксов
			{
				RenderGroup(PosIconMats, SelectedMat, Mats, StateMats, PriceMats, ExpMats, true);	  						// Отрисовываем материалы в магазине
				if(Imprvs.Count <= 0) 																						// Если список текстур улучшений пуст							
					GUI.Label(new Rect(727, 615, 200, 200), "Улучшения для этого объекта отсутствуют", "StoreInformText");	// Отрисовываем уведомление об этом
				else 																										// Иначе если у нас есть улучшения на этом объекте
					RenderGroup(PosIconImprs, SelectedImpr, Imprvs, StateImprs, PriceImpr, ExpImprs, false);				// Отрисовываем улучшения в магазине
			}
		}
		else if(ObjsStates[LastCell] == 'B')																				// Иначе если просматриваемый объект Куплен
		{
			GUI.DrawTexture(new Rect(664, 91, 322, 322), VideoTex);															// Отрисовываем Видео поток снимаемого объекта
			if(ActiveCategory == 0 || ActiveCategory == 1)																	// Если мы отрисовываем категорию бит или шайб
			{
				GUI.DrawTexture(new Rect(660, 426, 32, 32), ObjectWeight );													// Отрисовываем иконку массы прдемета									
				GUI.Label(new Rect(692, 427, 32, 32), "" + SelObj.mass, "StoreInformText");									// Отрисовываем массу предмета
			}																				
			if(ActiveCategory == 0)																							// Если активная категория биты
			{
				GUI.DrawTexture(new Rect(748, 426, 32, 32), ObjectStrength);												// Отрисовываем иконку мощности предмета
				GUI.Label(new Rect(780, 427, 32, 32), "" + ObjScr.Force, "StoreInformText");								// Отрисовываем силу биты
			}
			if(ActiveCategory == 3)																							// Если выбранная категория, категория скайбоксов
			{
				GUI.Label(new Rect(727, 441, 200, 200), "Системы не имеют расцветок", "StoreInformText");					// Отрисовываем уведомление что системы не имеют раскрасок
				GUI.Label(new Rect(727, 615, 200, 200), "Системы не имеют улучшений", "StoreInformText");					// Отрисовываем уведомление что системы не имеют улучшений
			}
			else 																											// Иначе если выбранная категория не категория скайбоксов
			{
				GUI.Label(new Rect(900, 60, 130, 30), ExpObjs[SelectedCell].ToString(""));															// Отрисовываем опыт объекта
				RenderGroup(PosIconMats, SelectedMat, Mats, StateMats, PriceMats, ExpMats, true);	  												// Отрисовываем материалы выбранного объекта в магазине
				if(Imprvs.Count <= 0) 																												// Если список текстур улучшений пуст							
					GUI.Label(new Rect(727, 615, 200, 200), "Улучшения для этого объекта отсутствуют", "StoreInformText");							// Отрисовываем уведомление об этом
				else 																																// Иначе если у нас есть улучшения на этом объекте
					RenderGroup(PosIconImprs, SelectedImpr, Imprvs, StateImprs, PriceImpr, ExpImprs, false);										// Отрисовываем улучшения выбранного объекта в магазине
			}
		}
		else 																							// Иначе если для этой ячейки не сохранено состояния (Тоесть объект отсутствует)
		{
			GUI.DrawTexture(new Rect(664, 91, 322, 322), VideoTex);										// Отрисовываем Видео поток космоса
		}

		if(GUI.Button(new Rect(6, 9, 186, 42), "Купить предмет"))										// Если была нажата кнопка Купить предмет
		{
			if(ObjsStates[SelectedCell] == 'O')															// И если выбранный объект или скайбокс находиться в состоянии открыт
			{
				if(ActiveCategory != 3)																	// Если мы работаем с объектами а не скабоксами
				{
					if(Kep.Credits[Kep.ActiveProfile] >= ObjScr.Price)									// И если у активного профиля кредитов больше или равно чем требуеться для покупки этого объекта
					{
						ObjsStates[SelectedCell] = 'B';													// Делаем состояние объекта в переменной для состояний текущей категории открытым
						Kep.ObjectsStates[Kep.ActiveProfile][ActiveObjNomber] = 'B';					// Делаем состояние объекта в сохранении купленным
						Kep.Credits[Kep.ActiveProfile] -= ObjScr.Price;									// Отнимаем от денег игрока сумму назначенную за объект	
				//		StartCoroutine(Kep.FillActiveObjTex(1, Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
					}
				}
				else 																					// Иначе если мы работаем со скайбоксами
				{
					if(Kep.Credits[Kep.ActiveProfile] >= SC.SkyboxPrices[ActiveSkyNomber])				// И если у активного профиля кредитов больше или равно чем требуеться для покупки этого скайбокса
					{
						ObjsStates[SelectedCell] = 'B';													// Делаем состояние скайбокса в переменной для состояний текущей категории купленной
						Kep.SkyboxesStates[Kep.ActiveProfile][ActiveSkyNomber] = 'B';					// Делаем состояние скайбокса в сохранении купленным
						Kep.Credits[Kep.ActiveProfile] -= SC.SkyboxPrices[ActiveSkyNomber];				// Отнимаем от денег игрока сумму назначенную за скайбокс
				//		StartCoroutine(Kep.FillActiveObjTex(1, Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));			// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
					}
				}
			}
		}
		if(GUI.Button(new Rect(198, 9, 186, 42), "Купить расцветку") && ActiveCategory != 3)										// Если была нажата кнопка Купить расцветку и мы работаем с объектами а не скайбоксами
		{
			if(ObjsStates[SelectedCell] == 'B' && Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][SelectedMat] == 'O')		// И если выбранный объект находиться в состоянии куплен и выбранный материал находиться в состоянии открыт
			{
				if(Kep.Credits[Kep.ActiveProfile] >= ObjScr.PricesMats[SelectedMat])												// И если у активного профиля кредитов больше или равно чем требуеться для покупки этого материала
				{
					Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][SelectedMat] = 'B';										// Делаем состояние материала в сохранении купленным
					Kep.Credits[Kep.ActiveProfile] -= ObjScr.PricesMats[SelectedMat];												// Отнимаем от денег игрока сумму назначенную за покупаемый материал
					ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, SelectedMat);												// Накладываем на объект его новый выбранный материал	
					StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));					// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
				}
			}
		}
		if(GUI.Button(new Rect(390, 9, 186, 42), "Купить улучшение") && ActiveCategory != 3)										// Если была нажата кнопка Купить улучшение и мы работаем с объектами а не скайбоксами
		{
			if(ObjsStates[SelectedCell] == 'B' && Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][SelectedImpr] == 'O')	// И если выбранный объект находиться в состоянии куплен и выбранное улучшение находиться в состоянии открыто
			{
				if(Kep.Credits[Kep.ActiveProfile] >= ObjScr.PricesImprs[SelectedImpr])												// И если у активного профиля кредитов больше или равно чем требуеться для покупки этого улучшения
				{
					Debug.Log("Улучшение купленно");
					Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][SelectedImpr] = 'B';									// Делаем состояние улучшения в сохранении купленным
					Kep.Credits[Kep.ActiveProfile] -= ObjScr.PricesImprs[SelectedImpr]; 											// Отнимаем от денег игрока сумму назначенную за покупаемое улучшение
					ObjectChangeImprovement(SelectedImpr);																			// Вызываем метод смены улучшения																				
				}
			}
		}
		if(GUI.Button(new Rect(945,4,52,52), "<Size=30><</Size>", "SquareButton"))													// По нажатии на квадратную кнопку "Назад"
		{
			CameraClone.GetComponent<Camera>().targetTexture = null;																// Удаляем рендер текстуру с камеры для камеры рендер текстуру
			GameObject.DestroyImmediate(CameraClone);																				// Удаляем со сцены клон префаба фотографа
			GameObject.DestroyImmediate(VideoObjClone);																				// Удаляем со сцены объект который мы обозревали
			LastProf = 10;																											// Ставим LastProfile 10 чтобы при повторном входе в магазин заного срабатывал метод FillActiveCellInf
			GM.CallSaveGame();																										// Вызываем метод вызывающий событие сохранения игры для сохранения изменений		
			MM.Window = MainMenuWins.MainMenu;																						// Переходим в окно главного меню
		}	
		if(ShowInformLabel) 												// Если переменная "Показывать информационное окно правда"
		{
			RenderInformWindow();											// Отрисовываем информационное окно
		}
		GUI.EndGroup();														// Заканчиваем группу
	}


	public void Inventory()													// Этот метод вызываеться из GUI другого скрипта для отрисовки инвентаря
	{
		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
		GUI.BeginGroup(new Rect(-501, -401.5f, 1002, 803), WindowStore);												// Начинаем группу магазина
		GUI.DrawTexture(new Rect(770, 10, 170, 40), FrameForCredits);													// Отрисовываем рамку для кредитов
		GUI.DrawTexture(new Rect(772, 12, 36, 36), CreditsIcon);														// Отрисовываем иконку кредитов
		GUI.Label(new Rect(812, 16, 130, 30), Kep.Credits[Kep.ViewedProfile].ToString("#,0"), "CreditsScore");			// Отрисовываем количество кредитов для просматриваемого профиля
		GUI.Label(new Rect(300, 62, 200, 25), "<Size=25><Color=black>Инвентарь</Color></Size>");						// Текст магазин
		GUI.Label(new Rect(750, 62, 300, 25), "<Color=black>Обзор предмета</Color>");									// Текст обзор предмета			
		GUI.Label(new Rect(788, 459, 200, 20), "<Color=black>Расцветки</Color>");										// Текст скины
		GUI.Label(new Rect(785, 620, 200, 20), "<Color=black>Улучшения</Color>");										// Текст улучшения
		ActiveLvl = GUI.Toolbar(new Rect(5, 123, 640, 35), ActiveLvl, LvlToolbar, "ButtonLevel"); 				  		// Выбор уровня предмета
		SelectedCell = RenderBuyedCells(PosIconObj, (byte)SelectedCell);												// Отрисовываем ячейки

		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer") 							// Если это сцена для игры на двоих
			ActiveCategory = GUI.Toolbar(new Rect(5, 89, 640, 35), ActiveCategory, CategoryToolbar, "ButtonCategory");	// То отрисовываем обычный выбор категорий раздела инвентаря
		else 																											// Иначе если это не сцена для игры на двоих а прохождение
		{
			ActiveCategory = GUI.Toolbar(new Rect(5, 89, 640, 35), ActiveCategory, ShortedCategory, "ButtonCategory");	// Мы отрисовываем ограниченный выбор категорий
		}
			
		if(LastCat != ActiveCategory || LastLevel != ActiveLvl || LastProf != Kep.ViewedProfile)	// Если изменилась категория или выбранный уровень или просматриваемый профиль
		{
			LastProf = (sbyte)Kep.ViewedProfile;				// Присваиваем LastProf номер просмтриваемого профиля
			LastCat = (sbyte)ActiveCategory;					// Присваиваем LastCat выбранную категорию
			LastLevel = (sbyte)ActiveLvl;						// Присваиваем LastLevel выбранный уровень
			FillActiveBuyedCategory();							// Заполняем все массивы активной категории для инвентаря
			FillActiveBuyedCellInf();							// Заполняем все массивы подробно описывающие выбранный объект
			LastCell = (sbyte)SelectedCell;						// Присваиваем LastCell выбранную ячейку после выполнения метода так как после вызова метода она изменилась
		}
			
		if(LastCell != SelectedCell)							// Если изменилась ячейка
		{
			LastCell = (sbyte)SelectedCell;						// Приравниваем LastCell выбранную ячейку
			FillActiveBuyedCellInf();							// Заполняем все массивы подробно описывающие выбранный объект
		}
	
		if(GUI.RepeatButton(new Rect(660, 87, 330, 330), OverviewWindowObject) && VideoObjClone != null)						// Если мы зажали кнопку обзора 3D предмета и переменная VideoObjClone существует
			Button3DObjDown = true;																								// То ставим состояние переменной Button3DObjDown правда
		if(Input.GetMouseButtonUp(0)) 																							// Иначе если мы отпустили левую кнопку мыши
			Button3DObjDown = false;																							// То ставим состояние переменной Button3DObjDown ложь

		if(ObjsTexs[LastCell].name != "NotExistElement")																		// Если в массиве ObjsTexs текстура не называеться "NotExistElement"
		{
			GUI.DrawTexture(new Rect(664, 91, 322, 322), VideoTex);																// Отрисовываем Видео поток снимаемого объекта
			if(ActiveCategory == 0 || ActiveCategory == 1)																		// Если мы отрисовываем категорию бит или шайб
			{	
				GUI.DrawTexture(new Rect(660, 426, 32, 32), ObjectWeight );														// Отрисовываем иконку массы прдемета									
				GUI.Label(new Rect(692, 427, 32, 32), "" + SelObj.mass, "StoreInformText");										// Отрисовываем массу предмета
			}																				
			if(ActiveCategory == 0)																								// Если активная категория биты
			{
				GUI.DrawTexture(new Rect(748, 426, 32, 32), ObjectStrength);													// Отрисовываем иконку мощности предмета
				GUI.Label(new Rect(780, 427, 32, 32), "" + ObjScr.Force, "StoreInformText");									// Отрисовываем силу биты
			}
			if(ActiveCategory == 3)																								// Если выбранная категория, категория скайбоксов
			{
				GUI.Label(new Rect(727, 441, 200, 200), "Системы не имеют расцветок", "StoreInformText");						// Отрисовываем уведомление что системы не имеют раскрасок
				GUI.Label(new Rect(727, 615, 200, 200), "Системы не имеют улучшений", "StoreInformText");						// Отрисовываем уведомление что системы не имеют улучшений
			}
			else 																												// Иначе если выбранная категория не категория скайбоксов
			{
				GUI.Label(new Rect(900, 60, 130, 30), ExpObjs[SelectedCell].ToString(""));										// Отрисовываем опыт объекта
				RenderBuyedGroup(PosIconMats, SelectedMat, Mats, StateMats, PriceMats, ExpMats, true);	  							// Отрисовываем материалы выбранного объекта в инвентаре
				if(Imprvs.Count <= 0) 																								// Если список текстур улучшений пуст							
					GUI.Label(new Rect(727, 615, 200, 200), "Купленные улучшения для этого объекта отсутствуют", "StoreInformText");// Отрисовываем уведомление об этом
				else 																												// Иначе если у нас есть улучшения на этом объекте
					RenderBuyedGroup(PosIconImprs, SelectedImpr, Imprvs, StateImprs, PriceImpr, ExpImprs, false);					// Отрисовываем улучшения выбранного объекта в инвентаре
			}
		}
		else 																													// Иначе если для этой ячейки имя текстуры "NotExistElement"
		{
			GUI.DrawTexture(new Rect(664, 91, 322, 322), VideoTex);																// Отрисовываем Видео поток космоса
			GUI.Label(new Rect(727, 441, 200, 200), "Объект не существует", "StoreInformText");									// Отрисовываем уведомление что объект не существует
			GUI.Label(new Rect(727, 615, 200, 200), "Объект не существует", "StoreInformText");									// Отрисовываем уведомление что объект не существует
		}
			
		if(GUI.Button(new Rect(945,4,52,52), "<Size=30><</Size>", "SquareButton"))										// По нажатии на квадратную кнопку "Назад"
		{
			CameraClone.GetComponent<Camera>().targetTexture = null;													// Удаляем рендер текстуру с камеры
			GameObject.DestroyImmediate(CameraClone);																	// Удаляем со сцены клон префаба фотографа
			GameObject.DestroyImmediate(VideoObjClone);																	// Удаляем со сцены объект который мы обозревали
			LastProf = 10;																								// Ставим LastProfile 10 чтобы при повторном входе в магазин заного срабатывал метод FillActiveCellInf
			GM.CalculatePosObjects();																					// То на случай если была изменена бита или шайба пересчитываем стартовые позиции биты и шайбы
			PM.Window = GameMenuWins.Start;																				// Переходим в окно старта игры
		}	
		if(ShowInformLabel) 												// Если переменная "Показывать информационное окно правда"
		{
			RenderInformWindow();											// Отрисовываем информационное окно
		}
		GUI.EndGroup();
	}


	void FillActiveCategory()								// Этот метод заполняет все массивы относящиеся к категории предметов для активного профиля
	{
		int ObjEl = (ActiveCategory*125)+(ActiveLvl*25);	// Определяем первый элемент ячейки выбранной категории для массива ObjectsStore
		int SkyEl = (ActiveLvl*25);							// Определяем первый элемент ячейки выбранной категории для массива SkyboxScreens и SkyboxMats
		ObjsStates = new char[25];							// Очищаем массив (состояний объектов в просматриваемой категории)
		ObjsTexs = new Texture2D[25];						// Очищаем массив (иконок объектов в просматриваемой категории)
		if(ActiveCategory < 3)								// Если мы работаем с категорией объектов в магазине
		{	
			if((Kep.ActiveObjects[Kep.ActiveProfile,ActiveCategory] - ((ActiveCategory * 125)+(ActiveLvl * 25))) >= 0)						// Если в этой категории в этом уровне есть активный объект
			{
				ActiveCatObj = (short)(Kep.ActiveObjects[Kep.ActiveProfile,ActiveCategory] - ((ActiveCategory * 125) + (ActiveLvl * 25))); 	// Переносим значение активного объекта для этой категории в переменную удобную для опроса
				SelectedCell = ActiveCatObj;																								// Делаем активную клетку выбранной клеткой
			}
			else 																															// Иначе если значение минусовое
				ActiveCatObj = -1;																											// Присваиваем ActiveCatObj значение -1 говорящее что в этих 25 отрисовываемых объектах нет активного

			for(byte a=0; a < ObjsTexs.Length; a++)		// Продолжаем цикл пока не заполним массивы относящиеся к активной категории
			{
				if(SC.ObjectsStore[ObjEl] != null)		// Если выбранный объект в массиве существует
				{	
					if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] != 0) 											// Если сохранение для этого объекта существует
					{
						if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] == 'O')										// Если этот объект открыт
						{
							ObjsTexs[a] = SC.ObjectsStore[ObjEl].GetComponent<ObjectScript>().FotoStore;			// То мы присваемваем его текстуру для магазина массиву текстур с номером цикла
							PriceObjs[a] = SC.ObjectsStore[ObjEl].GetComponent<ObjectScript>().Price;				// Присваиваем цену элементу массива с номером a цену объекта из массива ObjStore с номером ObjEl
							ObjsStates[a] = 'O';																	// Ставим в массиве состояний ObjState что объект открыт
						}
						else if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] == 'B')									// Если этот объект куплен
						{
							ObjsTexs[a] = SC.ObjectsStore[ObjEl].GetComponent<ObjectScript>().FotoStore;			// То мы присваемваем его текстуру для магазина массиву текстур с номером цикла
							ExpObjs[a] = Kep.ObjectsExpirience[Kep.ActiveProfile][ObjEl];							// Присваиваем опыт элементу массив ExpObjs
							ObjsStates[a] = 'B';																	// Ставим в массиве состояний ObjState что объект куплен
						}
					}
					else																							// Если сохранение для этого объекта не существует
					{
						ObjsTexs[a] = ClosedEl;																		// Тогда мы присваеваем спецальную текстуру для закрытых объектов(Замок)
						ObjsStates[a] = 'C';																		// И ставим что состояние объекта для этого цикла "закрыт"
					}

				}
				else									// Иначе если этого объекта несуществует
				{
					ObjsTexs[a] = NotExist;				// Тогда мы присваеваем спецальную текстуру для несуществующих объектов(Крест)		
				}
				ObjEl++;								// Прибавляем к переменной (ObjectElement) 1 чтобы в следующем цикле опросить следующий элемент массива ObjectStore
			}
		}
		else 											// Иначе если мы работаем с категорией скайбоксов в магазине
		{
			if((Kep.ActiveObjects[Kep.ActiveProfile,ActiveCategory] - ((ActiveLvl * 125))) >= 0)						// Если в категории скабоксов этом в этом уровне есть активный скайбокс
			{
				ActiveCatObj = (short)(Kep.ActiveObjects[Kep.ActiveProfile,ActiveCategory] - ((ActiveLvl * 125)));		// Переносим значение активного скайбокса в переменную удобную для опроса
				SelectedCell = ActiveCatObj;																			// Делаем активную клетку выбранной клеткой
			}
			else 																										// Иначе если значение минусовое
			{
				ActiveCatObj = -1;																						// Присваиваем ActiveCatObj значение -1 говорящее что в этих 25 отрисовываемых скайбоксах нет активного
			}

			for(byte a=0; a < ObjsTexs.Length; a++)											// Продолжаем цикл пока не заполним массивы относящиеся к активной категории
			{
				if(SC.SkyboxMats[SkyEl] != null)											// Если выбранный скайбокс в массиве существует
				{
					if(Kep.SkyboxesStates[Kep.ActiveProfile][SkyEl] != 0) 					// Если сохранение для этого скайбокса существует
					{
						if(Kep.SkyboxesStates[Kep.ActiveProfile][SkyEl] == 'O')				// Если этот скайбокс открыт
						{
							ObjsTexs[a] = SC.SkyboxScreens[SkyEl];							// То мы присваемваем скрин скайбокса для магазина массиву текстур с номером цикла
							PriceObjs[a] = SC.SkyboxPrices[SkyEl];							// Присваиваем цену скайбокса
							ObjsStates[a] = 'O';											// Ставим состояние скайбокса как открыт
						}
						else if(Kep.SkyboxesStates[Kep.ActiveProfile][SkyEl] == 'B')		// Если этот скайбокс куплен
						{
							ObjsTexs[a] = SC.SkyboxScreens[SkyEl];							// То мы присваемваем скрин скайбокса для магазина массиву текстур с номером цикла
							ObjsStates[a] = 'B';											// Ставим состояние скайбокса временной переменной для актиной категории как куплен
						}
					}
					else																	// Если сохранение для этого скайбокса не существует
					{
						ObjsTexs[a] = ClosedEl;												// Тогда мы присваеваем спецальную текстуру для закрытых объектов(Замок)
						ObjsStates[a] = 'C';													// По умолчанию в каждом цикле мы считаем что состояние скайбокса закрыт
					}
				}
				else 																		// Иначе если выбранный скайбокс отсутствует в массиве SkyboxScreens
				{
					ObjsTexs[a] = NotExist;													// Тогда мы присваеваем спецальную текстуру для несуществующих объектов(Крест)	
				}
				SkyEl++;																	// Прибавляем к переменной (SkyboxElement) 1 чтобы в следующем цикле опросить следующий элемент массива SkyboxScreens
			}
		}
	}


	void FillActiveBuyedCategory()											// Этот метод похож на метод FillActiveCategory с одним отличием он не для магазина а для инвентаря а там отображаються только купленные предметы
	{	// Выясняем есть ли в этой категории в этом уровне активный объект
		int ObjEl = (ActiveCategory*125)+(ActiveLvl*25);					// Определяем первый элемент ячейки выбранной категории для массива ObjectsStore
		int SkyEl = (ActiveLvl*25);											// Определяем первый элемент ячейки выбранной категории для массива SkyboxScreens и SkyboxMats
		byte FilledCell = 0;												// Номер заполняемой ячейки в инвентаре
		byte NomActiveCell = 0;												// Здесь будет храниться номер высчитанной активной ячейки для инвентаря
		ObjsStates = new char[25];											// Очищаем массив (состояний объектов в просматриваемой категории)
		ObjsTexs = new Texture2D[25];										// Очищаем массив (иконок объектов в для просматриваемой категории и уровня)


		if(ActiveCategory < 3)												// Если мы работаем с категорией объектов в инвентаре
		{	// Определяем для первого игрока номер активного объекта, активной категории, активного уровня
			if((GM.Currentkep.ActiveObjects[Kep.ViewedProfile,ActiveCategory] - ((ActiveCategory * 125)+(ActiveLvl * 25))) >= 0)		// Если в этой категории в этом уровне для просматриваемого профиля есть активный объект
			{						
				for(int a=ObjEl; a<GM.Currentkep.ActiveObjects[Kep.ViewedProfile,ActiveCategory]; a++)	// Проходим по части массива ObjectsStore чтобы вычислить номер ячейки отрисовывающей активный объект
				{
					if(GM.Currentkep.ObjectsStates[Kep.ViewedProfile][a] == 'B')						// Если мы наткнулись на купленный объект
					{
						NomActiveCell ++;														// Прибавляем к купленной ячейке +1 чтобы засчитать ещё один найденный купленный объект
					}
				}
				ActiveCatObj = NomActiveCell;													// Когда мы высчитали номер активной ячейки в инвентаре присваиваем его  временной переменной хранящей его ActiveCatObj
				SelectedCell = ActiveCatObj;													// Делаем активную клетку выбранной клеткой
			}
			else 																				// Иначе если значение минусовое
				ActiveCatObj = -1;																// Присваиваем ActiveCatObj значение -1 говорящее что в этих 25 отрисовываемых объектах нет активного
			

			for(byte a=0; a < ObjsTexs.Length; a++)																		// Продолжаем цикл пока не пройдём 25 объектов для выбранного профиля категории и уровня
			{
				if(SC.ObjectsStore[ObjEl] != null)																		// Если выбранный объект в массиве существует
				{	
					if(Kep.ObjectsStates[Kep.ViewedProfile][ObjEl] != 0)												// Если сохранение для этого объекта существует
					{
						if(Kep.ObjectsStates[Kep.ViewedProfile][ObjEl] == 'B')											// Если этот объект куплен
						{
							ObjsTexs[FilledCell] = SC.ObjectsStore[ObjEl].GetComponent<ObjectScript>().FotoStore;		// То мы присваемваем его текстуру для магазина массиву текстур с номером цикла
							ExpObjs[FilledCell] = Kep.ObjectsExpirience[Kep.ViewedProfile][ObjEl];						// Присваиваем опыт элементу массив ExpObjs
							ObjsStates[a] = 'B';																		// Ставим в массиве состояний ObjState что объект куплен
							FilledCell++;																				// Увеличиваем FilledCell  на один чтобы при следующем обнаружении купленного объекта заполнить следующую ячейку купленным объектом
						}
						else 																							// Иначе если этот объект не куплен
							ObjsTexs[a] = NotExist;																		// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)	
					}
					else 																								// Иначе если для этого объекта нет сохранения
						ObjsTexs[a] = NotExist;																			// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)	
				}
				else									// Иначе если этого объекта несуществует
					ObjsTexs[a] = NotExist;				// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)		
				
				ObjEl++;								// Прибавляем к переменной (ObjEl) 1 чтобы в следующем цикле опросить следующий элемент массива ObjectStore
			}
		}
		else 											// Иначе если мы работаем с категорией скайбоксов в магазине
		{	// Вычисляем активную ячейку для этой категории скайбоксов и ложим во временную переменную ActiveCatObj
			if((Kep.ActiveObjects[Kep.ViewedProfile,ActiveCategory] - ((ActiveLvl * 125))) >= 0)		// Если в этой категории в этом уровне есть активный скайбокс
			{
				// Проходим по части массива SkyboxMats чтобы вычислить номер ячейки отрисовывающей активный объект
				for(int a=SkyEl; a<Kep.ActiveObjects[Kep.ViewedProfile,ActiveCategory]; a++)			// Продолжаем цикл до тех пор пока не переберём все купленные скайбоксы для этой категории		
				{
					if(Kep.SkyboxesStates[Kep.ViewedProfile][a] == 'B')									// Если мы наткнулись на купленный скайбокс
					{
						NomActiveCell ++;																// Прибавляем к купленной ячейке +1 чтобы засчитать ещё один найденный купленный объект
					}
				}
				ActiveCatObj = NomActiveCell;															// Когда мы высчитали номер активной ячейки в инвентаре присваиваем его временной переменной хранящей его ActiveCatObj
				SelectedCell = ActiveCatObj;															// Делаем активную клетку выбранной клеткой
			}
			else 																						// Иначе если значение минусовое
				ActiveCatObj = -1;																		// Присваиваем ActiveCatObj значение -1 говорящее что в этих 25 отрисовываемых скайбоксах нет активного
			
			for(byte a=0; a < ObjsTexs.Length; a++)											// Продолжаем цикл пока не заполним массивы относящиеся к активной категории
			{
				if(SC.SkyboxMats[SkyEl] != null)											// Если выбранный скайбокс в массиве существует
				{
					if(Kep.SkyboxesStates[Kep.ViewedProfile][SkyEl] != 0) 					// Если сохранение для этого скайбокса существует
					{
						if(Kep.SkyboxesStates[Kep.ViewedProfile][SkyEl] == 'B')				// Если этот скайбокс куплен
						{
							ObjsTexs[FilledCell] = SC.SkyboxScreens[SkyEl];					// То мы присваемваем скрин скайбокса для магазина массиву текстур с номером цикла
							ObjsStates[FilledCell] = 'B';									// Ставим состояние скайбокса временной переменной для актиной категории как куплен
							ObjsStates[a] = 'B';											// Ставим в массиве состояний ObjsStates что скайбокс куплен
							FilledCell ++;													// Увеличиваем FilledCell на один чтобы при следующем обнаружении купленного скайбокса заполнить следующую ячейку купленным скайбоксом
						}
						else 																// Иначе если этот скайбокс не куплен
							ObjsTexs[a] = NotExist;											// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)	
					}
					else 								// Иначе если этот скайбокс не куплен
						ObjsTexs[a] = NotExist;			// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)	
				}
				else									// Иначе если этого скайбокса несуществует
				{
					ObjsTexs[a] = NotExist;				// Тогда мы присваиваем спецальную текстуру для несуществующих объектов(Крест)		
				}
				SkyEl++;								// Прибавляем к переменной (SkyEl) 1 чтобы в следующем цикле опросить следующий элемент массива SkyboxMats
			}
		}
	}


	void FillActiveCellInf()							//  Этот метод находит номер сохранения в списках и загружает всю необходимую информацию для отображения подробной информации об этом конкретном объекте и отображения самого объекта в магазине
	{
		short ObjEl = (short)((ActiveCategory*125)+(ActiveLvl*25)+SelectedCell);	// Определяем номер выбранного объекта в массиве ObjStore
		short SkyEl = (short)((ActiveLvl*25)+SelectedCell);							// Определяем номер ячейки выбранной категории для массива SkyboxScreens и SkyboxMats
		Mats.Clear();																// Очищаем список текстур материалов выбранной ячейки
		Imprvs.Clear();																// Очищаем список текстур улучшений выбранной ячейки
		StateMats.Clear();															// Очищаем список состояний материалов
		StateImprs.Clear();															// Очищаем список состояний улучшений
		PriceMats.Clear();															// Очищаем список цен материалов
		PriceImpr.Clear();															// Очищаем список цен улучшений
		ExpMats.Clear();															// Очищаем список необходимого опыта для материалов
		ExpImprs.Clear();															// Очищаем список необходмого опыта для улучшений

		if(VideoObjClone)															// Если VideoObjClone не пуст
		{
			if(VideoObjClone.GetComponent<MeshCollider>())							// Если мы работаем с категорией объектов
			{
				VideoObjClone.GetComponent<MeshCollider>().enabled = false;    		// Сначала отключаем его меш коллайдер чтобы он не столкнулься с объектом который прыгнет на его место
				GameObject.Destroy(VideoObjClone);									// Удаляем со сцены старый объект который мы обозревали(Я использую Destroy потому что с DestroyImmediate конкретно сдесь юнити вылетает)
			}
			else 																	// Иначе если мы работаем с категорией скайбоксов
			{
				GameObject.DestroyImmediate(VideoObjClone);							// Используем мнгновенное удаление чтобы удалить панель обзора скайбокса
			}
		}
		FirstCall = true;															// Ставим переменной FirstCall состояние правда

		if(ActiveCategory < 3)														// Если мы работаем с категорией объектов
		{
			if(SC.ObjectsStore[ObjEl] != null)										// Если такой объект есть в магазине 	
			{	
				if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] != 0)				// Если мы нашли сохранение для этого объекта
				{	// Значит для этого объекта есть сохранение и он может быть открыт или даже куплен, мы не проверяем закрыт объект или нет потому что если бы он был закрыт он бы не был сохранён
					VideoObjClone = (GameObject)Instantiate(SC.ObjectsStore[ObjEl], new Vector3(5, 100, 0), Quaternion.AngleAxis(90, new Vector3(-1, 0, 0)));	// Помещаем на сцену клон префаба объекта а сслыку на этот объект отправляем в ObjClone
					float Size = SC.ObjectsStore[ObjEl].GetComponent<ObjectScript>().ViewSize;	// Берём размер объекта для фотографии и копируем его в переменную Size
					Vector3 Size3 = new Vector3(Size, Size, Size);								// Создаём новый вектор3 с именем Size3 и заполняем все его оси из переменной Size
					VideoObjClone.transform.localScale = Size3;									// Придаём новому клону объекта размер указанный в Size3
					SelObj = VideoObjClone.GetComponent<Rigidbody>();							// Получаем Rigidbody для клона просматриваемого объекта
					ObjScr = VideoObjClone.GetComponent<ObjectScript>();						// Получаем скрипт ObjectScript для клона просматриваемого объекта
					ActiveMat = (byte)(Kep.ActiveMats[Kep.ActiveProfile][ObjEl]);				// Присваиваем переменной ActiveMat номер активного материала для этого объекта
					SelectedMat = ActiveMat;													// Присваиваем переменной SelectedMat ActiveMat	
					ActiveObjNomber = ObjEl;													// Присваиваем номер выбранного объекта в массиве ObjectsStore переменной ActiveObjNomber
					ActiveSkyNomber = SkyEl;													// Присваиваем номер выбранного скайбокса в массиве SkyboxMats переменной ActiveSkyNomber
					ActiveImpr = (byte)(Kep.ActiveImprs[Kep.ActiveProfile][ObjEl]);				// Присваиваем переменной ActiveImpr номер активного улучшения для этого объекта
					SelectedImpr = ActiveImpr;													// Делаем выделенным активное улучшение
					ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, ActiveMat);			// Накладываем на объект его выбранный материал
					if(ActiveImpr != 10) ObjectChangeImprovement(SelectedImpr);					// Если у этого объекта есть активное улучшение устанавливаем на объект активное улучшение											

					for(byte b=0; b < Kep.StatesMaterials[Kep.ActiveProfile][ObjEl].Count; b++)	// Продолжаем итерацию пока не переберём все материалы существующие у объекта активного профиля										
					{
						PriceMats.Add(ObjScr.PricesMats[b]);									// Присваиваем элементу списка PriceMats Цену необходимую для открытия этого материала
						if(ObjScr.tag != "Table") ExpMats.Add(ObjScr.RequireExpirienceMats[b]);	// Если это не стол присваиваем элементу списка ExpMats опыт необходимый для открытия этого материала
													
						if(Kep.StatesMaterials[Kep.ActiveProfile][ObjEl][b] == 'C')				// Если этот материал закрыт
						{
							Mats.Add(ClosedMatAndImpr);											// Присваиваем ему иконку для закрытых материалов и улучшений
							StateMats.Add('C');													// И ставим элементу списка char что материал закрыт
						}
						else if(Kep.StatesMaterials[Kep.ActiveProfile][ObjEl][b] == 'O')		// Если этот материал просто открыт
						{
							Mats.Add(ObjScr.FotoMaterials[b]);									// Мы загружаем фотографию материала в список материалов
							StateMats.Add('O');													// И ставим элементу списка char что материал открыт
						}
						else if(Kep.StatesMaterials[Kep.ActiveProfile][ObjEl][b] == 'B')		// Если этот материал куплен
						{
							Mats.Add(ObjScr.FotoMaterials[b]);									// Мы загружаем фотографию материала в список материалов
							StateMats.Add('B');													// И ставим элементу списка char что материал куплен
						}
					}
					if(Kep.StatesImprovements[Kep.ActiveProfile][ObjEl].Count > 0)							// Если в сохранении для объекта записанны улучшения
					{
						for(byte c=0; c < Kep.StatesImprovements[Kep.ActiveProfile][ObjEl].Count; c++)		// Продолжаем цикл пока не переберём все сохранения для этого объекта
						{
							PriceImpr.Add(ObjScr.PricesImprs[c]);											// Присваиваем элементу списка PriceImpr Цену необходимую для открытия этого улучшения
							if(ObjScr.tag != "Table") ExpImprs.Add(ObjScr.RequireExpirienceImprs[c]);		// Присваиваем элементу списка ExpImprs опыт необходимый для открытия этого улучшения
							if(Kep.StatesImprovements[Kep.ActiveProfile][ObjEl][c] == 'C')					// Если улучшение с номером этого цикла записанно в сохранении как закрыто
							{
								Imprvs.Add(ClosedMatAndImpr);												// То добавляем в список текстур улучшений текстуру для закрытых материалов и улучшений
								StateImprs.Add('С');														// И ставим списку состояний StateImps что улучшение закрыто
							}
							else if(Kep.StatesImprovements[Kep.ActiveProfile][ObjEl][c] == 'O')				// Если улучшение с номером этого цикла открыто но не куплено
							{
								Imprvs.Add(ObjScr.IconImprovements[c]);										// То добавляем в список текстур улучшений текстуру этого улучшения
								StateImprs.Add('O');														// И ставим списку состояний StateImps что улучшение открыто
							}
							else if(Kep.StatesImprovements[Kep.ActiveProfile][ObjEl][c] == 'B')				// Если улучшение с номером этого цикла куплено
							{
								Imprvs.Add(ObjScr.IconImprovements[c]);										// То добавляем в список текстур улучшений текстуру этого улучшения
								StateImprs.Add('B');														// И ставим списку состояний StateImps что улучшение куплено
							}
						}
					}
					ActiveObjNomber = ObjEl;												// Присваиваем ActiveObjectNomber значение переменной ObjEl
				}
				else 																		// Иначе если такого объекта нет в массиве
				{
					ActiveObjNomber = -1;													// Если такой объект не найден в ObjectsStore присваиваем ActiveOnjNomber -1
				}
			}
		}
		else if(ActiveCategory == 3) 														// Иначе если мы работаем с категорией скайбоксов в магазине
		{
			if(SC.SkyboxMats[SkyEl] != null)												// Если такой объект есть в массиве SkyboxMats
			{
				if(Kep.SkyboxesStates[Kep.ActiveProfile][SkyEl] != 0)						// Если мы нашли сохранение для этого скайбокса
				{// Значит для этого скайбокса есть сохранение и он может быть открыт или даже куплен, мы не проверяем закрыт скайбокс или нет потому что если бы он был закрыт он бы не был сохранён
					VideoObjClone = (GameObject)Instantiate(Resources.Load("Models/Prefabs/SkyboxReviewPanel"), new Vector3(5, 100, 0), Quaternion.AngleAxis(90, new Vector3(-1, 0, 0)));	// Помещаем на сцену панель обзора скайбокса а ссылку на неё в VideoObjClone
					Material[] TimeMass = VideoObjClone.GetComponent<Renderer>().materials;	// Присваиваем массив материалов новому массиву материалов (Так как по одиночке материал не меняеться)
					TimeMass[2] = SC.MatsForReviewPanel[SkyEl];								// Меняем уже в этом массиве материал на тот что надо
					VideoObjClone.GetComponent<Renderer>().materials = TimeMass;			// И присваиваем этот массив обратно панели обзора скайбоксов отображая тем самым текущий скайбокс на панели обзора скайбоксов
					ActiveSkyNomber = SkyEl;												// Присваиваем ActiveSkyNomber значение переменной SkyEl
				}
			}
			else 																// Иначе если такой скайбокс не найден в SkyboxMats
			{
				ActiveSkyNomber = -1;											// Присваиваем ActiveSkyNomber -1
			}
			Debug.Log("Теперь переменная ActiveSkyNomber равна " + ActiveSkyNomber);
		}
	}


	void FillActiveBuyedCellInf()												//  Этот метод находит номер сохранения в списках и загружает всю необходимую информацию для отображения подробной информации об этом конкретном объекте и отображения самого объекта в инвентаре
	{
		short FirstObjEl = (short)((ActiveCategory*125)+(ActiveLvl*25));		// Номер элемента массива ObjectsStore соответствующий первой ячейке инвентаря просматриваемой категории и уровня
		short ObjEl = FirstObjEl;												// Опрашиваемый элемент массива ObjectsStore
		short SelObjEl = 0;														// Сдесь будет храниться номер выбранного купленного объекта в массиве ObjectsStore для выбранной категории и уровня
		byte BuyedObject = 0;													// Номер найденного купленного объекта для этих 25 ячеек
		short FirstSkyEl = (short)(ActiveLvl*25);								// Номер элемента в массиве SkyboxMats соответствующий первой ячейке инвентаря категории скайбоксов просматриваемого уровня
		short SkyEl = FirstSkyEl;												// Опрашиваемый элемент массива SkyboxMats
		short SelSkyEl = 0;														// Сдесь будет храниться номер выбранного купленного скайбокса в массиве SkyboxMats для выбранного уровня скайбоксов
		byte BuyedSkybox = 0;													// Номер найденного купленного скайбокса для этих 25 ячеек

		Mats.Clear();															// Очищаем список текстур материалов выбранной ячейки
		Imprvs.Clear();															// Очищаем список текстур улучшений выбранной ячейки
		StateMats.Clear();														// Очищаем список состояний материалов
		StateImprs.Clear();														// Очищаем список состояний улучшений
		PriceMats.Clear();														// Очищаем список цен материалов
		PriceImpr.Clear();														// Очищаем список цен улучшений
		ExpMats.Clear();														// Очищаем список необходимого опыта для материалов
		ExpImprs.Clear();														// Очищаем список необходмого опыта для улучшений

		// Определяем номер выбранной ячейки в массиве ObjectsStore или SkyboxMats
		if(ActiveCategory < 3)													// Если мы работаем с категорией объектов в инвентаре
		{	
			for(byte a=0; a < ObjsTexs.Length; a++)								// Продолжаем цикл до тех пор пока не переберём все 25 скриншотов объектов в массиве ObjectsStore соответствующих 25 ячеекам в инвентаре для данной категории и уровня
			{
				if(SC.ObjectsStore[ObjEl] != null)								// Если выбранный объект в массиве существует
				{
					if(Kep.ObjectsStates[Kep.ViewedProfile][ObjEl] != 0)		// Если сохранение для этого объекта существует
					{
						if(Kep.ObjectsStates[Kep.ViewedProfile][ObjEl] == 'B')	// Если этот объект куплен
						{
							if(BuyedObject == SelectedCell)						// Если номер выбранной ячейки "SelectedCell" соответствует очередному найденному купленному объекту "BuyedObject" 
							{
								SelObjEl = (short)(FirstObjEl + a);				// Прибавляем к FirsObjEl номер цикла (Вычисляя таким образом номер этого объекта в массиве ObjStore) и присваиваем переменной SelObjEl
								break;											// И заканчиваем цикл
							}
							else 												// Иначе если номер выбранной ячейки "SelectedCell" не соответствует очередному найденному купленному объекту "BuyedObject" 
							{
								BuyedObject ++;									// Прибавляем "BuyedObject" +1 чтобы засчитать очередной найденный купленный объект
							}
						}
					}
				}
				ObjEl++;														// Прибавляем к переменной (ObjEl) 1 чтобы в следующем цикле опросить следующий элемент массива ObjectStore
			}
		}
		else 																	// Иначе если мы работаем с категорией скайбоксов в инвентаре
		{
			for(byte a=0; a < ObjsTexs.Length; a++)								// Продолжаем цикл до тех пор пока не переберём все 25 скриншотов скайбоксов в массиве SkyboxMats соответствующих 25 ячеекам в инвентаре для данного уровня
			{
				if(SC.SkyboxMats[SkyEl] != null)								// Если выбранный скайбокс в массиве существует
				{
					if(Kep.SkyboxesStates[Kep.ViewedProfile][SkyEl] != 0)		// Если сохранение для этого скайбокса существует
					{
						if(Kep.SkyboxesStates[Kep.ViewedProfile][SkyEl] == 'B')	// Если этот скайбокс куплен
						{
							if(BuyedSkybox == SelectedCell)						// Если номер выбранной ячейки "SelectedCell" соответствует очередному найденному купленному скайбоксу "BuyedSkybox"
							{
								SelSkyEl = (short)(FirstSkyEl + a);				// Прибавляем к FirstSkyEl номер цикла (Вычисляя таким образом номер этого объекта в массиве SkyBoxMats) и присваиваем переменной SelSkyEl
								break;											// И заканчиваем цикл
							}
							else 												// Иначе если номер выбранной ячейки "SelectedCell" не соответствует очередному найденному купленному скайбоксу "BuyedSkybox"
							{
								BuyedSkybox ++;									// Прибавляем "BuyedSkybox" +1 чтобы засчитать очередной найденный купленный скайбокс
							}
						}
					}
				}
			}
		}

		if(VideoObjClone)														// Если VideoObjClone не пуст
		{
			if(VideoObjClone.GetComponent<MeshCollider>())						// Если мы работаем с категорией объектов
			{
				VideoObjClone.GetComponent<MeshCollider>().enabled = false;    	// Сначала отключаем его меш коллайдер чтобы он не столкнулься с объектом который прыгнет на его место
				GameObject.Destroy(VideoObjClone);								// Удаляем со сцены старый объект который мы обозревали(Я использую Destroy потому что с DestroyImmediate конкретно сдесь юнити вылетает)
			}
			else 																// Иначе если мы работаем с категорией скайбоксов
			{
				GameObject.DestroyImmediate(VideoObjClone);						// Используем мнгновенное удаление чтобы удалить панель обзора скайбокса
			}
		}

		FirstCall = true;															// Ставим переменной FirstCall состояние правда

		if(ObjsTexs[SelectedCell].name != "NotExistElement")						// Если в инвентаре для этого объекта имя текстуры не равно NotExistElement
		{
			if(ActiveCategory < 3)													// Если мы работаем с категорией объектов
			{
				if(GM.Currentkep.ObjectsStates[Kep.ViewedProfile][SelObjEl] != 0)	// Если мы нашли сохранение для этого объекта
				{	// Значит для этого объекта есть сохранение а так как мы в инвентаре то от куплен
					// Мы не проверяем закрыт объект или нет потому что если бы он был закрыт он бы не был сохранён
					VideoObjClone = (GameObject)Instantiate(SC.ObjectsStore[SelObjEl], new Vector3(5, 100, 0), Quaternion.AngleAxis(90, new Vector3(-1, 0, 0)));	// Помещаем на сцену клон префаба объекта а сслыку на этот объект отправляем в ObjClone
					float Size = SC.ObjectsStore[SelObjEl].GetComponent<ObjectScript>().ViewSize;								// Берём размер объекта для фотографии и копируем его в переменную Size
					Vector3 Size3 = new Vector3(Size, Size, Size);																// Создаём новый вектор3 с именем Size3 и заполняем все его оси из переменной Size
					VideoObjClone.transform.localScale = Size3 ;																// Придаём новому клону объекта размер указанный в Size3 
					SelObj = VideoObjClone.GetComponent<Rigidbody>();															// Получаем Rigidbody для клона просматриваемого объекта
					ObjScr = VideoObjClone.GetComponent<ObjectScript>();														// Получаем скрипт ObjectScript для клона просматриваемого объекта
					ActiveObjNomber = SelObjEl;																					// Присваиваем номер выбранного объекта в массиве ActiveObjNomber переменной SelectedObjStore
					byte RealActiveMat = (byte)(GM.Currentkep.ActiveMats[Kep.ViewedProfile][SelObjEl]);							// Присваиваем переменной ActiveMat номер активного материала для этого объекта
					byte RealActiveImpr = (byte)(GM.Currentkep.ActiveImprs[Kep.ViewedProfile][SelObjEl]);						// Присваиваем переменной ActiveImpr номер активного улучшения для этого объекта

					ActiveMat = CalculateTNMatAndImr((byte)(GM.Currentkep.ActiveMats[Kep.ViewedProfile][SelObjEl]), true);		// Высчитываем временный номер активного материала и присваеваем его переменной ActiveMat
					SelectedMat = ActiveMat;																					// Присваиваем переменной SelectedMat значение переменной ActiveMat	

					ActiveImpr = CalculateTNMatAndImr((byte)(GM.Currentkep.ActiveImprs[Kep.ViewedProfile][SelObjEl]), false);	// Высчитываем временный номер активного улучшения и присваиваем его переменной ActiveImpr
					SelectedImpr = ActiveImpr;																					// Делаем выделенным активное улучшение

					ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, RealActiveMat);			// Накладываем на объект его выбранный материал
					if(ActiveImpr != 10) ObjectChangeImprovement(RealActiveImpr);					// Если у этого объекта есть активное улучшение устанавливаем его на объект

					for(byte b=0; b < Kep.StatesMaterials[Kep.ViewedProfile][SelObjEl].Count; b++)	// Продолжаем итерацию пока не переберём все материалы существующие у объекта активного профиля										
					{
						if(Kep.StatesMaterials[Kep.ViewedProfile][SelObjEl][b] == 'B')				// Если этот материал куплен
						{
							PriceMats.Add(ObjScr.PricesMats[b]);									// Присваиваем элементу списка PriceMats Цену необходимую для открытия этого материала
							ExpMats.Add(ObjScr.RequireExpirienceMats[b]);							// Присваиваем элементу списка ExpMats опыт необходимый для открытия этого материала
							Mats.Add(ObjScr.FotoMaterials[b]);										// Мы загружаем фотографию материала в список материалов
						}
					}
					if(Kep.StatesImprovements[Kep.ViewedProfile][SelObjEl].Count > 0)						// Если в сохранении для объекта записанны улучшения
					{
						for(byte c=0; c < Kep.StatesImprovements[Kep.ViewedProfile][SelObjEl].Count; c++)	// Продолжаем цикл пока не переберём все сохранения для этого объекта
						{
							if(Kep.StatesImprovements[Kep.ViewedProfile][SelObjEl][c] == 'B')				// Если улучшение с номером этого цикла куплено
							{
								PriceImpr.Add(ObjScr.PricesImprs[c]);										// Присваиваем элементу списка PriceImpr Цену необходимую для открытия этого улучшения
								ExpImprs.Add(ObjScr.RequireExpirienceImprs[c]);								// Присваиваем элементу списка ExpImprs опыт необходимый для открытия этого улучшения
								Imprvs.Add(ObjScr.IconImprovements[c]);										// То добавляем в список текстур улучшений текстуру этого улучшения
							}
						}
					}
					ActiveObjNomber = SelObjEl;												// Присваиваем ActiveObjectNomber значение переменной SelObjEl	
				}
			}
			else if(ActiveCategory == 3) 													// Иначе если мы работаем с категорией скайбоксов
			{
				if(Kep.SkyboxesStates[Kep.ViewedProfile][SelSkyEl] != 0)					// Если мы нашли сохранение для этого скайбокса
				{	// Значит для этого объекта есть сохранение а так как мы в инвентаре то от куплен
					VideoObjClone = (GameObject)Instantiate(Resources.Load("Models/Prefabs/SkyboxReviewPanel"), new Vector3(5, 100, 0), Quaternion.AngleAxis(90, new Vector3(-1, 0, 0)));	// Помещаем на сцену панель обзора скайбокса а ссылку на неё в VideoObjClone
					Material[] TimeMass = VideoObjClone.GetComponent<Renderer>().materials;	// Присваиваем массив материалов новому массиву материалов (Так как по одиночке материал не меняеться)
					TimeMass[2] = SC.MatsForReviewPanel[SelSkyEl];							// Меняем уже в этом массиве материал на тот что надо
					VideoObjClone.GetComponent<Renderer>().materials = TimeMass;			// И присваиваем этот массив обратно панели обзора скайбоксов отображая тем самым текущий скайбокс на панели обзора скайбоксов
					ActiveSkyNomber = SelSkyEl;												// Присваиваем ActiveSkyNomber номер SelSkyEl
				}
			}
		}
	}


	byte RenderObjCells(Rect[] Pos, byte Selected)											// Этот метод отрисовывает клетки объектов в магазине и объекты внутри них
	{
		for(byte a=0; a < Pos.Length; a++)													// Продолжаем цикл до тех пор пока не отрисуем все иконки объектов в активной категории
		{
			if(a == ActiveCatObj && a == Selected)											// Если мы отрисовываем (Активный и выбраный) объект
			{
				if(ActiveCategory < 3)														// Если мы отрисовываем категорию объектов а не скайбоксов
				{
					GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelected");		// Отрисовываем выделенную кнопку
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх объекта зелёную маску выделения
					GUI.Label(PosIconMask[a], ObjsTexs[a]);									// Отрисовываем объект
				}
				else 																		// Иначе если мы отрисовываем категорию скайбоксов
				{
					GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected");				// Отрисовываем выделенную кнопку а в качестве заднего фона скайбокс который означает эта кнопка
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх скайбокса зелёную маску выделения
				}
			}
			else if(a == ActiveCatObj && a != Selected)										// Если мы отрисовываем (Активный но не выбранный) объект
			{
				if(ActiveCategory < 3)														// Если мы отрисовываем категорию объектов а не скайбоксов
				{
					if(ObjsStates[a] == 'B')												// Если состояние объекта Куплен
					{
						if(GUI.Button(Pos[a],ObjsBackgrounds[0], "ButtonElementActive"))	// Отрисовываем выделенную кнопку и если она нажата
							Selected = a;													// Присваиваем Selected номер кнопки нажатой в этом цикле
						GUI.Label(PosIconMask[a], ActiveShadeObject);						// Отрисовываем поверх объекта зелёную маску выделения
						GUI.Label(PosIconMask[a],  ObjsTexs[a]);							// Отрисовываем объект
					}
				}
				else 																		// Иначе если мы отрисовываем категорию скабоксов
				{
					if(ObjsStates[a] == 'B')												// Если состояние скайбокса куплен
					{																		
						if(GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementActive"))			// Отрисовываем выделенную кнопку а в качестве заднего фона скайбокс который означает эта кнопка
							Selected = a;													// Присваиваем Selected номер кнопки нажатой в этом цикле
						GUI.Label(PosIconMask[a], ActiveShadeObject);						// Отрисовываем поверх объекта зелёную маску выделения
					}
				}
			}
			else if(a != ActiveCatObj && a == Selected)										// Если мы отрисовываем (Неактивный но выбранный) объект
			{
				if(ActiveCategory < 3)														// Если мы работаем с категорией объектов
				{
					if(ObjsStates[a] == 'C')												// Если состояние объекта закрыт
					{
						GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelected");			// Отрисовываем выделенную кнопку
					}
					else if(ObjsStates[a] == 'O')											// Если состояние объекта открыт
					{
						GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelected");	// Отрисовываем выделенную кнопку
						GUI.Label(PosIconMask[a],  ObjsTexs[a]);							// Отрисовываем объект
						GUI.Label(PosIconMask[a],ShadingObject);							// Отрисовываем маску затенения поверх той ячейки
						GUI.Label(PosIconMask[a], PriceObjs[a] + "", "StorePriceText");		// И отрисовываем цену поверх неё для этого скайбокса
					}
					else if(ObjsStates[a] == 'B')											// Если этот объект что мы выделили куплен
					{
						if(GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelected"))	// Отрисовываем выделенную кнопку и если она была нажата
						{
							ActiveCatObj = a;														// То мы заносим его в массив ActiveCatObj как новый активный объект выбранный для игры для своей категории
							Kep.ActiveObjects[Kep.ActiveProfile,ActiveCategory] = ActiveObjNomber;	// А также заносим в сохранение в скрипте Keeper что у нас новый активный объект
							StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для массива 4 активных объектов
						}
						GUI.Label(PosIconMask[a], ObjsTexs[a]);								// Отрисовываем объект
					}
					else 																	// Иначе если для него нет сохранения
						GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelected");			// Отрисовываем выделенную кнопку
				}
				else	 																	// Иначе если мы работаем с категорией скайбоксов
				{
					if(ObjsStates[a] == 'C')												// Если состояние скайбокса закрыт
					{
						GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelected");			// Отрисовываем выделенную кнопку с текстурой замка
					}
					else if(ObjsStates[a] == 'O')											// Если состояние объекта открыт
					{
						GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected");			// Отрисовываем выделенную кнопку
						GUI.Label(PosIconMask[a],ShadingObject);							// Отрисовываем маску затенения поверх той ячейки
						GUI.Label(PosIconMask[a], PriceObjs[a] + "", "StorePriceText");		// И отрисовываем цену поверх неё
					}
					else if(ObjsStates[a] == 'B') 											// Если состояние скайбокса куплен
					{
						if(GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected"))		// Отрисовываем выделенную кнопку и если она была нажата
						{
							ActiveCatObj = a;														// Заносим его в массив ActiveCatObj как новый активный объект выбранный для игры для своей категории
							Kep.ActiveObjects[Kep.ActiveProfile, ActiveCategory] = ActiveSkyNomber;	// А также заносим в сохранение в скрипте Keeper
							StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveSkyNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для массива 4 активных объектов
						}
					}
					else 																	// Иначе если для него нет сохранения
						GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelected");			// Отрисовываем выделенную кнопку	
				}	
			}
			else if(a != ActiveCatObj && a != Selected)										// Если мы отрисовываем (Неактивный и невыбранный) объект
			{
				if(ObjsStates[a] == 'C')													// Если состояние объекта закрыт
				{
					if(GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelection"))			// Отрисовываем кнопку
						Selected = a;														// Присваиваем Active номер кнопки нажатой в этом цикле
				}
				else if(ObjsStates[a] == 'O')												// Если состояние объекта открыт
				{
					if(GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelection"))	// Отрисовываем кнопку и если она была нажата
						Selected = a;														// Присваиваем Active номер кнопки нажатой в этом цикле	
					GUI.Label(PosIconMask[a],  ObjsTexs[a]);								// Отрисовываем объект
					GUI.Label(PosIconMask[a],ShadingObject);								// Отрисовываем маску затенения поверх той ячейки
					GUI.Label(PosIconMask[a], PriceObjs[a] + "", "StorePriceText");			// И отрисовываем цену поверх неё
				}
				else if(ObjsStates[a] == 'B')												// Если состояние объекта открыт)																		
				{
					if(GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelection"))	// Отрисовываем невыделенную кнопку
						Selected = a;														// Присваиваем Active номер кнопки нажатой в этом цикле	
					GUI.Label(PosIconMask[a],  ObjsTexs[a]);								// Отрисовываем объект
				}
				else // Иначе если он куплен или не существует
				{
					if(GUI.Button(Pos[a],  ObjsTexs[a], "ButtonElementSelection"))			// Отрисовываем кнопку и если она была нажата
						Selected = a;														// Присваиваем Active номер кнопки нажатой в этом цикле		
				}
			}
		}
		return Selected;																	// Возвращаем выбранную кнопку
	}


	byte RenderBuyedCells(Rect[] Pos,  byte Selected)										// Этот метод отрисовывает клетки купленных объектов в инвентаре
	{
		for(byte a=0; a < Pos.Length; a++)													// Продолжаем цикл до тех пор пока не отрисуем все иконки объектов в активной категории
		{
			if(a == ActiveCatObj && a == Selected)											// Если мы отрисовываем (Активный и выбраный) объект
			{
				if(ActiveCategory < 3)														// Если мы отрисовываем категорию объектов а не скайбоксов
				{
					GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelected");		// Отрисовываем выделенную кнопку
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх объекта зелёную маску выделения
					GUI.Label(PosIconMask[a], ObjsTexs[a]);									// Отрисовываем объект
				}
				else
				{
					GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected");				// Отрисовываем выделенную кнопку а в качестве заднего фона скайбокс который означает эта кнопка
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх скайбокса зелёную маску выделения
				}
			}
			else if(a == ActiveCatObj && a != Selected)										// Если мы отрисовываем (Активный но не выбранный) объект
			{
				if(ActiveCategory < 3)														// Если мы отрисовываем категорию объектов а не скайбоксов
				{
					if(GUI.Button(Pos[a],ObjsBackgrounds[0], "ButtonElementActive"))		// Отрисовываем выделенную кнопку и если она нажата
					{
						Selected = a;														// Присваиваем Selected номер кнопки нажатой в этом цикле
					}
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх объекта зелёную маску выделения
					GUI.Label(PosIconMask[a], ObjsTexs[a]);									// Отрисовываем объект
				}
				else 																		// Иначе если мы отрисовываем категорию скабоксов
				{
					if(GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementActive"))				// Отрисовываем выделенную кнопку а в качестве заднего фона скайбокс который означает эта кнопка
						Selected = a;														// Присваиваем Selected номер кнопки нажатой в этом цикле
					GUI.Label(PosIconMask[a], ActiveShadeObject);							// Отрисовываем поверх объекта зелёную маску выделения
				}
			}
			else if(a != ActiveCatObj && a == Selected)												// Если мы отрисовываем (Неактивный но выбранный) объект
			{
				if(ActiveCategory < 3)																// Если мы работаем с категорией объектов
				{
					if(ObjsStates[a] == 'B')														// Если состояние этого объекта в во временном массиве состояний куплен
					{
						if(GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelected"))			// Отрисовываем выделенную кнопку и если она была нажата
						{	
							ActiveCatObj = a;														// То для переменной активного объекта этой категории и уровня мы ставим этот объект как новый активный
							GM.Currentkep.ActiveObjects[Kep.ViewedProfile, ActiveCategory] = ActiveObjNomber; // А также заносим в текущую базу данных что у нас новый активный объект для отрисовываемого игрока
							StartCoroutine(Kep.FillActiveObjTex(Kep.ViewedProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов	
							if(ActiveCategory != 0)													// Если мы изменили объект не в категории бит (так как бита для каждого игрока своя)
								GM.PAON[ActiveCategory - 1] = Kep.NomberPlayer;						// Ложим в массив номеров игроков GM.PAON номер игрока с которым мы щас работаем
							GM.RestartGameObjects();												// Вызываем метод RestartGameObjects чтобы поставить новые выбранные объекты и скайбокс на сцену
						}
						GUI.Label(PosIconMask[a], ObjsTexs[a]);										// Отрисовываем объект
					}
					else 																			// Иначе если объект для этого цикла не куплен
					{
						GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected");					// Просто отрисовываем текстуру крестика из массива текстур для 25 объектов этой категории и уровня
					}	
				}
				else	 																			// Иначе если мы работаем с категорией скайбоксов
				{
					if(ObjsStates[a] == 'B')														// Если состояние этого объекта в во временном массиве состояний куплен
					{
						if(GUI.Button(Pos[a], ObjsTexs[a], "ButtonElementSelected"))				// отрисовываем выделенную кнопку
						{
							ActiveCatObj = a;														// То для переменной активного скайбокса этой ктаегории и уровня мы ставим этот скайбокс как новый активный
							GM.Currentkep.ActiveObjects[Kep.ViewedProfile,ActiveCategory] = ActiveSkyNomber;	// А также заносим в текущую базу данных что у нас новый активный скайбокс для отрисовываемого игрока
							StartCoroutine(Kep.FillActiveObjTex(Kep.ViewedProfile, ActiveSkyNomber, (byte)ActiveCategory));		// Используем метод FillActiveObjTex чтобы положить новый скайбокс в переменную для активного скайбокса
							GM.PAON[ActiveCategory - 1] = Kep.NomberPlayer;							// Ложим в массив номеров игроков GM.PAON номер игрока с которым мы щас работаем
							GM.RestartGameObjects();												// Вызываем метод RestartGameObjects чтобы поставить новые выбранные объекты и скайбокс на сцену	
						}
					}
					else 																			// Иначе если текстура не существует
						GUI.Button(Pos[a], NotExist, "ButtonElementSelected");						// Отрисовываем кнопку с текстурой крестика
				}
			}
			else if(a != ActiveCatObj && a != Selected)												// Если мы отрисовываем (Неактивный и невыбранный) объект
			{
				if(GUI.Button(Pos[a], ObjsBackgrounds[0], "ButtonElementSelection"))				// Отрисовываем невыделенную кнопку
				{
					Selected = a;																	// Присваиваем Active номер кнопки нажатой в этом цикле
				}
				GUI.Label(PosIconMask[a], ObjsTexs[a]);												// Отрисовываем объект
			}
		}
		return Selected;
	}


	// Этот метод отрисовывает кнопки материалов, либо улучшений и возвращает номер выбранного объекта
	void RenderGroup(Rect[] Rects, byte Selected, List<Texture2D> Textures, List<char> States, List<int> Prices, List<int> Exps, bool RenderMats)	// Этот метод отрисовывает раскраски и улучшения в магазине
	{
		if(RenderMats)																				// Если значение рендерим материалы правда
		{																							// Начинаем рендерить материалы
			for(byte a=0; a<Textures.Count; a++)													// Продолжаем цикл пока у нас есть текстуры материалов для отрисовки
			{
				if(a == ActiveMat && a == Selected)													// Если мы отрисовываем (Активный и выбранный) материал										
				{
					GUI.Button(Rects[a], MiniBackgrounds[0], "SelectedSquareButton");				// Отрисовываем активную и выдделенную кнопку
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх зелёную маску
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
				}
				else if(a == ActiveMat && a != Selected)											// Иначе если в этом цикле мы отрисовываем (Активный но невыбранный) материал
				{
					if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'C' | Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')			// Если этот материал закрыт или открыт
					{
						if(GUI.Button(Rects[a], Textures[a], "ActiveSquareButton"))					// Отрисовываем кнопку активного материала и если она нажата
						{
							SelectedMat = a;														// Делаем её выбранной
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх материала зелёную маску
					}
					else if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')		// Иначе если этот материал куплен
					{
						if(GUI.Button(Rects[a], MiniBackgrounds[0], "ActiveSquareButton"))			// Отрисовываем кнопку активного материала и если она нажата
						{
							SelectedMat = a;														// Делаем её выбранной
							ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, a);				// Накладываем на объект его новый выбранный материал	
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх материала зелёную маску
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
					}
				}
				else if(a != ActiveMat && a == Selected)												// Иначе если мы отрисовываем (Неактивный но выбранный) материал
				{
					if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'C')				// Если этот материал закрыт
					{
						GUI.Button(Rects[a], Textures[a], "SelectedSquareButton");						// Отрисовываем выделенную кнопку
						if(ActiveCategory != 2) GUI.Label(Rects[a], Exps[a] + "", "ExpImprsAndMats");	// И отрисовываем количество опыта необходимое для открытия материала
					}
					else if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')			// Иначе если этот материал открыт
					{
						GUI.Button(Rects[a], MiniBackgrounds[0], "SelectedSquareButton");														// Отрисовываем выделенную кнопку
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);		// Отрисовываем материал
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ShadingSquareButon);	// Отрисовываем поверх материала маску
						GUI.Label(Rects[a], Prices[a] + "", "PriceImprsAndMats");																// Отрисовываем поверх открытого материала количество денег для его покупки
					}
					else if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')		// Иначе если этот материал куплен
					{
						if(GUI.Button(Rects[a], MiniBackgrounds[0], "SelectedSquareButton"))		// И если кнопка отрисовывающая этот материал была нажата
						{																			// Делаем материал активным
							ActiveMat = a;															// Указываем во временной переменной что этот материал для выделенного объекта стал активным
							Kep.ActiveMats[Kep.ActiveProfile][ActiveObjNomber] = a;					// Указываем в переменной сохранения что этот материал для выделенного объекта стал активным
							StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
							if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисыовываем инвентарь а не магазин
								GM.RestartGameObjects();											// Мы вызываем метод RestartGameObjects
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
					}
				}
				else if(a != ActiveMat && a != Selected)												// Иначе если мы отрисовываем (Неактивный и невыбранный) материал
				{
					if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'C')				// Если этот материал закрыт
					{
						if(GUI.Button(Rects[a], Textures[a], "SelectionSquareButton"))					// Отрисовываем выделенную кнопку и если она нажата
						{
							SelectedMat = a;															// Ставим этот материал как выделенный
						}
						if(ActiveCategory != 2) GUI.Label(Rects[a], Exps[a] + "", "ExpImprsAndMats");	// И если мы отрисовываем не категорию столы то отрисовываем количество опыта необходимое для открытия этого материала
					}
					else if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')			// Иначе если этот материал открыт
					{
						if(GUI.Button(Rects[a], MiniBackgrounds[0], "SelectionSquareButton"))			// Отрисовываем выделенную кнопку и если она нажата
							SelectedMat = a;															// Ставим этот материал как выделенный
						
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);		// Отрисовываем материал
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2 ,Rects[a].width - 4, Rects[a].height - 4), ShadingSquareButon);	// Отрисовываем поверх материала маску затенения
						GUI.Label(Rects[a], Prices[a] + "", "PriceImprsAndMats");																// Отрисовываем поверх открытого материала количество денег для его покупки
					}
					else if(Kep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')		// Иначе если этот материал куплен
					{
						if(GUI.Button(Rects[a], MiniBackgrounds[0], "SelectionSquareButton"))		// И если кнопка отрисовывающая этот материал была нажата
						{
							SelectedMat = a;														// Ставим этот материал как выделенный
							ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, a);				// Накладываем на объект его новый выбранный материал
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);		// Отрисовываем материал
					}
				}
			}
		}
		else 																			// Иначе если значение рендерим материалы ложь
		{																				// Начинаем рендерить улучшения
			if(GUI.Button(new Rect(872, 638, 125, 22), "Снять", "RemoveImprovement"))	// Отрисовываем кнопку снятия улучшения и если она была нажата
			{
				byte ObjEl = (byte)((ActiveCategory*125)+(ActiveLvl*25)+SelectedCell);	// Определяем номер выбранного объекта в массиве ObjStore
				ActiveImpr = 10;														// Ставим активное улучшение 10
				SelectedImpr = 10;														// Ставим выбранное улучшение 10
				Selected = 10;															// Ставим переменную Selecet в данном методе равной 10
				SelObj.mass -= VideoImprClone.GetComponent<ImprovementScript>().Mass;	// Отнимаем вес улучшения от веса объекта
				ObjScr.Force -= VideoImprClone.GetComponent<ImprovementScript>().Force;	// Отнимаем силу улучшения если она есть от силы объекта если она есть
				DestroyImmediate(VideoImprClone);										// Удаляем со сцены это улучшение
				Kep.ActiveImprs[Kep.ActiveProfile][ObjEl] = 10;							// Заносим в сохранение несуществующий номер улучшения
				StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
				if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисыовываем инвентарь а не магазин
					GM.RestartGameObjects();											// Мы вызываем метод RestartGameObjects
			}										
			for(byte a=0; a<Textures.Count; a++)										// Продолжаем цикл пока у нас есть текстуры улучшений для отрисовки
			{
				if(a == ActiveImpr && a == Selected)																							// Если мы отрисовываем (Активное и выбранное) улучшение
				{
					GUI.Button(Rects[a], Textures[a], "SelectedSquareButton");																	// Отрисовываем активную и выдделенную кнопку
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх улучшения зелёную маску
				}
				else if(a == ActiveImpr && a != Selected)																						// Иначе если мы отрисовываем (Активное но невыбранное) улучшение
				{
					if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'C' | Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')	// Если это улучшение закрыто или открыто
					{
						if(GUI.Button(Rects[a], Textures[a], "ActiveSquareButton"))					// Отрисовываем кнопку активного материала и если она нажата
						{
							SelectedImpr = a;														// Делаем её выбранной
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх улучшения зелёную маску
					}
					else if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')	// Иначе если это улучшение купленно
					{
						if(GUI.Button(Rects[a], Textures[a], "ActiveSquareButton"))					// Отрисовываем кнопку активного улучшения и если она нажата
						{
							SelectedImpr = a;														// Делаем её выбранной
							ObjectChangeImprovement(SelectedImpr);									// Меняем на выбранное улучшение
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх улучшения зелёную маску
					}
				}
				else if(a != ActiveImpr && a == Selected)											// Иначе если мы отрисовываем (Неактивное но выбранное) улучшение
				{
					if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'C')			// Если это улучшение закрыто
					{
						GUI.Button(Rects[a], Textures[a], "SelectedSquareButton");						// Отрисовываем выделенную кнопку
						if(ActiveCategory != 2) GUI.Label(Rects[a], Exps[a] + "", "ExpImprsAndMats");	// И если мы отрисовываем не категорию столы то отрисовываем количество опыта необходимое для открытия этого улучшения
					}
					else if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')		// Иначе если это улучшение открыто
					{
						GUI.Button(Rects[a], Textures[a], "SelectedSquareButton");																// Отрисовываем выделенную кнопку
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2 ,Rects[a].width - 4, Rects[a].height - 4), ShadingSquareButon);	// Отрисовываем поверх материала маску
						GUI.Label(Rects[a], Prices[a] + "", "PriceImprsAndMats");																// Отрисовываем поверх открытого материала количество денег для его покупки
					}
					else if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')	// Иначе если это улучшение купленно
					{
						if(GUI.Button(Rects[a], Textures[a], "SelectedSquareButton"))				// Отрисовываем выделенную кнопку и если она была нажата
						{																			// Делаем улучшение активным
							ActiveImpr = a;															// Указываем во временной переменной что это улучшение для выделенного объекта стало активным
							Kep.ActiveImprs[Kep.ActiveProfile][ActiveObjNomber] = a;				// Указываем в переменной сохранения что это улучшение для выделенного объекта стало активным
							StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
							if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисыовываем инвентарь а не магазин
								GM.RestartGameObjects();											// Мы вызываем метод RestartGameObjects
						}
					}
				}
				else if(a != ActiveImpr && a != Selected)												// Иначе если мы отрисовываем (Неактивное и невыбранное) улучшение
				{
					if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'C')			// Если это улучшение закрыто
					{
						if(GUI.Button(Rects[a], Textures[a], "SelectionSquareButton"))					// Отрисовываем выделенную кнопку и если она нажата
						{
							SelectedImpr = a;															// Ставим это улучшение как выделенное
						}
						if(ActiveCategory != 2) GUI.Label(Rects[a], Exps[a] + "", "ExpImprsAndMats");	// И если мы отрисовываем не категорию столы то отрисовываем количество опыта необходимое для открытия этого улучшения
					}
					else if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'O')		// Иначе если это улучшение открыто
					{
						if(GUI.Button(Rects[a], Textures[a], "SelectionSquareButton"))															// Отрисовываем выделенную кнопку и если она нажата
						{
							SelectedImpr = a;																									// Ставим это улучшение как выделенное
						}
						GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), ShadingSquareButon);	// Отрисовываем поверх материала маску
						GUI.Label(Rects[a], Prices[a] + "", "PriceImprsAndMats");																// Отрисовываем поверх открытого улучшения количество денег для его покупки
					}
					else if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')	// Иначе если это улучшение купленно
					{
						if(GUI.Button(Rects[a], Textures[a], "SelectionSquareButton"))				// И если кнопка отрисовывающая этот материал была нажата
						{
							SelectedImpr = a;														// Досрочно изменяем номер SelectedImpr
							ObjectChangeImprovement(SelectedImpr);									// Меняем на выбранное улучшение
						}
					}
				}
			}
		}
	}


	void RenderBuyedGroup(Rect[] Rects, byte Selected, List<Texture2D> Textures, List<char> States, List<int> Prices, List<int> Exps, bool RenderMats) 		// Этот метод отрисовывает купленные раскраски и улучшения для инвентаря
	{
		if(RenderMats)																				// Если значение рендерим материалы правда
		{																							// Начинаем рендерить материалы
			for(byte a=0; a<Textures.Count; a++)													// Продолжаем цикл пока у нас есть текстуры материалов для отрисовки
			{
				if(a == ActiveMat && a == Selected)													// Если мы отрисовываем (Активный и выбранный) материал										
				{
					GUI.Button(Rects[a], MiniBackgrounds[0], "SelectedSquareButton");				// Отрисовываем активную и выдделенную кнопку
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх зелёную маску
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
				}
				else if(a == ActiveMat && a != Selected)											// Иначе если в этом цикле мы отрисовываем (Активный но невыбранный) материал
				{
					if(GUI.Button(Rects[a], MiniBackgrounds[0], "ActiveSquareButton"))				// Отрисовываем кнопку активного материала и если она нажата
					{
						SelectedMat = a;															// Делаем её выбранной
						ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, a);					// Накладываем на объект его новый выбранный материал	
					}
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх материала зелёную маску
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
				}
				else if(a != ActiveMat && a == Selected)											// Иначе если мы отрисовываем (Неактивный но выбранный) материал
				{
					if(GUI.Button(Rects[a], MiniBackgrounds[0], "SelectedSquareButton"))			// И если кнопка отрисовывающая задний фон для этого материала была нажата
					{																				// Делаем материал активным
						ActiveMat = a;																// Указываем во временной переменной что этот материал для выделенного объекта стал активным


					
						GM.Currentkep.ActiveMats[Kep.ViewedProfile][ActiveObjNomber] = CalculateRNMatAndImpr(a, true);							// Высчитываем реальный номер материала и сохраняем его в постоянной переменной активных материалов
					


						StartCoroutine(Kep.FillActiveObjTex(Kep.ViewedProfile, ActiveObjNomber, (byte)ActiveCategory));							// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
						if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисыовываем инвентарь а не магазин
							GM.RestartGameObjects();												// Мы вызываем метод RestartGameObjects
					}
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
				}
				else if(a != ActiveMat && a != Selected)														// Иначе если мы отрисовываем (Неактивный и невыбранный) материал
				{
					if(GUI.Button(Rects[a], MiniBackgrounds[0], "SelectionSquareButton"))						// И если кнопка отрисовывающая этот материал была нажата
					{
						SelectedMat = a;																		// Ставим этот материал как выделенный
						ObjectChangeMaterial(ActiveObjNomber, VideoObjClone, CalculateRNMatAndImpr(a, true));	// Накладываем на объект его новый выбранный материал
					}
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2, Rects[a].width - 4, Rects[a].height - 4), Textures[a]);			// Отрисовываем материал
				}
			}
		}
		else 																			// Иначе если значение рендерим материалы ложь
		{																				// Начинаем рендерить улучшения
			if(GUI.Button(new Rect(872, 638, 125, 22), "Снять", "RemoveImprovement"))	// Отрисовываем кнопку снятия улучшения и если она была нажата
			{
				byte ObjEl = (byte)((ActiveCategory*125)+(ActiveLvl*25)+SelectedCell);	// Определяем номер выбранного объекта в массиве ObjStore
				ActiveImpr = 10;														// Ставим активное улучшение 10
				SelectedImpr = 10;														// Ставим выбранное улучшение 10
				Selected = 10;															// Ставим переменную Selecet в данном методе равной 10
				SelObj.mass -= VideoImprClone.GetComponent<ImprovementScript>().Mass;	// Отнимаем вес улучшения от веса объекта
				ObjScr.Force -= VideoImprClone.GetComponent<ImprovementScript>().Force;	// Отнимаем силу улучшения если она есть от силы объекта если она есть
				DestroyImmediate(VideoImprClone);										// Удаляем со сцены это улучшение
				Kep.ActiveImprs[Kep.ActiveProfile][ObjEl] = 10;							// Заносим в сохранение несуществующий номер улучшения
				StartCoroutine(Kep.FillActiveObjTex(Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));		// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
				if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисыовываем инвентарь а не магазин
					GM.RestartGameObjects();											// Мы вызываем метод RestartGameObjects
			}										
			for(byte a=0; a<Textures.Count; a++)										// Продолжаем цикл пока у нас есть текстуры улучшений для отрисовки
			{
				if(a == ActiveImpr && a == Selected)																							// Если мы отрисовываем (Активное и выбранное) улучшение
				{
					GUI.Button(Rects[a], Textures[a], "SelectedSquareButton");																	// Отрисовываем активную и выдделенную кнопку
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх улучшения зелёную маску
				}
				else if(a == ActiveImpr && a != Selected)																						// Иначе если мы отрисовываем (Активное но невыбранное) улучшение
				{
					if(GUI.Button(Rects[a], Textures[a], "ActiveSquareButton"))					// Отрисовываем кнопку активного улучшения и если она была нажата в этом цикле
					{
						SelectedImpr = a;														// Делаем её выбранной
					}
					GUI.Label(new Rect(Rects[a].xMin + 2, Rects[a].yMin + 2,Rects[a].width - 4, Rects[a].height - 4), ActiveShadeSquareButton);	// Отрисовываем поверх улучшения зелёную маску
				}
				else if(a != ActiveImpr && a == Selected)										// Иначе если мы отрисовываем (Неактивное но выбранное) улучшение
				{
					if(GUI.Button(Rects[a], Textures[a], "SelectedSquareButton"))				// Отрисовываем выделенную кнопку и если она была нажата
					{																			// Делаем улучшение активным
						ActiveImpr = a;															// Указываем во временной переменной что это улучшение для выделенного объекта стало активным
						GM.Currentkep.ActiveImprs[Kep.ActiveProfile][ActiveObjNomber] = CalculateRNMatAndImpr(a, false); 						// Высчитываем реальный номер улучшения и сохраняем его в постоянной переменной акимвных улучшений



						StartCoroutine(Kep.FillActiveObjTex(Kep.ViewedProfile, ActiveObjNomber, (byte)ActiveCategory));							// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов

						if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && PM.Window == GameMenuWins.Inventory)	// И если мы отрисовываем инвентарь а не магазин
							GM.RestartGameObjects();											// Мы вызываем метод RestartGameObjects
					}
				}
				else if(a != ActiveImpr && a != Selected)										// Иначе если мы отрисовываем (Неактивное и невыбранное) улучшение
				{
					if(GUI.Button(Rects[a], Textures[a], "SelectionSquareButton"))				// И если кнопка отрисовывающая этот материал была нажата
					{
						SelectedImpr = a;														// Досрочно изменяем номер SelectedImpr
						ObjectChangeImprovement(CalculateRNMatAndImpr(a, false));				// Меняем на выбранное улучшение
					}
				}
			}
		}
	}


	void ObjectChangeMaterial(short ObjectNomber, GameObject GO, byte MatNomber)		// Этот метод при своём вызове меняет материал объекта
	{			
		if(ObjectNomber <= 249)															// Если этот объект шайба или бита наносим материал как на шайбу или биту
		{
			GO.GetComponent<Renderer>().material = ObjScr.FirstMaterials[MatNomber];	// Присваиваем объекту новый материал
		}
		else if(ObjectNomber > 249)														// Иначе если этот объект стол значит наносим на него материалы как на стол
		{
			GO.transform.GetChild(2).GetComponent<Renderer>().material = ObjScr.FirstMaterials[MatNomber];			// Присваиваем материал бордюру стола
			GO.transform.GetChild(1).GetComponent<Renderer>().material = ObjScr.SecondMaterials[MatNomber];			// Присваиваем первый материал полю стола
			if(ObjScr.ThirdMaterials.Length > 0 && ObjScr.ThirdMaterials[MatNomber])								// Если длинна третьего массива текстур больше ноля и имеет третий материал с номером MatNomber
			{
				Material[] Matsmass = new Material[2];																// Создаём новый массив материалов длинной два материала
				Matsmass[0] = ObjScr.SecondMaterials[MatNomber];													// Назначаем как первый элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
				Matsmass[1] = ObjScr.ThirdMaterials[MatNomber];														// Назначаем как второй элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
				GO.transform.GetChild(1).GetComponent<Renderer>().materials = Matsmass;								// Присваиваем MatsMats как новые материалы поля
			}
		}													
	}
		

	void ObjectChangeImprovement(byte NomImpr)														// Этот метод при своём вызове меняет улучшение
	{
		if(VideoImprClone)																			// Если до этого у биты уже было выбранное улучшение
		{
			if(ActiveCategory != 2)																	// Если мы отрисовываем не категорию столов
			{
				SelObj.mass -= VideoImprClone.GetComponent<ImprovementScript>().Mass;				// Отнимаем вес улучшения от веса объекта
				ObjScr.Force -= VideoImprClone.GetComponent<ImprovementScript>().Force;				// Отнимаем силу улучшения если она есть от силы объекта если она есть
			}
			Destroy(VideoImprClone);																// Удаляем со сцены это улучшение
		}

		// Помещаем на сцену клон префаба улучшения объекта а сслыку на этот объект отправляем в VideoImprClone
		VideoImprClone = (GameObject)Instantiate(ObjScr.Improvements[NomImpr], VideoObjClone.transform.position, VideoObjClone.transform.rotation);
		VideoImprClone.GetComponent<ImprovementScript>().enabled = true;							// Включаем скрипт улучшения
		VideoImprClone.GetComponent<ImprovementScript>().Pose();

		VideoImprClone.transform.parent = VideoObjClone.transform;									// Прикрепляем к объекту покупаемое улучшение
		VideoImprClone.transform.localScale = new Vector3(1, 1, 1);									// Ставим локальный размер улучшения 1, 1, 1

		if(ActiveCategory != 2) 																	// Если мы отрисовываем не категорию столов
		{
			SelObj.mass += VideoImprClone.GetComponent<ImprovementScript>().Mass;					// Добавляем вес улучшения к весу объекта
			ObjScr.Force += VideoImprClone.GetComponent<ImprovementScript>().Force;					// Прибавляем силу улучшения если она есть к силе объекта если она есть
		}
	}


//	void ObjectChangeBuyedImprovement()																// Этот метод при своём вызове меняет улучшение в инвентаре
//	{
//		for()				// В цикле высчитываем реальный номер выбранного улучшения у этого объекта (а не тот что выбран без учёта купленных и открытых улучшений)
//		{
							// Для этого мы берём реальный номер объекта
//		}

//		if(VideoImprClone)																							// Если до этого у биты уже было выбранное улучшение
//		{
//			SelObj.mass -= VideoImprClone.GetComponent<ImprovementScript>().Mass;									// Отнимаем вес улучшения от веса объекта
//			ObjScr.Force -= VideoImprClone.GetComponent<ImprovementScript>().Force;									// Отнимаем силу улучшения если она есть от силы объекта если она есть
//			DestroyImmediate(VideoImprClone);																		// Удаляем со сцены это улучшение
//		}

		// Помещаем на сцену клон префаба улучшения объекта а сслыку на этот объект отправляем в VideoImprClone
//		VideoImprClone = (GameObject)Instantiate(ObjScr.Improvements[SelectedImpr], VideoObjClone.transform.position, VideoObjClone.transform.rotation);
//		VideoImprClone.GetComponent<ImprovementScript>().enabled = true;											// Включаем скрипт улучшения
//		VideoImprClone.transform.parent = VideoObjClone.transform;													// Прикрепляем к объекту покупаемое улучшение
//		VideoImprClone.transform.localScale = new Vector3(1, 1, 1);													// Ставим локальный размер улучшения 1, 1, 1
//		SelObj.mass += VideoImprClone.GetComponent<ImprovementScript>().Mass;										// Отнимаем вес улучшения от веса объекта
//		ObjScr.Force += VideoImprClone.GetComponent<ImprovementScript>().Force;										// Прибавляем силу улучшения если она есть к силе объекта если она есть
//		StartCoroutine(Kep.FillActiveObjTex(1, Kep.ActiveProfile, ActiveObjNomber, (byte)ActiveCategory));			// Делаем фотографию этого объекта для инвентаря а также обновляем фотографии 4 активных объектов
//	}


	byte CalculateRNMatAndImpr(byte Temporary, bool Material) 								// (Calculate Real Nomber Material And Impreovements) этот метод при своём вызове высчитывает "реальный" номер материала или улучшения на выбранном объекте
	{
		byte Buyed = 0;																		// Сдесь будет храниться очередное найденное купленное улучшение перебираемое в цикле ниже
		byte Length = 0;																	// Длинна массива улучшений или материалов в зависимости от того с чем мы работаем

		if(Material)																		// Если мы расчитываем номер материала выбранного объекта в инвентаре
		{
			
			Length = (byte)ObjScr.FirstMaterials.Length;									// Получаем количество всех материалов у выбранного объекта в инвентаре
			for(byte a=0; a<=Length; a++)													// Для этого проходим по всему массиву доступных материалов для этого объекта
			{
				if(GM.Currentkep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')		// Опрашиваем состояние очередного материала и если оно купленно 
				{
					if(Temporary == Buyed)													// Спаршиваем номер выбранного материала в инвентаре и номер найденного купленного материала соответствуют
						return a; 															// То мы прерываем цикл и возвращаем из метода реальный номер выбранного материала
					else 																	// Иначе если номер выбранного материала в инвентаре и номер найденного купленного материала не соответствуют
						Buyed ++;															// Прибавляем 1 к переменной Comparsion
				}
			}
		}
		else 																				// Иначе если мы высчитываем номер выбранного улучшения в инвентаре
		{
			Length = (byte)ObjScr.Improvements.Length;										// Получаем количество всех улучшений у выбранного объекта в инвентаре
			for(byte a=0; a<=Length; a++)													// Для этого проходим по всему массиву доступных улучшений для этого объекта
			{
				if(GM.Currentkep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')	// Опрашиваем состояние очередного улучшения и если оно купленно 
				{
					if(Temporary == Buyed)													// Спрашиваем номер выбранного улучшения в инвентаре и номер найденного купленного улучшения соответствуют
						return a; 															// То мы прерываем цикл и возвращаем из метода реальный номер выбранного улучшения
					else 																	// Иначе если номер выбранного улучшения в инвентаре и номер найденного купленного улучшения не соответствуют
						Buyed ++;															// Прибавляем 1 к переменной Comparsion
				}
			}
		}
		return 0; 															// В конце метода возвращаем реальный номер выбранного материала или улучшения
	}


	byte CalculateTNMatAndImr(byte Real, bool Material)						// (Calculate Timed Nomber Mat And Impr) этот метод при своём вызове высчитывает "временный" номер материала или улучшения на выбранном объекте
	{
		byte Buyed = 0;														// Сдесь будет храниться очередное найденное купленное улучшение перебираемое в цикле ниже

		if(Material)														// Если мы высчитываем материал
		{
	//		Debug.Log("Номер активного материала " + ActiveMat);
	//		Debug.Log("Номер посылаемого сюда реального номера " + Real);
			for(byte a=0; a<=Real; a++)										// Повторяем цикл до тех пор пока не пройдём количество итераций равных переменной Real
			{
				if(GM.Currentkep.StatesMaterials[Kep.ActiveProfile][ActiveObjNomber][a] == 'B') // Если мы наткунлись на очередной купленый материал в сохранении для этого объекта
				{
					if(Real == Buyed)										// И если "реальный" номер активного материала соответствует перебираемому купленному номеру материала значит мы нашли этот номер
					{
	//					Debug.Log("Номер активного материала " + ActiveMat);
						return Buyed;										// Возвращаем номер купленного материала без учёта открытых и закрытых материалов
					}
					else 													// Иначе если активный (Реальный) номер улучшения ещё не соответствует найденному купленному номеру улучшения
						Buyed++;											// То указываем в переменной Buyed что мы нашли ещё одно купленное улучшение
				}
			}
		}
		else 																			// Иначе если мы высчитываем улучшение
		{
			if(GM.Currentkep.ActiveImprs[Kep.ViewedProfile][ActiveObjNomber] != 10)		// Если у этого объекта есть активное улучшение
			{
				for(byte a=0; a<=Real; a++)										// Повторяем цикл до тех пор пока не пройдём количество итераций равных переменной Real
				{
					if(Kep.StatesImprovements[Kep.ActiveProfile][ActiveObjNomber][a] == 'B')	// Если мы наткунлись на очередной купленное улучшение в сохранении для этого объекта
					{	
						if(Real == Buyed)										// И если реальный номер активного улучшения соответствует перебираемому купленному номеру улучшения значит мы нашли этот номер
						{
							return Buyed;										// Возвращаем номер купленного улучшения без учёта открытых и закрытых улучшений
						}
						else 													// Иначе если активный (Реальный) номер улучшения ещё не соответствует найденному купленному номеру улучшения
							Buyed++;											// То указываем в переменной Buyed что мы нашли ещё одно купленное улучшение
					}
				}
			}
			else  															// Иначе если у него нет активного улучшения
				return 10 ; 												// Возвращаем 10
		}
		return 0;
	}


	void RotationObject()													// Этот метод позволяет вращать объект в магазине мышкой
	{
		if(Button3DObjDown)													// Если кнопка обзора 3D объекта зажата
		{
			if(FirstCall)													// И если это первый вызов метода для этого объекта
			{
				X = VideoObjClone.transform.eulerAngles.x;					// Задаём оси X вращение объекта по оси X
				Z = VideoObjClone.transform.eulerAngles.z;					// Задаём оси Z вращение объекта по оси Z
			} 
			else 															// Иначе если это не первый вызов этого метода для этого объекта
			{
				X += Input.GetAxis("Mouse Y") * Time.deltaTime * 300;		// Отнимаем от переменной X координаты мыши по оси X
				Z -= Input.GetAxis("Mouse X") * Time.deltaTime * 300;		// Прибавляем к переменной Y координаты мыши по оси Y
			}
			VideoObjClone.transform.rotation = Quaternion.Euler(X,0,Z);		// Придаём объекту высчитанное вращение
			FirstCall = false;												// Ставим переменной FirstCall Состояние ложь
		}
	}


	Vector2 RecalculateMousePos()																		// Этот метод пересчитывает позицию мыши для корректного определения прямоугольника позиция и размер которого изменены матрицей и группой
	{
		// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
		// GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
		// GUI.BeginGroup(new Rect(-501, -401.5f, 1002, 803), WindowStore);								// Начинаем группу магазина
		Vector2 Pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);		// Инвертируем позицию мыши по высоте для правильного определения прямоугольника по высоте
		Pos = new Vector2(((Pos.x - Screen.width/2) / RatioH), ((Pos.y - Screen.height/2) / RatioH));	// Учитываем то что позиция курсора ищет квадрат изменённый матрицей (Отсчёт координат от центра по ширине и от центра по высоте)
		Pos = new Vector2(Pos.x + 501, Pos.y + 401);													// Учитываем что курсор выискивает прямоугольник находящийся в группе изменённой матрицей по размеру
		return Pos;																						// Возвращаем позицию	
	}


	void RenderInformWindow()																			// Этот метод отрисовывает окно информации об объектах их материалах и улучшениях
	{	
		float MTAD = 0;																					// MiniTextureAdditionalDistance Дополнительное расстояние для маленького окна информации
		short LabelWidth = 244;
		short LabellHeight = 150;

		if(((RecalculatedMousePos.x + LabelWidth) > 1010 || (RecalculatedMousePos.y + 16 + LabellHeight) > 800))	// Если окошко информации вылазиет с правой стороны или с низу за пределы окна магазина
		{
			GUI.BeginGroup(new Rect(RecalculatedMousePos.x - LabelWidth, RecalculatedMousePos.y - 166, 230, LabellHeight));	// Начинаем группу с лева с верху от курсора
			MTAD = 130; 																									// Присваиваем MWAD
		}
		else  																												// Иначе если оно не вылазиет за пределы магазина
		{
			GUI.BeginGroup(new Rect(RecalculatedMousePos.x, RecalculatedMousePos.y + 16, LabelWidth, LabellHeight));		// Начинаем группу с низу с лева от курсора
		}

		if(ActiveCategory == 0)																		// Если это категория бит
		{
			if(InformCellType == 'O')																// Если курсор завис над клеткой объекта
			{
				GUI.DrawTexture(new Rect(0, 0, LabelWidth, LabellHeight), InformWindow);			// Отрисовываем информационное окно									
				GUI.Label(new Rect(0, 2, 230, 16), InformObjScr.ObjectName, "InformTextHeader"); 	// Отрисвываем название биты
				GUI.Label(new Rect(5, 20, 150, 15), "Вес биты");									// Отрисовываем слова "Вес биты"	
				GUI.Label(new Rect(5, 40, 150, 15), "Сила биты");									// Отрисовываем слова "Сила биты"
				GUI.Label(new Rect(5, 60, 150, 15), "Удельная мощность");							// Отрисовываем слова "Удельная мощность"
				GUI.Label(new Rect(5, 80, 150, 15), "Диаметр биты");								// Отрисовываем слова "Диаметр биты"
				GUI.Label(new Rect(5, 100, 150, 15), "Высота биты");								// Отрисовываем слова "Высота биты"

				if(SelectedCell == Nom)																												// Если выбранная ячейка объекта и та над которой завис курсор одинаковые по номеру
				{		
					GUI.Label(new Rect(165, 20, 40, 15), "" + InformObjScr.Mass);																	// Отрисовываем массу биты без улучшения
					GUI.Label(new Rect(165, 40, 40, 15), "" + InformObjScr.Force);																	// Отрисовываем силу биты без улучшения
					GUI.Label(new Rect(165, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100));							// Отрисовываем удельную мощность биты (Соотношение мощности к массе)
					GUI.Label(new Rect(165, 80, 40, 15), "" + Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100);	// Отрисовываем диаметр биты
					GUI.Label(new Rect(165, 100, 40, 15), "" + Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100);	// Отрисовываем высоту биты
				}
				else																						// Иначе если выбранная ячейка объекта и та над которой завис курсор разные по номеру
				{																							// То обеспечиваем их сравнение
					if(InformObjScr.Mass == ObjScr.Mass)													// Если предмет над которым завис курсор равен по массе выделенному в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "WhiteInformText");	// Отрисовываем вес предмета без улучшения белыми цифрами
					else if(InformObjScr.Mass > ObjScr.Mass)												// Иначе если предмет над которым завис курсор больше по массе выделенного в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "GreenInformText");	// Отрисовываем вес предмета без улучшения зелёными цифрами
					else if(InformObjScr.Mass < ObjScr.Mass)												// Иначе если предмет над которым завис курсор меньше по массе выделенного в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "RedInformText");		// Отрисовываем вес предмета без улучшения красными цифрами

					if(InformObjScr.Force == ObjScr.Force)													// Если бита над которым завис курсор равен по силе выделенному в магазине
						GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "WhiteInformText");	// Отрисовываем силу предмета без улучшения белыми цифрами
					else if(InformObjScr.Force > ObjScr.Force)												// Если бита над которым завис курсор больше по силе выделенного в магазине
						GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "GreenInformText");	// Отрисовываем силу предмета над которой завис курсор без улучшения зелёными цифрами
					else if(InformObjScr.Force < ObjScr.Force)												// Если бита над которым завис курсор меньше по силе выделенного в магазине
						GUI.Label(new Rect(155, 40, 40, 15 ), "" + InformObjScr.Force, "RedInformText");	// Отрисовываем силу предмета без улучшения красными цифрами

					if(InformObjScr.Force/InformObjScr.Mass == ObjScr.Force/ObjScr.Mass)															// Если удельная мощность биты над которой завис курсор равна удельной мощности выделенной в магазине
						GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "WhiteInformText");	// Отрисовываем удельную мощность биты белыми цифрами
					else if(InformObjScr.Force/InformObjScr.Mass > ObjScr.Force/ObjScr.Mass)														// Если удельная мощность биты над которой завис курсор больше удельной мощности выделенной в магазине
						GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "GreenInformText");	// Отрисовываем удельную мощность биты зелёными цифрами
					else if(InformObjScr.Force/InformObjScr.Mass < ObjScr.Force/ObjScr.Mass)														// Если удельная мощность биты над которой завис курсор меньше удельной мощности выделенной в магазине
						GUI.Label(new Rect(155, 60, 40, 15), "" + (Mathf.Round(InformObjScr.Force/InformObjScr.Mass*100)/100), "RedInformText");	// Отрисовываем удельную мощность биты красными цифрами

					GUI.Label(new Rect(155, 80, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100, "WhiteInformText");	// Отрисовываем диаметр предмета белыми цифрами
					GUI.Label(new Rect(155, 100, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100, "WhiteInformText");	// Отрисовываем высоту предмета белыми цифрами

					GUI.Label(new Rect(199, 20, 5, 15), "/");														// Отрисовываем 1 знак "косая черта"
					GUI.Label(new Rect(199, 40, 5, 15), "/");														// Отрисовываем 2 знак "косая черта"
					GUI.Label(new Rect(199, 60, 5, 15), "/");														// Отрисовываем 3 знак "косая черта"
					GUI.Label(new Rect(199, 80, 5, 15), "/");														// Отрисовываем 4 знак "косая черта"
					GUI.Label(new Rect(199, 100, 5, 15), "/");														// Отрисовываем 5 знак "косая черта"

					GUI.Label(new Rect(206, 20, 40, 15 ), "" + ObjScr.Mass);										// Отрисовываем массу сравниваемого объекта белыми цифрами
					GUI.Label(new Rect(206, 40, 40, 15 ), "" + ObjScr.Force);										// Отрисовываем силу сравниваемого объекта белыми цифрами
					GUI.Label(new Rect(206, 60, 40, 15 ), "" + (Mathf.Round(ObjScr.Force/ObjScr.Mass*100)/100));	// Отрисовываем удельную мощность сравниваемого объекта белым цветом
					GUI.Label(new Rect(206, 80, 40, 15 ), "" + Mathf.Round(SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.x*100)/100);// Отрисовываем диаметр биты сравниваемого объекта белыми цифрами
					GUI.Label(new Rect(206, 100, 40, 15 ), "" + Mathf.Round(SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.y*100)/100);// Отрисовываем высоту биты сравниваемого объекта белыми цифрами
				}
			}
			else if(InformCellType == 'M')																// Иначе если курсор завис над клеткой материала 
			{
				GUI.DrawTexture(new Rect(0, 0 + MTAD, 230, 20), InformWindow);							// Отрисовываем информационное окно	
				if(StateMats[Nom] != 'C')																// Проверяем если материал над которым завис курсор не закрыт
				{
					GUI.Label(new Rect(0, 2 + MTAD, 230, 16), InformMatName, "InformTextHeader");		// Отрисвываем название материала
				}
				else 																					// Иначе если материал над которым завис курсор закрыт
				{
					GUI.Label(new Rect(0, 2 + MTAD, 230, 16), "Информация закрыта", "InformTextHeader");// Отрисвываем что информация о материале закрыта
				}
			}
			else if(InformCellType == 'I')																// Иначе если курсор завис над клеткой улучшения
			{
				if(StateImprs[Nom] != 'C')																// Проверяем если улучшение над которым завис курсор не закрыто
				{
					GUI.DrawTexture(new Rect(0, 0, 230, 150), InformWindow);							// Отрисовываем информационное окно
					GUI.Label(new Rect(0, 2, 230, 16), InformImprName, "InformTextHeader");				// Отрисовываем название улучшения
					GUI.Label(new Rect(5, 20, 150, 15), "Тип улучшения");								// Отрисовываем слова "Тип улучшения"	
					GUI.Label(new Rect(5, 40, 150, 15), "Вес  улучшения");								// Отрисовываем слова "Вес улучшения"

					GUI.Label(new Rect(130, 20, 150, 15), "" + InformImprType); 						// Отрисовываем тип улучшения														
					GUI.Label(new Rect(130, 40, 150, 15), "" + InformImprMass);							// Отрисовываем вес улучшения

					if(InformImprScr.TypeImprovement == TypeImpr.Backlight_B || InformImprScr.TypeImprovement ==  TypeImpr.Illuminator_B)	// Если тип улучшения Подсветка, Осветитель		
					{
						GUI.Label(new Rect(5, 60, 150, 15), "Цвет  улучшения");							// Отрисовываем слова "Цвет улучшения"
						GUI.Label(new Rect(130, 55, 32, 32), ColorImprTexture);							// Отрисовываем цвет улучшения	
					}
					else 																				// Иначе если тип улучшения Ускоритель, Замедлитель, Притягатель, Отталкиватель
					{
						GUI.Label(new Rect(5, 60, 150, 15), "Сила улучшения");							// Отрисовываем слова "Сила улучшения"				
						GUI.Label(new Rect(130, 60, 150, 15), "" + InformImprScr.Force);				// Отрисовываем его силу
					}

				}
				else
				{
					GUI.Label(new Rect(0, 2 + MTAD, 230, 16), "Информация закрыта", "InformTextHeader");// Отрисвываем что информация об улучшении закрыта
				}
			}
		}
		else if(ActiveCategory == 1)																// Если это категория шайб
		{
			if(InformCellType == 'O')																// Если курсор завис над клеткой объекта
			{
				GUI.DrawTexture(new Rect(0, 0, LabelWidth, LabellHeight), InformWindow);			// Отрисовываем информационное окно									
				GUI.Label(new Rect(0, 2, 230, 16), InformObjScr.ObjectName, "InformTextHeader"); 	// Отрисвываем название биты
				GUI.Label(new Rect(5, 20, 150, 15), "Вес шайбы");									// Отрисовываем слова "Вес шайбы"	
				GUI.Label(new Rect(5, 40, 150, 15), "Диаметр шайбы");								// Отрисовываем слова "Диаметр шайбы"
				GUI.Label(new Rect(5, 60, 150, 15), "Высота шайбы");								// Отрисовываем слова "Высота шайбы"

				if(SelectedCell == Nom)																												// Если выбранная ячейка шайбы и та над которой завис курсор одинаковые по номеру
				{								
					GUI.Label(new Rect(155, 20, 30, 15),"" + InformObjScr.Mass);																	// Отрисовываем массу шайбы без улучшения
					GUI.Label(new Rect(155, 40, 30, 15),"" + Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100);		// Отрисовываем диаметр шайбы
					GUI.Label(new Rect(155, 60, 30, 15),"" + Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100);		// Отрисовываем высоту шайбы
				}
				else																						// Иначе если выбранная ячейка объекта и та над которой завис курсор разные по номеру
				{																							// То обеспечиваем их сравнение
					if(InformObjScr.Mass == ObjScr.Mass)													// Если предмет над которым завис курсор равен по массе выделенному в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "WhiteInformText");	// Отрисовываем вес шайбы без улучшения белыми цифрами
					else if(InformObjScr.Mass > ObjScr.Mass)												// Иначе если предмет над которым завис курсор больше по массе выделенного в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "GreenInformText");	// Отрисовываем вес шайбы без улучшения зелёными цифрами
					else if(InformObjScr.Mass < ObjScr.Mass)												// Иначе если предмет над которым завис курсор меньше по массе выделенного в магазине
						GUI.Label(new Rect(155, 20, 40, 15 ), "" + InformObjScr.Mass, "RedInformText");		// Отрисовываем вес шайбы без улучшения красными цифрами

					GUI.Label(new Rect(155, 40, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.x * 100) / 100, "WhiteInformText");	// Отрисовываем диаметр предмета белыми цифрами
					GUI.Label(new Rect(155, 60, 40, 15), "" +  Mathf.Round(InformObjScr.GetComponent<MeshRenderer>().bounds.size.y * 100) / 100, "WhiteInformText");	// Отрисовываем высоту предмета белыми цифрами

					GUI.Label(new Rect(199, 20, 5, 15), "/");												// Отрисовываем 1 знак "косая черта"
					GUI.Label(new Rect(199, 40, 5, 15), "/");												// Отрисовываем 2 знак "косая черта"
					GUI.Label(new Rect(199, 60, 5, 15), "/");												// Отрисовываем 3 знак "косая черта"

					GUI.Label(new Rect(206, 20, 30, 15 ), "" + ObjScr.Mass);								// Отрисовываем массу сравниваемого объекта белыми цифрами
					GUI.Label(new Rect(206, 40, 30, 15 ), "" + Mathf.Round(SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.x*100)/100);// Отрисовываем диаметр биты сравниваемого объекта белыми цифрами
					GUI.Label(new Rect(206, 60, 30, 15 ), "" + Mathf.Round(SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().GetComponent<MeshRenderer>().bounds.size.y*100)/100);// Отрисовываем высоту биты сравниваемого объекта белыми цифрами
				}
			}
			else if(InformCellType == 'M')																	// Иначе если курсор завис над клеткой материала 
			{
				GUI.DrawTexture(new Rect(0, 0 + MTAD, 230, 20), InformWindow);								// Отрисовываем информационное окно	
				if(StateMats[Nom] != 'C')																	// Проверяем если материал над которым завис курсор не закрыт
				{
					GUI.Label(new Rect(0, 2 + MTAD, 230, 16), InformMatName, "InformTextHeader");			// Отрисвываем название материала
				}
				else 																						// Иначе если материал над которым завис курсор закрыт
				{
					GUI.Label(new Rect(0, 2 + MTAD, 230, 16), "Информация закрыта", "InformTextHeader");	// Отрисвываем что информация о материале закрыта
				}
			}
		}
		GUI.EndGroup();
	}


	void CursorDelayTimer()											// Этот метод отсчитывает определённое время задержки курсора над ячейкой магазина
	{	
		bool Coincidence = false;									// Переменная говорящая обнаружилось ли совпадение позиции курсора и какого нибуть прямоугольника в магазине
		Rect NowRect = new Rect(0,0,0,0);							// Квадрат на котором стрелка в данный момент
		short StoreSlot = 0;										// Номер объекта над которым завис курсор в массиве ObjectsStore

		for(byte a=0; a<PosIconObj.Length; a++)						// Проходим первый массив (Массив объектов)
		{
			if(PosIconObj[a].Contains(RecalculatedMousePos))		// Узнаём находиться ли курсор над прямоугольником объекта этого цикла и если находиться
			{
				if(ObjsStates[a] != 0 & ObjsStates[a] != 'C')		// Спрашиваем этот объект существует? Этот объект не закрыт? и если ответ на оба этих вопроса да
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconObj[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'O';							// Ставим что тип клетки над которой завис курсор "Объект"
					break;											// Прерываем цикл
				}
				break;												// Иначе если объект не существует или закрыт прерываем цикл						
			}
		}
		if(Coincidence == false)									// Если после проверки массива объектов совпадения не нашлось
		{
			for(byte a=0; a<Mats.Count; a++)						// Проходим второй массив (Массив материалов)
			{
				if(PosIconMats[a].Contains(RecalculatedMousePos))	// Узнаём находиться ли курсор над прямоугольником материала этого цикла и если находиться
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconMats[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'M';							// Ставим что тип клетки над которым завис курсор "Материал"
					break;											// Прерываем цикл
				}	
			}
		}
		if(Coincidence == false)									// Если после проверки массива материалов совпадения не нашлось
		{
			for(byte a=0; a<Imprvs.Count; a++)						// Проходим третий масси массив (Улучшений)
			{
				if(PosIconImprs[a].Contains(RecalculatedMousePos))	// Узнаём находиться ли курсор над прямоугольником улучшения этого цикла и если находиться
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconImprs[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'I';							// Ставим что тип клетки над которым завис курсор "Улучшение"
					break;											// Прерываем цикл
				}
			}
		}

		if(Coincidence & CheckedRect == NowRect)					// Если мы нашли какой либо квадрат над которым отрисовываеться курсор и это тот же самый квадрат что и раньше
		{
			CursorDelay += Time.deltaTime;							// Прибавляем ко времени задержки время отрисовки текущего кадра
			if(CursorDelay > 1)										// Если курсор удерживаеться на этой клетке более 1х секунды
			{
				if(ActiveCategory < 3)								// Если мы работаем с категорией объектов
				{
					if(InformCellType == 'O')															// И если это объект а не материал или улучшение
					{
						StoreSlot = (byte)((ActiveCategory* 125) + (ActiveLvl * 25) + Nom);				// Высчитываем каким будет по счёту ячейка над которой завис курсор объект в массиве ObjectsStore
//						InformObj = SC.ObjectsStore[StoreSlot].GetComponent<Rigidbody>();				// Получаем Rigitbody того объекта над которым завис курсор
						InformObjScr = SC.ObjectsStore[StoreSlot].GetComponent<ObjectScript>();			// Получаем ObjctScript того объекта над которым завис курсор
					}
					else if(InformCellType == 'M')														// Иначе если это материал
					{
						InformMatName = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesMaterials[Nom];			// Ложим имя материала над которым завис курсор в переменную InformMatName					
					}
					else if(InformCellType == 'I')														// Иначе если это улучшение
					{
						InformImprScr = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().Improvements[Nom].GetComponent<ImprovementScript>();			// Получаем из выбранного объекта номер улушения над которым завис курсор
						InformImprName = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesImprovements[Nom];										// Ложим имя улучшения над которым завис курсор в переменную InformMatName	
						InformImprType = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesTypesImprovements[Nom];									// Ложим имя типа улучшения над которым завис курсор в переменную InformImprType
						InformImprMass = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().Improvements[Nom].GetComponent<ImprovementScript>().Mass;	// Ложим массу типа улучшения над которым завис курсор в переменную InformImprType

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
					}
				}
				ShowInformLabel = true;									// Ставим переменной (Показывать информационную метку) состояние правда
			}
		}
		else															// Если совпадения так и не случилось либо просматриваемый квадрат изменилься
		{	
			CheckedRect = NowRect;										// Присваиваем проверяемому прямоугольнику прямоугольник над которым находиться сейчас курсор
			CursorDelay = 0;											// Обнуляем переменную накопленного времени задержки курсора над ячейкой CursorDelay
			ShowInformLabel = false;									// Ставим переменной (Показывать информационную метку) состояние ложь
		}
	}


	void InventoryCursorDelayTimer()								// Этот метод отсчитывает определённое время задержки курсора над ячейкой инвентаря
	{
		bool Coincidence = false;									// Переменная говорящая обнаружилось ли совпадение позиции курсора и какого нибуть прямоугольника в магазине
		Rect NowRect = new Rect(0,0,0,0);							// Квадрат на котором стрелка в данный момент
		short StoreSlot = 0;										// Номер объекта над которым завис курсор в массиве ObjectsStore

		for(byte a=0; a<PosIconObj.Length; a++)						// Проходим первый массив (Массив объектов)
		{
			if(PosIconObj[a].Contains(RecalculatedMousePos))		// Узнаём находиться ли курсор над прямоугольником объекта этого цикла и если находиться
			{
				if(ObjsTexs[a].name != "NotExistElement")			// Спрашиваем имя текстуры в переменной массива ObjsTexs с номером цикла не равно NotExistElement
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconObj[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'O';							// Ставим что тип клетки над которой завис курсор "Объект"
					break;											// Прерываем цикл
				}
			}
		}
		if(Coincidence == false)									// Если после проверки массива объектов совпадения не нашлось
		{
			for(byte a=0; a<Mats.Count; a++)						// Проходим второй массив (Массив материалов)
			{
				if(PosIconMats[a].Contains(RecalculatedMousePos))	// Узнаём находиться ли курсор над прямоугольником материала этого цикла и если находиться
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconMats[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'M';							// Ставим что тип клетки над которым завис курсор "Материал"
					break;											// Прерываем цикл
				}	
			}
		}
		if(Coincidence == false)									// Если после проверки массива материалов совпадения не нашлось
		{
			for(byte a=0; a<Imprvs.Count; a++)						// Проходим третий масси массив (Улучшений)
			{
				if(PosIconImprs[a].Contains(RecalculatedMousePos))	// Узнаём находиться ли курсор над прямоугольником улучшения этого цикла и если находиться
				{
					Coincidence = true;								// Ставим что совпадение случилось
					NowRect = PosIconImprs[a];						// Присваиваем NowRect прямоугольник над которым находитья курсор
					Nom = a;										// Запоминаем номер ячейки над которой завис курсор
					InformCellType = 'I';							// Ставим что тип клетки над которым завис курсор "Улучшение"
					break;											// Прерываем цикл
				}
			}
		}

		if(Coincidence & CheckedRect == NowRect)					// Если мы нашли какой либо квадрат над которым отрисовываеться курсор и это тотже самый квадрат что и раньше
		{
			CursorDelay += Time.deltaTime;							// Прибавляем ко времени задержки время отрисовки текущего кадра
			if(CursorDelay > 1)										// Если курсор удерживаеться на этой клетке более 3х секунд
			{
				if(ActiveCategory < 3)								// Если мы работаем с категорией объектов
				{
					if(InformCellType == 'O')													// И если это объект а не материал или улучшение
					{
						short FirstEl = (short)((ActiveCategory*125)+(ActiveLvl*25));			// Номер элемента в массиве ObjectsStore соответствующий первой ячейке инвентаря просматриваемой категории и уровня
						short ObjEl = FirstEl;													// Опрашиваемый элемент массива ObjectsStore

						byte BuyedObject = 0;													// Номер найденного купленного объекта в массиве ObjectsStore соответствующий ячейке просматриваемой категории и уровня
						// Определяем номер выбранной ячейки в массиве ObjectsStore
						if(ObjEl <= 374)														// Если мы работаем с категорией объектов в инвентаре а не скабоксами
						{	
							for(byte a=0; a < ObjsTexs.Length; a++)								// Продолжаем цикл до тех пор пока не переберём все 25 объектов в массиве ObjectsStore соответствующих 25 ячеекам в инвентаре для данной категории и уровня
							{
								if(SC.ObjectsStore[ObjEl] != null)								// Если выбранный объект в массиве существует
								{
									if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] != 0)		// Если сохранение для этого объекта существует
									{
										if(Kep.ObjectsStates[Kep.ActiveProfile][ObjEl] == 'B')	// Если этот объект куплен
										{
											if(BuyedObject == Nom)								// Если номер выбранной ячейки "SelectedCell" соответствует очередному найденному купленному объекту "BuyedObject" 
											{
												StoreSlot = (short)(FirstEl + a);				// Прибавляем к FirsObjEl номер цикла (Вычисляя таким образом номер этого объекта в массиве ObjStore) и присваиваем переменной ObjEl
												break;											// И заканчиваем цикл
											}
											else 												// Иначе если номер выбранной ячейки "SelectedCell" не соответствует очередному найденному купленному объекту "BuyedObject" 
											{
												BuyedObject ++;									// Прибавляем "BuyedObject" +1 чтобы засчитать очередной найденный купленный объект
											}
										}
									}
								}
								ObjEl++;														// Прибавляем к переменной (ObjEl) 1 чтобы в следующем цикле опросить следующий элемент массива ObjectStore
							}
						}
						else 																	// Иначе если мы работаем с категорией скайбоксов в инвентаре
						{

						}
						InformObjScr = SC.ObjectsStore[StoreSlot].GetComponent<ObjectScript>();			// Получаем ObjctScript того объекта над которым завис курсор
					}
					else if(InformCellType == 'M')														// Иначе если это материал
					{
						InformMatName = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesMaterials[Nom];			// Ложим имя материала над которым завис курсор в переменную InformMatName					
					}
					else if(InformCellType == 'I')														// Иначе если это улучшение
					{
						InformImprScr = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().Improvements[Nom].GetComponent<ImprovementScript>();			// Получаем из выбранного объекта номер улушения над которым завис курсор
						InformImprName = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesImprovements[Nom];										// Ложим имя улучшения над которым завис курсор в переменную InformMatName	
						InformImprType = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().NamesTypesImprovements[Nom];									// Ложим имя типа улучшения над которым завис курсор в переменную InformImprType
						InformImprMass = SC.ObjectsStore[ActiveObjNomber].GetComponent<ObjectScript>().Improvements[Nom].GetComponent<ImprovementScript>().Mass;	// Ложим массу типа улучшения над которым завис курсор в переменную InformImprType

						if(InformImprScr.TypeImprovement == TypeImpr.Backlight_B || InformImprScr.TypeImprovement ==  TypeImpr.Illuminator_B)						// Если тип улучшения Подсветка, Осветитель		
						{
							Color32[] ColorMass = new Color32[400];		// Создаём масив цветов для текстуры цвета улучшения
							for(short a=0; a<ColorMass.Length; a++)		// Продолжаем цикл пока не заполним массив цветов
							{
								ColorMass[a] = InformImprScr.GetComponentInChildren<Light>().color;		// Заполняем массив ColorMass цветом из переменной источника освещения
							}
							ColorImprTexture = new Texture2D(20, 20);	// Создаём текстуру отображающую цвет освещения
							ColorImprTexture.SetPixels32(ColorMass);	// Заполняем текстуру цветом из массива цветов ColorMass
							ColorImprTexture.Apply(false);				// Применяем текстуру
						}
						else  											// Иначе если тип улучшения Ускоритель, Замедлитель, Притягатель, Отталкиватель
						{

						}
					}
				}
				ShowInformLabel = true;									// Ставим переменной (Показывать информационную метку) состояние правда
			}
		}
		else															// Если совпадения так и не случилось либо просматриваемый квадрат изменилься
		{	
			CheckedRect = NowRect;										// Присваиваем проверяемому прямоугольнику прямоугольник над которым находиться сейчас курсор
			CursorDelay = 0;											// Обнуляем переменную накопленного времени задержки курсора над ячейкой CursorDelay
			ShowInformLabel = false;									// Ставим переменной (Показывать информационную метку) состояние ложь
		}
	}
}
