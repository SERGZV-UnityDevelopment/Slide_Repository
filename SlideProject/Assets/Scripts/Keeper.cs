// В этом скрипте храниться вся информация о профилях. Тут происходит извлечение информации для профилей и сохранение в профили, сдесь же находяться и переменные в которые мы сохраняем игру.
using UnityEngine;
//using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Keeper : MonoBehaviour 
{
	public Texture2D DefaultAvatar;				// Аватар по умолчанию("No avatar") для пустых профилей
	public GameObject Fotographer;				// Переменная фотограф сюда помещаеться префаб камеры с источником света
//	public GameObject IObj;						// Переменная для объекта "IndestructibleObject"
	public Texture2D NotExist;					// Стандартная текстура для несуществующих объектов в магазине
	public GameManager GM;						// Переменная для скрипта "GameManager"
	public PauseMenu PM;						// Переменная для скрипта "Меню паузы"
	public PauseMenuPlus PMP;					// Сдесь будет храниться скрипт меню паузы плюс
	public MainMenu MM;							// Если это нулевой уровень то тут будет лежать скрипт MainMenu
	public StoreContent SC;						// Переменная для скрипта "Контент магазина" (StoreContent)
	public StoreAndInventory SAI;				// Сдесь будет храниться переменная "Магазин и инвентарь"
	public string path;							// Сдесь будет лежать путь к папке мои документы
	public Texture2D[] Test = new Texture2D[4];	// Тестовая переменная для показа картинок с улучшением

//-------------------------------------------------------------------------------------Общие данные для профилей--------------------------------------------------------------------------------------------
	public Texture2D[] PlayersAv = new Texture2D[5];						// (Players Avatars) В этом массиве храняться аватары всех 5 профилей по одному на каждого
	public Texture2D[] FPAA;												// (FirstPlayerActiveAvatars) Отрендеренные аватарки для каждого активного объекта 1 игрока
	public Texture2D[] SPAA;												// (FirstPlayerActiveAvatars) Отрендеренные аватарки для каждого активного выбранного объекта 2 игрока
//	public Texture2D[] FPB;													// Задние фоны для последних объектов первого игрока
//	public Texture2D[] SPB;													// Задние фоны для последних объектов второго игрока
	public byte NomberPlayer = 1;											// Номер игрока для которого мы выбираем настройки и отрисовываем инвентарь игрок может быть либо 1 либо 2
	public byte ViewedProfile;												// Номер просматриваемого профиля(Либо первого либо второго игрока) профиль может быть 0,1,2,3,4
	public byte SecondActiveProfile = 10;									// Активный профиль для второго игрока
//-----------------------------------------------------------------------------------------------Данные из сохранения------------------------------------------------------------------------------------------------------------
	public decimal HashTab;													// Контрольная сумма
	public int SaveNomber;													// Номер сохранения
	public byte ActiveProfile;												// Активный профиль первого игрока - тот профиль которым мы играем при прохождении (10 значит нет активного профиля)
	public string[] NamesProfiles = new string[5];							// Массив имён профилей игроков(Номер в массиве означает номер профиля)
	public string[] PlayersAvs = new string[5];								// Массив адресов аватарок для профилей игроков (Номер в массиве означает номер профиля)
	public int[] Credits = new int[5];										// В этом массиве храняться "Кредиты" или деньги всех профилей (Номер в массиве означает номер профиля)
	public byte[] Progress = new byte[5];									// Массив номеров следующего уровеня который доступен для прохождения для каждого профиля (Номер в массиве означает номер профиля)
	public byte[] PlayedLevels = new byte[5];								// Последний сыгранный уровень для данного профиля
	public bool[] RightSide = new bool[5];									// Массив сторон на которых предпочитают играть игроки
	public bool[] AlienStyles = new bool[5];								// Массив предпочитаемых игроком стилей цифр. Земной или инопланетный (true означает инопланетный)
	public byte[] Difficulty = new byte[5];									// Уровни сложности выбранные для каждого игрока (0 - Детский) или (1 - Нормальный)

//----------------------------------------------------------------------Сохранения закрытых, открытых и купленных элементов магазина игроков-------------------------------------------------------------------------------------

	public short[,] ActiveObjects = new short[5,4];						// Массив последние 4 выбранных объекта активного игрока. Бита, шайба, стол и скабокс. (Для каждого профиля)
	public byte[][] ActiveMats = new byte[5][];							// Массив массивов номеров активных материалов для объектов в массиве ObjectsStore
	public byte[][] ActiveImprs = new byte[5][];						// Массив массивов номеров активных материалов для улучшений в массиве ObjectsStore
	public char[][] ObjectsStates = new char[5][];						// Массив массивов состояний объектов для массива ObjectsStore (Закрыт - C) (Открыт - O) (Куплен - B) 
	public char[][] SkyboxesStates = new char[5][];						// Массив массивов состояний скайбоксов для массива SkyboxMats (Закрыт - C) (Открыт - O) (Купленн - B)
	public int[][] ObjectsExpirience = new int[5][];					// Массив массивов опыта объектов в массиве Objects Store 
	public List<char>[][] StatesMaterials = new List<char>[5][];		// В этом массиве массивов храняться состояния материалов для выделенной ячейки (C - Закрыт),(O - Открыт),(B - Куплен)
	public List<char>[][] StatesImprovements = new List<char>[5][];		// В этом массиве списков храняться состояния улучшений для выделенной ячейки (C - Закрыт),(O - Открыт),(B - Куплен)

	public RenderTexture rt;
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	


	void OnEnable()
	{
		path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 												// Создаём/получаем путь к папке мои документы
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();														// Ложим объект скрипт GameManager в переменную GM
		GM.SaveGameEvent += SaveGame;																							// Подписываем метод SaveGame на событие SaveGameEvent
		GM.LoadGameEvent += LoadGame;																							// Подписываем метод LoadGame на событие LoadGameEvent	
//	 	GM.StartFirstStep += _StartFirstStep_;																					// Подписываем метод _StartFirstStep_ на событие StartFirstStep
	}


	void Start()
	{
		SC = gameObject.GetComponent<StoreContent>();																			// Ложим скрипт StoreContent в переменную SC
		SAI = gameObject.GetComponent<StoreAndInventory>();																		// Ложим скрипт StoreAndInventory в переменную SAI
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 && ActiveProfile != 10)					// Если загруженная сцена это сцена главного меню и есть выбранный активный профиль
			StartCoroutine(FillActiveObjsTexs(ActiveProfile, false));															// Фотографируем иконки для активных объектов 1 игрока

	}
	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


	public void FillLocalDataBase(byte NomSelProf)												// Этот метод записывает информацию в локальную базу данных
	{
		PlayersAv.CopyTo(GM.LocalKep.PlayersAv, 0);												// Копируем массив PlayersAv из глобальной базы в локальную
		NamesProfiles.CopyTo(GM.LocalKep.NamesProfiles,0);										// Заполняем массив имена игроков локальной базы данных
		Credits.CopyTo(GM.LocalKep.Credits,0);													// Заполняем массив количество кредитов игроков локальной базы данных
		for(byte b=0; b<ActiveObjects.GetLength(NomSelProf); b++) {GM.LocalKep.ActiveObjects[NomSelProf,b] = ActiveObjects[NomSelProf,b];}	// Копируем активные объекты для данного профиля
		GM.LocalKep.ActiveMats[NomSelProf] = new byte[375];										// Создаём массив материалов для данного профиля в локальной базе данных
		GM.LocalKep.ActiveImprs[NomSelProf] = new byte[375];									// Создаём массив улучшений для данного профиля в локальной базе данных
		GM.LocalKep.ObjectsExpirience[NomSelProf] = new int[375];								// Создаём массив опыта объектов для текущего профиля в локальной базе данных
		GM.LocalKep.ObjectsStates[NomSelProf] = new char[375];									// Создаём массив состояний объектов для данного профиля в локальной базе данных
		GM.LocalKep.SkyboxesStates[NomSelProf] = new char[125]; 								// Создаём массив состояний скайбоксов для данного профиля в локальной базе данных
		GM.LocalKep.StatesMaterials[NomSelProf] = new System.Collections.Generic.List<char>[375];		// Создаём массив состояний материалов объектов в локальной базе данных
		GM.LocalKep.StatesImprovements[NomSelProf] = new System.Collections.Generic.List<char>[375];	// Создаём массив состояний улучшений объектов в локальной базе данных
		ActiveMats[NomSelProf].CopyTo(GM.LocalKep.ActiveMats[NomSelProf],0);					// Копируем массив материалов для данного профиля из глобальной базы в локальную
		ActiveImprs[NomSelProf].CopyTo(GM.LocalKep.ActiveImprs[NomSelProf],0);					// Копируем массив улучшений для данного профиля из глобальной базы в локальную
		ObjectsExpirience[NomSelProf].CopyTo(GM.LocalKep.ObjectsExpirience[NomSelProf],0);		// Копируем массив опыта объектов для данного профиля  из глобальной базы в локальную
		ObjectsStates[NomSelProf].CopyTo(GM.LocalKep.ObjectsStates[NomSelProf],0);				// Копируем массив состояний объектов для данного профиля из глобальной базы в локальную 
		SkyboxesStates[NomSelProf].CopyTo(GM.LocalKep.SkyboxesStates[NomSelProf],0);			// Копируем массив состояний скайбоксов для данного профиля из глобальной базы в локальную 
		StatesMaterials[NomSelProf].CopyTo(GM.LocalKep.StatesMaterials[NomSelProf],0);			// Копируем массив состояний материалов объектов для данного профиля из глобальной базы в локальную
		StatesImprovements[NomSelProf].CopyTo(GM.LocalKep.StatesImprovements[NomSelProf],0);	// Копируем массив состояний улучшений объектов для данного профиля из глобальной базы в локальную
		GM.LocalKep.NamesProfiles[NomSelProf] = "Гость";										// Ставим ник второго игрока как гость
		PM.StartEnabled = true;																	// Ставим переменной PM.StartEnabled значение правда означающее что можно нажать кнопку старта

//		if(PM.Loading == false)													// Если переменная загрузка равна ложь значит мы производим загрузку при старте и нужно установить начальные значения
//		{
//			NomberPlayer = 1;													// Выбираем окно настроек первого игрока
//			ViewedProfile = ActiveProfile;										// Номер просматриваемого профиля берём у 1 игрока
//			GM.Currentkep = this;												// Указываем как текущую базу данных, глобальную базу данных
//			GM.AlternateFirstStep();  											// Вызываем метод продолжающий первый шаг
//		}
	}


	public IEnumerator FillActiveObjsTexs(byte NomberProfile, bool LocalBaseLoad)				// Этот метод фотографирует 4 активных объекта для выбранного игрока и заполняет ими соответствующие переменные
	{
		RenderTexture RendTex = new RenderTexture(120, 120, 0, RenderTextureFormat.ARGB32);		// Рендер текстура в которую будет рендериться изображение
		RendTex.antiAliasing = 8;							// Ставим уровень сглаживания рендер текстуры на 8
		Texture2D[] Texs = new Texture2D[4];				// Создаём новый массив временных текстур
		Texture2D[] Backgrounds = new Texture2D[3];			// Создаём новый массив задних фонов для объектов
		GameObject CamEmpty;								// Сдесь лежит пустышка на которой весит компонент камера
		Skybox Sky;											// Переменная для компонента фотографа скайбокс
		Camera CamScript;									// Ссылка на клон камеры
		GameObject ObjClone;								// Ссылка на клон фотографируемого объекта
		ObjectScript ObjScr;								// Компонент ObjectScript фотографируемого объекта
		GameObject Improvement;								// Ссылка на клон улучшения фотографируемого объекта
		short ObjectNom;									// Сдесь будет лежать номер фотографируемого объекта в массиве ObjectsStore
		byte NomActiveMat;									// Номер активного материала текущего объекта
		byte ImprNom;										// Номер сохранённого улучшения для этого объекта

		CamEmpty = Instantiate(Fotographer, new Vector3(-5, 100, -1.4f), Quaternion.identity) as GameObject;	// Помещаем на сцену префаб фотограф и ложим ссылку на него в переменную CamEmpty
		Sky = CamEmpty.GetComponent<Skybox>();										// Ложим компонент фотографа "Скайбокс" в переменную Sky
		CamScript = CamEmpty.GetComponent<Camera>();								// Из CamEmpty компонент Camera ложим в переменную CamScript	
		CamScript.clearFlags = CameraClearFlags.Skybox;								// Настраиваем камеру на фотографию объекта с задним фоном
		CamScript.targetTexture = RendTex;											// Устанавливаем для камеры рендер текстуру
		CamScript.pixelRect = new Rect(0, 0, 120, 120);								// Устанавливаем разрешение для второй камеры

	
		for(byte a=0; a<4; a++)														// Продолжаем цикл пока не пройдём 4 итерации
		{
			if(a != 3)																// Если в этом цикле мы работаем с объектом
			{
				if(ActiveObjects[NomberProfile,a] != -1)							// Если есть активный объект для этой категори
				{
					ObjectNom = ActiveObjects[NomberProfile,a];									// Помещаем номер активного объекта для этой категории в массиве ObjectsStore в переменную ObjectNom	
					NomActiveMat = ActiveMats[NomberProfile][ActiveObjects[NomberProfile,a]];	// Присваиваем номер активного материала для текущего объекта
					ObjClone = Instantiate(SC.ObjectsStore[ObjectNom], new Vector3(-5, 100.12f, 0f), Quaternion.AngleAxis(45, new Vector3(-1, 0, 0))) as GameObject;	// Инстантируем на сцену префаб фотографируемого объекта
					ObjScr = ObjClone.GetComponent<ObjectScript>();								// Помещаем компонент ObjectScript Фотографируемого объекта в переменную ObjScr

					if(a < 2)																	// Если мы фотографируем биту или шайбу
					{
						ObjClone.GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomActiveMat];			// Присваиваем объекту номер его сохранённого материала

						if(ObjScr.SecondMaterials.Length > 0)														// Если у обрабатываемого объекта существует второй материал
							ObjClone.GetComponent<Renderer>().materials[1] = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Мы присваиваем сохранённый не стандартный второй материал
							
						if(ObjScr.ThirdMaterials.Length > 0)														// Если у обрабатываемого объекта существует третий материал
						{
							ObjClone.GetComponent<Renderer>().materials[2] = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Мы присваиваем сохранённый не стандартный третий материал
						}

						if(ActiveImprs[NomberProfile][ObjectNom] != 10)							// Если для этого объекта присутствует активное улучшение
						{
							ImprNom = ActiveImprs[NomberProfile][ObjectNom];					// Узнаём номер улучшения для этого объекта в скрипте ObjScr и помещаем в переменную ImprNomber
							Improvement = (GameObject)Instantiate(ObjScr.Improvements[ImprNom], ObjClone.transform.position, ObjClone.transform.rotation);	// Ставим улучшение на место фотографируемого объекта
							Improvement.transform.parent = ObjClone.transform;					// Назначаем фотографируемый объект родителем улучшения 
	
						}
					}
					else if(a == 2)																// Иначе если мы работаем с игровым столом
					{
						ObjClone.transform.GetChild(2).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomActiveMat]; 	// Присваиваем материал бордюру стола
						ObjClone.transform.GetChild(1).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Присваеваем материал полю стола
						if(ObjScr.ThirdMaterials.Length > 0 && ObjScr.ThirdMaterials[NomActiveMat])						// Если у поля есть второй материал
						{
							Material[] Matsmass = new Material[2];														// Создаём новый массив материалов длинной два материала
							Matsmass[0] = ObjScr.SecondMaterials[NomActiveMat];											// Назначаем как первый элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
							Matsmass[1] = ObjScr.ThirdMaterials[NomActiveMat];											// Назначаем как второй элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
							ObjClone.transform.GetChild(1).GetComponent<Renderer>().materials = Matsmass;				// Присваиваем MatsMats как новые материалы поля
						}
					}

					float Size = SC.ObjectsStore[ObjectNom].GetComponent<ObjectScript>().ViewSize;						// Берём размер объекта для фотографии и копируем его в переменную Size
					Vector3 Size3 = new Vector3(Size, Size, Size);														// Создаём новый вектор3 с именем Size3 и заполняем все его оси из переменной Size
					ObjClone.transform.localScale = Size3;																// Придаём новому клону объекта размер указанный в Size3
			
					for(byte b=1; b<6; b++)
					{
						if(ObjScr.ObjectLevel == b) Sky.material = SAI.Skyboxes[b-1];									// Если уровень обрабатываемого объекта равен "b" то мы присваиваем камере скабокс соответствующий его уровню
					}

					yield return new WaitForEndOfFrame();																// Ждём конца отрисовки текущего кадра
					CamScript.Render();																					// Рендерим изображение с камеры клона в рендер текстуру
					RenderTexture.active = RendTex;																		// Ставим RendText активной рендер текстурой
					Texs[a] = new Texture2D(120, 120, TextureFormat.ARGB32, false);										// Создаём текстуру 2D и ложим её во временный массив иконок активных объектов
					Texs[a].ReadPixels(new Rect(0, 0, 120, 120), 0,0, false);											// Считываем изображение с активной рендер-текстуры в Texs[a]
					Texs[a].Apply();																					// Применяем изображение
					RenderTexture.active = null;																		// Обнуляем активную рендер текстуру
					GameObject.DestroyImmediate(ObjClone);																// удаляем объект
				}
				else 											// Иначе если для этой категории нет активного объекта
				{
					Texs[a] = NotExist;							// В этом случае мы ставим ему текстуру NotExist
				}
			}
			else 												// Иначе если в этом цикле мы работаем со скайбоксом
			{
				if(ActiveObjects[NomberProfile, a] != -1)		// И Если этот слот не равен -1 значит для этой категории есть активный объект
				{
					Texs[a] = SC.SkyboxScreens[ActiveObjects[NomberProfile, a]]; 	// В этом случае присваиваем сохранённую текстуру скришота скайбокса 4 элементу
				}
				else 											// Иначе если 4 переменная LastObjs равна -1 значит для этого объекта ещё нет сохранения				
				{
					Texs[3] = NotExist;							// В этом случае мы ставим ему текстуру NotExist
				}
			}
		}

		if(NomberPlayer == 1)									// Если мы рендерили изображения для 1 игрока
		{
			FPAA = Texs;										// Присваиваем массив временных текстур массиву FPAA
		}
		else if(NomberPlayer == 2)								// Иначе если мы рендерили изображения для 2 игрока
		{		
			GM.Currentkep.SPAA = Texs;							// Присваиваем массив временных текстур массиву SPAA текущей базы данных
			SecondActiveProfile	= NomberProfile;				// Присваиваем переменной Kep.SecondActiveProfile номер выбранного профиля
			ViewedProfile = SecondActiveProfile;				// Присваиваем переменной Viewed profie номер второго профиля
			PMP.ButtonProfileSelectionPushed = false;			// Ставим состояние переменной ButtonProfileSelectionPushed
			PMP.SecondProfileSelection = false;					// Ставим значение переменной SecondProfileSelection false
			PM.LevelSettingsEnabled = true;						// Ставим значение переменной LevelSettingsEnabled true
			if(LocalBaseLoad) 									// Если переменная загрузка локальной базы правда
				FillLocalDataBase(NomberProfile);				// Вызываем метод загрузки глобальной базы в локальную базу
			
			GM.RestartGameObjects();							// Вызываем метод RestartGameObjects чтобы поставить новые выбранные объекты и скайбокс на сцену
		}
		GameObject.DestroyImmediate(CamEmpty);					// Удаляем префаб фотограф
	}


	public IEnumerator FillActiveObjTex(byte NomberProfile, short NomberObject, byte NomberCat)	// Этот метод фотографирует выбранный объект для выбранного игрока и заполняет ими соответствующие переменные
	{
		RenderTexture RendTex = new RenderTexture(120, 120, 24, RenderTextureFormat.ARGB32);	// Рендер текстура в которую будет рендериться изображение
		Texture2D Tex;										// Создаём временную текстуру
		Texture2D Background = new Texture2D(120, 120);		// Это временная переменная для заднего фона
		GameObject CamEmpty;								// Сдесь лежит пустышка на которой весит компонент камера
		Skybox Sky;											// Переменная для компонента фотографа скайбокс
		Camera CamScript;									// Ссылка на клон камеры
		GameObject ObjClone;								// Ссылка на клон фотографируемого объекта
		ObjectScript ObjScr;								// Компонент ObjectScript фотографируемого объекта
		GameObject Improvement;								// Ссылка на клон улучшения фотографируемого объекта
		byte NomActiveMat;									// Номер активного материала текущего объекта
		byte ImprNom;										// Номер сохранённого улучшения для этого объекта

		CamEmpty = Instantiate(Fotographer, new Vector3(-5, 100, -1.4f), Quaternion.identity) as GameObject;	// Помещаем на сцену клон префаба фотографа и ложим ссылку на него в переменную CamEmpty
		Sky = CamEmpty.GetComponent<Skybox>();										// Ложим компонент фотографа "Скайбокс" в переменную Sky
		CamScript = CamEmpty.GetComponent<Camera>();								// Из CamEmpty компонент Camera ложим в переменную CamClone	
		CamScript.clearFlags = CameraClearFlags.Skybox;								// Настраиваем камеру на фотографию объекта с задним фоном
		CamScript.targetTexture = RendTex;											// Устанавливаем для камеры рендер текстуру
		CamScript.pixelRect = new Rect(0, 0, 120, 120);								// Устанавливаем разрешение для второй камеры

		if(NomberCat != 3)															// Если в этом цикле мы работаем с объектом
		{
			if(ActiveObjects[NomberProfile,NomberCat] != -1)						// Если есть активный объект для этой категории
			{
				NomActiveMat = GM.Currentkep.ActiveMats[NomberProfile][GM.Currentkep.ActiveObjects[NomberProfile,NomberCat]];		// Присваиваем номер активного материала для текущего объекта

				ObjClone = Instantiate(SC.ObjectsStore[NomberObject], new Vector3(-5, 100.12f, 0f), Quaternion.AngleAxis(45, new Vector3(-1, 0, 0))) as GameObject;	// Инстантируем на сцену префаб фотографируемого объекта
				ObjScr = ObjClone.GetComponent<ObjectScript>();											// Помещаем компонент ObjectScript Фотографируемого объекта в переменную ObjScr

				if(NomberObject<249)																	// Если мы фотографируем биту или шайбу
				{
					ObjClone.GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomActiveMat];			// Присваиваем объекту номер его сохранённого материала

					if(ObjClone.GetComponent<ObjectScript>().SecondMaterials.Length > 0)														// Если у обрабатываемого объекта существует второй материал
						ObjClone.GetComponent<Renderer>().materials[1] = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Мы присваиваем сохранённый не стандартный второй материал

					if(ObjClone.GetComponent<ObjectScript>().ThirdMaterials.Length > 0)															// Если у обрабатываемого объекта существует третий материал
					{
						ObjClone.GetComponent<Renderer>().materials[2] = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Мы присваиваем сохранённый не стандартный третий материал
					}

					if(GM.Currentkep.ActiveImprs[NomberProfile][NomberObject] != 10)						// Если для этого объекта присутствует активное улучшение
					{
						
						ImprNom = GM.Currentkep.ActiveImprs[NomberProfile][NomberObject];	// Узнаём номер улучшения для этого объекта в скрипте ObjScr и помещаем в переменную ImprNomber
						Improvement = (GameObject)Instantiate(ObjScr.Improvements[ImprNom], ObjClone.transform.position, ObjClone.transform.rotation);	// Ставим улучшение на место фотографируемого объекта
						Improvement.transform.parent = ObjClone.transform;					// Назначаем фотографируемый объект родителем улучшения 
					}
				}
				else 																		// Иначе если мы работаем с игровым столом
				{
					ObjClone.transform.GetChild(2).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomActiveMat]; 	// Присваиваем материал бордюру стола
					ObjClone.transform.GetChild(1).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomActiveMat];	// Присваеваем материал полю стола
					if(ObjScr.ThirdMaterials.Length > 0 && ObjScr.ThirdMaterials[NomActiveMat])
					{
						Material[] Matsmass = new Material[2];														// Создаём новый массив материалов длинной два материала
						Matsmass[0] = ObjScr.SecondMaterials[NomActiveMat];											// Назначаем как первый элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
						Matsmass[1] = ObjScr.ThirdMaterials[NomActiveMat];											// Назначаем как второй элемента массива Matsmass материал с номером MatNomber из массива SecondMaterials
						ObjClone.transform.GetChild(1).GetComponent<Renderer>().materials = Matsmass;				// Присваиваем MatsMats как новые материалы поля
					}
				}

				float Size = SC.ObjectsStore[NomberObject].GetComponent<ObjectScript>().ViewSize;					// Берём размер объекта для фотографии и копируем его в переменную Size
				Vector3 Size3 = new Vector3(Size, Size, Size);														// Создаём новый вектор3 с именем Size3 и заполняем все его оси из переменной Size
				ObjClone.transform.localScale = Size3;																// Придаём новому клону объекта размер указанный в Size3
			
				for(byte b=1; b<6; b++)
				{
					if(ObjScr.ObjectLevel == b) Sky.material = SAI.Skyboxes[b-1];									// Если уровень обрабатываемого объекта равен "b" то мы присваиваем камере скабокс соответствующий его уровню
				}

				yield return new WaitForEndOfFrame();																// Ждём конца отрисовки текущего кадра
				CamScript.Render();																					// Рендерим изображение с камеры клона в рендер текстуру
				RenderTexture.active = RendTex;																		// Ставим RendText активной рендер текстурой
				Tex = new Texture2D(120, 120, TextureFormat.ARGB32, false);											// Создаём текстуру 2D и ложим её во временную переменную 
				Tex.ReadPixels(new Rect(0, 0, 120, 120), 0,0, false);												// Считываем изображение с активной рендер-текстуры во временную переменную
				Tex.Apply();																						// Применяем изображение
				RenderTexture.active = null;																		// Обнуляем активную рендер текстуру
				GameObject.DestroyImmediate(ObjClone);																// удаляем объект

		//		if(ObjScr.ObjectLevel == 1)	Background = SAI.ObjsBackgrounds[0];		// Если уровень этого объекта указан как первый то присваиваем задний фон для первого уровня
		//		else if(ObjScr.ObjectLevel == 2) Background = SAI.ObjsBackgrounds[1];	// Если уровень этого объекта указан как второй то присваиваем задний фон для второго уровня
		//		else if(ObjScr.ObjectLevel == 3) Background = SAI.ObjsBackgrounds[2];	// Если уровень этого объекта указан как третий то присваиваем задний фон для третьего уровня
		//		else if(ObjScr.ObjectLevel == 4) Background = SAI.ObjsBackgrounds[3];	// Если уровень этого объекта указан как четвёртый то присваиваем задний фон для четвёртого уровня
		//		else if(ObjScr.ObjectLevel == 5) Background = SAI.ObjsBackgrounds[4];	// Если уровень этого объекта указан как пятый то присваиваем задний фон для пятого уровня
			}
			else 										// Иначе если для этой категории нет активного объекта
			{
				Tex = NotExist;							// В этом случае мы ставим ему текстуру NotExist
			}
		}
		else 											// Иначе если в этом цикле мы работаем со скайбоксом
		{
			if(ActiveObjects[NomberProfile, NomberObject] != -1)						// И Если этот слот не равен -1 значит для этой категории есть активный объект
			{
				Tex = SC.SkyboxScreens[NomberObject]; 	// В этом случае присваиваем текстуру выбранного скайбокса переменной Tex
			}
			else 										// Иначе если 4 переменная LastObjs равна -1 значит для этого объекта ещё нет сохранения				
			{
				Tex = NotExist;							// В этом случае мы ставим ему текстуру NotExist
			}
		}
			
		if(NomberPlayer == 1)							// Если мы рендерили изображения для 1 игрока
		{
			FPAA[NomberCat] = Tex;						// Присваиваем временную текстуру высчитанному элементу массива FPAA
//			if(NomberCat < 3)							// Если мы работали не со скайбоксом
//				FPB[NomberCat] = Background;			// Присваиваем временный задний фон высчитанному элементу массива FPB
		}
		else if(NomberPlayer == 2)						// Иначе если мы рендерили изображения для 2 игрока
		{
			GM.Currentkep.SPAA[NomberCat] = Tex;			// Присваиваем временную текстуру высчитанному элементу массива SPAA текущей базы данных
//			if(NomberCat < 3)								// Если мы работали не со скайбоксом
//				GM.Currentkep.SPB[NomberCat] = Background;	// Присваиваем временный задний фон высчитанному элементу массива SPB текущей базы данных
		}

		GameObject.DestroyImmediate(CamEmpty);				// Удаляем префаб фотограф
	}








	/*
	// Этот метод фотографирует купленные объекты из массива ObjectsStore и заполняет этими фотографиями переменные фотографий для инвентаря
	public IEnumerator FillObjTex(byte[] ActiveMats_, char[] ObjectState)		
	{
		RenderTexture RendTex = new RenderTexture(100, 100, 24, RenderTextureFormat.ARGB32);				// Рендер текстура в которую сначала будет рендериться изображение
		GameObject CamEmpty;		// Сдесь лежит пустышка на которой весит компонент камера
		Camera CamScript;			// Ссылка на клон камеры
		GameObject ObjClone;		// Ссылка на клон фотографируемого объекта
		ObjectScript ObjScr;		// Компонент ObjectScript фотографируемого объекта
		GameObject Improvement;		// Ссылка на клон улучшения фотографируемого объекта
		byte ImprNom;				// Номер сохранённого улучшения для этого объекта

		CamEmpty = Instantiate(Fotographer, new Vector3(0, -6, -1.4f), Quaternion.identity) as GameObject;	// Помещаем на сцену префаб фотограф и ложим ссылку на него в переменную CamEmpty
		CamScript = CamEmpty.GetComponent<Camera>();								// Из CamEmpty компонент Camera ложим в переменную CamClone					
		CamScript.targetTexture = RendTex;											// Устанавливаем для камеры рендер текстуру
		CamScript.pixelRect = new Rect(0,0,100,100);								// Устанавливаем разрешение для второй камеры

		for(int a=0; a<ObjectState.Length; a++)							 			// Продолжаем цикл пока не пройдём весь массив ObjectState для активного профиля
		{
			if(ObjectState[a] == 'B')			// Если объект с номером цикла в массиве ObjectState куплен а не просто открыт
			{	
			//	ME = (short)ObjectNombers[a];	// То мы получаем его номер в массиве SC.ObjectStore и ложим в переменную ME
				// помещаем очередной клон очередного объекта на сцену а также ложим ссылку на этот клон в переменную ObjClone
				ObjClone = Instantiate(SC.ObjectsStore[a], new Vector3(0,-6,0), Quaternion.AngleAxis(45, new Vector3(-1, 0, 0))) as GameObject;
				if(a<249)											// Если мы работаем с шайбой или битой а не столом
				{
					if(ActiveMats_[a] != 0)							// Если номер материала у этого объекта не стандартный (Не нулевой)
					{
						// Мы присваиваем сохранённый не стандартный первый или единственный материал
						ObjClone.GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[ActiveMats_[a]];	// То мы присваиваем ему номер сохранённого нестандартного материала
						
						// Если у обрабатываемого объекта второй материал существует
						if(ObjClone.GetComponent<ObjectScript>().SecondMaterials.Length > 0)
						{
							// Мы присваиваем сохранённый не стандартный второй материал
							ObjClone.GetComponent<Renderer>().materials[1] =
								ObjClone.GetComponent<ObjectScript>().SecondMaterials[ActiveMats_[a]];
						}
						
						// Если у обрабатываемого объекта третий материал существует
						if(ObjClone.GetComponent<ObjectScript>().ThirdMaterials.Length > 0)
						{
							// Мы присваиваем сохранённый не стандартный третий материал
							ObjClone.GetComponent<Renderer>().materials[2] =
								ObjClone.GetComponent<ObjectScript>().SecondMaterials[ActiveMats_[a]];
						}
					}

					if(ActiveImprs[ActiveProfile][a] != 10)									// Если для этого объекта присутствует активное улучшение															
					{
						ObjScr = ObjClone.GetComponent<ObjectScript>();						// Помещаем компонент ObjectScript Фотографируемого объекта в переменную ObjScr
						ImprNom = ActiveImprs[ActiveProfile][a];							// Узнаём номер улучшения для этого объекта в скрипте ObjScr и помещаем в переменную ImprNomber
						Improvement = (GameObject)Instantiate(ObjScr.Improvements[ImprNom], ObjClone.transform.position, ObjClone.transform.rotation);	// Ставим улучшение на место фотографируемого объекта
						Improvement.transform.parent = ObjClone.transform;					// Назначаем фотографируемый объект родителем улучшения 
					}
				}
				else 												// Иначе если мы работаем с игровым столом
				{
					if(ActiveMats_[a] != 0)							// Если номер материала у этого стола не стандартный (Не нулевой)
					{
						// Присваиваем полю этого стола первый материал
						ObjClone.transform.GetChild(0).GetComponent<Renderer>().materials[0] = 
							ObjClone.GetComponent<ObjectScript>().FirstMaterials[a];
						
						// Присваеваем бордюру этого стола третий материал
						ObjClone.transform.GetChild(1).GetComponent<Renderer>().materials[0] = 
							ObjClone.GetComponent<ObjectScript>().ThirdMaterials[a];
						
						if(ObjClone.GetComponent<ObjectScript>().SecondMaterials.Length > 0) // Если у поля стола имееться второй материал
						{
							// То мы присваеваем ему второй материал
							ObjClone.transform.GetChild(0).GetComponent<Renderer>().materials[1] = 
								ObjClone.GetComponent<ObjectScript>().SecondMaterials[a];
						}
					}
				}
				// Придаём новому клону объекта размер указанный в скрипте его префаба
				ObjClone.transform.localScale = SC.ObjectsStore[a].GetComponent<ObjectScript>().ViewSize;
				// Создаём текстуру 2D и ложим её в переменную для инвентаря того объекта который мы сфотографировали
				SC.ObjectsStore[a].GetComponent<ObjectScript>().FotoInventory = new Texture2D(100, 100, TextureFormat.ARGB32, false);
				yield return new WaitForEndOfFrame();									// Ждём конца отрисовки текущего кадра
				CamScript.Render();														// Рендерим изображение с камеры клона в рендер текстуру
				RenderTexture.active = RendTex;											// Ставим RendText активной рендер текстурой
				// Присваеваем изображение сделанное камерой переменной texture его же скрипта
				SC.ObjectsStore[a].GetComponent<ObjectScript>().FotoInventory.ReadPixels(new Rect(0, 0, 100, 100), 0,0, false);
				SC.ObjectsStore[a].GetComponent<ObjectScript>().FotoInventory.Apply();			// Применяем текстуру
				RenderTexture.active = null;													// Обнуляем активную рендер текстуру
				GameObject.DestroyImmediate(ObjClone);											// удаляем объект
			}
		}
		GameObject.DestroyImmediate(CamEmpty);													// Удаляем префаб фотограф
		FPAA = FillLastTextures();																// Заполняем массив текстур последних 4 объектов 1 игрока
	}


	public IEnumerator FotoObjTex(short ObjNom)													// Этот метод фотографирует одну текстуру для только что изменённого объекта
	{
		RenderTexture RendTex = new RenderTexture(100, 100, 24, RenderTextureFormat.ARGB32);	// Рендер текстура в которую сначала будет рендериться изображение
		GameObject CamEmpty;								// Сдесь лежит клон префаба фотограф на которой весит компонент камера
		Camera CamScript;									// Ссылка скрипт клона камеры
		GameObject ObjClone;								// Ссылка на клон фотографируемого объекта
		ObjectScript ObjScr;								// Компонент ObjectScript фотографируемого объекта
		GameObject Improvement;								// Ссылка на клон улучшения фотографируемого объекта
		byte ImprNom;										// Номер сохранённого улучшения для этого объекта
		byte MatNom = ActiveMats[ActiveProfile][ObjNom];	// Номер материала с которым мы работем

		CamEmpty = Instantiate(Fotographer, new Vector3(0, -6, -1.4f), Quaternion.identity) as GameObject;	// Помещаем на сцену клон префаба фотограф и ложим ссылку на него в переменную CamEmpty
		CamScript = CamEmpty.GetComponent<Camera>();								// Из CamEmpty компонент Camera ложим в переменную CamClone					
		CamScript.targetTexture = RendTex;											// Устанавливаем для камеры рендер текстуру
		CamScript.pixelRect = new Rect(0,0,100,100);								// Устанавливаем разрешение для второй камеры

		// помещаем клон объекта на сцену а также ложим ссылку на этот клон в переменную ObjClone
		ObjClone = Instantiate(SC.ObjectsStore[ObjNom], new Vector3(0,-6,0), Quaternion.AngleAxis(45, new Vector3(-1, 0, 0))) as GameObject;
		if(ObjNom<249)											// Если мы работаем с шайбой или битой а не столом
		{
			ObjClone.GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[MatNom];	// Мы присваиваем сохранённый не стандартный первый или единственный материал
			// Если у обрабатываемого объекта второй материал существует
			if(ObjClone.GetComponent<ObjectScript>().SecondMaterials.Length > 0)
			{
				// Мы присваиваем сохранённый не стандартный второй материал
				ObjClone.GetComponent<Renderer>().materials[1] =
					ObjClone.GetComponent<ObjectScript>().SecondMaterials[MatNom];
			}
			// Если у обрабатываемого объекта третий материал существует
			if(ObjClone.GetComponent<ObjectScript>().ThirdMaterials.Length > 0)
			{
				// Мы присваиваем сохранённый не стандартный третий материал
				ObjClone.GetComponent<Renderer>().materials[2] =
					ObjClone.GetComponent<ObjectScript>().SecondMaterials[MatNom];
			}

			if(ActiveImprs[ActiveProfile][ObjNom] != 10)							// Если для этого объекта присутствует активное улучшениеулучшение																
			{
				ObjScr = ObjClone.GetComponent<ObjectScript>();						// Помещаем компонент ObjectScript Фотографируемого объекта в переменную ObjScr
				ImprNom = ActiveImprs[ActiveProfile][ObjNom];						// Узнаём номер улучшения для этого объекта в скрипте ObjScr и помещаем в переменную ImprNomber
				Improvement = (GameObject)Instantiate(ObjScr.Improvements[ImprNom], ObjClone.transform.position, ObjClone.transform.rotation);	// Ставим улучшение на место фотографируемого объекта
				Improvement.transform.parent = ObjClone.transform;					// Назначаем фотографируемый объект родителем улучшения 
			}
		}
		else // Иначе если мы работаем с игровым столом
		{
			ObjClone.transform.GetChild(0).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[MatNom]; 		// Присваиваем материал бордюру стола
			ObjClone.transform.GetChild(1).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().SecondMaterials[MatNom];		// Присваиваем первый материал полю стола

			if(ObjClone.GetComponent<ObjectScript>().ThirdMaterials.Length > 0 && ObjClone.GetComponent<ObjectScript>().ThirdMaterials[MatNom]) 	// Если длинна третьего массива текстур больше ноля и имеет третий материал с номером MatNom
			{
				ObjClone.transform.GetChild(1).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().ThirdMaterials[MatNom];	// Присваиваем его как второй материал полю стола
			}
		}

		ObjClone.transform.localScale = SC.ObjectsStore[ObjNom].GetComponent<ObjectScript>().ViewSize;	// Придаём новому клону объекта размер указанный в скрипте его префаба
		SC.ObjectsStore[ObjNom].GetComponent<ObjectScript>().FotoInventory = new Texture2D(100, 100, TextureFormat.ARGB32, false); // Создаём текстуру 2D и ложим её в переменную для инвентаря того объекта который мы сфотографировали
		yield return new WaitForEndOfFrame();															// Ждём конца отрисовки текущего кадра
		CamScript.Render();																				// Рендерим изображение с камеры клона в рендер текстуру
		RenderTexture.active = RendTex;																	// Ставим RendText активной рендер текстурой
		SC.ObjectsStore[ObjNom].GetComponent<ObjectScript>().FotoInventory.ReadPixels(new Rect(0, 0, 100, 100), 0,0, false); // Присваеваем изображение сделанное камерой переменной texture его же скрипта
		SC.ObjectsStore[ObjNom].GetComponent<ObjectScript>().FotoInventory.Apply();			// Применяем текстуру
		RenderTexture.active = null;														// Обнуляем активную рендер текстуру
		GameObject.DestroyImmediate(ObjClone);												// удаляем объект
		GameObject.DestroyImmediate(CamEmpty);												// Удаляем префаб фотограф
		FPAA = FillLastTextures();															// Заполняем массив текстур последних 4 объектов 1 игрока
	}


	public Texture2D[] FillLastTextures()						// Этот метод используя массив LastObjects ложит текстуры сохранённых 4 объектов в массив FPLMA
	{
		Texture2D[] Texs = new Texture2D[4];					// Создаём новый массив текстур

		for(byte a=0; a<4; a++)									// Продолжаем цикл пока не заполним 3 ячейки с объектами
		{
			if(a<3)												// Если мы проходим не последний цикл
			{
				if(ActiveObjects[ActiveProfile, a] != -1)		// Если LastObjects с номером цикла не равен -1 значит в нём сохранён один из нужных нам объектов
				{
					// Заполняем очередную переменную массива текстурой из переменной FotoInventory обрабатываемого объекта
					Texs[a] = SC.ObjectsStore[ActiveObjects[ActiveProfile,a]].GetComponent<ObjectScript>().FotoInventory;
				}
				else 											// Иначе если он равен -1 значит мы ещё не разу не сохраняли последний объект этого типа
				{
					Texs[a] = NotExist;							// Тогда этому слоту мы присваиваем текстуру NoExist
				}
			}
			else												// Иначе если мы проходим последний цикл
			{
				if(ActiveObjects[ActiveProfile, a] != -1)		// И Если этот слот не равен -1 значит он сохранён
				{
					Texs[a] = SC.SkyboxScreens[ActiveObjects[ActiveProfile, a]]; 	// В этом случае присваиваем сохранённую текстуру скришота скайбокса 4 элементу
				}
				else 											// Иначе если 4 переменная LastObjs равна -1 значит для этого объекта ещё нет сохранения				
				{
					Texs[3] = NotExist;							// В этом случае мы ставим ему текстуру NotExist
				}
			}
		}
		return Texs;											// Возвращаем из метода массив заполненный текстурами
	}
		*/

	void LoadGame()												// Этот метод загружает из последнего удачного сохранения всю информацию для всех профилей, если сохранений нет задаёт начальные параметры профилей
	{
		if(!Directory.Exists(path + "/My Games/Slide/Saves")) 						// Если на компьютере нет папки мои документы а в ней нет папки Slide>Saves
		{
			Directory.CreateDirectory(path + "/My Games/Slide/Saves");				// То создаём её а внутри папку Slide>Saves
		}

		string[] Saves = System.IO.Directory.GetFiles(path + "/My Games/Slide/Saves/" , "Save_*.bin");

		string LastestSave = "StartString";			// Сдесь будет храниться последнее сохранение
		int Comparsion = -1;						// Переменная для сравнения номера сохранений
		for(int a=0; a<Saves.Length; a++)			// Продолжаем цикл пока не пройдём весь массив
		{
			IFormatter Form = new BinaryFormatter();															// Создаём дессериализатор
			FileStream LoadInf = new FileStream(Saves[a], FileMode.Open, FileAccess.Read); 						// Создаём поток читающий файл SaveProfiles
			SaveProfiles SP = (SaveProfiles)Form.Deserialize(LoadInf);											// Создаём объект и десериализуем в него данные из файла SaveProfiles

			if(Comparsion == -1)					// Если в переменной Comparsion находиться цифра -1
			{
				Comparsion = SP.SaveNomber; 		// То присваиваем заместо этого номера номер сохранения которое мы сейчас дессериализовали
				LastestSave = Saves[a];				// А в переменную LastSave ложим путь к этому сохранению
			}
			else 									// Иначе если там уже находиться номер какого либо сохранения то сравниваем его с номером дессериализованного в данный момент сохранения
			{
				if(Comparsion < SP.SaveNomber)		// И если в переменной "Сравнение" число меньше чем в элементе массива с номером итерации
				{
					Comparsion = SP.SaveNomber;		// То присваиваем это число переменной "Сравнение"
					LastestSave = Saves[a];			// И ложим в переменную LastSave путь к этому сохранению
				}
			}

			LoadInf.Close();						// Закрываем поток LoadInf
		}

		if(Saves.Length > 0)																					// Если существует хотя бы одно сохранение загружаем игру
		{
			IFormatter Form = new BinaryFormatter();															// Создаём дессериализатор
			FileStream LoadInf = new FileStream(LastestSave, FileMode.Open, FileAccess.Read); 					// Создаём поток читающий файл SaveProfiles
			SaveProfiles SP = (SaveProfiles)Form.Deserialize(LoadInf);											// Создаём объект и десериализуем в него данные из файла SaveProfiles

			// Загружаем простые данные профилей
			SaveNomber = SP.SaveNomber;									// Загружаем номер этого сохранения
			ActiveProfile = SP.ActiveProfile;							// Загружаем в переменную номер активного профиля из сохранения
			NamesProfiles = SP.NamesProfiles;							// Загружаем в массив имена профилей
			PlayersAvs = SP.PlayersAvs;									// Загружаем адреса аватарок
			Credits = SP.Credits;										// Загружаем кредиты "Деньги" всех профилей
			Progress = SP.Progress;										// Загружаем прогресс профилей. (Следующий уровень доступный для игры)
			PlayedLevels = SP.PlayedLevels;								// Загружаем последний сыгранный уровень для всех профилей
			RightSide = SP.RightSide;									// Загружаем массив сторон на которых предпочитают играть игроки
			AlienStyles = SP.AlienStyles;								// Загружаем массив сторон на которых предпочитают играть игроки
			Difficulty = SP.Difficulty;									// Загружаем уровни сложности выбранные для каждого игрока 0 или 1 Детский или Нормальный

			// Загружаем Сохранения закрытых, открытых и купленных элементов магазина игроков

			ActiveObjects = SP.ActiveObjects;							// Загружаем последние 4 выбранных объекта активного игрока. Бита, шайба, стол и скабокс. (Для каждого профиля)
			ActiveMats = SP.ActiveMats;									// Загружаем массив списков номеров активных материалов для объектов в массиве ObjectsStore
			ActiveImprs = SP.ActiveImprs;								// Загружаем массив списков номеров активных улучшений для объектов в массиве ObjectsStore
			ObjectsStates = SP.ObjectsStates;							// Загружаем массив списков состояний объектов в массиве Objects Store Открыт - O или Куплен - B
			ObjectsExpirience = SP.ObjectsExpirience;					// Загружаем массив списков опыта объектов в массиве ObjectsStore
			StatesMaterials = SP.StatesMaterials;						// Загружаем массив списков списков состояний материалов Закрыт - С, Открыт - O, Купленн - B
			StatesImprovements = SP.StatesImprovements;					// Загружаем массив списков списков состояний улучшений Закрыт - C, Открыт - O, Купленн - B
			SkyboxesStates = SP.SkyboxesStates;							// Загружаем массив списков состояний скайбоксов Открыт - O или Купленн - B

			ViewedProfile = ActiveProfile;								// Ложим номер активного профиля в переменную ViewedProfile

			for(byte a = 0; a != 5; a++)								// Продолжаем цикл пока не загрузим аватарки всех профилей на которые есть сохранения
			{
				if(NamesProfiles[a] != null & NamesProfiles[a] != "_ _ _ _ _")		// Если профиль этого цикла не пуст
				{
					PlayersAv[a] = new WWW("file:///" + PlayersAvs[a]).texture;		// Загружаем сохранённую аватарку главного игрока в переменную FirstPlayerAv
					if(ActiveProfile == a)											// Если этот профиль активный
					{
						MM.TextPlayerName = NamesProfiles[a];						// Загружаем в переменную отображения имён профилей и предупреждений имя активного профиля
						MM.GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (MM.TextPlayerName.Length * 1.8f));// Высчитываем размер имени профиля
					}
				}
				else if(NamesProfiles[a] == null | NamesProfiles[a] == "_ _ _ _ _")	// Если профиль этого цикла имеет начальное или пустое имя
				{
					PlayersAv[a] = DefaultAvatar;									// Загружаем аватар по умолчанию
				}
			}
			if(ActiveProfile == 10)												// Если нет ни одного активного профиля
			{
				MM.TextPlayerName = "Создайте профиль чтобы начать игру";		// Переменной MM.TextPlayerName ставим предупреждение что нужно создать профиль
			//	MM.GameSkin.GetStyle("PlayerProfileTextColor").fontSize = (int)(60 - (MM.TextPlayerName.Length * 1.23f));			// Высчитываем размер предупреждения
				MM.GameSkin.GetStyle("PlayerProfileTextColor").fontSize = 18;	// Ставим размер предупреждения 18
			}
			LoadInf.Close();											// Закрываем поток LoadInf
		}
		else 															// Иначе если ни одного сохранения нету
		{
			MM.TextPlayerName = "Создайте профиль чтобы начать игру";	// Переменной MM.TextPlayerName ставим предупреждение что нужно создать профиль
			MM.GameSkin.GetStyle("PlayerProfileTextColor").fontSize = 18; // Ставим размер предупреждения 18
			for(byte i=0; NamesProfiles.Length > i; i++)				// Продолжаем цикл пока не пройдём весь массив Profiles
			{
				if(NamesProfiles[i] == "")								// Если NamesProfiles с номером итерации не содержит символов
				{
					NamesProfiles[i] = "_ _ _ _ _";						// Присваиваем пустое имя профилю с номером итерации
					PlayersAv[i] = DefaultAvatar;						// И присваиваем ему "пустой" аватар по умолчанию
				}
			}
		}
	}

		
	public void SaveGame()							// Этот метод перезаписывает изменения в профилях и их прогресс
	{
		string[] Saves = System.IO.Directory.GetFiles(path + "/My Games/Slide/Saves/" , "Save_*.bin");		// Заносим в массив все сохранения
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

			if(OSComparsion == -1)						// Если в переменной OSComparsion находиться цифра -1
			{
				OSComparsion = SPr.SaveNomber; 			// То присваиваем вместо этого номера номер сохранения которое мы сейчас дессериализовали
				OldestSave = Saves[b];					// Переменной OldestSave присваиваем объект массива Saves с номером итерации
			}
			else 										// Иначе если там уже находиться номер какого либо сохранения то сравниваем его с номером дессериализованного в данный момент сохранения
			{
				if(OSComparsion > SPr.SaveNomber)		// И если в переменной "OSComparsion" число больше чем в элементе массива с номером итерации
				{
					OSComparsion = SPr.SaveNomber;		// То присваиваем это число переменной "Сравнение"
					OldestSave = Saves[b];				// Переменной OldestSave присваиваем объект массива Saves с номером итерации
				}
			}
			LoadInf.Close();							// Закрываем поток LoadInf
		}

		LSComparsion++;									// Прибавляем к переменной LSComparsion еденицу

		SaveProfiles SP = new SaveProfiles();			// Создаём объект SP класса SaveProfile
		SP.ActiveProfile = ActiveProfile;				// Записываем в объект SP значение ActiveProfile
		SP.NamesProfiles = NamesProfiles;				// Записываем в объект SP массив Profiles
		SP.HashTab = HashTab;							// Записываем в объект SP его HashTab номер
		SP.SaveNomber = LSComparsion;					// Записываем в объект SP номер сохранения
		SP.PlayersAvs = PlayersAvs;						// Записываем в объект SP массив адресов аватаров профилей
		SP.Credits = Credits;							// Записываем в объект SP количество денег (Кредитов)
		SP.Progress = Progress;							// Записываем в объект SP следующие уровени доступные для прохождения
		SP.PlayedLevels = PlayedLevels;					// Записываем в объект SP массив последних сыгранных уровеней
		SP.RightSide = RightSide;						// Записываем в объект SP правую сторону предпочитаемую для игры всех профилей
		SP.AlienStyles = AlienStyles;					// Записываем в объект SP "стиль пришельцев" для цифр
		SP.Difficulty = Difficulty;						// Записываем в объект SP "уровень сложности" для всех профилей
		SP.ActiveObjects = ActiveObjects;				// Записываем в объект SP последние 4 выбранные стартовых предмета всех профилей
		SP.ActiveMats = ActiveMats;						// Записываем в объект SP номера активных материалов для объектов в массиве ObjectsStore
		SP.ActiveImprs = ActiveImprs;					// Записываем в объект SP номера активных улучшений для объектов в массиве ObjectsStore
		SP.ObjectsStates = ObjectsStates;				// Записываем в объект SP состояния объектов в массиве Открыт - O или Куплен - B
		SP.SkyboxesStates = SkyboxesStates;				// Записываем в объект SP Состояния скайбоксов Открыт - O или Куплен - B
		SP.ObjectsExpirience = ObjectsExpirience;		// Записываем в объект SP опыт объектов в массиве ObjectsStore
		SP.StatesMaterials = StatesMaterials;			// Записываем в объект SP состояния материалов Закрыт - C, Открыт - O, Купленн - B
		SP.StatesImprovements = StatesImprovements;		// Записываем в объект SP состояния улучшений Закрыт - С, Открыт - O, Купленн - B



		// И создаём новое сохранение с номером на 1 больше самого последнего сохранения
		FileStream SaveGame = new FileStream(path + "/My Games/Slide/Saves/Save_" + (LSComparsion) + ".bin", FileMode.Create);	// Создаём сохранение с номером Kep.Savenomber
		BinaryFormatter BinFor2 = new BinaryFormatter();					// Создаём объект класса серриализатора
		BinFor2.Serialize(SaveGame, SP);									// Серриализуем объект SP в поток SaveInf
		SaveGame.Close();													// Закрываем поток

		if(Saves.Length >= 3) File.Delete(OldestSave);	// Если количество сохранений в папке для сохранений больше или равно 3 удаляем самое старое сохранение
	}


	public Texture2D[] CalculateTexsPrizes(/*short[] Objects ,byte[] Skyboxes*/)	// Этот метод высчитывает какие призы открыты а какие нет и заполняет массив текстур призов
	{
		byte Nom = 0;																// Номер заполняемого элемента в массиве текстур призов
		Texture2D[] TexsPrizes;														// Здесь будут храниться текстуры для отображения призов
		// Задаём размер массива TexsPrizes равным размеру массивов GM.ObjPrize + GM.SkyPrize + GM.TableNomForMat + GM.TableNomForImpr вместе взятых (Это будет количество ячеек с призами)

		TexsPrizes = new Texture2D[GM.ObjPrize.Length + GM.TableNomForMat.Length + GM.TableNomForImpr.Length + GM.SkyPrize.Length];		
		if(GM.ObjPrize.Length > 0)													// Если массив призовых объектов не пуст
		{
			for(byte a=0; a<GM.ObjPrize.Length; a++)								// Продолжаем итерации до тех пор пока не переберём все элементы массива GM.ObjPrize
			{
				if(SC.ObjectsStore[GM.ObjPrize[a]] != null)							// Если такой объект существует в массиве ObjectsStore
				{
					if(ObjectsStates[ActiveProfile][GM.ObjPrize[a]] == 0)			// Если для этого объекта ещё нет сохранения в этом профиле
					{
						TexsPrizes[Nom] = PM.QuestionIcon;							// То мы присваиваем элементу "a" массива текстурок призов текстуру знака вопроса
					}
					else  															// Иначе если для этого объекта есть сохранение в этом профиле
						TexsPrizes[Nom]	= PM.PrizeReceived;							// То мы присваиваем элементу "a" массива текстурок призов иконку для полученных призов
					Nom ++;															// Прибавляем nom ++ чтобы в следующем цикле заполнить следующий элемент массива TexsPrizes
				}
				else 																// Иначе если я ошибочно задал не тот номер или в массиве не оказалось нужного объекта то			
					Debug.Log("Приз по указанному номеру " + GM.ObjPrize[a] + " не существует в массиве ObjectsStore");	// Выводим предупреждение что такого объекта в магазине нет и небыло
			}
		}
		if(GM.TableNomForMat.Length > 0)											// Если массив призовых материалов для столов не пуст 
		{
			for(byte a=0; a<GM.TableNomForMat.Length; a++)							// Продолжаем итерации до тех пор пока не переберём все элементы массива GM.TableNomForMat
			{	// Если такой объект (Стол) существует в массиве ObjectsStore и длинна списка всех материалов этого стола больше чем номер материала указанный для открытия в массиве GM.TableMatPrize[a]
				if(SC.ObjectsStore[GM.TableNomForMat[a]] != null && StatesMaterials[ActiveProfile][GM.TableNomForMat[a]].Count > GM.TableMatPrize[a])
				{
					if(StatesMaterials[ActiveProfile][GM.TableNomForMat[a]][GM.TableMatPrize[a]] == 'C')				// Если у этого материала состояние равно "C" закрыт
					{
						TexsPrizes[Nom] = PM.QuestionIcon;																// То мы присваиваем элементу "a" массива текстурок призов текстуру знака вопроса
					}
					else
						TexsPrizes[Nom]	= PM.PrizeReceived;																// То мы присваиваем элементу "a" массива текстурок призов иконку для полученных призов
				}
				else
					Debug.Log("Стол или материал для него по указанному номеру " + GM.TableNomForMat[a] + " не существует в массиве ObjectsStore");	// Выводим предупреждение что такого объекта или материала в магазине или в сохранении нет
				Nom ++;																// Прибавляем nom ++ чтобы в следующем цикле заполнить следующий элемент массива TexsPrizes
			}

		}
		if(GM.TableNomForImpr.Length > 0)											// Если массив призовых улучшений для столов не пуст 
		{
			for(byte a=0; a<GM.TableNomForImpr.Length; a++)							// Продолжаем итерации до тех пор пока не переберём все элементы массива GM.TableNomForImpr
			{	// Если такой объект (Стол) существует в массиве ObjectsStore и длинна списка всех улучшений этого стола больше чем номер указанный для открытия в массиве GM.TableImprPrize[a]
				if(SC.ObjectsStore[GM.TableNomForImpr[a]] != null && StatesImprovements[ActiveProfile][GM.TableNomForImpr[a]].Count > GM.TableImprPrize[a])	
				{
					if(StatesImprovements[ActiveProfile][GM.TableNomForImpr[a]][GM.TableImprPrize[a]] == 'C')			// Если у этого материала состояние равно "C" закрыт
					{
						TexsPrizes[Nom] = PM.QuestionIcon;																// То мы присваиваем элементу "a" массива текстурок призов текстуру знака вопроса
					}
					else
						TexsPrizes[Nom]	= PM.PrizeReceived;																// То мы присваиваем элементу "a" массива текстурок призов иконку для полученных призов
					Nom ++;															// Прибавляем nom ++ чтобы в следующем цикле заполнить следующий элемент массива TexsPrizes
				}
				else
					Debug.Log("Стол или улучшение для него по указанному номеру " + GM.TableNomForImpr[a] + " не существует в массиве ObjectsStore");	// Выводим предупреждение что такого объекта или материала в магазине или в сохранении нет
			}
		}
		if(GM.SkyPrize.Length > 0)													// Если массив призовых скайбоксов не пуст 
		{
			for(byte a=0; a<GM.SkyPrize.Length; a++)								// Продолжаем цикл до тех пор пока не переберём все элементы массива GM.SkyPrize
			{
				if(SC.SkyboxScreens[GM.SkyPrize[a]] != null)						// Если такой скайбокс существует в массиве SkyboxScreens
				{
					if(SkyboxesStates[ActiveProfile][GM.SkyPrize[a]] == 0)			// Если для этого скайбокса ещё нет сохранения в этом профиле
					{
						TexsPrizes[Nom] = PM.QuestionIcon;							// То мы присваиваем элементу "a" массива текстурок призов текстуру знака вопроса
					}
					else  															// Иначе если для этого скайбокса есть сохранение в этом профиле
						TexsPrizes[Nom]	= PM.PrizeReceived;							// То мы присваиваем элементу "a" массива текстурок призов иконку для полученных призов
					Nom ++;															// Прибавляем nom ++ чтобы в следующем цикле заполнить следующий элемент массива TexsPrizes
				}
				else 																// Иначе если я ошибочно задал не тот номер или в массиве не оказалось нужного скайюокса то			
					Debug.Log("Приз по указанному номеру " + GM.ObjPrize[a] + " не существует в массиве ObjectsStore");	// Выводим предупреждение что такого скайбокса в магазине нет и небыло
			}
		}
		return TexsPrizes;															// Возвращаем массив TexsPrizes
	}


	public Texture2D GiveMePrize(short NomCellPrize)								// Этот метод сохраняет выбранный объект игроку в профиль и возвращает его аватар
	{	
		// Теперь вместо выбора либо объект либо скайбокс нужно выбирать между 4 вариантами (1 объект, 2 материал стола, 3 улучшение стола, 4 скайбокс 
		// 1 если номер выбранной ячейки меньше чем массив объектов GM.ObjPrize значит это объект 1 массива объектов, если больше или равно то это объект одного из трёх следующих массивов чтобы выяснить задаём следующий вопрос
		// 2 номер выбранной ячейки меньше чем общая длинна двух следующих указанных массивов вместе взятых? (GM.ObjPrize + GM.TableNomForMat) если меньше то это объект второго массива иначе если равно или больше задаём следующий вопрос
		// 3 номер выбранной ячейки меньше чем общая длинна трёх следующих указанных массивов вместе взятых? (GM.ObjPrize + GM.TableNomForMat + GM.TableNomForImpr)  если меньше то это объект третьего массива иначе если равно или больше то
		// . этот объект пренадлежит скайбоксам

		if(NomCellPrize < GM.ObjPrize.Length)										// Если выбранная ячейка меньше чем длинна массива призовых объектов GM.ObjPrize
		{																			// Значит этот приз лежит в массиве объектов
			if(SC.ObjectsStore[GM.ObjPrize[NomCellPrize]])							// Если такой объект существует в массиве ObjectsStore
			{
				ObjectScript ObjScr = SC.ObjectsStore[GM.ObjPrize[NomCellPrize]].GetComponent<ObjectScript>();	// У сохраняемого объекта берём компонент ObjectScript и ложим его в ObjScr
				List<char> StatesMatsList = new List<char>(1);						// Создаём пустой список состояний материалов
				List<char> StatesImprList = new List<char>(1);						// Создаём пустой список состояний улучшений
				for(byte c=0; c < ObjScr.FirstMaterials.Length; c++)				// Дальше в цикле в конец списка StatesMaterials добавляем состояния всех материалов существующих у объекта
				{
					if(c==0) StatesMatsList.Add('B');								// Если это первый материал ставим его состояние как купленное
					else 															// Иначе если это уже любой последующий материал
					{
						StatesMatsList.Add('C');									// Ставим его состояние как закрытое
					}
				}
				for(byte d=0; d < ObjScr.Improvements.Length; d++)					// Далее в цикле в конец списка StatesImrovements добавляем состояния всех улучшений существующих у объекта
				{
					StatesImprList.Add('C');										// Ставим состояние улучшения как закрытое
				}

				ActiveMats[ActiveProfile][GM.ObjPrize[NomCellPrize]] = 0; 									// Указываем номер активного материала для нового объекта как нулевой
				ActiveImprs[ActiveProfile][GM.ObjPrize[NomCellPrize]] = 10;									// Устанавливаем номер активного улучшения 10 (Это значение обозначает нету выбранного улучшения) потому что улучшения ещё не открыты
				ObjectsStates[ActiveProfile][GM.ObjPrize[NomCellPrize]] = 'O';								// Указываем что этот объект открыт
				ObjectsExpirience[ActiveProfile][GM.ObjPrize[NomCellPrize]] = 0;							// Указываем начальный опыт объекта как 0
				StatesMaterials[ActiveProfile][GM.ObjPrize[NomCellPrize]] = StatesMatsList;					// Указываем список состояний материалов StatesMatsList для нового объекта
				StatesImprovements[ActiveProfile][GM.ObjPrize[NomCellPrize]] = StatesImprList;				// Указываем список состояний улучшений StatesImprList для нового объекта
				PM.PrizeSelection = false;																	// Указываем что приз открыт и другие призы открыть нельзя
				SaveGame();																					// Вызываем метод сохранения игры
				return SC.ObjectsStore[GM.ObjPrize[NomCellPrize]].GetComponent<ObjectScript>().FotoStore;	// Возвращаем аватар этого объекта
			}
			else 																				// Иначе если объект отсутствует в массиве
			{
				Debug.Log("Ошибка (Объект) отсутствует в массиве!!!"); 							// Выдаём предупреждение об этом
				return null;																	// Возвращаем пустое значение
			}
		}
		else if(NomCellPrize < (GM.ObjPrize.Length + GM.TableNomForMat.Length))					// Если выбранная ячейка меньше чем общая длинна массивов (GM.ObjPrize + GM.TableNomForMat)
		{																						// Значит этот приз лежит в массиве материалов для столов
			byte NomTableEl = (byte)(GM.TableNomForMat[NomCellPrize - GM.ObjPrize.Length]);		// Вычисляем номер стола для которого открываеться материал в массиве TableNomForMat
			byte NomMatEl = (byte)(GM.TableMatPrize[NomCellPrize - GM.ObjPrize.Length]);		// Вычисляем номер открываемого материала	
																			
			// Если такой объект (Стол) существует в массиве ObjectsStore и длинна списка всех материалов этого стола больше или равняеться номеру указанному для открытия в массиве GM.TableMatPrize[NomMatEl]
			if(SC.ObjectsStore[NomTableEl] != null && StatesMaterials[ActiveProfile][NomTableEl].Count >= NomMatEl) 
			{
				if(ObjectsStates[ActiveProfile][NomTableEl] == 'B')									// Если у игрока есть стол для этого улучшения
				{
					ObjectScript ObjScr = SC.ObjectsStore[NomTableEl].GetComponent<ObjectScript>();	// Получаем ObjectScript стола
					StatesMaterials[ActiveProfile][NomTableEl][NomMatEl] = 'O';						// Записываем в сохранение данный материал как открытый материал
					PM.PrizeSelection = false;														// Указываем что приз открыт и другие призы открыть нельзя
					SaveGame();																		// Вызываем метод сохранения игры
					return ObjScr.FotoMaterials[NomMatEl];											// Возвращаем фотографию этого материала
				}
				else 																				// Иначе если у игрока нет стола для этого улучшения
				{
					Debug.Log("Вы не купили в магазине стол для которого преднозначен этот приз, пожалуйста выберите другой приз. Если других доступных призов нет то просто продолжайте игру");	// Выводим уведомление об этом
					return PM.QuestionIcon;															// Возвращаем иконку вопроса(Оставляя её такой какая она есть)
				}
			}
			else 																				// Иначе если стол или материал для него отсутствует в массиве
			{
				Debug.Log("Ошибка (Стол или материал для него) отсутствует в массиве!!!"); 		// Выдаём предупреждение об этом
				return null;																	// Возвращаем пустое значение
			}
		}
		else if(NomCellPrize < (GM.ObjPrize.Length + GM.TableNomForMat.Length + GM.TableNomForImpr.Length))					// Если выбранная ячейка меньше чем общая длинна массивов (GM.ObjPrize + GM.TableNomForMat + GM.TableNomForImpr)
		{																													// Значит этот приз лежит в массиве улучшений для столов
			byte NomTableEl = (byte)(GM.TableNomForImpr[NomCellPrize - GM.ObjPrize.Length - GM.TableNomForMat.Length]);		// Вычисляем номер стола для которого открываеться материал в массиве TableNomForMat
			byte NomImprEl = (byte)(GM.TableImprPrize[NomCellPrize - GM.ObjPrize.Length - GM.TableNomForMat.Length ]);		// Вычисляем номер открываемого материала	

			// Если такой объект (Стол) существует в массиве ObjectsStore и длинна списка всех материалов этого стола больше или равняеться номеру указанному для открытия в массиве GM.TableImprPrize[NomImprEl]
			if(SC.ObjectsStore[NomTableEl] != null && StatesImprovements[ActiveProfile][NomTableEl].Count >= NomImprEl)
			{
				if(ObjectsStates[ActiveProfile][NomTableEl] == 'B')									// Если у игрока есть стол для этого улучшения
				{
					ObjectScript ObjScr = SC.ObjectsStore[NomTableEl].GetComponent<ObjectScript>();	// Получаем ObjectScript стола
					StatesImprovements[ActiveProfile][NomTableEl][NomImprEl] = 'O';					// Записываем в сохранение данное улучшение как открытое
					PM.PrizeSelection = false;														// Указываем что приз открыт и другие призы открыть нельзя
					SaveGame();																		// Вызываем метод сохранения игры
					return ObjScr.IconImprovements[NomImprEl];										// Возвращаем фотографию этого материала
				}
				else 																				// Иначе если у игрока нет стола для этого улучшения
				{
					Debug.Log("Вы не купили в магазине стол для которого преднозначен этот приз, пожалуйста выберите другой приз. Если других доступных призов нет то просто продолжайте игру");	// Выводим уведомление об этом
					return PM.QuestionIcon;															// Возвращаем иконку вопроса(Оставляя её такой какая она есть)
				}
			}
			else
			{
				Debug.Log("Ошибка (Стол или улучшение для него) отсутствует в массиве!!!"); 	// Выдаём предупреждение об этом
				return null;																	// Возвращаем пустое значение
			}
		}
		else 																					// Иначе если номер нажатой ячейки равен или больше длинны массива призовых объектов
		{ 																						// Значит этот приз лежит в массиве скайбоксов
			byte NomSkyEl = (byte)(GM.SkyPrize[NomCellPrize - GM.ObjPrize.Length - GM.TableNomForMat.Length - GM.TableNomForImpr.Length]);		// Вычисляем номер открываемого скайбокса в массиве SkyboxMats
			if(SC.SkyboxMats[NomSkyEl])															// Если такой объект существует в массиве SkyboxMats
			{
				SkyboxesStates[ActiveProfile][NomSkyEl] = 'O';									// Указываем что этот скайбокс открыт
				PM.PrizeSelection = false;														// Указываем что приз открыт и другие призы открыть нельзя
				SaveGame();																		// Вызываем метод сохранения игры
				return SC.SkyboxScreens[NomSkyEl];												// Возвращаем скриншот выбранного скайбокса из метода
			}
			else 																				// Иначе если скайбокс отсутствует в массиве
			{
				Debug.Log("Ошибка (Скайбокс) отсутствует в массиве!!!"); 						// Выдаём предупреждение об этом
				return null;
			}
		}
	}
		

	public void OpenMatsAndImprs()					// Этот метод при своём вызове открывает материалы и улучшения для биты и шайбы игрока если они набрали достаточно количества опыта
	{
		ObjectScript Bat = SC.ObjectsStore[ActiveObjects[ActiveProfile, 0]].GetComponent<ObjectScript>();		// Берём компонент ObjectScript активной биты и ложим в переменную Bat
		ObjectScript Puck = SC.ObjectsStore[ActiveObjects[ActiveProfile, 1]].GetComponent<ObjectScript>();	// Берём компонент ObjectScript активной шайбы и ложим в переменную Puck
		int BatExp = 0;												// В этой переменной будет храниться опыт активной биты
		int PuckExp = 0;											// В этой переменной будет храниться опыт активной шайбы
		short BatNom = 0;											// В этой переменной будет храниться номер обрабатываемой биты в массиве ObjectsStore
		short PuckNom = 0;											// В этой переменной будет храниться номер обрабатываемой шайбы в масссиве ObjectsStore

		for(short a=0; a < 250; a++)								// Продолжаем цикл пока не пройдём массив ObjectsStore до раздела столы
		{	
			if(SC.ObjectsStore[a] != null)							// Если номер объекта в массиве ObjectsStore с номером цикла существует в массиве ObjectsStore
			{
				if(Bat.name == SC.ObjectsStore[a].name)				// Если имя обрабатываемой биты совпало с именем объекта в массиве ObjectsStore
					BatNom = a;										// Ложим номер биты в массиве ObjectsStore в переменную BatNom

				if(Puck.name == SC.ObjectsStore[a].name)			// Если имя обрабатываемой шайбы совпало с именем объекта в массиве ObjectsStore
					PuckNom = a;									// Ложим номер шайбы в массиве ObjectsStore в переменную PuckNom
			}
		}

		BatExp = ObjectsExpirience[ActiveProfile][BatNom];			// Присваиваем опыт обрабатываемой биты переменной BatExp
		PuckExp = ObjectsExpirience[ActiveProfile][PuckNom];		// Присваиваем опыт обрабатываемой шайбы переменной PuckExp

		for(byte b=0; b<9; b++)										// Продолжаем цикл пока не пройдём все материалы и улучшения активной биты
		{
			// Проходя очередной материал у биты спрашиваем он существует? Если да спрашиваем он закрыт? Если да мы спрашиваем (количество опыта у обрабатываемой биты больше чем у её материала обрабатываемого в этом цикле?)
			if(b < StatesMaterials[ActiveProfile][BatNom].Count && StatesMaterials[ActiveProfile][BatNom][b] == 'C' && BatExp >= Bat.RequireExpirienceMats[b])	
				StatesMaterials[ActiveProfile][BatNom][b] = 'O';		// Если ответ на все вопросы да мы указываем состояние этого материала как открытый
			
			// Проходя очередное улучшение у биты спрашиваем оно существует? Если да спрашиваем оно закрыто? Если да мы спрашиваем (количество опыта у обрабатываемой биты больше чем у её улучшения обрабатываемого в этом цикле?)
			if(b < StatesImprovements[ActiveProfile][BatNom].Count && StatesImprovements[ActiveProfile][BatNom][b] == 'C' && BatExp >= Bat.RequireExpirienceImprs[b])
				StatesImprovements[ActiveProfile][BatNom][b] = 'O';		// Если ответ на все вопросы да мы указываем состояние этого улучшения как открытый
		}	
		for(byte c=0; c<9; c++)											// Продолжаем цикл пока не пройдём все материалы и улучшения активной шайбы
		{
			// Проходя очередной материал у шайбы спрашиваем он существует? Если да спрашиваем он закрыт? Если да спрашиваем (количество опыта у обрабатываемой шайбы больше чем у её материала обрабатываемого в этом цикле?)
			if(c < StatesMaterials[ActiveProfile][PuckNom].Count && StatesMaterials[ActiveProfile][PuckNom][c] == 'C' && PuckExp >= Puck.PricesMats[c])
				StatesMaterials[ActiveProfile][PuckNom][c] = 'O';		// Если ответ на все вопросы да мы указываем состояние этого материала как открытый

			// Проходя очередное улучшение у шайбы спрашиваем оно существует? Если да спрашиваем оно закрыто? Если да спрашиваем (количество опыта у обрабатываемой шайбы больше чем у её улучшения обрабатываемого в этом цикле?)
			if(c < StatesImprovements[ActiveProfile][PuckNom].Count && StatesImprovements[ActiveProfile][PuckNom][c] == 'C' && PuckExp >= Puck.PricesMats[c])
				StatesImprovements[ActiveProfile][PuckNom][c] = 'O';	// Если ответ на все вопросы да мы указываем состояние этого улучшения как открытый
		}
	}


	void OnDisable()								// Вызываем этот метод перед уничтожением объекта
	{
	//	GM.LoadGameEvent -= LoadGame;				// Отписываем метод LoadGame от события LoadGameEvent
	//	GM.SaveGameEvent -= SaveGame;				// Отписываем метод SaveGame от события SaveGamEvent
	}
}
