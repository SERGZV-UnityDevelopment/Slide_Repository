// Скрипт шайбы отвечающий за все действия и реакции шайбы в игре

using UnityEngine;
using System.Collections;


public class Puck : MonoBehaviour 
{
	public GameManager GM;				// Переменная для скрипта GameManager
	public float Radius;				// Радиус шайбы
	public Vector3 StartPath;			// Позиция где высчитывалься путь до столкновения
	float PathLenght;					// Длинна пути от точки просчёта до точки где столкнулься луч с коллайдером
	Vector3 MoveVector;					// Вектор движения
	Vector3 ReflectVector;				// Вектор отражения от точки куда удариться шайба
	RaycastHit SphereRayHit;			// Точка куда ударилься сферический луч
	RaycastHit RayHit;					// Точка куда ударилься простой луч
	Rigidbody PuckRb;					// Это Rigidbody шайбы
	int layerMask = 1<<8;				// Побитовое обозначение слоя для столкновения луча
	public bool ColStay = false;		// Эта переменная говорит находиться ли шайба в контакте с одной из бит

	void Awake()
	{
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();	// Ложим в переменную GM скрипт GameManager с пустышки на сцене под темже именем GameManager
		GM.GoalEvent += Goal;												// Подписываем метод гол на событие GoalEvent
		GM.LastGoalEvent += LastGoal;										// Подписываем метод LastGoal на событие LastGoalEvent
		PuckRb = GetComponent<Rigidbody>();									// Ложим Rigitbody в переменную PuckRb	
		Radius = GetComponent<MeshRenderer>().bounds.size.x/2;				// Узнаём радиус шайбы
	}
	

	void FixedUpdate()																											// Этот метод вызываеться с постоянной частотой определённое в насройках Unity количество раз в секунду
	{
		MoveVector = Vector3.Normalize(PuckRb.velocity);																		// Вычисляем нормализованный вектор движения
		if(Physics.SphereCast(new Ray(transform.position, MoveVector), Radius, out SphereRayHit, Mathf.Infinity, layerMask))	// Если сферичесский рейкаст врезалься во чтото то возвращаем результат столкновения в переменную Hit
		{
			if(SphereRayHit.distance <= 0.06f)																					// Если от шайбы до борта дистанция меньше или равна 0
			{
				PuckRb.velocity = Vector3.Reflect(PuckRb.velocity, SphereRayHit.normal);										// Высчитываем вектор который будет после отражения и присваиваем вектору ReflectVector
			}
		}
		else 																													// Иначе если луч не во что не врезалься
		{
			if(Physics.Raycast(new Ray(transform.position, MoveVector), out SphereRayHit, Mathf.Infinity, layerMask))			// Мы пускаем обычный луч и если он во чтото врезалься
			{
				if(RayHit.distance <= Radius & ColStay == false)																// Если луч во время движения врезалься в объект который перед ним а не в дальний объект
				{
					PuckRb.velocity = Vector3.Reflect(PuckRb.velocity, SphereRayHit.normal);
				}
				else if(RayHit.distance <= Radius & ColStay == true)
				{
					PuckRb.velocity = Vector3.Reflect(PuckRb.velocity * 1.3f, SphereRayHit.normal);
				}
			}
		}
	}


	void OnCollisionStay(Collision col)
	{
		if(col.collider.tag == "Bat")		// Если шайбы коснулась какая либо бита
		{
			ColStay = true;
		}
	}


	void OnCollisionExit(Collision col)
	{
		if(col.collider.tag == "Bat")		// Если шайба больше не контактирует ни с одной битой
		{
			ColStay = false;
		}
	}

//----------------------------------------------------------------------------------------------Событие гола---------------------------------------------------------------------------------------------------------------------


	void Goal()																// Этот метод вызываеться один раз во время гола
	{
		GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);			// Ставим скорость шайбы во всех направлениях 0
		GetComponent<Rigidbody>().angularVelocity = new Vector3 (0,0,0);	// Ставим вращение шайбы во всех направлениях 0
		StartCoroutine(AfterGoal());										// Вызываем коронтину AfterGoal	
	}


	IEnumerator AfterGoal()													// Этот метод вызываеться после выполнения метода гол
	{
		yield return new WaitForSeconds(4);									// Ждём 4 секунды
		if(GM.LastGoal == 1)												// Если гол игроку 1
			transform.position = GM.FirstPos;								// Помещаем фишку в первую просчитанную позицию
		else if(GM.LastGoal == 2)											// Если гол игроку 2
			transform.position = GM.SecondPos;								// Помещаем фишку во вторую просчитанную позицию
	}


	void LastGoal()															// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);			// Ставим скорость шайбы во всех направлениях 0
		GetComponent<Rigidbody>().angularVelocity = new Vector3 (0,0,0);	// Ставим вращение шайбы во всех направлениях 0
	}


//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	

	void OnDisable()
	{
		GM.GoalEvent -= Goal;								// Отписываем метод гол от события GoalEvent
		GM.LastGoalEvent -= LastGoal;						// Отписываем метод LastGoal на отбытия LastGoalEvent
	}
}
