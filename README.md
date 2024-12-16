# Stend
### О проекте
Приложение создано для презентации возможностей модели маашинного обучения, по определению местоположения на предоставленном спутниковом изображении.
Приложение является клиентом, для полноценной работы необходимо подключение к удаленноу серверу, взаимодействие с которым происходит встроенными средствами C#.

---

### Запуск 
Дл запуска необходимо открыть файл Stend.sln в среде Microsofr Visual Studio, все необходимые библиотеки уснановлены в директорию проекта, пути установлены в самом проекте, по этому никаких дополнительных манипусяций не требуется.

---

### Инструкция:
Перетаскивание карты осуществляется зажатием ЛКМ.
Изменение масштаба карты осуществляется в помощью колесика мышы.

При введении координат (дробные числа через ",") нажать применить, 
после чего на карте будет отображен указанный квадрат, с заранее заданным приближением

1. Выбрать режима снимка "Основная карта".
2. Нажать кнопку "сделать снимок",
3. В директории sourse появился файл с имненем map_im.png.
4. Выбрать режим "Снимок местности".
5. Нажать кнопку "сделать снимок".
6. В директории sourse появился файл с имненем satelite_im.jpg.
7. Нажать кнопку "Поиск".
8. В директории sourse появился файл с имненем result.jpg, на котором отображена найденная область.

В случае загрузки изображения выбор типа изображения изменяется анологично выше описааному методу.

---
### Серверная часть

---

### Пример запроса на сервер


Запрос
```bash
curl -X POST https://a6a6-192-162-250-97.ngrok-free.app/process-images \
-F "map_image=@C:\Users\...\22.png" \
-F "satellite_image=@C:\Users\...\23.png" \
-o result.jpg --max-time 60
```

Запрос отправляет два изображения на сервер для обработки. Сервер принимает изображения: эталонную карту и спутниковый снимок, а затем возвращает изображение, на котором отмечена область совпадения между двумя входными изображениями.

https://a6a6-192-162-250-97.ngrok-free.app/process-images: URL-адрес эндпоинта, на который отправляется запрос.

Домен создан через ngrok для создания публичного туннеля на локальный сервер.
Конечная точка /process-images отвечает за обработку изображений.

*map_image*: название поля формы, которое сервер ожидает для эталонной карты.
@C:\...\22.png: указание пути к файлу изображения, которое будет загружено как содержимое этого поля.

*satellite_image*:
Аналогично, указывает путь к спутниковому изображению, которое сервер примет через поле формы satellite_image.


-o result.jpg:
Указывает, что результат запроса (обработанное изображение) должен быть записан в файл с именем result.jpg.

--max-time 60:
Ограничивает максимальное время выполнения запроса до 60 секунд. Если сервер не отвечает в течение этого времени, запрос будет завершён с ошибкой.

   
### Возможные ответы сервера
*Успешный ответ (200 OK)*:
  - Обработанное изображение возвращается и сохраняется как result.jpg.

*Ошибка (400 Bad Request)*:
   Отправлены некорректные данные.
   Пример: отсутствие одного из изображений.
   
*Ошибка (500 Internal Server Error)*:
   Внутренняя ошибка на сервере.
   Пример: сбой в работе моделей SuperPoint/SuperGlue.
