// Этот скрипт запускаеться отдельно (при выключенных других скриптах) чтобы сфотографировать и сохранить фотографию префаба
using UnityEngine;
using System.Collections;
using System.IO;
using UnityStandardAssets.ImageEffects;

// Чтобы сделать скриншот объекта нужно положить в переменную Object_ фотографируемый объект
// Чтобы сделать снимок скайбокса ложить объект ненужно
// Искать готовый скриншот нужно в корневой папке проекта unity
// 1) Почему мы сделали в инвентрае отдельно биту и скайбокс? Чтобы можно было отрисовать скайбокс потом рамку выделеиня и потом сверху на ней объект была бы это одна картинка такое было бы невозможно
// 2) А почему мы не можем отрисовывать для 4 активных объектов отдельно скайбокс и отдельно биту? потому что нам нужно фотографировать их с улучшенийми у которых присутствуют частицы а движок не хочет рендерить частицы на прозрачном фоне

public class Photographer : MonoBehaviour 
{
	public enum Mode : byte {FotoObject, FotoAvatar, FotoMaterial, FotoSkybox, FotoMiniBackground}
//	public enum SmoothM : byte {NFAA, FXAA3}
	public Mode PhotographingMode;	// Режим фотографии
//	public SmoothM SmoothMode;		// Режим сглаживания
	public float SharpStrength;		// Сила остроты (Противоположное сглаживанию)
	public GameObject Obj;			// Сюда помещаеться объект (префаб) магазина который нужно сфотографировать
	public GameObject Fotographer;	// Префаб фотограф
	public byte NomberFotoMatrial;	// Вводим номер материала который нужно сфотографировать
	public Texture2D texture;		// Текстура получающаяся на выходе
//	short Resoluton;				// Разрешение с коротым будет рендериться картинка
	Camera CamComponent;			// Ссылка на компонент камера
	GameObject ObjClone;			// Ссылка на клон фотографируемого объекта
	byte[] bytes;					// Сюда сохраним байты фотографии


	void Start () 
	{
		if(Obj && PhotographingMode == Mode.FotoObject)					// Если в переменной Obj чтото лежит и PhotographingMode равен FotoObject
		{
			StartCoroutine(TakeAPictureObject(120));					// Мы запускаем метод для фотографии объекта
		}
		else if(Obj && PhotographingMode == Mode.FotoAvatar)			// Иначе если в переменной Obj чтото лежит и PhotographingMode равен FotoAvatar
		{
			StartCoroutine(TakeAPictureObject(120));					// Мы запускаем метод для фотографии аватара
		}
		else if(Obj && PhotographingMode == Mode.FotoMaterial)			// Если в переменной Obj чтото лежит и PhotographingMode равен FotoMaterial
		{
			StartCoroutine(TakeAPictureObject(46));						// Мы запускаем метод для фотографии материала на объекте
		}
		else if(!Obj && PhotographingMode == Mode.FotoSkybox)			// Иначе если у нас нет указанного для фотографии объекта в obj и выбран режи FotoSkybox
		{
			StartCoroutine(TakeAScreen(120));							// Мы запускаем метод для фотографии скриншота скайбокса
		}
		else if(!Obj && PhotographingMode == Mode.FotoMiniBackground)	// Иначе если у нас нет указанного для фотографии объекта в obj и выбран режи FotoMiniBackground
		{
			StartCoroutine(TakeAScreen(46));							// Мы запускаем метод для фотографии скриншота скайбокса
		}
	//	else if(!Obj && PhotographingMode != Mode.FotoSkybox)			// Если у нас нет указанного для фотографии объекта в obj и при этом не выбран режим FotoSkybox
	//	{	
	//		Debug.LogError("В скрипте Фотограф не указан объект для фотографии. Укажите объект или выберите режим фотографии скайбокса"); // Выводим ошибку
	//	}
		else if(Obj && PhotographingMode == Mode.FotoSkybox)			// Иначе если у нас указан объект в obj и выбран режи FotoSkybox
		{
			Debug.LogError("Выбран режим фотографии скайбокса но при этом указан объект для фотографии. Режим фотографии скайбокса был выбран намеренно?"); // Выводим ошибку
		}
		else if(Obj && PhotographingMode == Mode.FotoMiniBackground)	// Иначе если у нас указан объект в obj и выбран режи FotoMiniBackground
		{
			Debug.LogError("Выбран режим фотографии заднего минифона но при этом указан объект для фотографии. Режим фотографии минифона был выбран намеренно?"); // Выводим ошибку
		}
	}

	
	IEnumerator TakeAPictureObject(short Res)							// Этот метод делает скриншот объекта или улучшения
	{
		RenderTexture RendTex = new RenderTexture(Res, Res, 24, RenderTextureFormat.ARGB32);	// Рендер текстура в которую сначала будет рендериться изображение
		Instantiate(Fotographer, new Vector3(-5 ,100 , -1.4f), Quaternion.identity);			// Помещаем на сцену клон префаба фотографа 
		GameObject CamClone = GameObject.Find("Fotographer(Clone)");							// Находим на сцене объект фотограф и ложим его в переменную CamClone
		Antialiasing Smooth = CamClone.GetComponent<Antialiasing>();							// Ложим скрипт сглавживание в переменную Smooth
		CamComponent = CamClone.GetComponent<Camera>();											// Достаём компонент камера из объекта cam и ложим в переменную CamClone
		CamComponent.targetTexture = RendTex;													// Устанавливаем для 2ой камеры рендер текстуру
		Smooth.edgeSharpness = SharpStrength;													// Устанавливаем силу пиксельности указанную в переменной SharpStrength
		CamComponent.pixelRect = new Rect(0, 0, Res, Res);										// Устанавливаем разрешение для второй камеры

		Smooth.mode = AAMode.NFAA;																// Ставим режим сглаживания NFAA

		if(PhotographingMode == Mode.FotoMaterial) 												// Если режим фотографии FotoMaterial
		{
			Light Light = CamClone.gameObject.GetComponentInChildren<Light>();					// Ложим свет в переменную Light
			Light.intensity = 6f;																// Ставим интенсивность света на 0.5f
			ObjClone = (GameObject)(Instantiate(Obj, new Vector3(-5, 100f, 0f), Quaternion.AngleAxis(90, new Vector3(-1, 0, 0))));		// Помещаем клон "Цели" на сцену под углом 90 градусов а также в переменную ObjClone
			if(ObjClone.tag != "Table")
				ObjClone.GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomberFotoMatrial];	// Меняем на материал с номером указанным в переменной NomberFotoMatrial
			if(ObjClone.tag == "Table")
			{
				ObjClone.transform.GetChild(2).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().FirstMaterials[NomberFotoMatrial];		// Присваиваем материал бордюру стола
				ObjClone.transform.GetChild(1).GetComponent<Renderer>().material = ObjClone.GetComponent<ObjectScript>().SecondMaterials[NomberFotoMatrial];	// Присваиваем первый материал полю стола	
			}
		}
		else 																															// Иначе если режим фотографии другой
			ObjClone = (GameObject)(Instantiate(Obj, new Vector3(-5, 100.12f, 0f), Quaternion.AngleAxis(45, new Vector3(-1, 0, 0))));	// Помещаем клон "Цели" на сцену под углом 45 градусов а также в переменную ObjClone
		if(PhotographingMode == Mode.FotoObject || PhotographingMode == Mode.FotoMaterial)												// Если режим фотографии FotoObject или FotoMaterial
		{
			CamComponent.clearFlags = CameraClearFlags.SolidColor;					// То настраиваем камеру на фотографию объекта без заднего фона
		}
		texture = new Texture2D(Res, Res, TextureFormat.ARGB32, false);				// Создаём текстуру 2D в переменной texure
		float Size = Obj.GetComponent<ObjectScript>().ViewSize;						// Берём размер объекта для фотографии и копируем его в переменную Size
		Vector3 Size3 = new Vector3(Size, Size, Size);								// Создаём новый вектор3 с именем Size3 и заполняем все его оси из переменной Size
		ObjClone.transform.localScale = Size3;										// Придаём клону объекта размер указанный в Size3
		yield return new WaitForEndOfFrame();										// Ждём конца отрисовки текущего кадра
		CamComponent.Render();														// Рендерим изображение с камеры клона в рендер текстуру
		RenderTexture.active = RendTex;												// Указываем активную рендер текстуру откуда считаем методом ReadPixels 
		texture.ReadPixels(new Rect(0, 0, Res, Res), 0,0, false);					// Читаем изображение в переменную texture
		texture.Apply();															// Применяем текстуру
		bytes = texture.EncodeToPNG();												// Превращаем нашу текстуру в поток битов и ложим их в массив "bytes"
		File.WriteAllBytes("Temp/Screens/" +Obj.name + ".png", bytes);				// Сохраняем фотографию в файл
	}


//	IEnumerator TakeAPhotoForAvatar()	// Этот метод делает скриншот объекта для аватарки
//	{
//		RenderTexture RendTex = new RenderTexture(120, 120, 24, RenderTextureFormat.ARGB32);	// Рендер текстура в которую сначала будет рендериться изображение
//		CamClone = GameObject.Find("Fotographer(Clone)").GetComponent<Camera>();				// Помещаем ссылку на этот клон в переменную CamClone
//		CamClone.targetTexture = RendTex;														// Устанавливаем для 2ой камеры рендер текстуру
//		CamClone.pixelRect = new Rect(0, 0, 120, 120);											// Устанавливаем разрешение для второй камеры


//		yield return new WaitForEndOfFrame();						// Ждём конца отрисовки текущего кадра
//	}


	IEnumerator TakeAScreen(short Res) // Этот метод делает скриншот скайбокса
	{
		RenderTexture RendTex = new RenderTexture(Res,Res, 24, RenderTextureFormat.ARGB32); // Рендер текстура в которую сначала будет рендериться изображение
		Instantiate(Fotographer, new Vector3(-5 ,100 , -1.4f), Quaternion.identity);		// Помещаем на сцену клон префаба фотографа
		CamComponent = GameObject.Find("Fotographer(Clone)").GetComponent<Camera>();		// Помещаем ссылку на этот клон в переменную CamClone
		CamComponent.targetTexture = RendTex;												// Устанавливаем для 2ой камеры рендер текстуру
		CamComponent.pixelRect = new Rect(0,0,Res,Res);										// Устанавливаем разрешение для второй камеры
		texture = new Texture2D(Res, Res, TextureFormat.ARGB32, false); 					// Создаём текстуру 2D в переменной texure
		yield return new WaitForEndOfFrame();												// Ждём конца отрисовки текущего кадра
		CamComponent.Render();																// Рендерим изображение с камеры клона в рендер текстуру
		RenderTexture.active = RendTex;														// Указываем активную рендер текстуру откуда считаем методом ReadPixels 
		texture.ReadPixels(new Rect(0, 0, Res, Res), 0,0, false);							// Читаем изображение в переменную texture
		texture.Apply();																	// Применяем текстуру
		bytes = texture.EncodeToPNG();														// Превращаем нашу текстуру в поток битов и ложим их в массив "bytes"
		File.WriteAllBytes("Temp/Screens/" + ".png", bytes);								// Сохраняем фотографию в файл
	}
}
