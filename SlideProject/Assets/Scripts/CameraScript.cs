using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
	public PauseMenu PM;						// Сдесь храниться скрипт меню паузы
	public Transform GMTransform;				// Сдесь находиться трансформация ГеймМенеджера
	float Min = 14;								// Минимальная дистанция до поля
	float Max = 25;								// Максимальная дистанция от поля
	float RotMin = 1;							// Минимально допустимое вращение пустышки GameObject
	float RotMax = 90;							// Максимально допустимое вращение пустышки GameObject
	float distance ;							// Дистанция мжду камерой и целью
	Vector3 target = new Vector3(0,0,0);		// Сдесь храняться координаты цели камеры
	float X = 36;								// Сдесь находияться накапливаемое смещение курсора по оси X


	void Update () 
	{
		if(PM.Window == GameMenuWins.Game)
		{
			CamTransform();				// Вызываем метод CamTransform
		}
	}


	void CamTransform()					// Этот метод позволяет менять трансформации камеры
	{
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			distance = Vector3.Distance(transform.position, target);			// Узнаём дистанцию от камеры до цели
			distance = (Input.GetAxis("Mouse ScrollWheel") + distance);			// Изменяем значение переменной дистанция	
			distance = ZoomLimit();												// Вызываем метод ZoomLimit чтобы откорректировать дистанцию и не дать ей выдти за пределы
			transform.position = (-transform.forward * distance);				// Присваиваем новую позицию камере
		}

		if(Input.GetKey(KeyCode.Mouse0))
		{
			X += Input.GetAxis("Mouse Y") * Time.deltaTime * 300;				// Отнимаем от переменной X координаты мыши по оси X
			X = RotateLimit();													// При необходимости ограничиваем вращение
			GMTransform.transform.rotation = Quaternion.Euler(X,0,0);			// Придаём объекту GMTransform высчитанное вращение
		}

		if(Input.GetKeyDown(KeyCode.Mouse1))								// Если была нажата вторая кнопка мыши
		{
			X = 36;															// То ставим вращение X на по умолчанию
			GMTransform.transform.rotation = Quaternion.Euler(X,0,0);		// И приравниваем это значение вращению пустышки на которой висит наша камера и скрипт GameManager

		}
	}


	float ZoomLimit()								// Этот метод не позволяет вывести камеру за пределы допустимых лимитов
	{
		if(distance < Min) distance = Min;			// Если вдруг камера выйдет за пределы допустимого минимума ставим камеру на допутисый минимум
		else if (distance > Max) distance = Max;	// Если вдруг камера выйдет за пределы допустимого максимума ставим камеру на допутисый максимум
		return distance;							// Возвращаем дистанцию
	}


	float RotateLimit()								// Этот метод ограничивает вращение GMTransform
	{
		if(X < RotMin) X = RotMin;					// Если X меньше RotMin то приравниваем X RotMin
		else if(X > RotMax) X = RotMax;				// Если X меньше RotMax то приравниваем X RotMax
		return X;									// Возвращаем вращение по оси X
	}
}
