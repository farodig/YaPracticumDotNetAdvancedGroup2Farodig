# YaPracticumDotNetAdvancedGroup2Farodig

## Инструкция по установке

1. Скачать актуальный [SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
2. Скачать [репозиторий](https://github.com/farodig/YaPracticumDotNetAdvancedGroup2Farodig.git) себе на компьютер
3. В git переключиться на последнюю актуальную ветку
4. Установить [PostgreSQL](https://www.postgresql.org/download/) и добавить пользователя со всеми правами Username=postgres;Password=postgres
   или скачать и запустить [образ docker postgresql](https://github.com/farodig/YaPracticumDotNetAdvancedGroup2Farodig/blob/sprint-9/docker-compose.yml)
5. Настроить подключение к [базе данных](#бд-postgresql)
и авторизацию см раздел [Настройка секрета JWT в конфигурации](#настройка-секрета-jwt-в-конфигурации)

6. Зайти в консоль от администратора
7. В корневой папке проекта выполнить команду dotnet test подробнее в разделе [Тестирование](#тестирование)

⚠️ **Важно:** Для запуска итеграционных тестов на компьютере должен быть установлен Docker

8. Зайти в подпапку скачанного репозитория каждого из проектов EventService.Presentation, BookingService.Presentation, PersonService.Presentation
и  Выполнить команду dotnet run

📝 **Примечание:** См. [структуру проектов](#структура-проектов)

10. Открыть в браузере [booking service](https://localhost:7108/swagger/index.html), [event service](https://localhost:7240/swagger/index.html), [person service](http://localhost:5132/swagger)
11. Некоторые команды API сервисов EventService и BookingService требуют авторизации. Для авторизации пользователя необходимо выполнить [вход](#авторизация) пользователя, после чего ввести токен в поле после нажатия кнопки [Authorize]

## Телеметрия
В проекте производится сбор данных с сервисов PersonService, EventService, BookingService

### Инструменты
* Prometheus — сбор метрик
* Grafana — визуализация метрик
* Jaeger — трассировка запросов
* OpenTelemetry:
    HTTP - входящие и исходящие запросы,
    EF Core - запросы к бд,
    Kafka - брокер сообщений,
    Prometheus и Jaeger - экспорт данных.

### UI-интерфейс доступа
Prometheus	http://localhost:9090
Grafana	http://localhost:3000
Jaeger	http://localhost:16686

## Структура проектов

Сервисы и компоненты:
- PersonService.* - сервис позволяет зарегистрировать и авторизовать по пользователей
- EventService.* - сервис позволяет создавать, изменять и удалять события
- BookingService.* - сервис позволяет бронировать события
- BrokerService.* - сервис обмена сообщениями между сервисов
- SharedContracts - содержит контракты обмена сообщениями
- TokenService - вспомогательный сервис(компонент/библиотека) для создания и проверки токенов

1. Domain - всё, что описывает предметную область и не зависит от технологий
    - доменные сущности и перечисления;
    - доменные исключения
          
2. Application - бизнес-логика и абстракции
    - интерфейсы сервисов и их реализации
    - интерфейсы портов — абстракции для доступа к данным (репозитории) и внешним сервисам
    - объекты передачи данных между слоями
    - фоновые сервисы
  
3. Infrastructure - реализации, которые зависят от внешних технологий
    - реализации интерфейсов репозиториев с использованием DbContext
    - сам DbContext, конфигурации маппинга сущностей, миграции
    - любые другие адаптеры к внешним системам

4. Presentation
    - эндпоинты/контроллеры
    - обработчики глобальных исключений с маппингом доменных исключений в HTTP-статусы
    - регистрация всех зависимостей

## Авторизация
### Роли
- Аноним - неавторизованный пользователь
- User - обычный пользователь (авторизованный)
- Admin - пользователь с правами администратора (авторизованный)

### Получение и работа с токеном
Авторизация с использованием JWT токена по схеме bearer
Для авторизации необходимо получить токен через запрос POST /auth/login
Предварительно пользователь должен быть создан, создать пользователя можно через запрос POST /auth/register
полученный токен необходимо добавить в заголовок авторизованного http запроса
```markdown
Authorization: Bearer <token>
```
📝 **Примечание:** В swagger есть отдельная кнопка [Authorize] для добавления заголовка с авторизацией

### Настройка секрета JWT в конфигурации

Для проектов PersonService.Presentation, BookingService.Presentation и EventService.Presentation
в файле конфигурации appsettings.json в разделе TokenSettings задаются следующие поля 

- Secret - приватный секретный ключ используемый для подписи токенов
- Issuer - издатель
- Audience - аудитория
- ExpirationMin - время жизни токена в минутах

пример:

```markdown
  "TokenSettings": {
    "Secret": "your-super-secret-key-with-minimum-16-characters",
    "Issuer": "EventApiIssuer",
    "Audience": "EventApiAudience",
    "ExpirationMin": 60
  }
```
⚠️ **Важно:** Нельзя хранить файл конфигурации с секретом в публичном репозитории, во избежание подделки токенов. Серкетный ключ необходимо настраивать в конфигурации прямо на развёртываемом ПК.

## Возможные варианты ответов от сервера
|Код ответа |Тип    |Описание                 |
|-----------|-------|-------------------------|
|200        |Успех  |Событие успешно получено |
|201        |Успех  |Событие успешно создано  |
|400        |Неудача|Запрос содержит ошибки   |
|401        |Неудача|Ошибка авторизации       |
|403        |Неудача|Операция запрещена       |
|404        |Неудача|Событие не найдено       |
|409        |Неудача|Конфликт бронирования    |
|500        |Неудача|Внутренняя ошибка сервера|

## Формат ответа при ошибках
| Поле    | Тип     | Описание                                                   |
|---------|---------|------------------------------------------------------------|
| type    | string  | URI ссылка на спецификацию типа ошибки (RFC 9110).         |
| title   | string  | Краткое описание ошибки.                                   |
| status  | integer | HTTP статус-код ошибки.                                    |
| errors  | object  | (опциональный) Список ошибок валидации полей.              |
| detail  | string  | (опциональный) Дополнительная информация об ошибке.        |
| traceId | string  | Уникальный идентификатор запроса для трассировки и отладки.|

## Логика фоновой обработки
1. бронирование создаётся и помещается в очередь запросом POST /events/{id}/book в состоянии Pending
2. сервис периодически опрашивает хранилище на наличие бронирований в статусе Pending;
3. для каждой необработанной брони бронь переводится в статус Confirmed и заполняется поле ProcessedAt
4. обновлённая бронь сохраняется в хранилище.
5. проверить состояние бронирования можно запросом GET /bookings/{id}

## БД PostgreSQL
В файл конфигурации appsettings.json в корневой узел каждого проекта Presentation добавить строку подключения базе данных

📝 **Примечание:** Если БД пустая, то структура будет создана при первом запуске через Migrate

### Строка подключения для PersonService.Presentation

```markdown
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventapi;Username=postgres;Password=postgres"
  }
```

### Строка подключения для EventService.Presentation

```markdown
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventapi;Username=postgres;Password=postgres"
  }
```

### Строка подключения для BookingService.Presentation

```markdown
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventapi;Username=postgres;Password=postgres"
  }
```

### Особенности

Cхема управляется миграциями EF Core для сервисов PersonService, EventService, BookingService

#### Добавление изменений

Изменения в структуру базы вносятся через код. После внесения изменений необходимо выполнить команду add добавления миграции

> dotnet ef migrations add [ИмяМиграции] --project [ИмяСервиса].Infrastructure --startup-project [ИмяСервиса].Presentation

После того как миграция будет создана и код миграции проверен, можно внести изменения командой update

> dotnet ef database update --project [ИмяСервиса].Infrastructure --startup-project [ИмяСервиса].Presentation

#### Откат изменений

Если нужно откатить изменения то можно выполнить следующие команды:

Откатить изменения в БД последней миграции

> dotnet ef database update [Имя_Предыдущей_Миграции] --project [ИмяСервиса].Infrastructure --startup-project [ИмяСервиса].Presentation

Или откатить изменения всех миграций

> dotnet ef database update 0 --project [ИмяСервиса].Infrastructure --startup-project [ИмяСервиса].Presentation

После отката изменений в БД можно удалить саму миграцию

> dotnet ef migrations remove --project [ИмяСервиса].Infrastructure --startup-project [ИмяСервиса].Presentation

## Тестирование

Решение содержит два проекта
- UnitTests - юнит тесты

```markdown
Запустить только юнит тесты:
dotnet test --filter "Category=Unit"
```
⚠️ **Важно:** В юнит тестах используется БД InMemory-провайдер

- IntegrationTests интеграционные тесты

```markdown
Запустить только интеграционные тесты:
dotnet test --filter "Category=Integration"
```

⚠️ **Важно:** Для запуска итеграционных тестов на компьютере должен быть установлен Docker

