using UnityEngine;
using System.Collections;

public class GateTrigger : MonoBehaviour 
{
	public GameManager GM;		// Переменная для скрипта GameManager
	public byte PlayerGate;		// Сдесь храниться номер игрока которому принадлежат эти ворота

	void Awake()				// Как только объект с этим скриптом попадёт на сцену
	{
		// Ложим в переменную GM скрипт GameManager с пустышки на сцене под темже именем GameManager
		GM = GameObject.Find("GameManager").GetComponent<GameManager>();
	}


	void OnTriggerEnter(Collider Col)								// Это метод вызываеться при попадании в ворота любого объекта
	{
		if(Col.gameObject.tag == "Puck" && GM.GoalTrue == false)	// Если объект с тегом шайба коснулься тригера и в этом раунде ещё ни кому не было гола
		{
			GM.CallGoalEvent(PlayerGate); 							// Вызываем метод вызывающий событие гола
		} 
	}

}
