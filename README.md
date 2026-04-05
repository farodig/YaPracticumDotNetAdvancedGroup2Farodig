# YaPracticumDotNetAdvancedGroup2Farodig

## Инструкция по установке

1. Скачать актуальный [SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
2. Скачать [репозиторий](https://github.com/farodig/YaPracticumDotNetAdvancedGroup2Farodig.git) себе на компьютер
3. В git переключиться на ветку sprint-1
5. Зайти в консоль от администратора
6. Зайти в подпапку скачанного репозитория LearningWebApi/ 
7. выполнить команду dotnet run
8. Открыть в браузере [http](http://localhost:5120/swagger/index.html) или [https](https://localhost:7112/swagger/index.html)

## API

|Метод |Адрес       |Запрос                                   |Ответ                            |Описание                          |
|------|------------|-----------------------------------------|---------------------------------|----------------------------------|
|GET   |/events     |                                         |[EventResponse](#EventResponse)[]|Получить список всех событий      |
|GET   |/events/{id}|                                         |[EventResponse](#EventResponse)  |Получить событие по идентификатору|
|POST  |/events     |[CreateEventRequest](#CreateEventRequest)|[EventResponse](#EventResponse)  |Создать событие                   |
|PUT   |/events/{id}|[UpdateEventRequest](#UpdateEventRequest)|                                 |Изменить событие                  |
|DELETE|/events/{id}|                                         |                                 |Удалить событие                   |


## Схемы запросов/ответов

### EventResponse

|Поле       |Тип данных|Описание                         |Пример                                |
|------     |----------|---------------------------------|--------------------------------------|
|id         |Guid      |Идентификатор события            |"3fa85f64-5717-4562-b3fc-2c963f66afa6"|
|title      |string    |Заголовок события                |"Заголовок события"                   |
|description|string    |Описание события (необязательный)|                                      |
|startAt    |DateTime  |Дата и время начала события      |"2027-04-05T00:51:58.951Z"            |
|endAt      |DateTime  |Дата и время окончания события   |"2027-04-05T00:51:58.951Z"            |

### CreateEventRequest

|Поле       |Тип данных|Описание                         |Пример                                |
|------     |----------|---------------------------------|--------------------------------------|
|title      |string    |Заголовок события                |"Заголовок события"                   |
|description|string    |Описание события (необязательный)|                                      |
|startAt    |DateTime  |Дата и время начала события      |"2027-04-05T00:51:58.951Z"            |
|endAt      |DateTime  |Дата и время окончания события   |"2027-04-05T00:51:58.951Z"            |

### UpdateEventRequest
|Поле       |Тип данных|Описание                         |Пример                                |
|------     |----------|---------------------------------|--------------------------------------|
|title      |string    |Заголовок события                |"Заголовок события"                   |
|description|string    |Описание события (необязательный)|                                      |
|startAt    |DateTime  |Дата и время начала события      |"2027-04-05T00:51:58.951Z"            |
|endAt      |DateTime  |Дата и время окончания события   |"2027-04-05T00:51:58.951Z"            |