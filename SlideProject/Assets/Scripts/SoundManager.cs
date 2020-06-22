using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	public GameManager GM;		// Переменная для скрипта GameManager
	public AudioClip Ovations;	// Переменная для аудиоклипа "Овации"
	public AudioClip Whistle;	// Переменная для аудиоклипа "Свист"


	void Awake()
	{
		// Ложим в переменную GM скрипт GameManager с пустышки на сцене под темже именем GameManager
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();
		GM.GoalEvent += Goal;			// Подписываем метод Гол на событие GoalEvent
		GM.LastGoalEvent += LastGoal;	// Подписываем метод Гол на событие LastGoalEvent
	}


//-----------------------------------------------------------------------------------------Событие гола--------------------------------------------------------------------------------------------------------------------------
	

	void Goal()													// Этот метод вызываеться после события гола
	{
		GetComponent<AudioSource>().PlayOneShot(Ovations);		// Проигрываем звук овации	
		StartCoroutine(AfterGoal());							// Вызываем коронтину AfterGoal					
	}


	IEnumerator AfterGoal()										// Этот метод вызываеться после выполнения метода гол
	{
		yield return new WaitForSeconds(5); 					// Ждём 5 секунд
		GetComponent<AudioSource>().PlayOneShot(Whistle);		// выводим звук на старт
	}


	void LastGoal()												// Этот метод вызываеться в тот момент когда забиваеться последний гол на текущем уровне
	{
		GetComponent<AudioSource>().PlayOneShot(Ovations);		// Проигрываем звук овации	
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
}
