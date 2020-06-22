using UnityEngine;
using System.Collections;

// Создаём перечисление всех типов предметов вне класса чтобы он был виден другим скриптам
// public enum Cat : byte {Bats, Washers, Tables, Skyboxes}		// Перечисление категории объекта
// public enum Lev	: byte {One, Two, Three, Four, Five}		// Перечисление уровня объекта

// На заметку первый материал в массиве является стандартным материалом для этого объекта
// На заметку для улучшения используеться один стандартный материал или любой из открытых для этого объекта
public class ObjectScript : MonoBehaviour 
{
	public string ObjectName;				// Игровое имя объекта
	public string NPCName;					// Имя компьютера играющего против игрока гг компьютерный герой
	public byte ObjectLevel;				// Эта переменная указывает уровень объекта будь то стол, бита или шайба
	public Texture2D Avatar;				// Аватар компьютерного игрока (используеться только для биты),(заполняеться в ручную)
	public Texture2D FotoStore;				// Фотография для магазина (заполняеться в ручную)
//	public Texture2D FotoInventory;			// фотография для инвентаря	(заполняеться автоматичесски при старте игры)
	public int Price;						// Цена объекта в кредитах
	public float Mass;						// Масса объекта "без улучшений" "Rigidbody будет выдавать общую массу" (Применима только к битам и шайбам) 
	public float Force;						// Усилие которое способна генерировать для своего движения бита
//	public Vector3 ViewSize;				// Размер объекта при просмотре 3d окне
	public float ViewSize;					// Размер объекта при просмотре в 3D окне
	public string[] NamesMaterials;			// Имена материалов
	public string[] NamesImprovements;		// Имена улучшений
	public string[] NamesTypesImprovements;	// Имена типов улучшений
	public Material[] FirstMaterials;		// Главный или первый материал если их несколько, также это материал для бордюра если это игровой стол
	public Material[] SecondMaterials;		// Это второй материал для шайбы, биты или первый материал для поля 
	public Material[] ThirdMaterials; 		// Это третий материал для шайбы, биты, или второй материал для поля эсли это игровой стол
	public Texture2D[] FotoMaterials;		// Фотографии этих материалов на объектах
	public GameObject[] Improvements;		// Улучшения доступные для этого объекта
	public Texture2D[] IconImprovements;	// Иконки улучшений
	public int[] PricesMats;				// Цены для материалов в кредитах
	public int[] PricesImprs;				// Цены для улучшений в кредитах
	public int[] RequireExpirienceMats;		// Массив сколько опыта требуеться для открытия следующего материала этого объекта
	public int[] RequireExpirienceImprs;	// Массив сколько опыта требуеться для открытия следующего улучшения этого объекта
	//	public Vector3 FotoSize;			// Размер объекта при фотографии (Можно получать этот размер умножая ViewSize на 5 или на 2)
}
