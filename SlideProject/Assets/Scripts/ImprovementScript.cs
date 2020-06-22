// Скритп улучшения запускаеться выключенным
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Можно добавить ещё один тип улучшения Это резиновые борта для биты, для увеличения её размера
// Создаём перечисление типы улучшения для бит (Подсветка, Осветитель, Ускоритель, Замедлитель, Притягатель, Отталкиватель). Типы для шайб(). Типы для столов (Врата).
public enum TypeImpr : byte {Backlight_B, Illuminator_B, Accelerator_B, Retarder_B, Magnet_B, Pusher_B, Gate_T}		
public class ImprovementScript : MonoBehaviour 
{
	public TypeImpr TypeImprovement;			// Сдесь будет лежать тип улучшения
	public float Mass;							// Масса улучшения
	public float Force;							// Сила улучшения (Имеет разный смысл в зависимости от типа улучшения)
	Color32 Color;								// Цвет если тип улучшения Подсветка или осветитель
	public float TimeOfAction;					// Полное время действия улучшения
	public float RegSpeed;						// (Regeneration Speed)Скорость восстановления энергии
	float TimeLeft = 0;							// Оставшееся время действия улучшения
	public Rigidbody ObjRigb;					// Сдесь будет лежать Rigidbody объекта на котором висит улучшение
	public Texture2D ImprovementWeight;			// Иконка веса улучшения
	public Texture2D AdditionalProperty;		// Иконка силы улучшения, или его цвета
	public Texture2D ColorTex;					// Сдесь будет текстура отображающуая цвет освещения
	public Texture2D[] BackgrBand;				// Сдесь лежат задний фон для полосок улучшений для двух сторон (Для данного типа и уровня улучшения)
	public Texture2D[] BandTexsR;				// Сдесь лежат полоски улучшений для правой стороны (Для данного типа и уровня улучшения)
	public Texture2D[] BandTexsL;				// Сдесь лежат полоски улучшений для левой стороны (Для данного типа улучшения)
	public bool RSide = false;					// Сдесь находиться утверждение это улучшение весит на игроке который закреплён с права или с лева	
	char Side = '0';							// Эта переменная указывает сторону на которой разместяться иконки улучшения
	GameManager GM;								// Переменная для скрипта GameManager
	MainMenu MM;								// Сдесь будет лежать скрипт главного меню 
	PauseMenu PM;								// Сдесь будет лежать скрипт меню паузы
	StoreAndInventory SAI;						// Сдесь будет лежать скрипт Магазин и инвентарь
	public PlayerController PC;					// Сдесь будет лежать скрипт PlayerController того игрока на котором висит этот скрипт улушения
	Color32[] ColorMass = new Color32[400];		// Цвет если тип улучшения Подсветка или осветитель
	GUISkin GameSkin;							// Скин для всех гуи элементов
	float RatioH;								// Сюда заноситься результат деления оригинальной высоты экрана на текущую
	float OriginalHeight = 1080; 				// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	Vector3[] Accelerators;						// Массив векторов ускорителей на бите без учёта высоты
	float Height = 0;							// Сдесь будет лежать высота от пола до источника частиц ускорителя
	Vector3 SVP;								// (StartVectorPosition) Точка с помощью которой мы будем высчитывать вектора двигателей
	short SceneNomber;							// Сдесь будет храниться номер сцены
	Image Energy;								// Переменная для изображения энергии улучшения данного игрока	
	string GUIAdress = "Texs&Mats/GUIElements/SingleElements/";	// Основной адрес папки с загружаемыми гуи текстурами
	bool Posing = false;						// Эта переменная говорит позирует ли объект для фото и видео или нет
//	bool PosingSwitch = true;					// Переменная для первого вызова переключателя


	void Start()								// Запускаем при Start а не при Awake так как он не отключаеться вместе со скриптом
	{
		RatioH = Screen.height / OriginalHeight;													// Заносим в ScreenBalansHight результат деления описанный выше
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();							// Получаем компонент GameManager и ложим его в переменную GM
		SAI = GameObject.Find("IndestructibleObject").GetComponent<StoreAndInventory>();			// Получаем скрипт StoreAndInventory и помещаем его в переменную SAI
		SceneNomber = (short)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;	// Заносим номер сцены в переменную SceneNomber
		GM.StartGameEvent += StartGame;																// Подписываем метод "Старт игры" на событие StartGameEvent
		GM.GoalEvent += Goal;																		// Подписываем метод гол на событие GoalEvent
		GM.LastGoalEvent += LastGoal;																// Подписываем метод LastGoal на событие LastGoalEvent
		if(SceneNomber == 0)																		// Если мы работаем со сценой главного меню
		{
			MM = GameObject.Find("GameManager").GetComponent<MainMenu>();							// Находим на сцене GameManager берём с него скрипт MainMenu и ложим в переменную MM
			GameSkin = MM.GameSkin;																	// Присваиваем местной переменной GameSkin Значение GameSkin из переменной скрипта главное меню
		}
		else 																						// Иначе если мы работаем с какой либо другой сценой
		{
			PM = GameObject.Find("GameManager").GetComponent<PauseMenu>();							// Находим скрипт меню паузы и ложим его в переменную PM
			GameSkin = PM.GameSkin;																	// Берём GameSkin из PasuMenu
			PC = GetComponentInParent<PlayerController>();											// Получаем скрип PlayerController этого игрока и ложим его в переменную PC
		}
		if(TypeImprovement == TypeImpr.Backlight_B || TypeImprovement == TypeImpr.Illuminator_B)	// Если тип улучшнения Подсветка или Осветитель
		{
			ColorFilling();																			// Вызываем метод создания текстуры
		}
		else if(TypeImprovement == TypeImpr.Accelerator_B)											// Если тип этого улучшения "Ускоритель"
		{
			CalculationAcceleratorsVectors();														// То мы производим первоначальный просчёт сколько двигателей у биты и куда они направленны
		}
	}


	void StartGame()																				// Этот метод вызываеться при нажатии кнопки старт
	{
		if(RSide) Side = 'R';																		// Если этот игрок играет с права cтавим Side значение R																				
		else Side = 'L';																			// Иначе если игрок играет с лева ставим Side значение L
																			
		if(TypeImprovement == TypeImpr.Backlight_B || TypeImprovement == TypeImpr.Illuminator_B) 		// Если тип улучшнения Подсветка или Осветитель
		{
			PM.PlayersInform.transform.Find("BackgroundIlluminatorIcon_" + Side).gameObject.SetActive(true);		// Делаем видимым задний фон иконки осветителя
			PM.PlayersInform.transform.Find("IlluminatorIcon_" + Side).gameObject.SetActive(true);					// Делаем видимой иконку осветителя
			PC.ImprActive = true;																						// Включаем улучшение
		}
		else if(TypeImprovement == TypeImpr.Accelerator_B)																// Если тип этого улучшения "Ускоритель"
		{
			PM.PlayersInform.transform.Find("AcceleratorTimeBackgroundBand_" + Side).gameObject.SetActive(true); 	// Делаем видимым задний фон полоски ускорителя с права	
			PM.PlayersInform.transform.Find("AcceleratorBand_" + Side).gameObject.SetActive(true);					// Делаем видимой полоску ускорителя с права
			Energy = PM.PlayersInform.transform.Find("AcceleratorBand_" + Side).GetComponent<Image>();				// Ложим изображение полоски игрока в переменную Energy
			Energy.fillAmount = 0;																						// Присваиваем значению полоски энергии на экране значение ноль
		}
	}


	void Goal()		// Этот метод вызываеться один раз во время гола
	{
		if(TypeImprovement == TypeImpr.Accelerator_B)						// Если тип этого улучшения "Ускоритель"
			if(PC.ImprActive) PC.ImprActive = false;						// Если улучшение было включенно то выключаем его
	}


	void LastGoal()															// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		if(TypeImprovement == TypeImpr.Accelerator_B)						// Если тип этого улучшения "Ускоритель"
			if(PC.ImprActive) PC.ImprActive = false;						// Если улучшение было включенно то выключаем его
	}


	void Update() 
	{
		if(GM.RoundStarted)																				// Если раунд началься
		{
			if(TypeImprovement == TypeImpr.Backlight_B || TypeImprovement == TypeImpr.Illuminator_B)	// Если тип улучшнения Подсветка или Осветитель
			{
				Illuminator();												// Вызываем метод Осветитель					
			}
			else if(TypeImprovement == TypeImpr.Accelerator_B)				// Если тип этого улучшения "Ускоритель"
			{
				Accelerator();												// То вызываем метод ускоритель
			}
		} 
	//	else if(!GM.RoundStarted && PosingSwitch)							// Иначе если раунд не началься а переменная PosingSwitch равна правда
	//	{
	//		PosingSwitch = false;											// Выключаем возможность повторно вызвать эту ветку
	//		Pose(); 														// Вызываем метод PosingSwitch
	//	}
	}

			
	void OnGUI()																							// Этот метод мы используем чтобы отрисовывать в магазине и инвентаре дополнительную информацию о надетом улучшении
	{
		GUI.skin = GameSkin;																				// Устанавливаем гуи скин
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH, RatioH,1));	// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с середины экрана
		GUI.BeginGroup(new Rect(-501, -401.5f, 1002, 803));													// Начинаем группу магазина

		if(SceneNomber == 0 && MM.Window == MainMenuWins.Store || SceneNomber != 0 && PM.Window == GameMenuWins.Inventory)	// Если мы работаем с меню магазина или меню инвентаря
		{
			if(SAI.ActiveCategory != 2) 																	// Если мы отрисовываем не категорию столов
			{
				GUI.DrawTexture(new Rect(837, 426, 32, 32), ImprovementWeight);								// Отрисовываем иконку массы улучшения
				GUI.Label(new Rect(869, 427, 32, 32), "" + Mass, "StoreInformText");						// Отрисовываем массу предмета
				GUI.DrawTexture(new Rect(926, 426, 32, 32), AdditionalProperty);							// Отрисовываем иконку дополнительного свойства
			}
			else 																							// Иначе если мы отрисовываем категорию столов
			{
				GUI.DrawTexture(new Rect(926, 426, 32, 32), AdditionalProperty);							// Отрисовываем иконку дополнительного свойства
			}


			if(TypeImprovement == TypeImpr.Accelerator_B || TypeImprovement == TypeImpr.Retarder_B)			// Если мы работаем с улучшением ускоритель или замедлитель
				GUI.Label(new Rect(958, 426, 32, 32), "" + Force, "StoreInformText");						// То отрисовываем силу улучшения
			else if(TypeImprovement == TypeImpr.Backlight_B || TypeImprovement == TypeImpr.Illuminator_B)	// Иначе если тип улучшения подсветка или осветитель
				GUI.Label(new Rect(964, 426, 32, 32), ColorTex);											// Отрисовываем цвет улучшения			
		}
		GUI.EndGroup();
	}


	void Illuminator()				// Этот метод работает с улучшением подстветки и осветителя
	{
	 	if(PC.ImprActive)										// И если улучшение включенно
		{
			GetComponentInChildren<Light>().enabled = true;		// Ставим сотостояние источника освещения как включен
			Texture2D Light = Resources.Load(GUIAdress + "ImprovementsTexs/ActivatedIlluminatorIcon") as Texture2D;	// Загружаем из файлов текстуру обозначающую что улучшение включенно и ложим ей в переменную Light
			PM.PlayersInform.transform.Find("IlluminatorIcon_" + Side).GetComponent<Image>().sprite = Sprite.Create(Light, new Rect(0, 0, 176, 33), new Vector2(0, 0)); // Создаём из спрайт из переменной Light и ставим его иконке улучшения игрока
		}
		else 													// Иначе если улучшение выключенно
		{
			GetComponentInChildren<Light>().enabled = false;	// Ставим сотостояние источника освещения как выключен
			Texture2D Light = Resources.Load(GUIAdress + "ImprovementsTexs/DeactivatedIlluminatorIcon") as Texture2D;	// Загружаем из файлов текстуру обозначающую что улучшение выключенно и ложим ей в переменную Light
			PM.PlayersInform.transform.Find("IlluminatorIcon_" + Side).GetComponent<Image>().sprite = Sprite.Create(Light, new Rect(0, 0, 176, 33), new Vector2(0, 0)); // Создаём из спрайт из переменной Light и ставим его иконке улучшения игрока
		}
	}


	void Accelerator()				// Этот метод работает с улучшением ускорителя
	{ 
		if(PC.ImprActive) 			// Если переменная PC.ImprovementActivate указывает что улучшение активно
		{			
			ObjRigb.AddForce(PC.Dir * Force * 1200 * Time.deltaTime);					// Добавляем силу указанную в улучшении в направлении движения биты

			for (byte a = 0; a < Accelerators.Length; a++) 								// Продолжаем цикл пока не пройдём весь массив векторов Accelerators
			{								
				float Result = Vector3.Dot(PC.Dir, Accelerators[a]);					// Производим сравнение векторов. Вектор движения биты и вектор расположения двигателя
				if(Result < -0.1f) transform.GetChild(a).gameObject.SetActive(true);	// Если сравнение векторов показало что частицы для двигателя этого цикла нужно включить то включаем их
				else transform.GetChild(a).gameObject.SetActive(false);					// Иначе если сравнение показало что частицы для двигателя этого цикла нужно выключить то выключаем их
			}

			TimeLeft -= (1*Time.deltaTime);												// То отнимаем 1 умноженное на время для этого кадра
			Energy.fillAmount = TimeLeft/TimeOfAction;									// Обновляем значение полоски энергии на экране для данного игрока

			if(TimeLeft <= 0) PC.ImprActive = false;									// И если оставшееся время действия меньше или равно 0 ставим что улучшение не активно
		}
		else if(!PC.ImprActive)															// Иначе если улучшение не активно и игра не стоит на паузе
		{
			if(!GM.Pause)																// Если игра не находиться в режиме паузы
			{
				if(TimeLeft<TimeOfAction) TimeLeft += (RegSpeed * Time.deltaTime);		// И если оставшееся время меньше чем время действия прибавляем к оставшемуся времени действия
				Energy.fillAmount = TimeLeft/TimeOfAction;								// Обновляем значение полоски энергии на экране для данного игрока
			}
			for (byte a = 0; a < Accelerators.Length; a++)								// Продолжаем цикл пока не пройдём весь массив векторов Accelerators
			{ 			
				transform.GetChild (a).gameObject.SetActive (false);					// Выключаем поочерёдно все излучатели частиц
			}
		}
	}


	void ColorFilling()			// Этот метод создаёт текстуру улучшения осветителя и заполняет её цветом 
	{					
		Color = GetComponentInChildren<Light>().color;								// Задаём цвет переменной Color
		for(short a=0; a<ColorMass.Length; a++)										// Продолжаем цикл пока не заполним массив цветов
		{
			ColorMass[a] = Color;													// Заполняем массив ColorMass цветом из переменной Color
		}
		ColorTex = new Texture2D(20, 20);											// Создаём текстуру отображающую цвет освещения
		ColorTex.SetPixels32(ColorMass);											// Заполняем текстуру цветом из массива цветов ColorMass
		ColorTex.Apply(false);														// Применяем текстуру
	}



	void CalculationAcceleratorsVectors()	// Этот метод производит первоначальный расчёт сколько двигателей у биты и куда они направленны
	{
		if(!Posing)							// Спрашиваем если этот объект не позирует
		{
			Height = transform.GetChild (0).transform.position.y;						// Вычесляем высоту первого дочернего объекта чтобы потом проводить прямые вектора по высоте
			SVP = transform.localPosition + new Vector3 (0, Height, 0);					// (StartVectorPosition) Точка с помощью которой мы будем высчитывать вектора двигателей

			Accelerators = new Vector3[transform.childCount];							// Задаём размер массиву Accelerators
			for (byte a = 0; a < transform.childCount; a++) 							// Делаем цикл который будет работать пока мы не переберём всех дочерних объектов этого улучшения
			{							
				Accelerators[a] = new Vector3 (transform.GetChild (a).transform.localPosition.x, 0, transform.GetChild (a).transform.localPosition.z);	// Сохраняем в очередной элемент массива Accelerators 3х мерные координаты двигателя																			
				transform.GetChild (a).gameObject.SetActive (false);					// Отключаем излучатель частиц соответствующий этому циклу
			}
		}
	}


	public void Pose()	// Режим позирования, этот метод вызываеться когда делаеться фотография или снимаеться видео с улучшением и улучшение должно быть включенно и выглядеть как можно более красиво
	{
		Posing = true;	// Говорим что объект позирует
		if(TypeImprovement == TypeImpr.Backlight_B || TypeImprovement == TypeImpr.Illuminator_B)	// Если тип улучшнения Подсветка или Осветитель
		{
		}
		else if(TypeImprovement == TypeImpr.Accelerator_B)			// Если тип этого улучшения "Ускоритель"
		{
			for(byte a=0; a<transform.childCount; a++)				// Продолжаем цикл до тех пор пока мы можем найти ещё одного ребёнка улучшения
			{
				transform.GetChild(a).gameObject.SetActive(true);	// Включаем улучшение
			}
		}
	}


	void OnDisable()												// Вызываем этот метод перед уничтожением объекта
	{
		GM.StartGameEvent -= StartGame;								// Отписываем метод "Старт игры" от события StartGameEvent
		GM.GoalEvent -= Goal;										// Отписываем метод гол от события GoalEvent
		GM.LastGoalEvent -= LastGoal;								// Отписываем метод LastGoal на отбытия LastGoalEvent
	}
}
