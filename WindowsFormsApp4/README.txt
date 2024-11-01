# Проект: <DeliveryApp pre-alpha release>

## Общие сведения

- Назначение: <фильтрует заказы в зависимости от района и времени>
- Технологии: <Windows Forms, ADO.Net>
- Сторонние библиотеки: <Nlog>

## Комментарии по выполнению тестового задания

1. В условиях задачи указано:
- В результирующий файл либо БД необходимо вывести результат фильтрации
заказов для доставки в конкретный район города в ближайшие полчаса после времени
ПЕРВОГО заказа.
- данные для фильтрации Время первой доставки.
данные условия подвергаются двоечтению. В программе реализован сложнейший вариант:
время указанное во входных параметрах фильтрации считается стартовым для поиска, в отчет попадают записи 
время и дата которых находятся в получасовом диапазоне начиная 
с ВРЕМЕНИ ПЕРВОЙ НАЙДЕННОЙ ЗАПИСИ УДОВЛЕТВОРЯЮЩЕЙ УСЛОВИЯМ.
Пример:
указанное время 00:00:01, время первой записи удовлетворяющей условиям 00:15:00
отсюда имеем два варианта ограничения получасового диапазона 00:30:00 и 00:45:00
реализован второй.
2. логирование реализовано с использованием библиотеки NLog все уровни логирования записываются в 1 файл.
3. валидация реализована средствами пользовательского интерфейса
4. тестовые исходные данные прилагаются. В текстовом формате (input.txt) и дамп базы данных MS SQL Server (DeliveriesDB.bacpac).

## Установка

1. Клонируйте репозиторий: `git clone <ссылка на репозиторий>`
2. Установите зависимости: `npm install`
3. Настройте конфигурацию: `cp .env.example .env`
4. Запустите проект: `npm start`

## Использование

1. Укажите идентификатор района доставки
Примечания:
- в прилагаемых файлах (input.txt и DeliveriesDB.bacpac) значения идентификатора района принимает значения от 1 до 5.
- в отчет попадают записи с указанным идентификатом района.
2. Выберите стартовую дату и время для поиска
Примечания:
- в прилагаемых файлах (input.txt и DeliveriesDB.bacpac) значения даты и времени c 00:00:00 23 отктября 2024 года до 23:59:9 30 отктября 2024 года.
- в отчет попадают записи время и дата которых находятся в получасовом диапазоне начиная с ВРЕМЕНИ ПЕРВОЙ НАЙДЕННОЙ ЗАПИСИ УДОВЛЕТВОРЯЮЩЕЙ УСЛОВИЯМ.
3. Задайте путь сохранения результата выборки
4. Задайте путь сохранения логов
5. Укажите путь к текстовому файлу (.txt) с исходными данными (для поиска в базе данных SQL не требуется)
6. Выберите формат исходных данных.
Примечания:
- для режима где исходными данными является БД, строка подключения указывается в файле конфигурации.
- для режима где исходными данными является текстовый файл(.txt). Необходимо указать путь к нему.
7. Выберете формат результатов.
Примечания:
- для режима где исходными данными является БД, строка подключения указывается в файле конфигурации.
- для режима где результаты записываются в текстовый файл необходимо указать путь к нему.


## Разработка

1. Приложение расширяемое посредством наследования от интерфейсов.
1.1 для добавления нового формата исходных данных необходимо наследоваться от интерфейса ISourceDataSetter, 
создать экземпляр наследованного класса и передать в метод Program.MyForm.AddSourceDataSetter();
1.2 для добавления нового формата результатов необходимо наследоваться от интерфейса IReportCreator,
создать экземпляр наследованного класса и передать в метод Program.MyForm.AddReportCreator();

