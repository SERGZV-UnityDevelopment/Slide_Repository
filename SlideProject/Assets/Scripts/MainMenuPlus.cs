// Это дополнительный скрипт главного меню использующийся для отрисовки некторых окон и кнопок главного меню поверх элементов скрипта главного меню

using UnityEngine;
using System;
using System.Collections;


public class MainMenuPlus : MonoBehaviour
{
	public MainMenu MM; 						// Ссылка на главный скрипт меню MainMenu
	public Keeper Kep;							// Переменная для скрипта "Хранитель"
	public GUISkin GameSkin;					// Скин для всех гуи элементов
	public Texture[] TexArry = new Texture[1];	// Текстура для подверждения удаления профиля
	public byte NRP;							// (Nomer Removed Profile) Номер Удаляемого Профиля сюда передаёться номер удаляемого профиля
	float OriginalHight = 1080; 				// Заносим а переменную OriginalHight высоту экрана в которой разрабатывалась игра
	float RatioH;								// Сюда заноситься результат деления оригинальной высоты экрана на текущую


	void Start()
	{
		RatioH = Screen.height / OriginalHight;		// Заносим в ScreenBalansHight результат деления описанный выше
	}


	void OnGUI()
	{
		GUI.depth = 1;								// Устанавливаем дальность гуи от камеры на первый слой
		GUI.skin = GameSkin;						// Устанавливаем гуи скин

		if(MM.Window == MainMenuWins.MainMenu)		// Если мы находимся в окне главного уровня и
		{
			if(MM.ChoiseOfLevel == true) 			// Если переменная показывать окно для выбора уровня правда
			{
				// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
				GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
				MM.ButtonEnabled = false;																// Делаем кнопки меню неактивными
				GUI.BeginGroup(new Rect(-340, -292, 680, 600), TexArry[0]);								// Начинаем группу и рисуем текстуру бакграунда

				// Отрисовываем вид уровня в окошке
				GUI.Label(new Rect(25, 25, 600, 500), MM.LevelScrins[MM.SelectedLevel], "FrameLevelSelection");

				GUI.Label(new Rect(626, 240, 40, 40), MM.SelectedLevel + "", "NomberSelectedLevel");	// Отрисовываем выбранный уровень
				if(GUI.Button(new Rect(626, 200, 40, 40), "", "ButtonArrowUp"))							// Рисуем кнопку уровень вверх
				{
					if(Kep.Progress[Kep.ActiveProfile] > MM.SelectedLevel)								// Если текущий уровень доступный для прохождения больше выбранного уровня
					MM.SelectedLevel ++;																// То мы прибавляем переменной "SelectedLevel" +1										
				}	

				if(GUI.Button(new Rect(626, 280, 40, 40), "", "ButtonArrowDown"))						// Рисуем кнопку уровень вниз
				{
					if(MM.SelectedLevel >= 1)															// Если выбранный уровень больше или равен еденице
						MM.SelectedLevel --;															// То мы отнимаем у переменной "выбранный уровень" 1
				}

				if(GUI.Button(new Rect(103, 540, 186, 42), "Загрузить"))								// Если нажата кнопка загрузить
				{
					Kep.PlayedLevels[Kep.ActiveProfile] = MM.SelectedLevel; 						 		// Устанавливаем переменной Selected level этот уровень
					UnityEngine.SceneManagement.SceneManager.LoadScene(Convert.ToString(MM.SelectedLevel)); // Загружаем выбранный уровень
				}

				if(GUI.Button(new Rect(391, 540, 186, 42), "Отмена"))									// Если нажата кнопка отмена
				{
					MM.ChoiseOfLevel = false;															// Убираем окно выбора уровня
					MM.ButtonEnabled = true;															// Делаем кнопки меню активными
				}
				GUI.EndGroup();																			// Заканчиваем группу
			}
		}
		if(MM.Window == MainMenuWins.Profile)	// Если мы находимся в окне просмотра профиля
		{
			// Изменяем размер матрицы под новый экран и отрисовываем элементы с середины экрана по ширине и с середины по высоте
			GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width/2, Screen.height/2, 0), Quaternion.identity, new Vector3(RatioH,RatioH,1));
			if(MM.ConfirmDelProf)																// Если переемнная ConfirmDelProf равна правда
			{
				MM.ButtonEnabled = false;														// Блокируем кнопки главного меню
				GUI.DrawTexture(new Rect(-238, -100, 476, 140), TexArry[1]);					// Рисуем окошко подтверждения
				GUI.Label(new Rect(0, -75, 0, 0), "<color='#00fc00'>Удалить профиль?</color>", "ConfurmationText");	// Рисуем текст подтверждения
				if(GUI.Button(new Rect(-204, 10, 186, 42), "Да"))	// Рисуем кнопку "Да"
				{
					MM.DeleteProfile(NRP);							// Вызываем метод удалить профиль и передаём ему значение удаляемого профиля
					MM.ConfirmDelProf = false;						// Переменной "Показывать ли окно удаленя профиля ставим нет
					MM.ButtonEnabled = true;						// Отключаем блокирование кнопок главного меню
					if(NRP == MM.NumberProfile)						// Если номер удаляемого профиля и просматриваемого профиля одинаков
						MM.ReviewProfile = false;					// То ставим просмотр профиля ложь
				}

				if(GUI.Button(new Rect(18, 10, 186, 42), "Нет"))	// Если нажата кнопка "Нет"
				{
					MM.ConfirmDelProf = false;						// Переменной "Показывать ли окно удаленя профиля ставим нет
					MM.ButtonEnabled = true;						// Отключаем блокирование кнопок главного меню
				}
			}
		}

	}


}
