// Это скрипт дополняет скрипт меню паузы отрисовывая при необходимости некоторые кнопки и окна поверх элементов меню паузы (пока что это только кнопки для второго игрока поэтому этого скрипта нет в одиночоной игре)

using UnityEngine;
using System.Collections;

public class PauseMenuPlus : MonoBehaviour 
{
	public Texture2D BLSFT;						// (BlackoutLevelSettingsForTwo)Здесь лежит тектсра затемнения окна настроек уровня для двоих
	public PauseMenu PM;						// Сдесь лежит скрипт меню паузы
	public GameManager GM;						// Сдесь Лежит скрипт игрового менеджера
	public GUISkin GameSkin;					// Скин для всех гуи элементов
	Keeper Kep;									// Сдесь будет лежать скрипт хранитель
	GameObject IO;								// (IndestructibleObject) Переменная для игрового объекта
	public StoreAndInventory SAI;				// Сдесь будет лежать объект класса StoreAndInventory
	public bool SecondProfileSelection;			// Эта переменная говорит выбираем ли мы в данный момент профиль для второго игрока
	public bool ButtonProfileSelectionPushed;	// Эта переменная говорит была ли нажата кнопка выбора игрока

	float OriginalHight = 1080; 				// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	float RatioH;								// Сюда заноситься результат деления оригинальной высоты экрана на текущую


	void Start () 
	{
		RatioH = Screen.height / OriginalHight;			// Заносим в ScreenBalansHight результат деления описанный выше
		IO = GameObject.Find("IndestructibleObject");	// Находим в сцене скрипт StoreContent и ложим в переменную SC
		Kep = IO.GetComponent<Keeper>();				// Ложим в переменную Kep скрипт Keeper
		SAI = IO.GetComponent<StoreAndInventory>();		// Ложим в переменную SAI скрипт StoreAndInventory
	}
	

	void OnGUI()
	{
		GUI.depth = 1;							// Устанавливаем дальность гуи от камеры на первый слой
		GUI.skin = GameSkin;					// Устанавливаем игровой скин

		if(SecondProfileSelection)				// Если переменная SecondProfileSelection правда
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			GUI.BeginGroup(new Rect( -153, -450, 306, 390), BLSFT);																// Начинаем группу и затемняем окно настройки уровня

			if(Kep.SecondActiveProfile == 10)																					// Если у второго игрока нет активного профиля
			{
				if(GUI.Button(new Rect(15, 39, 276, 40), "_ _ _ _ _", "ButtonProfileSelection"))								// Отрисовываем кнопку с пустым именем профиля и если она нажата
					ButtonProfileSelectionPushed = ButtonProfileSelectionPushed ? false : true; 								// Меняем состояние переменной ButtonProfileSelectionPushed на противоположное													
			}
			else 																												// Иначе если у второго игрока есть актиивный профиль
			{
				if(GUI.Button(new Rect(15, 39, 276, 40), Kep.NamesProfiles[Kep.SecondActiveProfile], "ButtonProfileSelection"))	// Отрисовываем кнопку с выбранным именем профиля для второго игрока и если она нажата
					ButtonProfileSelectionPushed = ButtonProfileSelectionPushed ? false : true;									// Меняем состояние переменной ButtonProfileSelectionPushed на противоположное
			}

			Rect rect = new Rect(15, 95, 276, 40); 											// Объявляем новый прямоугольник

			for(byte a=0; a<5; a++)															// Продолжаем цикл пока не отрисуем все кнопки
			{
				if(ButtonProfileSelectionPushed)											// Если кнопка выбора профиля была нажата
				{
					if(GUI.Button(rect, Kep.NamesProfiles[a], "ButtonProfileSelection"))	// Отрисовываем кнопки отображающие имя профиля с номером цикла и если одна из них была нажата
					{
						if(Kep.ActiveProfile != a)											// И если мы выбрали друой профиль отличный от профиля игрока
						{
							if(Kep.NamesProfiles[a] != "_ _ _ _ _")							// И если это профиль не с пустым именем
							{
								if(GM.Currentkep != GM.Kep) GM.Currentkep = GM.Kep;			// Если в текущей базе данных лежит не глобальная база данных ложим в текущую базу данных глобальную базу данных 
								StartCoroutine(Kep.FillActiveObjsTexs(a, false));			// Заполняем переменные фотографий активных объектов для второго игрока
								PM.StartEnabled = true;										// Ставим переменной PM.StartEnabled значение правда означающее что можно нажать кнопку старта
							}
						}
						else 																// Иначе если мы выбрали для второго игрока тотже профиль что и для первого
						{
							if(GM.Currentkep != GM.LocalKep) GM.Currentkep = GM.LocalKep;   // Если в текущей базе данных лежит не локальная база данных ложим в текущую базу данных локальную базу данных 
							StartCoroutine(Kep.FillActiveObjsTexs(a, true));				// Заполняем переменные фотографий активных объектов для второго игрока
						}
						
					}
					rect.y += 45;															// Уменьшаем высоту следующей кнопки на 45 пикселей
				}
			}
			GUI.EndGroup();																	// Заканчиваем группу
		}
	}
}
