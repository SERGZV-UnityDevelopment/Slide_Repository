using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public ObjectScript OS;						// Переменная для скрипта ObjectScript
	public GameManager GM;						// Переменная для скрипта GameManager
	public bool PlayerControl = true;			// Находиться ли шайба под контролем игрока
	public bool RightPlayer = false;			// Этот скрипт указывает с какой стороны находиться игрок
	public byte NomberPlayer;					// Сдесь находиться номер игрока каоторый управляет этой шайбой 1 или 2
	public bool ImprActive;						// (Improvement Activate) Эта переменная говорит активно ли улучшение для этого игрока
	public Vector3 Dir;							// Вектор движения который необходимо придать бите основываясь на нажатых кнопках игрока
	Rigidbody TBR;								// (ThisBatRigitbody) Это игрок на котором висит этот скрипт


	void Awake()																// Как только объект с этим скриптом попадёт на сцену
	{
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();		// Ложим в переменную GM скрипт GameManager с пустышки на сцене под темже именем GameManager
		GM.GoalEvent += Goal;													// Подписываем метод гол на событие GoalEvent
		GM.LastGoalEvent += LastGoal;											// Подписываем метод LastGoal на событие LastGoalEvent
		OS = GetComponent<ObjectScript>();										// В переменную ObjectScript загружаем ObjectScript
		TBR = GetComponent<Rigidbody>();										// Ложим в переменную TBR Rigidbody биты на которой висит этот скрипт
	}


	void Update()
	{
		if(PlayerControl == true)												// Если переменная PlayerControl правда
		{
			if(RightPlayer)														// Если этот скрипт висит на бите играющей на правой стороне
			{
				if(Input.GetAxis("Right Vertical")>0.1f)						// Если поступил сигнал направления вверх
				{
					TBR.AddForce(0,0,OS.Force * Time.deltaTime * 1200);			// Придаём направление вверх
				}
				if(Input.GetAxis("Right Vertical")<-0.1f)						// Если поступил сигнал направления вниз
				{
					TBR.AddForce(0,0,-OS.Force * Time.deltaTime * 1200);		// Придаём направление вниз
				}
				if(Input.GetAxis("Right Horizontal")>0.1f)						// Если поступил сигнал направления вправо
				{
					TBR.AddForce(OS.Force * Time.deltaTime * 1200,0 ,0);		// Придаём направление вправо
				}
				if(Input.GetAxis("Right Horizontal")<-0.1f)						// Если поступил сигнал направления влево
				{
					TBR.AddForce(-OS.Force * Time.deltaTime * 1200,0 ,0);		// Придаём направление влево
				}
				if (Input.GetButtonDown("Right Action")) 						// Если у правого игрока была нажата кнопка активации улучшения
				{
					ImprActive = ImprActive == true ? ImprActive = false : ImprActive = true;
				}
				Dir = (new Vector3(Input.GetAxis("Right Horizontal"), 0, Input.GetAxis("Right Vertical")));	// Высчитываем вектор направления который необходимо придать бите основываясь на нажатых кнопках игрока
			}
			else 																// Иначе если этот скрипт висит на бите играющей на левой стороне
			{
				if(Input.GetAxis("Left Vertical")>0.1f)							// Если поступил сигнал направления вверх
				{
					TBR.AddForce(0,0,OS.Force * Time.deltaTime * 1200);			// Придаём направление вверх
				}
				if(Input.GetAxis("Left Vertical")<-0.1f)						// Если поступил сигнал направления вниз
				{
					TBR.AddForce(0,0,-OS.Force * Time.deltaTime * 1200);		// Придаём направление вниз
				}
				if(Input.GetAxis("Left Horizontal")>0.1f)						// Если поступил сигнал направления вправо
				{
					TBR.AddForce(OS.Force * Time.deltaTime * 1200,0 ,0);		// Придаём направление вправо
				}
				if(Input.GetAxis("Left Horizontal")<-0.1f)						// Если поступил сигнал направления влево
				{
					TBR.AddForce(-OS.Force * Time.deltaTime * 1200,0 ,0);		// Придаём направление влево
				}
				if (Input.GetButtonDown("Left Action")) 						// Если у правого игрока была нажата кнопка активации улучшения
				{
					ImprActive = ImprActive == true ? ImprActive = false : ImprActive = true;
				}
				Dir = (new Vector3(Input.GetAxis("Left Horizontal"), 0, Input.GetAxis("Left Vertical")));	// То высчитываем вектор силы который необходимо придать бите основываясь на нажатых кнопках игрока
			}
		}
	}
		

//-----------------------------------------------------------------------------------------Событие гола--------------------------------------------------------------------------------------------------------------------------
	

	void Goal()															// Этот метод вызываеться один раз во время гола
	{
		PlayerControl = false;											// Отбираем управление у игрока
		GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);    	// Ставим скорость биты во всех направлениях 0
		StartCoroutine(AfterGoal());									// Вызываем коронтину AfterGoal					
	}


	IEnumerator AfterGoal()												// Этот метод вызываеться после выполнения метода Goal
	{
		yield return new WaitForSeconds(4); 							// Ждём 4 секунды
		if(NomberPlayer == 1)											// Если это бита первого игрока
			transform.position = GM.SPP1;								// То мы устанавливаем его на просчитанную позицию для первого игрока
		else if(NomberPlayer == 2)										// Иначе если это второй игрок
			transform.position = GM.SPP2;								// То ставим его на просчитанную позицию для второго игрока
		yield return new WaitForSeconds(1);								// Ждём 1 секунду
		PlayerControl = true;      										// Возвращаем управление у игрока
	}


	void LastGoal()														// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		PlayerControl = false;											// Отбираем управление у игрока
		GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);    	// Ставим скорость биты во всех направлениях 0
		StartCoroutine(AfterLastGoal());								// Вызываем коронтину AfterLastGoal
	}


	IEnumerator AfterLastGoal()											// Этот метод вызываеться после выполнения метода LastGoal
	{
		yield return new WaitForSeconds(5);								// Ждём 1 секунду
		PlayerControl = true;      										// Возвращаем управление у игрока
	}

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


	void OnDisable()
	{
		GM.GoalEvent -= Goal;											// Отписываем метод гол от события GoalEvent
		GM.LastGoalEvent -= LastGoal;									// Отписываем метод LastGoal на отбытия LastGoalEvent
	}
}
