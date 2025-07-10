# TestProjectMap API

## Как запустить

1. Установите [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0), если он ещё не установлен.
2. В папке проекта выполните:
   ```
   dotnet run
   ```
3. API будет доступен по адресу http://localhost:5184 (см. launchSettings.json).

### Docker

Также можно запустить через Docker:

1. Соберите образ:
   ```
   docker build -t testprojectmap .
   ```
2. Запустите контейнер:
   ```
   docker run -p 5184:8080 testprojectmap
   ```
3. API будет доступен по адресу http://localhost:5184

## Endpoints

### Get all fields
```
GET /fields/
```

### Get field size by id
```
GET /fields/{id}/size
```

### Distance to field center
```
POST /fields/{id}/distance
Content-Type: application/json
{
  "lat": 45.705,
  "lon": 41.345
}
```

### Check if point is inside any field
```
POST /fields/contains
Content-Type: application/json
{
  "lat": 45.705,
  "lon": 41.345
}
```
- Returns `{ "id": ..., "name": ... }` if found, 404 if not found.

---

## Notes & Issues

- **Производительность XDocument**: Сначала использовал `Descendants`, но это сильно тормозило, потому что возвращает все вложенные элементы, а не только прямых потомков. Перешёл на `Element` и `Elements` — стало быстрее и предсказуемее.
- **XML namespace**: XDocument плохо работает с namespace как строкой. Решение — всегда использовать `XNamespace` для всех XML-запросов, тогда поиск элементов работает корректно.
- **Точки (Point)**: Использование tuple для точек (например, `(double, double)`) мешало сериализации и выглядело неудобно в API. Перешёл на отдельный класс (record) `Point` — стало и в коде, и в ответах API намного лучше.
- **Обработка отсутствующих данных**: Сначала выбрасывал exception в FieldService, если не хватало данных или поле не найдено. Потом решил возвращать null — так сервис работает мягче, и это удобнее для случаев, когда какие-то параметры могут отсутствовать или быть некорректными. API не падает, а просто возвращает пустой результат, что проще обрабатывать на клиенте. 