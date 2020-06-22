// Это скрипт для отрисовки главного меню

/* 
Подсказка 7 строк для отсчёта гуи с разных концов экрана с изменением размера гуи под размер экрана относительно его ширины

// Изменяем размер матрицы под новый экран и оставляем отрисовку с левого верхнего края
GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(RatioW, RatioW, 1));

//Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с верхнего правого края экрана
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width, RatioH, 0),Quaternion.identity,new Vector3(RatioW, RatioW, 1);

// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего левого края экрана
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.height, 0, 0),Quaternion.identity,new Vector3(RatioH, RatioH, 1));

// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с нижнего правого края экрана
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width, Screen.height, 0),Quaternion.identity,new Vector3(RatioH, RatioH, 1));

// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с середины экрана по ширине и с верху по высоте
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));

// Изменяем размер матрицы под новый экран и отрисовываем элементы гуи с середины экрана по ширине и с низу по высоте
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
	
// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
*/

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class MainMenu : MonoBehaviour 
{
	public MainMenuWins Window;								// Номер окна показываемого игроку
//---------------------------------------------------------------------------------------------------------------Текстуры для скрита MainMenu--------------------------------------------------------------------------------------------------------------------
	public Texture2D MMenu;									// (Main Menu) Текстура для главного меню
	public Texture2D WPP;									// (Window Player Profiles) Текстура для окна игровых профилей
	public Texture2D AvatarWindow;							// Текстура главного окна в меню выбора аватара
	public Texture2D InformationWindow;						// Текстура окна информации об игре
	public Texture2D ScoreFrame;							// Рамка для счёта очков - (денег)
	public MainMenuPlus MMP;								// Переменная для дополнительного скрипта главного меню MainMenuPlus
	public Keeper Kep;										// Переменная для скрипта "Хранитель"
	public GameManager GM;									// Переменная для скрипта GameManager
	public StoreAndInventory SAI;							// Переменная для скрипта магазина и инвенаря
	public GUISkin GameSkin;								// Скин для всех гуи элементов
	public byte SelectedLevel;								// Номер выбранного уровня для прохождения
	public Texture[] LevelScrins;							// Массив снимков уровней
//-----------------------------------------------------------------------------------------------------------------Звуки для главного меню-----------------------------------------------------------------------------------------------------------------------
	public AudioClip Error;									// Звук ошибки
//--------------------------------------------------------------------------------------------------------------Переменные для работы с профилями----------------------------------------------------------------------------------------------------------------
	public byte NumberProfile;								// Номер просматриваемого профиля
	public bool ChoiseOfLevel = false;						// Показать ли окно выбора уровня
	public bool ReviewProfile = false;						// Выдвинуть ли дополнительное окно для просмотра созданного профиля
	public bool ConfirmDelProf	= false;					// (Confirmation Delete Profile) Показывать ли окно подтверждениея удаления профиля
	int SelectedCell = 0;									// Выбранная ячейка при выборе аватара
	bool CreationProfile = false;							// Выдвинуть ли дополнительное окно для создания профиля
	public bool ButtonEnabled = true;						// Эта переменная говорит включенныли ли все кнопки в главном меню
	string InformationText;									// Текст информирмации в окне аватаров
	string ProfileText = "Введите имя игрока";				// Переменная для ввода ника игрока
	Texture2D ProfileAv;									// Текстура выбранной игроком аватарки в меню создания аватара
	Rect[] PosIconsAv = new Rect[50];						// (Position Icon Avatars) Позиции для иконок в меню выбора аватара
	Texture2D[] StAv = new Texture2D[50];					// (Standart Avatars) В этом массиве храняться все стандартные аватары которые может выбрать игрок
	string[] NamesAv = new string[0];						// В этом массиве храняться имена всех стандартных аватаров которые может выбрать игрок
	string WarningText = "";								// Текст предупреждения при создании профиля
	Vector2 ScrollProfile = Vector2.zero;					// Вектор необходимый для полосы прокрутки в профиле игрока
	Vector2 ScrollAvatar = Vector2.zero;					// Вектор необходимый для полосы прокрутки в окне выбора аватара
	Vector2 ScrollInf = Vector2.zero;						// (Scroll Information) Вектор необходимый для полосы прокрутки окна информации
	Vector2 NPCP = new Vector2(0,280);						// (New Position Creation Profile) Новая позиция окна создания профиля
	Vector2 RPCP = new Vector2(-1410,280);					// (Real Position Creation Profile) Реальная изменяемая позиция окна создания профиля
	Vector2 NPRP = new Vector2(0,280);						// (New Position Review Profile) Новая позиция окна просмотра профиля
	Vector2 RPRP = new Vector2(-1410,280);					// (Real Position Revirw Profile) Реальная позиция окна просмотра профиля
	Vector2 NPWP = new Vector2(0,0);						// (New Position Window Profiles) Новая позиция окна профилей
	Vector2 RPWP = new Vector2(-200,280);					// (Real Position Window Profiles) Реальная изменяемая позиция окна профилей
	float TimerCP = 0;										// (Timer Creation Profile) Таймер создания профиля
	float TimerRP = 0;										// (Timer Review Profile) Таймер обзора профиля
	float Difficulty;										// Сюда возвращаеться значение сложности игры для данного профиля (0 - детский, 1 - нормальный)
//-----------------------------------------------------Переменные для работы меню-----------------------------------------------------------------
	Camera Cam;															// Сюда ложим камеру из префаба
	string Text  = "Игра находиться на стадии разработки. SERG__ZV";	// Коментарий разработчика
	string Version = "v 0.0.1b";										// Номер версии игры
	float OriginalHeight = 1080; 										// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	float RatioH;														// Сюда заноситься результат деления оригинальной высоты экрана на текущую
	public string TextPlayerName;										// Здесь храниться текст имени активного игрока либо предупреждение для него
//	bool WarningTime = false;											// Эта переменная говорит в данный момент происходит предупреждение игрока или нет



	void Start()
	{
		RatioH = Screen.height / OriginalHeight;										// Заносим в ScreenBalansHight результат деления описанный выше
	//	Kep = GameObject.Find("IndestructibleObject").GetComponent<Keeper>();			// Ложим в переменную Kep скрипт Keeper
		GM.CallLoadGameEvent();															// Вызываем метод вызывающий событие загрузки профиля
		CalculatePosIcons();															// Расчитываем позиции для всех множественных прямоугольников в первой сцене
		GetNamesAvatars();																// Получаем имена аватаров
		FindPlayersAvatars();															// Находим аватары игроков

        if (Kep.ActiveProfile != 10)                                                    // Если выбран какой либо профиль игрока
            SelectedLevel = Kep.PlayedLevels[Kep.ActiveProfile];                        // Указываем в переменную Selected Level последний сыгранный уровень для этого профиля 

        InformationText = "Выберите себе аватар";										// Устанавливаем стартовый текст
	//	GameObject.Find("IndestructibleObject").GetComponentInChildren<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;		// Устанавливаем режим рендера канваса ScreenSpaceCamera
	}


	void Update()
	{
		if(ChoiseOfLevel == true)	// Если мы в окне выбора уровня
		{
			if(Input.GetAxis("Mouse ScrollWheel")< -0.1f && Kep.Progress[Kep.ActiveProfile] > SelectedLevel)	// Если колесо мыши прокрученно вверх и текущий уровень доступный для прохождения больше выбранного уровня
				SelectedLevel++;																				// То мы прибавляем переменной "выбранный уровень" 1

			if(Input.GetAxis("Mouse ScrollWheel")> 0.1f && SelectedLevel >= 1)	// Если колесо мыши было прокрученно вниз и выбранный уровень больше или равен еденице
				SelectedLevel--;												// То мы отнимаем у переменной "выбранный уровень" 1
		}
		if(TimerCP > 0)								// Если переменная TimerCP равна ложь
		{
			ChangeCreateProfileWindowPosition();	// Вызываем метод изменить позицию окна создания профиля
		}
		if(TimerRP > 0)								// Если переменная TimerRP равна ложь
		{
			ChangeReviewProfileWindowPosition();	// Вызываем метод изменить позицию окна просмотра профиля
		}
		EscapeButton ();							// Вызываем метод который просчитывает что произойдёт по нажатии клавиши Escape
	}

	
	void OnGUI()
	{
		GUI.depth = 2;						// Устанавливаем дальность гуи от камеры на второй слой
		GUI.skin = GameSkin;				// Устанавливаем гуи скин

		if(Window == MainMenuWins.MainMenu) // Главное меню
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.DrawTexture(new Rect(-190, 130, 380, 680), MMenu);															// Рисуем текстуру главного меню

			GUI.Label(new Rect(-175, 78, 350, 200), TextPlayerName, "PlayerProfileTextColor");	// Отрисовываем имя игрока либо предупреждение

			GUI.enabled = ButtonEnabled;											// Определяем состояние кнопок в меню активны или неактивны
			if(GUI.Button(new Rect(-93, 258, 186, 42), "Прохождение"))				// Если была нажата кнопка прохождение
			{
				if(Kep.ActiveProfile != 10)											// Если есть активный профиль
				{
					ChoiseOfLevel = true;											// То показываем окно выбора уровня
				}
				else 																// Иначе если профиль отсутствует
				{
					StartCoroutine(Warning("Создайте профиль чтобы начать игру"));	// Напоминаем что нужно создать профиль
				}
			}
			if(GUI.Button(new Rect(-93, 354, 186, 42), "Игра на двоих"))			// Если была нажата кнопка игра на двоих
			{
				if(Kep.ActiveProfile != 10)											// Если есть активный профиль
				{
					if(Kep.ActiveObjects[Kep.ActiveProfile, 2] != -1 && Kep.ActiveObjects[Kep.ActiveProfile, 3] != -1)	// Если у этого профиля есть активный стол и активный скайбокс
					{
						UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerVsPlayer"); // Загружаем уровень игры на двоих 	
					}
					else
					{
						StartCoroutine(Warning("Для игры на двоих должны быть купленны и выбранны активными для этого профиля стол и звёздная система"));	// Напоминаем что для игры на двоих должны быть выбранны активными стол и звёздная система
					}
				}
				else 																	// Иначе если профиль отсутствует
				{
					StartCoroutine(Warning("Создайте профиль чтобы начать игру"));		// Напоминаем что нужно создать профиль
				}
			}
			if(GUI.Button(new Rect(-93, 450, 186, 42), "Магазин"))						// Если была нажата кнопка магазин
			{
				if(Kep.ActiveProfile != 10)												// Если есть активный профиль
				{
					SAI.PreparationForTheOpeningStore();								// Вызываем метод подготовки к открытию магазина
					Window = MainMenuWins.Store;										// Вызываем окно магазина
				}
				else 																	// Иначе если профиль отсутствует
				{
					StartCoroutine(Warning("Создайте профиль чтобы начать игру"));		// Напоминаем что нужно создать профиль
				}
			}
			if(GUI.Button(new Rect(-93, 546, 186, 42), "Профиль"))						// Если была нажата кнопка профиль									
				Window = MainMenuWins.Profile;											// Вызываем окно профиля

			if(GUI.Button(new Rect(-93, 642, 186, 42), "Информация"))					// Если была нажата кнопка информация												
				Window = MainMenuWins.Information;										// Вызываем окно информации

			if(GUI.Button(new Rect(-93, 738, 186, 42), "Выход из игры"))				// Если была нажата кнопка выход из игры											
				Application.Quit();

			GUI.enabled = true;
		}

		if(Window == MainMenuWins.Profile)	// Меню профиля игрока
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
//--------------------------------------------------------------Группа для окна профилей--------------------------------------------------------
			GUI.BeginGroup(new Rect(RPWP.x, 280, 400, 422));									// Создаём группу для меню профилей
			GUI.DrawTexture(new Rect(0,0, 400, 422), WPP);										// Рисуем текстуру меню профилей
			ScrollProfile = GUI.BeginScrollView(new Rect(0,1, 400, 420), ScrollProfile, new Rect(0,0, 378, 580)); // Окно скролла
			GUI.enabled = ButtonEnabled;														// Определяем включенны ли кнопки или нет
			if(GUI.Button(new Rect(15, 20, 350, 60), "Создать новый профиль", "ButtonPlus"))	// Если кнопка (Создать новый профиль) нажата
				if(!ReviewProfile)																// Если блокинг ложь и ReviewProfile ложь
				{
					CreationProfile = true;														// Ставим значение переменной на правда
					TimerCP = 3;																// Ставим TimerCP на 3 секунды
				}
			if (GUI.Button(new Rect(15, 100, 350, 60), "В главное меню", "ButtonBack"))			// Если кнопка (В главное меню) нажата
			{
				GM.CallSaveGame();																// Вызываем событие сохранения игры
				Window = 0;																		// Возвращаемся в главное меню
				CreationProfile = false;														// То переменная создание профиля равно ложь
				ReviewProfile = false;															// И переменная обзор профиля равно ложь
				RPCP = new Vector2(-1410,280);	// И переменной реальная поз.. окна создания профиля ставим начальные координаты
				RPRP = new Vector2(-1410,280);	// Переменной реальная позиция окна просмотра профиля
				RPWP = new Vector2(-200,280);	// Переменной реальная поз.. окна профиля ставим координаты где окно находиться в центре
			}																			
//-----------------(1)
			SizeTextButtonProfile(0);																// Определяем размер текста на этой кнопке
			if(ProfileButton(new Rect(15, 181, 320, 60), Kep.NamesProfiles [0], Kep.PlayersAv[0]))	// Если была нажата кнопка (Первый профиль)
				StartCoroutine(ChangeActiveProfile(0));												// Вызываем метод меняющий активный профиль

			
			if(GUI.Button(new Rect(335, 181, 30, 30), "", "CrossButton"))							// Если была нажата кнопка удалить профиль 1
				if(Kep.NamesProfiles[0] != "_ _ _ _ _")												// И этот профиль не пустой
				{
					MMP.NRP = 0;																	// Отправляем в переменную NRP номер 0
					ConfirmDelProf = true;															// Переменная ConfirmDelProf равняется true
				}
			if(GUI.Button(new Rect(335, 211, 30, 30), "", "EditProfileButton"))						// Если была нажата кнопка редактировать пр. 1
				if(!CreationProfile && Kep.NamesProfiles[0] != "_ _ _ _ _")							// И если профиль существует
				{																					// и CreationProfile равна ложь
					ReviewProfile = true;															// То переменная ReviewProfile равно правда
					NumberProfile = 0;																// Говорим что была нажата кнопка 0
					TimerRP = 3;																	// Ставим TimerRP на 3 секунды
				}
//-----------------(2)
			SizeTextButtonProfile(1);																// Определяем размер текста на этой кнопке
			if(ProfileButton(new Rect(15, 261, 320, 60), Kep.NamesProfiles[1], Kep.PlayersAv[1]))	// Если была нажата кнопка (Второй профиль)
				StartCoroutine(ChangeActiveProfile(1));												// Вызываем метод меняющий активный профиль
														
			if(GUI.Button(new Rect(335, 261, 30, 30), "", "CrossButton"))							// Если была нажата кнопка удалить профиль 2
				if(Kep.NamesProfiles[1] != "_ _ _ _ _")												// И этот профиль не пустой
				{
					MMP.NRP = 1;																	// Отправляем в переменную NRP номер 1
					ConfirmDelProf = true;															// Переменная ConfirmDelProf равняется true
				}
			if(GUI.Button(new Rect(335, 291, 30, 30), "", "EditProfileButton"))						// Если была нажата кнопка редактировать пр. 2
				if(!CreationProfile && Kep.NamesProfiles[1] != "_ _ _ _ _")							// И если профиль существует
				{																					// и CreationProfile равна ложь
					ReviewProfile = true;															// То переменная ReviewProfile равно правда
					NumberProfile = 1;																// Говорим что была нажата кнопка 1
					TimerRP = 3;																	// Ставим TimerRP на 3 секунды
				}	
//-----------------(3)
			SizeTextButtonProfile(2);																// Определяем размер текста на этой кнопке
			if(ProfileButton(new Rect(15, 341, 320, 60), Kep.NamesProfiles[2], Kep.PlayersAv[2]))	// Если была нажата кнопка (Третий профиль)		
				StartCoroutine(ChangeActiveProfile(2));												// Вызываем метод меняющий активный профиль
		
			if(GUI.Button(new Rect(335, 341, 30, 30), "", "CrossButton"))							// Если была нажата кнопка удалить профиль 3
				if(Kep.NamesProfiles[2] != "_ _ _ _ _")												// И этот профиль не пустой
				{
					MMP.NRP = 2;																	// Отправляем в переменную NRP номер 2
					ConfirmDelProf = true;															// Переменная ConfirmDelProf равняется true
				}
			if(GUI.Button(new Rect(335, 371, 30, 30), "", "EditProfileButton"))						// Если была нажата кнопка редактировать пр. 3
				if(!CreationProfile && Kep.NamesProfiles[2] != "_ _ _ _ _")							// И если профиль существует
				{																					// и CreationProfile равна ложь
					ReviewProfile = true;															// То переменная ReviewProfile равно правда
					NumberProfile = 2;																// Говорим что была нажата кнопка 2
					TimerRP = 3;																	// Ставим TimerRP на 3 секунды
				}
//-----------------(4)
			SizeTextButtonProfile(3);																// Определяем размер текста на этой кнопке
			if(ProfileButton(new Rect(15, 421, 320, 60), Kep.NamesProfiles[3], Kep.PlayersAv[3]))	// Если была нажата кнопка (Четвёртый профиль)
				StartCoroutine(ChangeActiveProfile(3));												// Вызываем метод меняющий активный профиль

			if(GUI.Button(new Rect(335, 421, 30, 30), "", "CrossButton"))							// Если была нажата кнопка удалить профиль 4
				if(Kep.NamesProfiles[3] != "_ _ _ _ _")												// И этот профиль не пустой
				{
					MMP.NRP = 3;																	// Отправляем в переменную NRP номер 3
					ConfirmDelProf = true;															// Переменная ConfirmDelProf равняется true
				}
			if(GUI.Button(new Rect(335, 451, 30, 30), "", "EditProfileButton"))						// Если была нажата кнопка редактировать пр. 4
				if(!CreationProfile && Kep.NamesProfiles[3] != "_ _ _ _ _")							// И если профиль существует
				{																					// и CreationProfile равна ложь
					ReviewProfile = true;															// То переменная ReviewProfile равно правда
					NumberProfile = 3;																// Говорим что была нажата кнопка 3
					TimerRP = 3;																	// Ставим TimerRP на 2 секунды
				}
//-----------------(5)
			SizeTextButtonProfile(4);																// Определяем размер текста на этой кнопке
			if(ProfileButton(new Rect(15, 501, 320, 60), Kep.NamesProfiles[4], Kep.PlayersAv[4]))	// Если была нажата кнопка (Пятый профиль)
				StartCoroutine(ChangeActiveProfile(4));												// Вызываем метод меняющий активный профиль

			if(GUI.Button(new Rect(335, 501, 30, 30), "", "CrossButton"))							// Если была нажата кнопка удалить профиль 5
				if(Kep.NamesProfiles[4] != "_ _ _ _ _")												// И этот профиль не пустой
				{
					MMP.NRP = 4;																	// Отправляем в переменную NRP номер 4
					ConfirmDelProf = true;															// Переменная ConfirmDelProf равняется true
				}
			if(GUI.Button(new Rect(335, 531, 30, 30), "", "EditProfileButton"))						// Если была нажата кнопка редактировать пр. 5
				if(!CreationProfile && Kep.NamesProfiles[4] != "_ _ _ _ _")							// И если профиль существует
				{																					// и CreationProfile равна ложь
					ReviewProfile = true;															// То переменная ReviewProfile равно правда
					NumberProfile = 4;																// Говорим что была нажата кнопка 4
					TimerRP = 3;																	// Ставим TimerRP на 3 секунды
				}
			GUI.enabled = true;																		// Следующие элементы активны всегда
			GUI.EndScrollView();																	// Конец окна скролла
			GUI.EndGroup();																			// Конец группы
//----------------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------Группа для окна создания профиля-----------------------------------------------------------
			GUI.BeginGroup(new Rect(RPCP.x, 280, 400, 420));									// Создаём группу для меню создания профиля
			GUI.DrawTexture(new Rect(0,0, 400, 420), WPP);										// Рисуем текстуру меню создания профиля
			ProfileText = GUI.TextField(new Rect(160, 30, 235, 40), ProfileText, 20);			// Рисуем поле ввода ника игрока
			GUI.Label(new Rect(160, 75, 235, 70), WarningText, "WarningText");					// Рисуем предупреждение при необходимости
			GUI.enabled = ButtonEnabled;														// Определяем включенны ли кнопки или нет
			if(ProfileAv == null)																// Если переменная ProfileAv пуста
				ProfileAv = Kep.DefaultAvatar;													// Присваиваем переменной ProfileAv аватар по умолчанию
			if(GUI.Button(new Rect(25, 21, 128, 128), ProfileAv, "Avatar"))						// Если мы нажали на кнопку картинку аватара
			{
				Window = MainMenuWins.Profile_Avatar;											// То переходим в окно выбора аватара
			}

			if(GUI.Button(new Rect(25, 260, 350, 60), "Создать", "ButtonPlus"))					// Если кнопка создать профиль нажата
				StartCoroutine(CreateNewProfile());												// Вызываем метод создать новый профиль

			if(GUI.Button(new Rect(25, 340, 350, 60), "Отмена", "ButtonBack"))					// Если кнопка отмена нажата
			{
				CreationProfile = false;														// То переменная создать профиль равна ложь
				ProfileAv = Kep.DefaultAvatar;													// Ставим стандартный аватар в меню создания
				WarningText = "";																// Стираем надпись имя подходит
				ProfileText = "Введите имя игрока";												// Ставим текст введите имя пользователя
			}
			GUI.enabled = true;																	// Следующие элементы активны всегда
			GUI.EndGroup();																		// Конец группы
//---------------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------Группа для окна редактирования созданного профиля---------------------------------------------------
			GUI.BeginGroup(new Rect(RPRP.x, 280, 400, 420));																						// Создаём группу для меню просмотра профиля
			GUI.DrawTexture(new Rect(0, 0, 400, 420), WPP);																							// Рисуем текстуру меню просмотра профиля
			GUI.Label(new Rect(25, 10, 350, 40), Kep.NamesProfiles[NumberProfile], "ProfileNameText");												// Отрисовываем имя профиля
			GUI.DrawTexture(new Rect(160, 60, 170, 40), SAI.FrameForCredits);																		// Отрисовываем рамку для кредитов
			GUI.DrawTexture(new Rect(162, 62, 36, 36), SAI.CreditsIcon);																			// Отрисовываем иконку кредитов

			GUI.DrawTexture(new Rect(160, 104, 170, 40), SAI.FrameForCredits);																		// Отрисовываем рамку для кредитов
			GUI.DrawTexture(new Rect(160, 148, 170, 40), SAI.FrameForCredits);																		// Отрисовываем рамку для кредитов
			if(Kep.ActiveProfile != 10)																												// Если активный профиль не равен 10
			{
				GUI.Label(new Rect(202, 66, 130, 30), Kep.Credits[NumberProfile].ToString("#,0"), "CreditsScore");									// Отрисовываем количество кредитов																																	// Отрисовываем слайдер сложности																																		
				Kep.Difficulty[NumberProfile] = (byte)GUI.HorizontalSlider(new Rect(160, 200, 160, 30), Kep.Difficulty[NumberProfile], 0, 1, "DifficultySlider", "DifficultyThumb");
			}
			GUI.enabled = ButtonEnabled;																											// Определяем включенны ли кнопки или нет
			if(GUI.Button(new Rect(25, 60, 128, 128), Kep.PlayersAv[NumberProfile], "Avatar"))														// Если мы нажали на кнопку картинку аватара
				Window = MainMenuWins.Profile_Avatar;																								// То переходим в окно выбора аватара
			
			if(GUI.Button(new Rect(25, 340, 350, 60), "Назад", "ButtonBack"))																		// Если кнопка назад нажата
			{
				ReviewProfile = false;																												// То переменная обзор профиля равна ложь
			}
			GUI.enabled = true;																														// Следующие элементы активны всегда
			GUI.EndGroup();																															// Конец группы
//--------------------------------------------------------------------------------------------------------------------------------------------------
		}

		if(Window == MainMenuWins.Profile_Avatar)															// Выбор аватара
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.BeginGroup(new Rect(-410, 228, 820, 520));													// Начинаем группу
			GUI.DrawTexture(new Rect(0, 0, 820, 520), AvatarWindow);										// Рисуем меню выбора аватара
			ScrollInf = GUI.BeginScrollView(new Rect (2, 2, 762, 52), ScrollInf, new Rect(0, 0, 740, 138)); // Окно скролла информации
			GUI.Label(new Rect(0,0, 746, 102), InformationText, "InformationAvatars");						// Отрисовываем текст информации
			GUI.EndScrollView();																			// Заканчиваем окно скролла
			if(GUI.Button(new Rect(766,2,52,52), "<Size=30>?</Size>", "SquareButton"))						// по нажатии на кнопку с верху всплывёт подсказка
			{
				InformationText = "Вы можете установить свои аватары. Для этого положите их в папку по адресу " + Application.streamingAssetsPath
				+ "/AvatarsPlayers. Но для корректного отображения их размер должен быть 120x120. Также путь к игре и имена аватарок должны " +
				"быть без русских букв иначе аватарки просто не будут отображаться. Внимание только изображения в формате jpg будут видны. " +
				"Также если общее количество аватарок в этой папке будет больше 50 то последние просто не будут отображаться. Вы можете " +
				"освободить дополнительное место удалив стандартные аватары.";
			}
			ScrollAvatar = GUI.BeginScrollView(new Rect(0,56, 820, 462), ScrollAvatar, new Rect(0,0, 798, 1500)); // Окно скролла
			if(DrawButtonsAvatars())																// Отрисовываем кнопки аватары и при нажатии
			{
				if(CreationProfile)																	// Если создание профиля равно правда
				{
					Window = MainMenuWins.Profile;													// Отрисовываем меню профиля игрока
					ProfileAv = StAv[SelectedCell];													// Cтавим новую выбранную аватарку
					ProfileAv.name = NamesAv[SelectedCell];											// И задаём путь/имя в папке к этой текстуре
				}
				else if(ReviewProfile)																// Если обзор профиля равно правда
				{
					Window = MainMenuWins.Profile;													// Отрисовываем меню профиля игрока
					Kep.PlayersAv[NumberProfile] = StAv[SelectedCell];								// Cтавим новую выбранную аватарку
					Kep.PlayersAvs[NumberProfile] = NamesAv[SelectedCell];							// И задаём путь/имя в папке к этой текстуре
				}
			}
			GUI.EndScrollView();																	// Заканчиваем окно скролла
			GUI.EndGroup();																			// Заканчиваем группу
		}

		if(Window == MainMenuWins.Information) // Инофрмация об игре
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с верху по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, 0, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.DrawTexture(new Rect(-400, 200, 800, 600), InformationWindow);							// Рисуем меню информации об игре
			GUI.Label(new Rect(-387, 220, 0, 0), Text, "InformationText");								
			GUI.Label(new Rect(340,770,0,0), Version, "InformationText");
			if(GUI.Button (new Rect(-93, 720, 186, 42), "Вернуться") || Input.GetKey(KeyCode.Escape))	// если кнопка вернуться была нажата
				Window = MainMenuWins.MainMenu;															// Возвращаемся в главное меню
		}

		if(Window == MainMenuWins.Store)										// Магазин
		{
			SAI.Store();														// Вызываем метод магазин
		}
	}
//-------------------------------------------------------------------------------------------------------------------Методы--------------------------------------------------------------------------------------------------------------------------------------


	void EscapeButton ()								// Этот метод высчитывает что произойдёт по нажатии на кнопку Escape
	{
		if(Input.GetKeyDown(KeyCode.Escape))			// Если была нажата клавиша "Escape"
		{
			switch(Window)
			{
			case MainMenuWins.Store:					// Если окно было равно 1 "Magazine"
				Window = MainMenuWins.MainMenu;			// Переходим в окно главного меню
				GM.CallSaveGame();						// Вызываем метод вызывающий событие сохранения игры для сохранения изменений	
				break;
			case MainMenuWins.Profile:					// В случае если окно было равно 2 "Profile"
				Window = MainMenuWins.MainMenu;			// Переходим в окно главного меню
				GM.CallSaveGame();						// Вызываем метод вызывающий событие сохранения игры для сохранения изменений	
				break;
			case MainMenuWins.Information:				// В случае если окно было равно 4 "Information"
				Window = MainMenuWins.MainMenu;			// Переходим в окно главного меню
				break;
			}
		}
	}


	IEnumerator Warning(string Warning)								// Этот метод при своём вызове напоминает что для игры нужно создать профиль
	{
		TextPlayerName = Warning;									// Имени профиля присваиваем текст ошибки
		GameSkin.GetStyle("PlayerProfileTextColor").fontSize = 18;	// А также ставим размер текста

		GetComponent<AudioSource>().PlayOneShot(Error);				// Проигрываем звук ошибки

		GameSkin.GetStyle("PlayerProfileTextColor").normal.textColor = new Color(0.2F, 0.3F, 0.4F, 1F);
		yield return new WaitForSeconds(0.1f);
		GameSkin.GetStyle("PlayerProfileTextColor").normal.textColor = new Color(0.0F, 0.4F, 1F, 1F);
		yield return new WaitForSeconds(0.1f);
		GameSkin.GetStyle("PlayerProfileTextColor").normal.textColor = new Color(0.2F, 0.3F, 0.4F, 1F);
		yield return new WaitForSeconds(0.1f);
		GameSkin.GetStyle("PlayerProfileTextColor").normal.textColor = new Color(0.0F, 0.4F, 1F, 1F);


//		yield return new WaitForSeconds(10f);
//		TextPlayerName = Kep.NamesProfiles[Kep.ActiveProfile];						// Загружаем в переменную отображения имён профилей и предупреждений имя активного профиля
//		GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (TextPlayerName.Length * 1.8f));// Высчитываем размер имени профиля
	}


	void GetNamesAvatars()		// Этот метод загружает имена аватаров из папки streamingAssets/AvatarsPlayers
	{
		NamesAv = Directory.GetFiles(Application.streamingAssetsPath + "/AvatarsPlayers/", "*.jpg");
	}


	void SizeTextButtonProfile(byte nomber)	// Эта переменная определяет какой размер текста должен быть у кнопки этого профиля
	{
		if(Kep.NamesProfiles[nomber].Length<=16)				// Если длинна имени активного профиля меньше 16 символов
			GameSkin.GetStyle("ButtonProfile").fontSize = 20;	// Делаем стиль шрифта имени профиля размером в 20 едениц
		else if(Kep.NamesProfiles[nomber].Length>16)			// Иначе если длинна имени активного профиля больше 16 символов
			GameSkin.GetStyle("ButtonProfile").fontSize = 17;	// Делаем стиль шрифта имени профиля размером в 17 едениц
	}
	

	void CalculatePosIcons() 	// Этот метод расчитывает позиции для иконок магазина, окон аватаров и прочих прямоугольников
	{
		int Left = 30;			 // Позиция с лева
		int Top = 20;			 // Позиция с верху
		byte NomberOfString = 0; // Номер отрисовываемого квадрата в строке

		for(byte a=0; a<PosIconsAv.Length; a++)				// Продолжаем цикл пока не назначим позиции всему массиву масок для аватаров
		{
			PosIconsAv[a] = new Rect(Left, Top, 128, 128); 	// Расчитываем очередной квадрат
			NomberOfString ++;								// Прибавляем каждую итерацию к номеру элемента по горизонтали
			Left += 154;									// Прибавляем каждую итерацию позицию окна по ширине
			
			if(NomberOfString == 5)							// Если в этой строке был расположен последний элемент
			{
				NomberOfString = 0;							// То номер просчитываемого элемента в строке назначаем нулевым
				Left = 30;									// Позиции окна по ширине ставим начальную ширину
				Top += 148;									// И позиции окна по высоте прибавляем отступ
			}
		}
			
		Left = 5;												// Присваиваем новые начальные кординаты квадрата с лева для расчёта квадратов для отрисовки объектов магазина					
		Top = 158;												// Присваиваем новые начальные кординаты квадрата с верху для расчёта квадратов для отрисовки объектов магазина	
		NomberOfString = 0;										// Присваиваем NomberOfString начальное значение для расчёта категории квадратов PosIconMats

		for(byte b=0; b < SAI.PosIconObj.Length; b++) 			// Продолжаем цикл пока не назначим позиции всему массиву иконок объектов магазина
		{
			SAI.PosIconObj[b] = new Rect(Left, Top, 128, 128);	// Расчитываем очередной квадрат
			NomberOfString ++;									// Прибавляем каждую итерацию к номеру элемента по горизонтали
			Left += 128;										// Прибавляем каждую итерацию позицию окна по ширине

			if (NomberOfString == 5)							// Если в этой строке был расположен последний элемент
			{
				NomberOfString = 0;								// То номер просчитываемого элемента в строке назначаем нулевым
				Left = 5;										// Позиции окна по ширине ставим начальную ширину
				Top += 128;										// И позиции окна по высоте прибавляем отступ
			}
		}

		Left = 9;												// Присваиваем новые координаты позиции с лева для расчёта категории квадратов 
		Top = 162;												// Присваиваем новые координаты позиции с верху для расчёта категории квадратов PosIconMask
		NomberOfString = 0;										// Присваиваем NomberOfString начальное значение для расчёта категории квадратов PosIconMask

		for(byte c=0; c < SAI.PosIconMask.Length; c++)			// Продолжаем цикл пока не назначим позиции всему массиву масок для объектов в магазине
		{
			SAI.PosIconMask[c] = new Rect(Left, Top, 120, 120); // Расчитываем очередной квадрат
			NomberOfString ++;									// Прибавляем каждую итерацию к номеру элемента по горизонтали
			Left += 128;										// Прибавляем каждую итерацию позицию окна по ширине
			
			if(NomberOfString == 5)								// Если в этой строке был расположен последний элемент
			{
				NomberOfString = 0;								// То номер просчитываемого элемента в строке назначаем нулевым
				Left = 9;										// Позиции окна по ширине ставим начальную ширину
				Top += 128;										// И позиции окна по высоте прибавляем отступ
			}
		}

		Left = 660;												// Присваиваем новые координаты позиции с лева для расчёта категории квадратов PosIconMats
		Top = 488;												// Присваиваем новые координаты позиции с верху для расчёта категории квадратов PosIconMats
		NomberOfString = 0;										// Присваиваем NomberOfString начальное значение для расчёта категории квадратов PosIconMats

		for(byte d=0; d < SAI.PosIconMats.Length; d++)			// Продолжаем цикл пока не назначим позиции всему массиву PosIconMats (квадраты для материалов)
		{
			SAI.PosIconMats[d] = new Rect(Left, Top, 50, 50); 	// Расчитываем очередной квадрат
			NomberOfString ++;									// Прибавляем каждую итерацию к номеру элемента по горизонтали
			Left += 70;											// Прибавляем каждую итерацию отступ ячейки по ширине

			if(NomberOfString == 5)								// Если в этой строке был расположен последний элемент
			{
				NomberOfString = 0;								// То номер просчитываемого элемента в строке назначаем нулевым
				Left = 526;										// Позиции окна по ширине ставим начальную ширину
				Top += 58;										// И позиции окна по высоте прибавляем отступ
			}
		}

		Left = 660;
		Top = 665;												// Присваиваем новые координаты позиции с верху для расчёта категории квадратов PosIconImprs
		NomberOfString = 0;										// Присваиваем NomberOfString начальное значение для расчёта категории квадратов PosIconMats

		for(byte e=0; e < SAI.PosIconImprs.Length; e++)			// Продолжаем цикл пока не назначим позиции всему массиву PosIconImprs (квадраты для улучшений)
		{
			SAI.PosIconImprs[e] = new Rect(Left, Top, 50, 50);	// Расчитываем очередной квадрат
			NomberOfString ++;									// Прибавляем каждую итерацию к номеру элемента по горизонтали
			Left += 70;											// Прибавляем каждую итерацию отступ ячейки по ширине

			if(NomberOfString == 5)								// Если в этой строке был расположен последний элемент
			{
				NomberOfString = 0;								// То номер просчитываемого элемента в строке назначаем нулевым
				Left = 526;										// Позиции окна по ширине ставим начальную ширину
				Top += 58;										// И позиции окна по высоте прибавляем отступ
			}
		}
	}

	
	bool DrawButtonsAvatars()	// Этот метод отрисовывает кнопки аватаров по просчитанным позициям
	{
		bool pressed = false;
		// Продолжаем цикл до тех пор пока не пройдём 50 циклов или до тех пор пока в массиве StAv мы не наткнёмся на значение null
		for(byte i=0; StAv[i] != null; i++) 
		{
			if(GUI.Button(PosIconsAv[i], StAv[i], "Avatar"))		// Если очередная отрисовываемая в цикле кнопка была нажата
			{
				pressed = true;										// То переменной Pressed присваиваем значение правда
				SelectedCell = i;									// И помещаем номер нажатой кнопки в переменную SelectedAv
			}
			if(i == 49)												// Если цыкл прошёл 49 итерацию
				break;												// То мы завершаем цикл
		}
		return pressed;												// После отрисовки кнопок мы возвращаем значение была или нет нажата кнопка
	}

	
	void FindPlayersAvatars()	// Этот метод находит сами аватары и помещает их в массив StAv
	{
		for(byte i=0; i<NamesAv.Length & i<StAv.Length; i++)
		{
			string FolderPath = "file:///" + NamesAv[i]; 	// Задаём путь переменной FolderPath
			WWW www = new WWW(FolderPath);					// Присваиваем переменной www путь FolderPath
			StAv[i] = www.texture;							// Загружаем в массив StAv текстуру
		}
	}

	
	// Этот метод при своём вызове отрисовывает кнопку с дополнительным рисунком для профиля
	bool ProfileButton(Rect ButRectangle, string Buttontext, Texture ProfileImage)
	{
		bool Pressed = GUI.Button(ButRectangle, Buttontext, "ButtonProfile");
		GUI.Label(new Rect(ButRectangle.x + 6, ButRectangle.y + 6, 48, 48), ProfileImage);
		return Pressed;
	}
	

	void ChangeCreateProfileWindowPosition()	// Этот метод изменяет позицию окошек при создании профиля
	{
		if(CreationProfile == true)				// Если значение переменной "Создать профиль" равно правда
		{
			NPCP = new Vector2(-410,280);		// То переменной новая позиция окна создания профиля присваиваем координаты где окно видно
			NPWP = new Vector2(10,280);			// То переменной новая позиция окна профилей присваиваем координаты где окно сдвинуто вправо
		}
		else 									// Если же значение переменной "Создать профиль" равно ложь
		{
			NPCP = new Vector2(-1410,280);		// То переменной новая позиция окна создания профиля присваиваем координаты где окно спрятанно
			NPWP = new Vector2(-200,280);		// То переменной новая позиция окна профиля присваиваем координаты где окно находиться в центре
			TimerCP -= Time.deltaTime * 6;		// И отнимаем каждую секунду у TimerCP по 6 едениц
		}
		RPCP = Vector2.Lerp(RPCP, NPCP, Time.deltaTime * 6);	// Плавно изменяем положение окна создания профиля
		RPWP = Vector2.Lerp(RPWP, NPWP, Time.deltaTime * 6);	// Плавно изменяем положение окна профилей
	}


	void ChangeReviewProfileWindowPosition()	// Этот метод изменяет позицию окошек при просмотре профиля
	{
		if(ReviewProfile == true)				// Если значение еременной "Просмотр профиля" равно правда
		{
			NPRP = new Vector2(-410,280);		// То переменной новая позиция окна просмотра профиля присваиваем координаты где окно видно
			NPWP = new Vector2(10,280);			// То переменной новая позиция окна профилей присваиваем координаты где окно сдвинуто вправо
		}
		else
		{
			NPRP = new Vector2(-1410,280);		// То переменной новая позиция окна просмотра профиля присваиваем координаты где окно спрятанно
			NPWP = new Vector2(-200,280);		// То переменной новая позиция окна профиля присваиваем координаты где окно находиться в центре
			TimerRP -= Time.deltaTime * 6;		// И отнимаем каждую секунду у TimerRP по 6 едениц
		}
		RPRP = Vector2.Lerp(RPRP, NPRP, Time.deltaTime * 6);	// Плавно изменяем положение окна просмотра профиля
		RPWP = Vector2.Lerp(RPWP, NPWP, Time.deltaTime * 6);	// Плавно изменяем положение окна профилей
	}


	IEnumerator ChangeActiveProfile(byte NomberProfile)			// Этот метод изменяет номер активного профиля а также переписывает его в документе ProfileInf
	{
		if(Kep.NamesProfiles[NomberProfile] != "_ _ _ _ _" && NomberProfile != Kep.ActiveProfile)		// Если профиль который мы собираемся сделать активным не пуст или уже не являеться активным
		{	
			ButtonEnabled = false;								// Блокируем все кнопки в меню
			Kep.ActiveProfile = NomberProfile;					// Мы отмечаем этот профиль как активный
			Kep.ViewedProfile = Kep.ActiveProfile;				// Ложим значение Kep.ActiveProfile в переменную Kep.ViewedProfile
			GM.CallSaveGame();									// Вызываем метод вызывающий событие сохранения
			yield return new WaitForSeconds(1);					// Ждём 2 секунды
			Window = MainMenuWins.MainMenu;						// Отправляем игрока в главное меню
			CreationProfile = false;							// Ставим переменую CreationProfile в положение ложь
			ButtonEnabled = true;								// Разблокируем кнопки меню

			SelectedLevel = Kep.PlayedLevels[Kep.ActiveProfile];				// Загружаем последний сыгранный уровень для этого профиля в переменную Selected Level
			TextPlayerName = Kep.NamesProfiles[Kep.ActiveProfile];				// Загружаем в переменную TextPlayerName имя нового активного профиля
			GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (TextPlayerName.Length * 1.8f));// Высчитываем размер имени профиля

			StartCoroutine(Kep.FillActiveObjsTexs(Kep.ActiveProfile, false));	// Фотографируем иконки для активных объектов 1 игрока
		}
	}


	// Этот метод удаляет профиль и перемещает информацию о профилях ниже, вверх. Чтобы небыло пробелов
	public void DeleteProfile(byte RemuvableProfile)
	{ 
		for(int i = RemuvableProfile; i < Kep.NamesProfiles.Length; i++) 	// Продолжаем цикл пока не переберём оставшиеся профили
		{
			if(i < Kep.NamesProfiles.Length-1)								// Если это не последний цикл то перемещаем в верхний профиль информацию из нижнего
			{
				Kep.NamesProfiles[i] = Kep.NamesProfiles[i+1];				// Перемещаем имя профиля в верхний слот из нижнего
				Kep.PlayersAv[i] = Kep.PlayersAv[i+1];						// Перемещаем аватарку профиля в верхний слот из нижнего
				Kep.PlayersAvs[i] = Kep.PlayersAvs[i+1];					// Перемещаем адрес аватарки профиля в верхний слот из нижнего
				Kep.Credits[i] = Kep.Credits[i+1];							// Перемещаем количество денег с нижнего профиля в верхний
				Kep.Progress[i] = Kep.Progress[i+1];						// Перемещаем следующий уровень доступный для прохождения с нижнего профиля на верхний
				Kep.PlayedLevels[i] = Kep.PlayedLevels[i+1];				// Перемещаем последний сыгранный уровень с нижнего профиля в верхний
				Kep.ActiveObjects[i,0] = Kep.ActiveObjects[i+1,0];			// Перемещаем перемещаем последнюю выбранную биту с нижнего профиля на верхний
				Kep.ActiveObjects[i,1] = Kep.ActiveObjects[i+1,1];			// Перемещаем последнюю выбранную шайбу с нижнего профиля на верхний
				Kep.ActiveObjects[i,2] = Kep.ActiveObjects[i+1,2];			// Перемещаем последний выбранный стол с нижнего профиля на верхний
				Kep.ActiveObjects[i,3] = Kep.ActiveObjects[i+1,3];			// Перемещаем последний выбранный скайбокс с нижнего профиля на верхний
				Kep.RightSide[i] = Kep.RightSide[i+1];						// Перемещаем выбранную по умолчанию сторону с нижнего профиля на верхний
				Kep.AlienStyles[i] = Kep.AlienStyles[i+1];					// Перемещаем выбранный стиль цифр с нижнего профиля на верхний
				Kep.Difficulty[i] = Kep.Difficulty[i+1];					// Перемещаем выбранную сложность с нижнего профиля на верхний
				Kep.ActiveMats[i] = Kep.ActiveMats[i+1];					// Перемещаем список номеров активных материалов для объектов с нижнего профиля на верхний
				Kep.ObjectsStates[i] = Kep.ObjectsStates[i+1];				// Перемещаем список состояний объектов в массиве с нижнего профиля на верхний
				Kep.SkyboxesStates[i] = Kep.SkyboxesStates[i+1];			// Перемещаем список состояний скайбоксов с нижнего профиля на верхний
				Kep.StatesMaterials[i] = Kep.StatesMaterials[i+1];			// Перемещаем список списков состояний материалов с нижнего профиля на верхний
				Kep.StatesImprovements[i] = Kep.StatesMaterials[i+1];		// Перемещаем список списков состояний улучшений с нижнего профиля на верхний
			}
			else if(i == Kep.NamesProfiles.Length-1)						// Если это последний цикл и последний профиль то мы просто удаляем информацию из последнего
			{	// Так как программа наткнувшись на такое имя воспринимает профиль как пустой можно не очищать его а просто при создании нового записывать на старые данные
				Kep.NamesProfiles[i] = "_ _ _ _ _";							// Ставим имя последнего профиля как имя пустого профиля
			}
		}

		if(Kep.NamesProfiles[Kep.ActiveProfile] == "_ _ _ _ _")		// Если номер активного профиля теперь имеет пустое имя
		{
			if(Kep.ActiveProfile != 0)								// И если при этом номер активного профиля не нулевой тоесть ещё есть профили
			{
				Kep.ActiveProfile --;								// То мы уменьшаем номер переменной ActiveProfile на 1
				TextPlayerName = Kep.NamesProfiles[Kep.ActiveProfile];				// Загружаем в переменную TextPlayerName имя активного профиля
				GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (TextPlayerName.Length * 1.8f));// Высчитываем размер имени профиля
				StartCoroutine(Kep.FillActiveObjsTexs(Kep.ActiveProfile, false));	// Вызываем метод который фотографирует купленные объекты 1 игрока и заполняет массив FPAA
				SelectedLevel = Kep.PlayedLevels[Kep.ActiveProfile];				// Загружаем последний сыгранный уровень для этого профиля в переменную Selected Level
			}
			else 															// Иначе если номер активного профиля был равен 0, тоесть он был последний
			{
				Kep.ActiveProfile = 10;										// То ставим номер активного профиля 10 говорящий о том что профилей больше нет
				TextPlayerName = "Создайте профиль чтобы начать игру";		// Переменной MM.TextPlayerName ставим предупреждение что нужно создать профиль
				GameSkin.GetStyle("PlayerProfileTextColor").fontSize = 18; 	// Ставим размер предупреждения 18
				SelectedLevel = 0;											// Ставим последний сыгранный уровень в локальной переменной 0
			}
		}
		GM.CallSaveGame();													// Вызываем метод вызывающий событие сохранения игры (Всех профилей)
	}


	byte FindEmtySlotForSavingProfile()	// Этот метод проходит по массиву Profiles и находит в нём пустой слот для сохранения
	{
		byte Slot = 10;			// Назначаем переменной слот значение 10 по умолчанию. Которое означает что пустой слот не найден
		for(byte a = 0; ; a++)	// Проходим по массиву находя первый пустой слот для профиля
		{
			if(Kep.NamesProfiles[a] == "_ _ _ _ _")		// Если профиль имеет значение "_ _ _ _ _" значит он пустой
			{
				Slot = a;								// То переменной ProfileNumber задаём значение найденного пустого слота
				break;									// Брейкаем цикл
			}
			else if(a == Kep.NamesProfiles.Length -1)	// Если пройдя весь массив мы не нашли пустого профиля
			{
				break;									// Прерываем цикл
			}
		}
		return Slot;									// Возвращаем значение переменной Slot
	}


	bool NameCheck()										// Этот метод проверяет есть ли такое имя для создания нового профиля
	{
		for(byte i=0; i < Kep.NamesProfiles.Length; i++)	// Продолжаем цикл пока не пройдём весь массив Profiles
		{
			if(Kep.NamesProfiles[i] == "_ _ _ _ _")			// Если мы наткнулись на пустое имя
			{
					return false;							// Возвращаем false - (Значит такого же имени нет)
			}
				if(Kep.NamesProfiles[i] == ProfileText)		// Если очередной в цикле профиль равен тексту вписанному в создании профиля
				{
					return true;							// Возвращаем правда - (Значит такое имя уже используется для одного из профилей)
				}
		}
		return false;										// Если же мы прошли все профили и не нашли похожего имени то возвращаем false
	}


	IEnumerator CreateNewProfile()									// Этот метод создаёт новый профиль при нажатии кнопки создать новый профиль
	{
		byte EmptySlot;												// Пустой слот обнаруженный для сохранения в него профиля
		EmptySlot = FindEmtySlotForSavingProfile();					// Находим пустой слот и помещаем его в переменную EmptySlot
		if(EmptySlot == 10)											// Если пустой слот равен 10 значит свободных слотов не осталось
			WarningText = "Не осталось свободных профилей! Удалите один из старых чтобы создать новый."; // Тогда мы выводим предупреждение
		else if(ProfileText == "Введите имя игрока")				// Если вместо имени осталься (начальный текст)
			WarningText = "Сотрите эти буквы и введите ваше имя!";	// Пишем предупреждение
		else if(ProfileText == "_ _ _ _ _")							// Если игрок использует системное имя для обозначения пустого имени
			WarningText = "Это имя недопустимо для имени профиля!";	// Выводим текст что это имя недопустимо
		else if(ProfileText == "")									// Если имя отсутствует
			WarningText = "Введите ваше имя в игре!";				// То выводим текст в консоль "введите имя"
		else if(ProfileText.Length <= 2)							// Если в поле 2 или менее двух букв
			WarningText = "Слишком короткое имя!";					// То выводим предупреждение "Слишком короткий ник"
		else if(ProfileAv.name == Kep.DefaultAvatar.name)			// Если имя текстуры такое же как и у аватара стоящего по умолчанию
			WarningText = "Выберите себе аватар!";					// То выводим предупреждение что игрок должен выбрать себе аватар
		else if(NameCheck() == true)								// Если (метод NameCheck вернёт true) - профиль с таким именем уже существует 
			WarningText = "Такой профиль уже существует!";
		else 														// Если имя соответствует всем условиям 
		{	
		    Kep.NamesProfiles[EmptySlot] = ProfileText;					// Присваиваем вбитое в поле имя в создании профиля имя игрока
			Kep.PlayersAv[EmptySlot] = ProfileAv;						// Присваиваем аватарку слоту аватара игрока
			Kep.Credits[EmptySlot] = 0;									// Присваиваем переменной деньги число 0
			Kep.Progress[EmptySlot] = 1;								// Ставим уровень доступный для прохождения 1 (для тех кому не нужно обучение)
			Kep.PlayedLevels[EmptySlot] = 0;							// Ставим последний сыгранный уровень на 0 чтобы выбралься уровень обучения
			Kep.ActiveObjects[EmptySlot, 0] = 0;						// Номер последней выбранной биты в массиве ObjectsStore (Стартовая бита)
			Kep.ActiveObjects[EmptySlot, 1] = 125;						// Номер последней выбранной шайбы в массив ObjectsStore (Стартовая шайба)
			Kep.ActiveObjects[EmptySlot, 2] = -1;						// Номер последнего выбранного поля в массиве ObjectsStore -1 означает что нету
			Kep.ActiveObjects[EmptySlot, 3] = -1;						// Номер последнего выбранного скайбокса в массиве SkyboxMats -1 означает что нету
			Kep.RightSide[EmptySlot] = true;							// Присваиваем предпочитаемую по умолчанию сторону правую
			Kep.AlienStyles[EmptySlot] = true;							// Стиль цифр по умолчанию "стиль пришельцев"

			Kep.ActiveMats[EmptySlot] = new byte[375];					// Инициализируем массив номеров активных материалов для активного профиля
			Kep.ActiveImprs[EmptySlot] = new byte[375];					// Инициализируем массив номеров активных улучшений для активного профиля
			Kep.ObjectsStates[EmptySlot] = new char[375];				// Инициализируем массив состояний объектов для активного профиля
			Kep.SkyboxesStates[EmptySlot] = new char[125];				// Инициализируем массив состояний скайбоксов для активного профиля
			Kep.ObjectsExpirience[EmptySlot] = new int[375];			// Инициализируем массив опыта объектов для активного профиля
			Kep.StatesMaterials[EmptySlot] = new List<char>[375];		// Инициализируем новый массив списков для активного профиля
			Kep.StatesImprovements[EmptySlot] = new List<char>[375];	// Инициализируем новый массив списков для активного профиля

			List<char> StatMatBat = new List<char>(1);					// Инициализируем список состояний материалов для 0 элемента в массиве ObjectsStore (Стартовой биты)
			List<char> StatMatPuck = new List<char>(1);					// Инициализируем список состояний материалов для 125 элемента в массиве ObjectsStore (Стартовой шайбы)
			List<char> StatMatTable = new List<char>(2);				// Инициализируем список состояний материалов для 250 элемента в массиве ObjectsStore (Стартового стола)
			List<char> StatImprBat = new List<char>(0);					// Инициализируем пустой список улучшений для 0 элемента в массиве ObjectsStore (Стартовой биты)
			List<char> StatImprPuck = new List<char>(0);				// Инициализируем пустой список улучшений для 125 элемента в массиве ObjectsStore (Стартовой шайбы)
			List<char> StatImprTable = new List<char>(1);				// Инициализируем пустой список улучшений для 250 элемента в массиве ObjectsStore (Стартового стола)

			StatMatBat.Add('B');										// Присваиваем нулевому материалу стартовой биты состояние "Куплен"
			StatMatPuck.Add('B');										// Присваиваем нулевому материалу стартовой шайбы состояние "Куплен"
			StatMatTable.Add('B');										// Присваиваем нулевому материалу стартового стола состояние "Куплен"
			StatMatTable.Add('C');										// Присваиваем первому материалу стартового стола состояние "Закрыт"

			StatImprTable.Add('C');										// Присваиваем первому улучшению стартового стола состояние "Закрыт"

			Kep.ActiveMats[EmptySlot][0] = 0; 							// Указываем номер активного материала для 0 элемента в массиве ObjectsStore (Стартовой биты)
			Kep.ActiveMats[EmptySlot][125] = 0;							// Указываем номер активного материала для 125 элемента в массиве ObjectsStore (Стартовой шайбы)
			Kep.ActiveMats[EmptySlot][250] = 0;							// Указываем номер активного материала для 250 элемента в массиве ObjectsStore (Стартового стола)
			Kep.ActiveImprs[EmptySlot][0] = 10;							// Указываем что улучшение для 0 элемента в списке ObjectsStore (Стартовой биты) отсутствует
			Kep.ActiveImprs[EmptySlot][125] = 10;						// Указываем что улучшение для 125 элемента в массиве ObjectsStore (Стартовой шайбы) отсутствует
			Kep.ActiveImprs[EmptySlot][250] = 10;						// Указываем что улучшение для 250 элемента в массиве ObjectsStore (Стартового стола) отсутствует
			Kep.ObjectsStates[EmptySlot][0] = 'B';						// Указываем состояние 0 элемента в массиве ObjectsStore (Стартовой биты) "Куплен"
			Kep.ObjectsStates[EmptySlot][125] = 'B';					// Указываем состояние 125 элемента в массиве ObjectsStore (Стартовой шайбы) "Куплен"
			Kep.ObjectsStates[EmptySlot][250] = 'O';					// Указываем состояние 250 элемента в массиве ObjectsStore (Стартового стола) "Открыт"
			Kep.SkyboxesStates[EmptySlot][0] = 'O';						// Указываем состояние 0 элемента массива SkyboxesStates (Стартового скайбокса) "Открыт"
			Kep.ObjectsExpirience[EmptySlot][0] = 0;					// Указываем количество опыта у 0 элемента в массиве ObjectsStore (Стартовой биты) равное нулю
			Kep.ObjectsExpirience[EmptySlot][125] = 0;					// Указываем количество опыта у 125 элемента в массиве ObjectsStore (Стартовой биты) равное нулю
			Kep.ObjectsExpirience[EmptySlot][250] = 0;					// Указываем количество опыта у 125 элемента в массиве ObjectsStore (Стартового стола) равное нулю
			Kep.StatesMaterials[EmptySlot][0] = StatMatBat;				// Присваиваем 0 объекту массива ObjectsStore (Стартовой бите) список состояний её материалов
			Kep.StatesMaterials[EmptySlot][125] = StatMatPuck;			// Присваиваем 125 объекту массива ObjectsStore (Стартовой шайбе) список состояний её материалов
			Kep.StatesMaterials[EmptySlot][250] = StatMatTable;			// Присваиваем 250 объекту массива ObjectsStore (Стартовому столу) список состояний его материалов

			Kep.StatesImprovements[EmptySlot][0] = StatImprBat;			// Присваиваем 0 объекту ObjectsStore (Стартовой бите) пустой список состояний её улучшений
			Kep.StatesImprovements[EmptySlot][125] = StatImprPuck;		// Присваиваем 125 объекту ObjectsStore (Стартовой шайбе) пустой список состояний её улучшений
			Kep.StatesImprovements[EmptySlot][250] = StatImprTable;		// Присваиваем 250 объекту ObjectsStore (Стартовому столу) пустой список состояний его улучшений

			SelectedLevel = Kep.PlayedLevels[EmptySlot];		// Загружаем последний сыгранный уровень для этого профиля в переменную Selected Level

			Kep.SaveNomber ++;											// Прибавляем к номеру сохранения еденицу
			Kep.PlayersAvs[EmptySlot] = Kep.PlayersAv[EmptySlot].name;	// Записываем адрес нового аватара в массив имён адресов аватаров

			Kep.HashTab = Kep.SaveNomber.GetHashCode() + Kep.ActiveProfile.GetHashCode() + Kep.NamesProfiles.GetHashCode() + Kep.PlayersAvs.GetHashCode() + Kep.Credits.GetHashCode()
				+ Kep.Progress.GetHashCode() + Kep.PlayedLevels.GetHashCode() + Kep.ActiveObjects.GetHashCode() + Kep.RightSide.GetHashCode() + Kep.AlienStyles.GetHashCode() + Kep.Difficulty.GetHashCode()
				 + Kep.ActiveMats.GetHashCode() + Kep.ActiveImprs.GetHashCode() + Kep.ObjectsStates.GetHashCode() + Kep.ObjectsExpirience.GetHashCode() + Kep.StatesMaterials.GetHashCode() +
					Kep.StatesImprovements.GetHashCode() + Kep.SkyboxesStates.GetHashCode();	// Создаём контрольную сумму

			Kep.ActiveProfile = EmptySlot;														// Делаем созданный профиль активным

			string[] Saves = System.IO.Directory.GetFiles(Kep.path + "/My Games/Slide/Saves/" , "Save_*.bin");		// Заносим в массив все сохранения
			string OldestSave = "StartString";			// Самое старое сохранение
			int OSComparsion = -1;						// (Oldest Comparsion) Переменная сравнения номеров сохранений для выявления самого старого сохранения
			int LSComparsion = -1;						// (Lastest Comparsion) Переменная сравнения номеров сохранений для выявления самого нового сохранения

			for(int a=0; a<Saves.Length; a++)			// Продолжаем цикл пока не пройдём весь массив (Выявляем номер самого нового сохранения)
			{
				IFormatter Form = new BinaryFormatter();														// Создаём дессериализатор
				FileStream LoadInf = new FileStream(Saves[a], FileMode.Open, FileAccess.Read); 					// Создаём поток читающий файл SaveProfiles
				SaveProfiles SPr = (SaveProfiles)Form.Deserialize(LoadInf);										// Создаём объект SPr и десериализуем в него данные из файла SaveProfiles
				
				if(LSComparsion == -1)					// Если в переменной LSComparsion находиться цифра -1
				{
					LSComparsion = SPr.SaveNomber; 		// То присваиваем заместо этого номера номер сохранения которое мы сейчас дессериализовали
				}
				else 									// Иначе если там уже находиться номер какого либо сохранения то сравниваем его с номером дессериализованного в данный момент сохранения
				{
					if(LSComparsion < SPr.SaveNomber)	// И если в переменной "LSComparsion" число меньше чем в элементе массива с номером итерации
					{
						LSComparsion = SPr.SaveNomber;	// То присваиваем это число переменной "Сравнение"
					}
				}
				LoadInf.Close();						// Закрываем поток LoadInf
			}
			
			for(int b=0; b<Saves.Length; b++)			// Продолжаем цикл пока не пройдём весь массив (Выявляем номер самого старого сохранения и путь к нему
			{
				IFormatter Form = new BinaryFormatter();														// Создаём дессериализатор
				FileStream LoadInf = new FileStream(Saves[b], FileMode.Open, FileAccess.Read); 					// Создаём поток читающий файл SaveProfiles
				SaveProfiles SPr = (SaveProfiles)Form.Deserialize(LoadInf);										// Создаём объект SPr и десериализуем в него данные из файла SaveProfiles
				
				if(OSComparsion == -1)							// Если в переменной OSComparsion находиться цифра -1
				{
					OSComparsion = SPr.SaveNomber; 				// То присваиваем вместо этого номера номер сохранения которое мы сейчас дессериализовали
					OldestSave = Saves[b];						// Переменной OldestSave присваиваем объект массива Saves с номером итерации
				}
				else 											// Иначе если там уже находиться номер какого либо сохранения то сравниваем его с номером дессериализованного в данный момент сохранения
				{
					if(OSComparsion > SPr.SaveNomber)			// И если в переменной "OSComparsion" число больше чем в элементе массива с номером итерации
					{
						OSComparsion = SPr.SaveNomber;			// То присваиваем это число переменной "Сравнение"
						OldestSave = Saves[b];					// Переменной OldestSave присваиваем объект массива Saves с номером итерации
					}
				}
				LoadInf.Close();								// Закрываем поток LoadInf
			}

			LSComparsion++;										// Прибавляем к переменной LSComparsion еденицу

			SaveProfiles SP = new SaveProfiles();				// Создаём объект SP класса SaveProfiles
			SP.ActiveProfile = Kep.ActiveProfile;				// Записываем в объект SP значение ActiveProfile
			SP.NamesProfiles = Kep.NamesProfiles;				// Записываем в объект SP массив Profiles

			SP.HashTab = Kep.HashTab;							// Записываем в объект SP его HashTab номер
			SP.SaveNomber = Kep.SaveNomber;						// Записываем в объект SP номер сохранения
			SP.PlayersAvs = Kep.PlayersAvs;						// Записываем в объект SP массив адресов аватаров профилей
			SP.Credits[EmptySlot] = 0;							// Записываем в объект SP стартовое количество денег (Кредитов)
			SP.Progress[EmptySlot] = 1;							// Записываем в объект SP 1 уровень доступный для прохождения для создаваемого профиля
			SP.PlayedLevels[EmptySlot] = 0;						// Записываем в объект SP последний сыгранный уровень как нулевой для создаваемого профиля
			SP.ActiveObjects = Kep.ActiveObjects;				// Записываем в объект SP последние 4 выбранные стартовых предмета всех профилей
			SP.RightSide = Kep.RightSide;						// Записываем в объект SP правую сторону предпочитаемую для игры всех профилей
			SP.AlienStyles = Kep.AlienStyles;					// Записываем в объект SP "стиль пришельцев" для цифр
			SP.Difficulty = Kep.Difficulty;						// Записываем в объект SP "уровень сложности" для всех профилей
			SP.ActiveMats = Kep.ActiveMats;						// Записываем в объект SP номера активных материалов для объектов в массиве ObjectsStore
			SP.ActiveImprs = Kep.ActiveImprs;					// Записываем в объект SP номера активных материалов для объектов в массиве ObjectsStore
			SP.ObjectsStates = Kep.ObjectsStates;				// Записываем в объект SP состояния объектов в массиве Открыт - O или Куплен - B
			SP.SkyboxesStates = Kep.SkyboxesStates;				// Записываем в объект SP Состояния скайбоксов Открыт - O или Куплен - B
			SP.ObjectsExpirience = Kep.ObjectsExpirience;		// Записываем в объект SP опыт объектов в массиве ObjectsStore
			SP.StatesMaterials = Kep.StatesMaterials;			// Записываем в объект SP состояния материалов Закрыт - C, Открыт - O, Купленн - B
			SP.StatesImprovements = Kep.StatesImprovements;		// Записываем в объект SP состояния улучшений Закрыт - С, Открыт - O, Купленн - B

			// И создаём новое сохранение с номером на 1 больше самого последнего сохранения
			FileStream SaveGame = new FileStream(Kep.path + "/My Games/Slide/Saves/Save_" + (Kep.SaveNomber) + ".bin", FileMode.Create);	// Создаём сохранение с номером Kep.Savenomber
			BinaryFormatter BinFor2 = new BinaryFormatter();					// Создаём объект класса серриализатора
			BinFor2.Serialize(SaveGame, SP);									// Серриализуем объект SP в поток SaveInf
			SaveGame.Close();													// Закрываем поток

			if(Saves.Length >= 3) File.Delete(OldestSave);						// Если количество сохранений в папке для сохранений больше или равно 3 удаляем самое старое сохранение
		
			ProfileText = "";													// Стираем имя в поле ввода имени
			WarningText = "<color='#50ab59'>Профиль успешно создан</color>";	// Выводим сообщение что пофиль успешно создан
			ButtonEnabled = false;												// Блокируем все кнопки в меню
//			StartCoroutine(Kep.FillObjTex(Kep.ObjectNombers[Kep.ActiveProfile], Kep.ActiveMats[Kep.ActiveProfile], Kep.ObjectsStates[Kep.ActiveProfile]));	// Фотографируем текстуры для купленных объектов Активного профиля
			yield return new WaitForSeconds(2);									// Ждём 2 секунды
			Window = MainMenuWins.MainMenu;										// Отправляем игрока в главное меню
			CreationProfile = false;											// Ставим переменую CreationProfile в положение ложь
			ProfileAv = Kep.DefaultAvatar;										// Ставим аватар в меню создания аватара по умолчанию
			WarningText = "";													// Стираем надпись имя подходит
			ProfileText = "Введите имя игрока";									// Ставим текст введите имя пользователя
			ButtonEnabled = true;												// Разблокируем кнопки меню
			TextPlayerName = Kep.NamesProfiles[EmptySlot];						// Указываем для переменной имени профиля имя созданного профиля
			GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (TextPlayerName.Length * 1.8f));// Высчитываем размер имени профиля
			StartCoroutine(Kep.FillActiveObjsTexs(Kep.ActiveProfile, false)); 	// Вызываем метод который фотографирует активные объекты и заполняет массив FPAA
		}
	}
}



