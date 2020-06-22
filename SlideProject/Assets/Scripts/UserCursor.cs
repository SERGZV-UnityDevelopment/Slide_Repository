// Этот скрипт для отрисовки "игрового" курсора
using UnityEngine;
using System.Collections;


public class UserCursor : MonoBehaviour 
{
	public Texture2D[] Сurs;				// Состояния курсора
	public bool SetCursorVisible = true;	// Сделать пользовательский курсор видимым
	bool MouseDowned = false;				// Эта переменная говорит была ли уже нажата левая кнопка мыши


	void Start()
	{
		Cursor.SetCursor(Сurs[0], new Vector2(16,16), CursorMode.ForceSoftware);
	}
		

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Mouse0) && MouseDowned == false)	// Если левая кнопка мыши была нажата
		{
			Cursor.SetCursor(Сurs[1], new Vector2(16,16), CursorMode.ForceSoftware);
			MouseDowned = true;											// Ставим что кнопка мыши зажата
		}
		else if(Input.GetKeyUp(KeyCode.Mouse0))							// Если левая кнопка мыши была отпущена
		{
			Cursor.SetCursor(Сurs[0], new Vector2(16,16), CursorMode.ForceSoftware);
			MouseDowned = false;										// Ставим что кнопка мыши отжата
		}
	}
}
