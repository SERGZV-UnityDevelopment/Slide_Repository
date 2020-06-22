// Это скрипт искуственного интелекта управляющего битой противника
using UnityEngine;
using System.Collections;


public class ArtificialIntelligence : MonoBehaviour 
{
	public bool Think;					// Эта переменная говорит будет ли искуственный интеллект думать или нет
	GameObject GameM;					// Это переменная для игрового объекта GameManager
	Puck Pc;							// Ссылка на объект класса Puck
	Keeper Kep;							// Переменная для скрипта "Хранитель"
	GameManager GM;						// Переменная для скрипта GameManager
	ObjectScript OS;					// Переменная для скрипта ObjectScript этого объекта
	Vector3 StandPoint;					// Эта переменная указывает место ожидания биты управляемой компьютером
	Vector3 SafetyPoint;				// Эта переменная указывает место в котором надо стоять шайбе компьютера чтобы пассивно отбить шайбу
	bool ReachedSafetyPoint = false;	// Переменная говорит достиг ли компьютер SafetyPoint
	Vector3 Dirrection;					// Через эту переменную мы задаём направление и скорость движения биты компьютера
	Rigidbody TBR;						// (ThisBatRigitbody) Переменная для объекта класса Rigidbody этой биты
	Rigidbody PR;						// (PuckRigitbody) Переменная для объекта класса Rigidbody шайбы
	bool InningWas;						// Переменная говорит была ли выполнена подача
	Ray ray;							// Создаём объект класса луч
	Vector3 Pos;						// Точка откуда мы пустим луч
	Vector3 Dir;						// Направление в котором мы пустим луч
	RaycastHit Hit;						// Обозначаем переменную куда вернёться точка удара луча и коллайдер об который ударилься луч
	LayerMask Mask;						// Маска для луча
	LayerMask SCM;						// SphereCastMask Маска для сферичесского луча
	float d;							// Диаметр биты компьютерного игрока
	float td;							// (TurncatedDiametr) диаметр усечённый на 1/8 самого себя
	float ConditionalForce;				// Условная сила биты зависящая от уровня сложности игрока


	void Awake()																// Вызываеться при пробуждении
	{
		GameM = GameObject.Find("GameManager");									// Ложим объект GameManager в переменную GameM
		GM = GameM.GetComponent<GameManager>();									// Ложим в переменную GM скрипт GameManager
		Kep = GameObject.Find("IndestructibleObject").GetComponent<Keeper>();	// Находим компонент "Keeper" и ложим его в переменную Kep
		TBR = GetComponent<Rigidbody>();										// Ложим в переменную TBR Rigidbody биты на которой висит этот скрипт
		OS = GetComponent<ObjectScript>();										// В переменную ObjectScript загружаем ObjectScript
//		GM.StartSecondStep += _StartSecondStep_;								// В список события StartSecondStep добавляем метод _StartSecondStep_
		GM.GoalEvent += Goal;													// Подписываем метод гол на событие GoalEvent
		GM.LastGoalEvent += LastGoal;											// Подписываем метод LastGoal на событие LastGoalEvent
		SCM = LayerMask.GetMask("Border", "Gate");								// Ложим слои в переменную для маски SphereCastMask
		Mask = LayerMask.GetMask("SafetyPointCollider");						// Луч этой маски будет врезатьс только в слой SafetyPointCollider
		if(Kep.Difficulty[Kep.ActiveProfile] == 0)								// Если выбран "Детский" уровень сложности активного профиля 
		{
			ConditionalForce =	0.4f * OS.Force;								// То присваиваем переменной "ConditionalForce" 0.4 от оригинальной силы биты
		}
		else if(Kep.Difficulty[Kep.ActiveProfile] == 1)							// Иначе если выбран "Нормальный" уровень сложности активного профиля
		{
			ConditionalForce = OS.Force;										// То присваиваем переменной "ConditionalForce" Оригинальную силу биты
		}
	}


//-------------------------------------------------------------------------------------Событие старта уровня-----------------------------------------------------------------------------------------------------------------------
	
	
	public void AIActivation()																		// Этот метод вызываеться
	{
		byte LevelTable = GameObject.FindWithTag("Table").GetComponent<ObjectScript>().ObjectLevel;	// Узнаём уровень игрового стола
		float WidthOfField = (float)(((LevelTable-1) * 20) * 0.12 + 12);							// Высчитываем ширину поля
		Think = true;																				// Ставим переменную Think равной правда
		d = GetComponent<MeshRenderer>().bounds.size.x;												// Получаем диаметр биты компьютерного игрока
		td = d - d/8;																				// Высчитываем диаметр усечённый на 1/8
		if(Kep.RightSide[Kep.ActiveProfile] == true)												// Если переменная "Правая сторона" активного профиля равна правда
		{
			StandPoint.x = GM.SPP2.x + (d/2) + 0.2f;												// То StandPoint это позиция второго игрока + его радиус + условное расстояние					
		}
		else 																						// Иначе если переменная "Правая сторона" ложь
			StandPoint.x = GM.SPP2.x - (d/2) - 0.2f;												// То StandPoint это позиция второго игрока - его радиус - условное расстояние

		GameObject SPC = new GameObject("SafetyPointCollider", typeof(BoxCollider));				// Создаём пустышку с именем и компонентом BoxCollider на нём
		SPC.transform.position = StandPoint;														// Задаём ей позицию StandPoint
		SPC.transform.localScale = new Vector3(0, 0.1f, WidthOfField);
		SPC.layer = 10;																				// Перемещаем на 10 слой
		StartCoroutine(AIActivationStep2());														// Вызываем коронтину "Активация Искуственного интелекта 2 шаг"
	}
	
	
	IEnumerator AIActivationStep2()
	{
		yield return new WaitForSeconds(0.1f);						// Ждём 1 секунду
		if(IHaveThePuck())											// Вычисляем. Шайба у компьютера? Если метод вернёт правда
			StartCoroutine(Inning());								// Вызываем метод запускающий подачу
		else 														// Если вернёт ложь
		{
			InningWas = true;										// Сообщаем что подача была выполненна
		}
		PR = GM.Puck.GetComponent<Rigidbody>();						// Получаем объект класса Rigitbody и ложим в переменную PR
		Pc = GM.Puck.GetComponent<Puck>();							// Получаем объект класса Puck и ложим в переменную Pc
	}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


	void Update () 										
	{
		if(Think)										// Если переменная думать равна "правда"
			if(InningWas)								// И если подача была выполненна или она вовсе не нужна
				WhatToDo();								// Вызываем метод решающий что делать
	}
	
	
	void FixedUpdate()					
	{
		if(Think)										// Если переменная Думать равна true
		{
			TBR.AddForce(Dirrection);					// То задаём силу и направление указанное в переменной Dirrection
		}
	}
	

	IEnumerator Inning()																// Этот метод нужен для подачи в начале игры если выпала подача компьютера				
	{
		Vector3 BackVector;																// Направление нужное чтобы отдалиться от шайбы
		BackVector = gameObject.transform.position - GM.Puck.transform.position;		// Вычисляем направление в котором нужно двигаться
		BackVector.Normalize();															// Нормализуем вектор														
		Dirrection = BackVector * ConditionalForce;										// Умножаем вектор TemporaryVector на силу этой биты и присваиваем это значение вектору Dirrection		
		yield return new WaitForSeconds(1.5f);											// Ждём 1.5 секунды заменить время на рассояние
		Dirrection -= Dirrection * 2;													// Реверсируем направление шайбы чтобы выполнить удар
		yield return new WaitForSeconds(2.5f);											// Ждём 2 секунды заменить на соприкосновение с шайбой
		InningWas = true;																// Ставим что подача выполненна
	}
	
	
	void WhatToDo()		// Этот метод вызываеться каждый кадр он решает что делать встать на StandPoint, встать поперёк направлению шайбы или идти на неё
	{
		Pos = GM.Puck.transform.position;											// Ложим в позицию центра луча текущее местоположение шайбы
		Dir = PR.velocity;															// Ложим вектор направления шайбы в переменную Dir
		Debug.DrawRay(GM.Puck.transform.position, PR.velocity * 2, Color.red);		// Рисуем вектор для отладки

		for(byte a = 0; a<3; a++)													// Завершаем цикл если он повторилься 3 раза							
		{
			if(Physics.SphereCast(new Ray(Pos, Dir), Pc.Radius, out Hit, Mathf.Infinity, SCM.value))			// Мы пускаем луч из PosRay в направлении Dir если он обо чтото удариться																
			{
				if(Hit.collider.tag == "Gate")																	// Если луч ударилься об коллайдер ворот
				{
					if(Hit.collider.GetComponent<GateTrigger>().PlayerGate == 2 && Vector3.Magnitude(Dir) > 1)	// Если луч ударилься об коллайдер ворот компьютера и скорость шайбы больше 1 (еденицы)
					{
						ActiveProtection(a);										// Вызываем метод активной защиты
						break;														// Прерываем цикл так как мы уже знаем что шайба летит в ворота
					}																
					else 															// Иначе если луч ударилься об коллайдер ворот живого игрока или скорость шайбы меньше еденицы
					{
						PassiveProtection();										// Вызываем метод пассивной защиты
					}
				}
				else 																// Иначе если луч врезалися в другой коллайдер
				{
					ray = new Ray(Hit.point, Dir - (Dir*2));						// Задаём лучу координаты где луч хит врезалься и направление обратное тому что у него было
					Pos = ray.GetPoint(Pc.Radius);									// Высчитываем где будет центр шайбы когда она сюда врежеться и заносим его в переменную Pos
					Dir = Vector3.Reflect(Dir, Hit.normal);							// Высчитываем направление луча после отражения и присваиваем переменной Dir	

					PassiveProtection();											// Вызываем метод пассивной защиты
				}										
			}
			else 																	// Если луч не ударилься значит шайба либо совсем стоит либо за пределами поля в любом случае тогда..
			{
				PassiveProtection();												// Мы вызываем метод пассивной защиты
			}
		}
	}
	
	
	void PassiveProtection()			// Этот метод вызываеться когда луч шайбы ударилься во чтото кроме ворот компьютера или скорость шайбы меньше определённой
	{
		Vector3 Temporary = Vector3.Normalize(StandPoint - transform.position);							// Обновляем вектор движения в точку ожидания, нормализуем этот вектор и присваиваем вектору Temporary
		ReachedSafetyPoint = false;																		// Если был вызван этот метод значит шайба больше не летит в ворота. Ставим что компьютер не достиг ReachedSafetyPoint
		Vector3 Position = new Vector3(GM.G2.transform.position.x, 0, GM.Puck.transform.position.z); 	// Точка для измерения расстояния
		float Dist = Vector3.Distance(Position, GM.Puck.transform.position);							// Измеряем дистанцию от точки до шайбы

		if(Dist < 4 && GM.Puck.transform.position.z < GM.WidthGate/2 && GM.Puck.transform.position.z > GM.WidthGate - (GM.WidthGate + GM.WidthGate)) // Если расстояние от ворот до шайбы меньше 4 и шайба не выше и не ниже ворот
		{
			Dirrection = ConditionalForce * Vector3.Normalize(GM.Puck.transform.position - transform.position);	// Запускаем биту в шайбу чтобы отбить её
		}
		else 																		// Иначе если бита дальше условного расстояния или выше или ниже ширины ворот
		{																			// То мы смотрим с какой скоростью бите двигаться к StandPoint
			if(Vector3.Magnitude(StandPoint - transform.position) > 0.5f)			// Если от биты до StandPoint расстояние больше чем пол метра				
			{
				Dirrection = Temporary * ConditionalForce;							// То вектор Temporary умножаем на силу биты и присваиваем как новый вектор движения
			}
			else 	 																// Иначе если от StandPoint до биты расстояние меньше или равно пол метра
			{
				// То вектор движения биты умножаем на силу биты делённую умноженную на расстояние и присваиваем как новый вектор движения
				Dirrection = Temporary * (ConditionalForce * (Vector3.Magnitude(StandPoint - transform.position)));
			}
		}	
	}


	void ActiveProtection(byte Nom)													// Этот метод вызываеться когда луч шайбы ударилься в ворота компьютера и скорость шайбы больше определённой
	{
		Vector3 Position = new Vector3(SafetyPoint.x, 0, GM.Puck.transform.position.z); // Точка для измерения расстояния
		float Dist = Vector3.Distance(Position, GM.Puck.transform.position);			// Измеряем дистанцию от точки до шайбы

		if(Physics.Raycast(new Ray(Pos, Dir), out Hit, Mathf.Infinity, Mask.value)) 	// Если пущенный луч ударилься в SafetyPointCollider
		{
			SafetyPoint = Hit.point;													// То мы присваиваем точку в которую он врезалься как безопасную
		}
		else  																			// Иначе если луч ничего не коснулься значит шайба рядом с воротами за пределами SagetyPointCollider
		{
			SafetyPoint = StandPoint - new Vector3(td, 0, 0);							// Тогда мы ставим рискованную SafetyPoint внутри ворот
		}

		Vector3 Temporary = Vector3.Normalize(SafetyPoint - transform.position);						// Обновляем вектор движения в "Безопасную" точку, нормализуем этот вектор и присваиваем вектору Temporary

		if(!ReachedSafetyPoint && Dist < 4)																// Если переменная ReachedSafetyPoint равна ложь
		{
			if(Vector3.Distance(transform.position, SafetyPoint) < 0.05f) ReachedSafetyPoint = true;	// Проверяем достигла ли бита SafetyPoint, если достигла ставим ReachedSafetyPoin true

		}

		if(!ReachedSafetyPoint)														// Если бита не достигла SafetyPoint
		{
			if(Vector3.Magnitude(SafetyPoint - transform.position) > 0.5f)			// Если от биты до SafetyPoint расстояние больше чем пол метра
			{
				Dirrection = Temporary * ConditionalForce;							// То вектор Temporary умножаем на силу биты и присваиваем как новый вектор движения
			}
			else 	 																// Иначе если от StandPoint до биты расстояние меньше или равно пол метра
			{
				// То вектор движения биты умножаем на силу биты делённую умноженную на расстояние и присваиваем как новый вектор движения
				Dirrection = Temporary * (ConditionalForce * (Vector3.Magnitude(SafetyPoint - transform.position)));
			}
		}
		else 																		// Если бита достигла SafetyPoint
		{
			if(Nom == 0 && Dist < 4)												// Если шайба летит уже напрямую в ворота компьютера и расстояние от шайбы до ворот компьютера меньше заданного
			{
				Dirrection =  ConditionalForce * Vector3.Normalize(GM.Puck.transform.position - transform.position);	// То направляем биту компьютера в шайбу
			}
		}
	}
	

	bool IHaveThePuck()													// Метод вычисляет шайба у компьютера или у игрока если у компьютера возвращает правда 
	{
		bool Ihave = false;												// Хранит ответ на вопрос (шайба у компьютера ?) по умолчанию ответ нет
		// Если при старте игры расстояние между битой компьютера и шайбой больше меньше 2 метров
		if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(GM.Puck.transform.position.x, GM.Puck.transform.position.y))< 2)
		{
			Ihave = true;												// Указываем переменной что бита у компьютера
		}
		return Ihave;													// Возвращаем результат переменной Ihave
	}

	
	//-----------------------------------------------------------------------------------------Событие гола--------------------------------------------------------------------------------------------------------------------------
	
	void Goal()									// Этот метод вызываеться один раз во время гола
	{
		Think = false;							// Приказываем компьютеру остановиться
		StartCoroutine(AfterGoal());			// Вызываем коронтину AfterGoal					
	}
	
	
	IEnumerator AfterGoal()						// Этот метод вызываеться после выполнения метода гол
	{
		yield return new WaitForSeconds(4); 	// Ждём 4 секунды
		transform.position = GM.SPP2;			// Ставим биту управляемую компьютером на позицию просчитанную для второго игрока
		yield return new WaitForSeconds(1);		// Ждём 1 секунду
		Think = true;							// Приказываем компьютеру возобновить игру
	}

	void LastGoal()													// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		Think = false;												// Отключаем искуственный интеллект чтобы его можно было попинать )))
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	
	
	void OnDisable()								// Вызываем этот метод перед уничтожением объекта
	{
		GM.GoalEvent -= Goal;						// Отписываем метод Goal от события GoalEvent
//		GM.StartSecondStep -= _StartSecondStep_;	// Отписываем метод StartLevel от события StartSecondStep
		GM.LastGoalEvent -= LastGoal;				// Отписываем метод LastGoal от события LastGoalEvent
	}
}