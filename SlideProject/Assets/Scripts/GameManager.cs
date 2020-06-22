using UnityEngine;
using System;
using System.Collections;

// Создать ещё 4 уровня чтобы их было 5 не считая тренировочного с ним будет 6
// Похоже что в коде для стола придёться расширить количество доступных материалов для него до 5, и изменить назначения материалов. А именно (1 и 2 материал для поля), а (3, 4 и 5 для бордюра)
// При отображении четырёх активных объектов в игре на двоих проявляеться полоска, а также в игре на одного должно быть тоже самое, также эта полоска есть в финале тоже на аватарках всё из за того что три картинки накладываються друг на друга
// .Когда буду рендерить готовые окна для этих меню нужно рендерить с уже готовыми квадратами
// Добавить в скрипт фотограф выбор сглаживания с помощью перечисления. а также перенести туда переменные EdgeSharpness и EdgeTreshold они обе влияют на настройку сглаживания
// Проверить используеть ся ли гуи стиль "TotalScoreText" или его можно удалять
// Урезать силу начальной биты настолько чтобы она оставалась всё ещё сильнее тренирововчной биты но слабее её если на неё навесить улучшение
// Сделать чтобы при сохранении игры появлялься значёк в правом верхнем углу
// После того как мы сыграем раунд в игре и вернёмся в главное меню у нас сложиться ситуация что у нас 2 элемента IndestructbleObject по этому после того как я сделаю сохранение игры, То я сделаю чтобы по нажатии кнопки
// В главное меню удалялься IndestructbleObject
// После повторного старта кнопки начать заного после гола тебе и гола противнику вылазиет ошибка в связи с событием гола возможно не подписан заного или не отписано событие решить
// чтобы шайба не вылетала за пределы поля когда её тихо толкают через него. Можно попробовать брать её Velocity нормализовывать её и плюсовать к ней (Сила толкающей биты умноженная на массу толкающей биты)
// плюс нужно узнать массу шайбы и приплюсовать к ней такую силу чтобы она раза в два превышала ту силу что находиться у биты
// 1) Тоесть множим силу толкающей биты на её массу например 1*10 = 10
// 2) Берём массу шайбы например 1 = 1
// 3) Мы видим что масса шайбы в 10 раз меньше биты поэтому умножаем её значение на 10
// 4) Берём вектор шайбы как обычно отражаем его, нормализуем и умножаем на вдвое большее число чем это необходимо чтобы отталкивало биту это на 20
// 5) Но так бита становиться слишком быстрая а нам нужно чтобы она лишь отбросила биту, тогда можно оставлять ту же скорость но увеличивать на 1 секунду массу шайбы в 2 превышаюшую биту толкатель
// 6) Возможно шайба вылетает потому что когда она отталкиваеться например от нижнего борта и летит вверх то пускает луч в верхний борт и последняя точка Hit для неё вверху в этот момент бита пускает её в низ и она не
// успевает пустить луч в низ а по команде когда она не врезаеться в борт она использует последнюю точку в итоге используя последнюю точку она летит сквозь борт. Если не сработает последний способ надо придумать что нибуть
// ещё
// Исправить : Сделать чтобы размер текста (в меню создании профиля, в главном меню и в финальном окне уровня) высчитывалься тем же способом что и высчитываеться в скрипте PauseMenu методе StartGame проверяем 20 символами W
// В скрипте StoreAndInventory удалить метод CalculateTNMatAndImr если он не используеться
// В магазине в окошках улучшений и материалов не показываеться дополнительный задний мини-фон космоса
// В магазине и инвентаре сделать у улучшений чтобы информационное окно отображало энергию улучшения и скорость восстановления улучшений, также тип переключаемый или активируемый и количество активаций до перезарядки если есть
// Информационное окно отображает названия и информацию закрытых улучшений хотя должно писать как и у раскрасок информация закрыта
// Сделать чтобы в случае не совпадения HasTab или его отсутствия или не прочтения игра не загружала это сохранение а загружала новое
// Сделать наброски в Gimp 2 для новых изображений в игре
// Сделать сообщение в нормальной форме для игры, убрав сообщения в консоли. Это сообщение если не хватает денег чтобы купить в игре стол а также сообщение что если стола нет то для него нельзя получить улучшение или материал
// Написать PauseMenu + где будут выводиться дополнительные окна и использовать для них наброски
// Сделать чтобы правильно выставлялась шайба при старте и после гола
// Также после гола нужно ставить секундомер на 0
// Почемуто биты и шайба двигаються дёрганно. Как этого избежать?
// Сделать чтобы если игрок пытаеться купить в магазине предмет а денег не хватает или он попросту закрыт. То проигрываем звук а в окне с объектом пишем объект закрыт или недостаточно денег или если это материал или улучшение
// .....Недостаточно опыта берём тот же метод что и когда отсутствует профиль тоже самое и с деньгами
// 1) Дописать программу стартового окна и начала обучения (протестировать как работают обновлённая физика и скрипты управления)
// 2) Доделать магазин
// В скрипте лавное меню и меню паузы убрать закоментированные старые строчки кода, выяснить какие переменные необязательны быть публичными и 
// сделать их приватными а также убрать лишние строчки кода во всех скриптах
// Переместить методы по очереди мо смыслу и названиям в скрипте
// тоже самое сделать в скриптах ObjScript и ImprovementScript
// Исправить, когда стол отображаеться в окне магазина у его частей (это бордюр и поле) какие то странные углы поворота должны быть 0,0,0 Видимо стол так инстантируеться. А ещё так он инстантируеться в вигре на двоих и в окошке инвентаря там тоже это
// надо исправить.
// Если несколько раз создавать и удалять профили игра виснет при следующей загрузке. Скорей всего из за создания битого сохранения. Либо из за незавершённого процесса серриализации и открытого потока до следующей перезагрузки компа
// Предпринять две вещи 1) отрубать доступ у игрока к инвентарю до конца процесса серриализации. и 2) сделать
// чтобы битые сохранения не грузились. Определяем по весу файла хеш коду
// я переделал некоторые переменные сохранения short в sbyte теперь вылазиет ошибка.. Нужно проверить в каком она месте, когда найду нужно в 
// скрипте PauseMenu сделать ActiveProfile тоже такойже переменной Sbyte
// При нажатии на кнопку выхода из игры в главном меню сделать чтобы вылазило окошко подтверждения
// 3) Cделать инвентарь игрока
// 4) Сделать полное обучение Прохождение будет начинаться с нулевого уровня - обучения. далее будет обучение в магазине
// 5) Заново сделать режим игрок против игрока (в игре игрок против игрока 2 игрок будет ставиться с противоположной стороны сохранённой
// при прохождении первым игроком
// Улушить графику : В конце игры сделть тень от стрелок падующую на меню. Тень отрендерить в ту же картинку что и стрелки 
// Улушить графику : В конце игры текстурировать цифры показывающие счёт уровня
// Сделать магазин, разделить его на 4 категории первая биты, вторая шайбы, третяя игровые поля, четвёртая скайбоксы
// У начальной биты и тренировочной биты разный размер при фотографиях и просмотре в инвентаре я исправил прыгание бит но размер отличаеться на пару пикселов убрать эту разницу
// Убрать из скрипта Main menu переменную аудиоклипа и использовать её из специального скрипта работающего со звуками
// Сделать чтобы освещение биты выключалось не резко а плавно уменьшаясь в размере и освещённости
// В меню смены профиля и в магазине сделать отрисовку имени профиля для удобства
// В режиме прохождения при выборе предметов для игры сделать другую текстуру вместо крестов для недоступных для выбора предметов типа панели с заклёпками
// Разобраться со стилями которые я создавал отдельно для кнопок материалов(Если создавал), я решил не использовать для улучшений круглешки а остваить квадраты как и у материалов и переименовать таке стили как pricemats в другие говорящие что теперь...
// ... они используються для обоих видов окон для материалов и для улучшений.(Пройтись по всем своим стилям и поискать во всех файлах. И те что не используються удалить)
// ( Уровень - Сотоит из нескольких раундов)
// ( Раунд - Состоит из времени от начала раунда до гола одному из игроков)
// -------------------------------------------------------------------------------------------------------------Второстепенные задачи----------------------------------------------------------------------------------------------------------------------------
// 1) Появилась полоса в меню на фоне обзора предмета, такаяже как раньше была на аватарке нужно совместить несколько текстур
// картинках 4 активных объектах какие то полоски по бокам я выячнил что это из за сглаживания настраевосого в настройках качествра и вообще из за того что окна писксель в пиксель не поподают. Решить это можно отображая картинку сзади кнопки
// 2) тогда кнопка будет закрывать эти края
// 3) Если во время забитого гола нажать ескейп вылезет меню паузы хотя оно должно вылазить только после того как просвестит свисток и начнёться следующий раунд
// 4) При создании профиля при нажатии на поле имени буквы должны стираться
// 5) Убрать ограничение имени профиля на 2 буквы и сделать ограничение на одну
// -----------------------------------------------------------------------------------------------------------Мелочи 3 степенные задачи----------------------------------------------------------------------
// 1) Удалить Gui стиль avatar похоже он больше не используеться
// ------------------------------------------------------------------------------------------ Стандартные показатели бит и шайб -----------------------------------------------------------------------------------
// Все биты - Mass - 1, Drag - 5, AngularDrag 0.05
// Скрипт:
// NimbleBat:       Mass 1.3,   Force - 12
// OldBat:          Mass 1.2,   Force - 10
// TraningBat:      Mass 1.2,   Force - 5
// BeginnerPuck:    Mass 1      Force - 0
// EasyTouch:       Mass 1.1,   Force - 0
// -------------------------------------------------------------------------- Показатели высокоуровневых бит и шайб  для демонстрации физики и геймплея -----------------------------------------------------------
// Все биты - Mass - 1, Drag - 5, AngularDrag 0.05
// Сила бит * 10
// Масса бит * 2
// Масса шайб / 2
// NimbleBat:       Mass 2.6,   Force - 120
// OldBat:          Mass 2.4,   Force - 100
// TraningBat:      Mass 2.4,   Force - 50
// BeginnerPuck:    Mass 0.5    Force - 0
// EasyTouch:       Mass 0.55   Force - 0



public class GameManager : MonoBehaviour 
{ 
	public delegate void EventDelegate();					// Объявляем делегат для событий с возвращаемым типом значения Void
//	public delegate void ChangeProfDelegate(byte nomber);	// Делегат для события ChangeProfile (для обозначения отсутствия активных профилей теперь используеться цифра 10)
	public event EventDelegate StartFirstStep;				// Событие старта уровня "Первый шаг"
	public event EventDelegate StartSecondStep;				// Событие старта уровня "Второй шаг" "Запускаеться после завершения первого шага"
	public event EventDelegate StartGameEvent;				// Событие которое происходит при нажатии кнопки старт
	public event EventDelegate GoalEvent;					// Событие гола
	public event EventDelegate LastGoalEvent;				// Событие последнего гола
	public event EventDelegate SaveGameEvent;				// Событие сохранения файла игры
	public event EventDelegate LoadGameEvent;				// Событие загрузки файла игры
//	public event ChangeProfDelegate ChangeProfile;			// Событие смены активного профиля
//	public Mode GameMode;									// Режим игры
	public GameObject Table;								// Ссылка для клона стола
	public GameObject Player1; 								// Ссылка на клон префаба биты 1 ого игрока
	public GameObject Player2;								// Ссылка на клон префаба биты 2 ого игрока
	public GameObject AIBat;								// Клон префаба биты компьютерного противника для режима прохождения
	public GameObject Puck; 								// Ссылка на клон префаба шайбы
	public short[] ObjPrize;								// Призы - Номера объектов в массиве ObjectsStore который мы хотим открыть или несколько объектов
	public byte[] TableNomForMat;							// Номера столов для которых требуеться открыть материалы
	public byte[] TableNomForImpr;							// Номера столов для которых требуеться открыть улучшения
	public byte[] SkyPrize;									// Призы - Номерв Скайбоксов в массиве SkyboxMats и SkyboxScreens который мы хотим открыть или несколько скайбоксов
	public byte[] TableMatPrize;							// Призы материалы для столов в массиве TableNomForMat
	public byte[] TableImprPrize;							// Призы материалы для столов в массиве TableNomForImpr
	public byte[] PAON = new byte[] {1, 1, 1};				// (PlayersAvctiveObjectsNombers) номера игроков(тоесть каждая переменная принимает только 1 или 2 соответствующую номеру игрока) для каждой категории 1 шайба, 2 стол, 3 скайбокс
	public ushort Bonus;									// Дополнительный бонус кредитов на весь уровень для возмещения редко встречаемых сложностей от уровня к уровню
	public Vector3 StandPoint;								// Точка на которую бита возвращаеться для защиты ворот
	public PauseMenu PM;									// Переменная для скрипта "Меню паузы"
	public StoreContent SC;									// Переменная для скрипта "Контент магазина"
	public Keeper Kep;										// Переменная для скрипта "Хранитель" (или глобальная база данных)
	public Keeper LocalKep;									// Локальная база данных нужна для второго профиля на сулчай если в игре на двоих оба игрока решили выбрать один профиль
	public Keeper Currentkep;								// Сдесь в зависимости от условий будет находиться либо Kep либо LocalKep
	public StoreAndInventory SAI;							// Переменная для скрипта "Хранитель"
	public bool Pause = false;								// Переменная говорящая находиться ли игра в режиме паузы
	public bool GoalTrue = false;							// Становиться правда если комуто был забит гол (Предохранитель от повторных голов за одну секунду)
	public bool RoundStarted;								// Переменная говорящая начался ли раунд или игрок выбирает биты и шайбы
	ArtificialIntelligence AI;								// Переменная для скрипта "искуственный интеллект" для биты компьютера
	public GameObject Gate;									// Переменная для префаба ворот
	public Vector3 FirstPos; 								// Это позиция шайбы для первого игрока
	public Vector3 SecondPos; 								// Это позиция шайбы для второго игрока
	public Vector3 SPP1; 									// (Start Player Position 1) Это стартовая позиция первого игрока
	public Vector3 SPP2; 									// (Player Player Position 2) Это стартовая позиция второго игрока
	GameObject G1;											// Ссылка на клон префаба ворот для 1 игрока
	public GameObject G2;									// Ссылка на клон префаба ворот для 2 игрока
	Vector3 GP1;											// (Gate Position 1) Это позиция ворот первого игрока
	Vector3 GP2;											// (GatePosition 2) Это позиция ворот второго игрока
	public float WidthGate;									// Ширина ворот
	public byte LastGoal;									// Номер последних ворот в которые был забит гол
	public int WasherPosition; 								// Переменная "Позиция фишки" нужна чтобы при старте раунда определить у кого из игроков появиться фишка
	short residue;											// Длительность последнего раунда в секундах
//	byte Player2Profile = 0;								// Номер профиля второго игрока для игры на двоих


	void Start () 
	{
		StartGameEvent += StartGame;														// Подписываем метод "Старт игры" на событие StartGameEvent
		GoalEvent += Goal;																	// Подписываем метод "Гол" на событие GoalEvent
		LastGoalEvent += FinalGoal;															// Подписываем метод "Последний гол" на событие LastGoalEvent
		GameObject GaObj = GameObject.Find("IndestructibleObject");							// Находим на сцене объект "IndestructibleObject"
		SC = GaObj.GetComponent<StoreContent>();											// Скрипт "StoreContent" и ложим в переменную SC
		Kep = GaObj.GetComponent<Keeper>();													// Скрипт "Keeper" Ложим в переменную Kep
		SAI = GaObj.GetComponent<StoreAndInventory>();										// Скрипт "StoreAndInventory" Ложим в переменную SAI
		WasherPosition = UnityEngine.Random.Range (1, 3); 									// генерируем случайным образом число от 1 или 2
		StartFirstStep += _StartFirstStep_;													// Подписываем метод _StartFirstStep_ на событие StartFirstStep
		Currentkep = Kep;																	// Ложим в текущую базу данных глобальную
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)		// Если активная сцена не сцена главного меню
		{
			Kep.PM = GetComponent<PauseMenu>();												// Получаем скрипт PauseMenu и ложим в переменную PM скрипта Keeper
			Kep.PMP = GetComponent<PauseMenuPlus>();										// Получаем скрипт PauseMenuPlus и ложим его в переменную PMP скрипта Keeper
			SAI.PM = GetComponent<PauseMenu>();												// Получаем скрипт PauseMenu и ложим в переменную PM скрипта StoreAndInventory
			Kep.GM = this;																	// Ложим этот скрипт в переменную GM скрипта Keeper
			SAI.GM = this;																	// Ложми этот скрипт в переменную GM скрипта StoreAndInventory
			PM.PlayersInform = GaObj.transform.GetChild(0).GetComponent<Canvas>();			// Находим канвас PlayerInformation и ложим в переменную PM.PlayerInform
			LocalKep = this.gameObject.AddComponent<Keeper>();								// Создаём новый компонент Keeper на объеке GameManager и присваиваем его как новую локальную базу данных с уже заполненными значениями базы данных kep
			CallStartFirstStep(); 															// Вызываем событие StartFirstStep пользуясь для этого методом CallStartFirstStep
		}	
	}
		

	void _StartFirstStep_()																// Этот метод вызываеться после выполнения метода FillObjTex в скрипте keeper
	{
		CalculatePosObjects();													// Расчитываем позиции объектов
		SetStartPositions();													// Расставляем выбранные объекты по своим местам
		StartSecondStep();														// Вызываем событие второго шага
	}


	public void RepeatLevel()														// Этот метод обнуляет результаты и ставит стартовое окно для игры заного
	{
		PM.TimeMoves = true;														// Говорим что времени снова можно идти
		RoundStarted = false;														// Говорим что раунд ещё не началься
		GetComponent<AudioSource>().Stop();											// Прекращаем проигрывание музыки аплодисментов
		PM.Min = PM.Sec = 0;														// Обнуляем время раунда минуты и секунды
		PM.LPS = PM.RPS = 0;														// Обнуляем очки правого и левого игрока
		PM.ScorePlayer1 = PM.ScorePlayer2 = 0;										// Обнуляем очки первого и второго игрока
		PM.Rounds = 0;																// Обнуляем количество отыгранных раундов
		PM.LPFC = PM.RPFC = 0;														// Обнуляем количество заработанных кредитов за раунд у левого и правого игрока
		GoalTrue = false;															// Сообщаем переменной GoalTrue что в этом раунде ещё не кому не был забит гол
		RestartGameObjects();														// Расставляем объекты по местам
		PM.Window = GameMenuWins.Start;												// Ставим окно старта
		Debug.Log(PM.LPS + " " + PM.RPS);
	}


    void StartGame()
    {
        Debug.Log("Запущенно событие старта игры");
        if (WasherPosition == 1)                                                    // Если переменная WasherPosition равна 1
            Puck.transform.position = FirstPos;                                     // Ставим шайбу в первую позицию
        else if (WasherPosition == 2)                                               // Если переменная WasherPosition равна 2
            Puck.transform.position = SecondPos;                                    // Ставим шайбу во вторую позицию

		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "PlayerVsPlayer") 	// Если это не сцена для игры на двоих			
			Player2.GetComponent<ArtificialIntelligence>().AIActivation();						// Вызываем активацию искуственного интеллекта у второго игрока
		RoundStarted = true;																	// Ставим значение переменной RoundStarted правда
		Pause = false;																			// Сбрасываем игру с паузы
	}


	public void CalculatePosObjects()															// Этот метод на основе разных параметров определяет позиции объектов
	{
		byte LevelTable = 0;																									// Это временная переменная для стола
		float FP = SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 0]].GetComponent<MeshRenderer>().bounds.size.x;			// Получаем диаметр биты 1 игрока
		float PuckSize = SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 1]].GetComponent<MeshRenderer>().bounds.size.x;	// Получаем размер шайбы
		float LOTF = 0;																											// Длинна от центра поля до ворот
		float SP = 0;																											// Ставим диаметр биты второго игрока в начале как 0

		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "PlayerVsPlayer") 		// Если это не сцена для игры на двоих)
		{
			LevelTable = Table.GetComponent<ObjectScript>().ObjectLevel;							// Получаем уровень стола и присваиваем этот уровень переменной LevelTable
			WidthGate = (float)(((LevelTable-1) * 110) * 0.04 + 4);									// Получаем ширину ворот для данного поля
			SP = AIBat.GetComponent<MeshRenderer>().bounds.size.x;									// Получаем диаметр биты компьютерного противника и присваиваем как диаметр биты второго игрока
			LOTF = (float)(((LevelTable-1) * 25) * 0.18  + 18) / 2;									// Вычисляем длинну от центра поля до ворот и присваиваем её переменной LOTF(LengthOfThisField)
		}
		else 																															// Иначе если это сцена игры на двоих
		{
			if(Kep.SecondActiveProfile == 10)																							// Если для второго игрока не выбран активный профиль
			{
				SP = ((GameObject)Resources.Load("Models/Prefabs/Bats/1_Level/GhostBat")).GetComponent<MeshRenderer>().bounds.size.x;	// Получаем размер биты призрачной шайбы по оси x
				LevelTable = SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 2]].GetComponent<ObjectScript>().ObjectLevel;			// Получаем уровень активного стола первого игрока и присваиваем этот уровень переменной LevelTable
				LOTF = (float)(((LevelTable-1) * 25) * 0.18  + 18) / 2;																	// Вычисляем длинну от центра поля до ворот и присваиваем её переменной LOTF(LengthOfThisField)
			}
			else 																														// Иначе если для второго игрока выбран активный профиль
			{	// высчитываем позиции по другой схеме
				SP = SC.ObjectsStore[Kep.ActiveObjects[Kep.SecondActiveProfile,0]].GetComponent<MeshRenderer>().bounds.size.x;			// Получаем размер биты второго игрока по оси x для выбранного профиля

				if(PAON[0] == 2)		// Если активная шайба для этой сцены у 2 игрока
					PuckSize = SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 1]].GetComponent<MeshRenderer>().bounds.size.x;		// Пересчитываем размер шайбы на случай если для игры на двоих стоит другая активная шайба
				if(PAON[1] == 1)		// Если активный стол для этой сцены у 1 игрока
					LevelTable = SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 2]].GetComponent<ObjectScript>().ObjectLevel;		// Получаем уровень активного стола 1 игрока и присваиваем этот уровень переменной LevelTable
				else if(PAON[2] == 2)	// Иначе если активный стол для этой сцены у 2 игрока
					LevelTable = SC.ObjectsStore[Kep.ActiveObjects[PAON[1] ,2]].GetComponent<ObjectScript>().ObjectLevel;				// Получаем уровень стола активного для этой сцены
			
				LOTF = (float)(((LevelTable-1) * 25) * 0.18  + 18) / 2;																	// Вычисляем длинну от центра поля до ворот и присваиваем её переменной LOTF(LengthOfThisField)
			}
			WidthGate = (float)(((LevelTable-1) * 110) * 0.04 + 4);																		// Получаем ширину ворот для данного поля и заносим её в переменную WidthGate
		}	

		if(Kep.RightSide[Kep.ActiveProfile] == true)					// Если переменная "Правая сторона" игрока правда
		{
			SPP1 = new Vector3(LOTF + 0.1f, 0, 0);						// Назначаем первому игроку позицию с права
			GP1 = new Vector3(LOTF + 0.6f, 0, 0);						// И его ворота с права
			SPP2 = new Vector3(LOTF - LOTF*2 -0.1f, 0, 0);				// А второму игроку позицию слева
			GP2 = new Vector3(LOTF - LOTF*2 - 0.6f, 0, 0);				// И его ворота слева
			FirstPos.x = SPP1.x-(FP/2)-0.5f-(PuckSize/2);				// Первая позиция шайбы это позиция первого игрока - его радиус - условленное расстояние - радиус шайбы
			SecondPos.x = SPP2.x+(SP/2)+0.5f+(PuckSize/2);				// Вторая позиция шайбы это позиция второго игрока + его радиус + условленное расстояние + радиус шайбы
		}
		else 															// Иначе если переменная "Правая сторона" ложь
		{
			SPP1 = new Vector3(LOTF - LOTF*2 -0.1f, 0, 0);				// Назначаем первому игроку позицию с лева
			GP1 = new Vector3(LOTF - LOTF*2 - 0.6f, 0, 0);				// И его ворота с лева
			SPP2 = new Vector3(LOTF + 0.1f, 0, 0);						// А второму игроку позицию справа
			GP2 = new Vector3(LOTF + 0.6f, 0, 0);						// И его ворота справа
			FirstPos.x = SPP1.x+(FP/2)+0.5f+(PuckSize/2);				// Первая позиция шайбы это позиция первого игрока + его радиус + условленное расстояние + радиус шайбы
			SecondPos.x = SPP2.x-(SP/2)-0.5f-(PuckSize/2);				// Вторая позиция шайбы это позиция второго игрока - его радиус - условленное расстояние - радиус шайбы
		}
	}


	void SetStartPositions()					// Этот метод расставляет объекты согласно просчитанным для них позициям в методе CalculatePosObjects
	{
		Player1 = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 0]], SPP1, Quaternion.identity) as GameObject;	// Ставим 1 биту на просчитанную позицию для первого игрока
		byte FirstBatMatNom = Kep.ActiveMats[Kep.ActiveProfile][Kep.ActiveObjects[Kep.ActiveProfile,0]];							// У переменной LastObjects спрашиваем номер биты, по этому номеру узнаём номер активного материала у этой биты
		Player1.GetComponent<Renderer>().material = Player1.GetComponent<ObjectScript>().FirstMaterials[FirstBatMatNom]; 			// Присваиваем объекту новый материал
		SetBatImprovement(1);																										// Если есть активное улучшение устанавливаем улучшение на биту 1 игрока

		Gate = (GameObject)Resources.Load("Models/Prefabs/GateTrigger");								// Ложим префаб ворот в переменную Gate

		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "PlayerVsPlayer") 			// Если это не сцена для игры на двоих)
		{
			Player1.AddComponent<PlayerController>();													// Вешаем на первую биту скрипт управления битой
			Player1.GetComponent<PlayerController>().NomberPlayer = 1;									// Присваиваем переменной NomberPlayer этого скрипта значение 1

			Player2 = Instantiate(AIBat, SPP2, Quaternion.identity) as GameObject;						// Ставим 2 биту на просчитанную позицию второго игрока
			Player2.AddComponent<ArtificialIntelligence>();												// Вешаем на вторую биту скрипт управления компьютером

			Puck = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 1]]) as GameObject;	// Ставим выбранную шайбу в координаты указанные в её префабе и заносим ссылку на неё в переменную puck
			Puck.AddComponent<Puck>();																	// Вешаем на шайбу скрипт шайбы
			Puck.GetComponent<Puck>().StartPath = transform.position;									// Присваиваем переменной StartPath текущую позицию шайбы	

			if(Kep.RightSide[Kep.ActiveProfile] == true)												// Если переменная активного игрока "Правая сторона" равна правда
			{
				PM.RightAv = Kep.PlayersAv[Kep.ActiveProfile];											// Присваиваем переменной PM.RightAv аватар активного игрока
				PM.LeftAv = Player2.GetComponent<ObjectScript>().Avatar;								// Присваиваем переменной PM.LeftAv аватар компьютера
				PM.RPN = Kep.NamesProfiles[Kep.ActiveProfile];											// Присваиваем переменной PM.RPN имя активного игрока
				PM.LPN = AIBat.GetComponent<ObjectScript>().NPCName;									// Присваиваем переменной PM.LPN имя биты компьютера
				Player1.GetComponent<PlayerController>().RightPlayer = true;							// Указываем в скрипте PlayerController 1 игрока что он стоит с права
			}
			else 																						// Иначе если переменная активного игрока "Правая сторона" равна ложь
			{
				PM.LeftAv = Kep.PlayersAv[Kep.ActiveProfile];											// Присваиваем переменной PM.LeftAv аватар активного игрока
				PM.RightAv = Player2.GetComponent<ObjectScript>().Avatar;								// Присваиваем переменной PM.RightAv аватар компьютера
				PM.LPN = Kep.NamesProfiles[Kep.ActiveProfile];											// Присваиваем переменной PM.LPN имя биты компьютера
				PM.RPN = AIBat.GetComponent<ObjectScript>().NPCName;									// Присваиваем переменной PM.RPN имя активного игрока
				Player1.GetComponent<PlayerController>().RightPlayer = false;							// Указываем в скрипте PlayerController 1 игрока что он стоит с лева
			}
		}
		else																							// Иначе если режим игры игрок против игрока
		{	
			if(Kep.SecondActiveProfile == 10)															// Если для второго игрока не выбран активный профиль
			{
				Player2 = (GameObject)Instantiate(Resources.Load("Models/Prefabs/Bats/1_Level/GhostBat"), SPP2, Quaternion.identity);			// Инстантируем префаб призрачной биты на просчитанную позицию 2 игрока и ложим её копию в Player 2
				Puck = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 1]]) as GameObject;		// Ставим выбранную шайбу в координаты указанные в её префабе и заносим ссылку на неё в переменную puck
				Table = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 2]]);					// Инастантируем активный стол 1 игрока на сцену
				RenderSettings.skybox = SC.SkyboxMats[Kep.ActiveObjects[Kep.ActiveProfile, 3]];					// Назначаем сцене в качестве скайбокса, активный скайбокс выбранный для 1 игрока
			}
			else 																								// Иначе если для второго игрока выбран активный профиль
			{	// Расставляем объекты по другой схеме
				Player2 = (GameObject)Instantiate(SC.ObjectsStore[Currentkep.ActiveObjects[Kep.SecondActiveProfile,0]], SPP2,Quaternion.identity);// Инстантируем префаб активной биты 2 игрока на просчитанную позицию 2 игрока и ложим её копию в Player 2
				byte SecondBatMatNom = Currentkep.ActiveMats[Kep.SecondActiveProfile][Currentkep.ActiveObjects[Kep.SecondActiveProfile,0]];		// У переменной LastObjects спрашиваем номер биты, по этому номеру узнаём номер активного материала у этой биты
				Player2.GetComponent<Renderer>().material = Player2.GetComponent<ObjectScript>().FirstMaterials[SecondBatMatNom]; 				// Присваиваем бите второго игрока новый материал
				SetBatImprovement(2);	// Если есть активное улучшение устанавливаем улучшение на биту 2 игрока

				Player1.AddComponent<PlayerController>();													// Вешаем на первую биту скрипт управления битой
				Player1.GetComponent<PlayerController>().NomberPlayer = 1;									// Присваиваем переменной NomberPlayer этого скрипта значение 1

				Player2.AddComponent<PlayerController>();													// Вешаем на вторую биту скрипт управления битой
				Player2.GetComponent<PlayerController>().NomberPlayer = 2;									// Присваиваем переменной NomberPlayer этого скрипта значение 2

				if(PAON[0] == 1)		// Если активная шайба для этой сцены у 1 игрока
					Puck = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 1]]) as GameObject;					// Ставим активную шайбу 1 игрока в координаты указанные в её префабе и заносим ссылку на неё в переменную puck
				else if(PAON[0] == 2)	// Если активная шайба для этой сцены у 2 игрока
					Puck = Instantiate(SC.ObjectsStore[Currentkep.ActiveObjects[Kep.SecondActiveProfile, 1]]) as GameObject;	// Ставим выбранную шайбу в координаты указанные в её префабе и заносим ссылку на неё в переменную puck
				if(PAON[1] == 1)		// Если активный стол для этой сцены у 1 игрока
					Table = Instantiate(SC.ObjectsStore[Kep.ActiveObjects[Kep.ActiveProfile, 2]]);								// Инастантируем активный стол 1 игрока на сцену
				else if(PAON[1] == 2)	// Если активный стол для этой сцены у 2 игрока
					Table = Instantiate(SC.ObjectsStore[Currentkep.ActiveObjects[Kep.SecondActiveProfile, 2]]);					// Инастантируем активный стол выбранного игрока на сцену
				if(PAON[2] == 1)		// Если активный скайбокс для этогй сцены у 1 игрока
					RenderSettings.skybox = SC.SkyboxMats[Kep.ActiveObjects[Kep.ActiveProfile, 3]];								// Назначаем сцене в качестве скайбокса, активный скайбокс выбранный для 1 игрока
				if(PAON[2] == 2)		// Если активный скайбокс для этогй сцены у 2 игрока
					RenderSettings.skybox = SC.SkyboxMats[Currentkep.ActiveObjects[Kep.SecondActiveProfile, 3]];				// Назначаем сцене в качестве скайбокса, активный скайбокс выбранный для 1 игрока

				Puck.AddComponent<Puck>();																	// Вешаем на шайбу скрипт шайбы
				Puck.GetComponent<Puck>().StartPath = transform.position;									// Присваиваем переменной StartPath текущую позицию шайбы	

				if(Kep.RightSide[Kep.ActiveProfile] == true)												// Если переменная активного игрока "Правая сторона" равна правда
				{
					PM.RightAv = Kep.PlayersAv[Kep.ActiveProfile];											// Присваиваем переменной PM.RightAv аватар активного игрока
					PM.LeftAv = Currentkep.PlayersAv[Kep.SecondActiveProfile];								// Присваиваем переменной PM.LeftAv аватар второго активного игрока
					PM.RPN = Kep.NamesProfiles[Kep.ActiveProfile];											// Присваиваем переменной PM.RPN имя активного игрока
					PM.LPN = Currentkep.NamesProfiles[Kep.SecondActiveProfile];								// Присваиваем переменной PM.LPN имя второго активного игрока
					Player1.GetComponent<PlayerController>().RightPlayer = true;							// Указываем в скрипте PlayerController 1 игрока что он стоит с права
					Player2.GetComponent<PlayerController>().RightPlayer = false;							// Указываем в скрипте PlayerController 2 игрока что он стоит с лева
				}
				else 																						// Иначе если переменная активного игрока "Правая сторона" равна ложь
				{
					PM.LeftAv = Kep.PlayersAv[Kep.ActiveProfile];											// Присваиваем переменной PM.LeftAv аватар активного игрока
					PM.RightAv = Currentkep.PlayersAv[Kep.SecondActiveProfile];								// Присваиваем переменной PM.LeftAv аватар второго активного игрока
					PM.LPN = Kep.NamesProfiles[Kep.ActiveProfile];											// Присваиваем переменной PM.LPN имя биты активного игрока
					PM.RPN =  Currentkep.NamesProfiles[Kep.SecondActiveProfile];							// Присваиваем переменной PM.LPN имя второго активного игрока
					Player1.GetComponent<PlayerController>().RightPlayer = false;							// Указываем в скрипте PlayerController 1 игрока что он стоит с лева
					Player2.GetComponent<PlayerController>().RightPlayer = true;							// Указываем в скрипте PlayerController 2 игрока что он стоит с права
				}
			}
		}

		G1 = Instantiate(Gate, GP1, Quaternion.identity) as GameObject;									// Инстантируем ворота 1 игрока по просч. поз. и заносим ссылку на них в G1
		G2 = Instantiate(Gate, GP2, Quaternion.identity) as GameObject;									// Инстантируем ворота 2 игрока по просч. поз. и заносим ссылку на них
		G2.transform.localScale = G1.transform.localScale = new Vector3(0.8f, 0.5f, WidthGate);			// Присваиваем обоим воротам просчитанную ширину

		G1.GetComponent<GateTrigger>().PlayerGate = 1;													// Указываем 1 воротам что они ворота 1 игрока
		G2.GetComponent<GateTrigger>().PlayerGate = 2;													// Указываем 2 воротам что они ворота 2 игрока

		Table.transform.GetChild(0).GetChild(0).GetComponent<Light>().enabled = true;					// Включаем 1 источник освещения стола
		Table.transform.GetChild(0).GetChild(1).GetComponent<Light>().enabled = true;					// Включаем 2 источник освещения стола
		Table.transform.GetChild(0).GetChild(2).GetComponent<Light>().enabled = true;					// Включаем 3 источник освещения стола
		Table.transform.GetChild(0).GetChild(3).GetComponent<Light>().enabled = true;					// Включаем 4 источник освещения стола
	}


	void SetBatImprovement(byte NomberPlayer)							// Этот метод устанавливает улучшение на биту в будующем переделать и под остальные объекты шайбу и стол		
	{
		if(NomberPlayer == 1)											// Если нам нужно установить улучшение на 1 игрока
		{
			ObjectScript OS = Player1.GetComponent<ObjectScript>();		// Извлекаем из 1 игрока его ObjectScript
			short BatNom = Kep.ActiveObjects[Kep.ActiveProfile,0];		// Обращаемся к массиву LastObjects и узнаём номер биты в массиве ObjectsStore
			byte ImprNom = Kep.ActiveImprs[Kep.ActiveProfile][BatNom];	// Узнаём из сохранения номер активного улучшения в скрипте ObjectStore

			if(ImprNom != 10)	// Если у биты 1 игрока есть активное улучшение
			{	// Прикрепляем улучшение к бите 1 игрока
				GameObject BatImprovement = (GameObject)Instantiate(OS.Improvements[ImprNom], Player1.transform.position, Player1.transform.rotation); // Помещаем на сцену клон префаба улучшения а ссылку на клон отправляем в BatImprovement
				BatImprovement.transform.parent = Player1.transform;												// Прикрепляем к бите её активное улучшение
				Player1.GetComponent<Rigidbody>().mass += BatImprovement.GetComponent<ImprovementScript>().Mass;	// Добавляем к весу биты вес улучшения
				BatImprovement.GetComponent<ImprovementScript>().enabled = true;									// Включаем скрипт улучшения
				BatImprovement.GetComponent<ImprovementScript>().ObjRigb = Player1.GetComponent<Rigidbody>();		// Ложим компонент Ridgidbody Игрока в переменную скрипта улучшения ObjRidg

				if(Kep.RightSide[Kep.ActiveProfile])								// Если 1 игрок играет с права
					BatImprovement.GetComponent<ImprovementScript>().RSide = true;	// Указываем скрипту улучшения что он прикреплён к правой стороне
				else 																// Иначе если 1 игрок играет с лева
					BatImprovement.GetComponent<ImprovementScript>().RSide = false;	// Указываем скрипту улучшения что он прикреплён к левой стороне
			}
		}
		else if(NomberPlayer == 2)													// Если мы устанавливаем улучшение на 2 игрока
		{
			ObjectScript OS = Player2.GetComponent<ObjectScript>();					// Извлекаем из 2 игрока его ObjectScript
			short BatNom = 0;														// Номер биты
			byte ImprNom = 0;														// Номер активного улучшения в магазине ObjectStore
			if(Kep.ActiveProfile != Kep.SecondActiveProfile)						// Если у первого и второго игрока разные профили профили то обращаемся кглобальной базе данных
			{
				BatNom = Kep.ActiveObjects[Kep.SecondActiveProfile,0];				// Обращаемся к массиву LastObjects и узнаём номер биты в массиве ObjectsStore
				ImprNom = Kep.ActiveImprs[Kep.SecondActiveProfile][BatNom];			// Узнаём из сохранения номер активного улучшения в скрипте keeper
			}
			else 																	// Иначе если у них одинаковые профили обращаемся к локальной базе данных
			{
				BatNom = LocalKep.ActiveObjects[Kep.SecondActiveProfile,0];			// Обращаемся к массиву LastObjects и узнаём номер биты в массиве ObjectsStore
				ImprNom = LocalKep.ActiveImprs[Kep.SecondActiveProfile][BatNom];	// Узнаём из сохранения номер активного улучшения в скрипте keeper
			}
			if(ImprNom != 10)	// Если у биты 2 игрока есть активное улучшение
			{// Прикрепляем улучшение к бите 2 игрока
				GameObject BatImprovement = (GameObject)Instantiate(OS.Improvements[ImprNom], Player2.transform.position, Player2.transform.rotation); // Помещаем на сцену клон префаба улучшения а ссылку на клон отправляем в BatImprovement
				BatImprovement.transform.parent = Player2.transform;												// Прикрепляем к бите её активное улучшение
				Player2.GetComponent<Rigidbody>().mass += BatImprovement.GetComponent<ImprovementScript>().Mass;	// Добавляем к весу биты вес улучшения
				BatImprovement.GetComponent<ImprovementScript>().enabled = true;									// Включаем скрипт улучшения
				BatImprovement.GetComponent<ImprovementScript>().ObjRigb = Player2.GetComponent<Rigidbody>();		// Ложим компонент Ridgidbody Игрока в переменную скрипта улучшения ObjRidg

				if(Kep.RightSide[Kep.ActiveProfile])								// Если 1 игрок играет с права
					BatImprovement.GetComponent<ImprovementScript>().RSide = false;	// Указываем скрипту улучшения что он прикреплён к левой стороне
				else 																// Иначе если 1 игрок играет с лева
					BatImprovement.GetComponent<ImprovementScript>().RSide = true;	// Указываем скрипту улучшения что он прикреплён к правой стороне
			}
		}
	}


	public void RestartGameObjects()	// Этот метод вызываеться при нажатии кнопки "инвертировать позиции" при старте уровня и в других случаях когда настройки объетов были изменены и нужно их отобразить на объектах
	{
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "PlayerVsPlayer")	// Если уровень игры на двоих
			Destroy(Table);				// Уничтожаем стол
		Destroy(Player1);				// Уничтожаем биту первого игрока на сцене
		Destroy(Player2);				// Уничтожаем биту второго игрока на сцене
		Destroy(Puck);					// Уничтожаем шайбу на сцене
		Destroy(G1);					// Уничтожаем клон префаба ворот для 1 игрока
		Destroy(G2);					// Уничтожаем клон префаба ворот для 2 игрока
		CalculatePosObjects();			// Высчитываем заного позиции объектов
		SetStartPositions();			// Заного расставляем их по местам
	}
		

	public void CallStartFirstStep()				// Этот метод вызывает событие StartFirstStep
	{
		if(StartFirstStep != null)					// Если в списке события StartFirstStep есть подписанные на него методы
			StartFirstStep();						// Вызываем StartFirstStep
	}
		

	public void CallStartGame()
	{
		StartGameEvent();							//  Вызываем событие старта игры
	}
		

	public void CallGoalEvent(byte NomberGate)					// Этот метод вызываеться один раз чтобы вызвать событие гола, просчитать и записать статистичесские данные
	{
		GoalTrue = true;										// Ставим значение true переменной GoalTrue говорящее что в этом раунде уже был забит кому то гол
		PM.TimeMoves = false;									// Ставим значение переменной PM.TimeMoves ложь
		StatisticCalculation(NomberGate);						// Производим просчёт статистики
		if(PM.ScorePlayer1 < 3 & PM.ScorePlayer2 < 3)			// Если у первого и у второго игрока насчитываеться меньше 3 очков
		{
			GoalEvent();										// Вызываем событие GoalEvent
		}
		else if(PM.ScorePlayer1 >= 3 | PM.ScorePlayer2 >= 3)	// Если у одного из игроков уже 3 очка
		{
			LastGoalEvent();									// Вызываем событие LastGoalEvent
		}
	}


	void Goal()													// Этот метод вызываеться событием CallGoalEvent
	{
		Pause = true;											// Говорим что игра находиться в режиме паузы
		StartCoroutine(AfterGoal());							// Вызываем коронтину AfterGoal
	}


	IEnumerator AfterGoal()										// Этот метод вызываеться в методе Goal
	{
		yield return new WaitForSeconds(5);						// Ждём 4 секунды
		PM.TimeMoves = true;									// Ставим значение переменной PM.TimeMoves правда
		GoalTrue = false;										// Ставим значение false переменной GoalTrue говорящее что в этом раунде гол не был забит ещё не кому
		Pause = false;											// Говорим что игра вышла из режима паузы
	}


	void FinalGoal()											// Этот метод вызываеться событием LastGoalEvent
	{
		Pause = true;											// Говорим что игра находиться в режиме паузы
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "PlayerVsPlayer") 											// Если это не сцена для игры на двоих)
		{
			PM.BatExp = EC (out PM.PuckExp);																							// Высчитываем опыт для шайбы и биты для случая если победил первый игрок
			Kep.PlayedLevels[Kep.ActiveProfile] = Convert.ToByte(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);		// Заносим номер сыгранного уровня в переменную последнего сыгранного уровня

			if(PM.ScorePlayer2 < PM.ScorePlayer1)																						// Если победил игрок (Человек)
			{	
				PM.NextLevelTransition = true;																							// Делаем активной кнопку "Следующий уровень"
				if(Kep.Progress[Kep.ActiveProfile] == Convert.ToByte(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))	// Если переменная Progress равна номеру имени текущего уровня
				{
					Kep.Progress[Kep.ActiveProfile] ++;																					// То для этого профиля увеличиваем progress на 1
				}
			}
			else 																														// Иначе если победил компьютер
				if(Kep.Progress[Kep.ActiveProfile] > Convert.ToByte(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))	// Если переменная Progress больше текущего уровня значит следующий уровень уже открыт
					PM.NextLevelTransition = true;																						// Делаем активной кнопку "Следующий уровень"
		}


		// В конце уровня когда мы его сыграли мы смотрим 
		// 1) Победил ли игрок
		// 2) Совпадает ли переменная Kep.Progress которая отвечает за следующий уровень доступный для прохождения совпадает с текущим уровнем значит мы прошли текущий уровень
		// 3) Если ответ на оба эти условия да 

		if(PM.ScorePlayer2 < PM.ScorePlayer1)									// Если победил игрок (Человек)
		{
			PM.TexsPrizes = Kep.CalculateTexsPrizes(/*ObjPrize ,SkyPrize*/);	// Заполняем массив текстур призов 
			PM.ShowPrizes = true;												// Разрешаем PauseMenu показывать призы
		}
		if(PM.ScorePlayer2 > PM.ScorePlayer1)						// Если победил игрок 2 (Компьютер) урезаем опыт 1 игрока в 2 раза
		{
			if(PM.BatExp >= 1)										// И если опыт набранный в PM.BatExp Больше или равен 1 
				PM.BatExp = PM.BatExp/2;							// Cтавим заработанный опыт 1
			else if(PM.BatExp < 1)									// Иначе если опыт в PM.BatExp меньше 1 
				PM.BatExp = 0;										// Cтавим опыт биты 0
			if(PM.PuckExp >= 1)										// Если опыт набранный в PM.PuckExp Больше или равен 1
				PM.PuckExp = PM.PuckExp/2;							// Cтавим заработанный опыт 1
			else if(PM.PuckExp < 1)									// Иначе если опты в PM.PuckExp Меньше 1
				PM.PuckExp = 0;										// Cтавим опыт шайбы 0
		}																						
		Kep.OpenMatsAndImprs();										// Открываем какой нибуть материал или улучшение у биты или у шайбы игрока если у них для этого достаточно опыта
		Kep.SaveGame();												// Вызываем событие SaveGame
	}


	void StatisticCalculation(byte NomberGate)						// Этот метод подсчитывает статитстику каждый гол
	{
		PM.Scoregoal = LastGoal = NomberGate;						// Присваиваем переменной LastGoal номер ворот в который был забит гол а потом это значение присваиваем PM.Scoregoal

		PM.FullTimeMin[PM.Rounds] = PM.Min.ToString("00");			// Присваиваем отформатированные минуты переменной PM.FullTimeMin
		PM.FullTimeSec[PM.Rounds] = PM.Sec.ToString("00");			// Присваиваем отформатированные секунды переменной PM.FullTimeSec

		if(PM.Rounds == 0)											// Если это первый раунд
		{
			PM.RoundTimeMin[PM.Rounds] = PM.FullTimeMin[PM.Rounds];	// То присваиваем переменной PM.RoundTimeMin[PM.Rounds] переменную PM.FullTimeMin[PM.Rounds]
			PM.RoundTimeSec[PM.Rounds] = PM.FullTimeSec[PM.Rounds]; // А также присваиваем PM.RoundTimeSec[PM.Rounds] переменную PM.FullTimeSec[PM.Rounds]
			residue = (short)(PM.Min * 60 + PM.Sec);				// Переводим время первого раунда в секунды
		}
		else 														// Иначе если это уже не первый раунд
		{	
			int NowTime = PM.Min * 60 + PM.Sec;																					// Переводим полное время с этим раундом в секунды
			int LastTime = Convert.ToInt32(PM.FullTimeMin[PM.Rounds -1]) * 60 + Convert.ToInt32(PM.FullTimeSec[PM.Rounds -1]);	// Переводим полное время до этого раунда в секунды
			residue = (byte)(NowTime - LastTime); 					// Отнимаем общее время с этим раундом от общего времени до этого раунда получаем длительность этого раунда в секундах
			short ResultMin = (short)(residue / 60);				// Делим секунды на 60 получаем минуты
			float ResultSec = (float)(residue % 60);				// Заносим оставшиеся после деления секунды
			PM.RoundTimeMin[PM.Rounds] = ResultMin.ToString("00");	// Конвертируем в строку и заносим в массив минуты этого раунда
			PM.RoundTimeSec[PM.Rounds] = ResultSec.ToString("00");	// Конвертируем в сторку и заносим в массив секунды этого раунда
		}

		if(NomberGate == 1)									// Если гол игроку 1
		{
			PM.ScorePlayer2 ++;								// Засчитываем очко игроку 2
			if(Kep.RightSide[Kep.ActiveProfile] == true)	// Если значение переменной (Kep.RightSide равно правда)
			{
				PM.RightGoal[PM.Rounds] = true;				// То засчитываем гол правому игроку
				PM.LPS = PM.ScorePlayer2;					// И записываем левому игроку очки второго игрока
				PM.LPRC[PM.Rounds] = CC(Player1);			// Указываем количество заработанных кредитов левого игрока за этот раунд
				PM.LPFC += PM.LPRC[PM.Rounds];				// Прибавляем к финальному количеству кредитов левого игрока заработанных за весь уровень
			}
			else 											// Если значение переменной (Kep.RightSide равно ложь)
			{
				PM.RightGoal[PM.Rounds] = false;			// То засчитываем гол левому игроку
				PM.RPS = PM.ScorePlayer2;					// И записываем правому игроку очки второго игрока
				PM.RPRC[PM.Rounds] = CC(Player1);			// Указываем количество заработанных кредиты правого игрока за этот раунд
				PM.RPFC += PM.RPRC[PM.Rounds];				// Прибавляем к финальному количеству кредитов правого игрока заработанных за весь уровень
			}
		}
		else if(NomberGate == 2)							// Иначе если гол игроку 2
		{
			PM.ScorePlayer1 ++;								// Засчитываем очко игроку 1
			if(Kep.RightSide[Kep.ActiveProfile] == false)	// Если значение переменной (Kep.RightSide равно ложь)
			{
				PM.RightGoal[PM.Rounds] = true;				// То засчитываем гол правому игроку
				PM.LPS = PM.ScorePlayer1;					// И записываем левому игроку очки первого игрока
				PM.LPRC[PM.Rounds] = CC(Player2);			// Указываем количество заработанных кредиты левого игрока за этот раунд
				PM.LPFC += PM.LPRC[PM.Rounds];				// Прибавляем к финальному количеству кредитов левого игрока заработанных за весь уровень
				Kep.Credits[Kep.ActiveProfile] += PM.LPRC[PM.Rounds];	// Прибавляем к счёту 1 игрока
			}
			else 											// Если значение переменной (Kep.RightSide равно правда)
			{
				PM.RightGoal[PM.Rounds] = false;			// То засчитываем гол левому игроку
				PM.RPS = PM.ScorePlayer1;					// И записываем правому игроку очки первого игрока
				PM.RPRC[PM.Rounds] = CC(Player2);			// Указываем количество заработанных кредиты правого игрока за этот раунд
				PM.RPFC += PM.RPRC[PM.Rounds];				// Прибавляем к финальному количеству кредитов правого игрока заработанных за весь уровень
				Kep.Credits[Kep.ActiveProfile] += PM.RPRC[PM.Rounds];	// Прибавляем к счёту 1 игрока
			}
		}
		PM.LPSs[PM.Rounds] = PM.LPS;						// Запоминаем счёт для этого раунда для левого игрока
		PM.RPSs[PM.Rounds] = PM.RPS;						// Запоминаем счёт для этого раунда для правого игрока

		PM.Rounds ++;										// Ставим что был отыгран ещё один раунд
	}


	int CC(GameObject PlayerGoal)	// (CreditsCalculation) Этот метод расчитывает количество кредитов игрока
	{
		float mass;					// Масса игрока которому забит гол помноженная X2
		float Force;				// Сила игрока которому забит гол помноженная X2
		byte LvlBat;				// Уровень биты игрока которому забит гол  X8
		byte LvlTable;				// Уровень стола на котором происходит игра X8
		short Bonustime;			// Время раунда в секундах - 2 минуты используеться для конвертации в бонусные кредиты за быстрое прохождение
		int Result;					// Результат - Сколько кредитов получает победитель этого раунда

		mass = PlayerGoal.GetComponent<Rigidbody>().mass*2;						// Берём массу биты игрока которому забили гол умножаем на 2 и заносим в переменную mass
		Force = PlayerGoal.GetComponent<ObjectScript>().Force*2;				// Берём силу биты игрока которому забили гол умножаем на 2 и заносим в переменную Force
		LvlBat = (byte)(PlayerGoal.GetComponent<ObjectScript>().ObjectLevel*8);	// Берём уровень биты игрока которому забили гол умножаем на 8 и заносим в переменную LvlBat
		LvlTable = (byte)(Table.GetComponent<ObjectScript>().ObjectLevel*8);	// Берём уровень стола на котором происходит игра умножаем на 8 и заносим в переменную LvlTable
		Bonustime = (short)(120 - residue);										// Берём среднее время прохождения уровня 2 минуты отнимаем время прохождения игроком в сек. и присваиваем переменной BonusTime
		Result = (int)(mass + Force + LvlBat + LvlTable + Bonustime + Bonus);	// Подсчитываем все переменные влияющие на награду в кредитах и присваиваем переменной Result
		return Result;
	}


	int EC(out int PuckExp)				// (ExpirienceCalculation) Этот метод расчитывает количество опыта для шайбы игрока и его биты и возвращает результаты (Вызываем в конце уровня перед выведением статистики)
	{
		int BatExp;						// Здесь будет храниться высчитанный опыт для биты

		short LastBat =	Kep.ActiveObjects[Kep.ActiveProfile,0];					// Получаем номер выбранной биты игрока в массиве ObjectsStore и ложим в переменную LastBat
		short LastPuck = Kep.ActiveObjects[Kep.ActiveProfile,1];				// Получаем номер выбранной шайбы игрока в массиве ObjectsStore и ложим в переменную LastPuck

		ObjectScript ObjScrAi = Player2.GetComponent<ObjectScript>();			// Ложим компонент компьютерного игрока "ObjectScript" в переменную ObjScrAi
		ObjectScript ObjScrPlayer = Player1.GetComponent<ObjectScript>();		// Ложим компонент игрока "ObjectScript" в переменную ObjScrPlayer
		ObjectScript ObjScrPuck = Puck.GetComponent<ObjectScript>();			// Ложим компонент шайбы игрока "ObjectScript" в переменную ObjScrPuck

		BatExp = (int)(((ObjScrAi.Mass - ObjScrPlayer.Mass) + 2) * 10);			// Отнимаем от массы биты игрока компьютера массу биты игрока +2 * 10 и присваиваем опыту биты
		BatExp += (int)(((ObjScrAi.Force - ObjScrPlayer.Force) + 2) * 2);		// Отнимаем от силы биты игрока компьютера силу биты игрока +2 * 2 и плюсуем к опыту биты
		PuckExp = (int)((ObjScrAi.Mass - ObjScrPuck.Mass)* 4);					// Отнимаем от массы биты компьютерного игрока массу шайбы игрока * 4 и присваиваем к опыту шайбы

		Kep.ObjectsExpirience[Kep.ActiveProfile][LastBat] += BatExp;       		// Добавляем в сохранениях бите заработанный за раунд опыт
		Kep.ObjectsExpirience[Kep.ActiveProfile][LastPuck] += PuckExp;			// Добавляем в сохранениях шабе заработанный за раунд опыт
									
		return BatExp;															// Возвращаем высчитанный опты для биты
	}


	public void CallLoadGameEvent()									// Этот метод используеться для вызова события LoadProfiles
	{
		LoadGameEvent();											// Вызываем событие LoadGameEvent
	}
	

	public void CallSaveGame()										// Этот метод используеться для вызова события перезаписывания профиля
	{		
		SaveGameEvent();											// Вызываем событие SaveGameEvent
	}
		

	void OnDisable()												// Вызываем этот метод перед уничтожением объекта
	{
		StartGameEvent -= StartGame;								// Отписываем метод "Старт игры" от события StartGameEvent
		GoalEvent -= Goal;											// Отписываем метод "Гол" от события GoalEvent
		LastGoalEvent -= FinalGoal;									// Отписываем метод "Последний гол" от события LastGoalEvent
		StartFirstStep -= _StartFirstStep_;							// Отписываем метод "_Начать первый шаг_" от события StartFirstStep
	}
}













