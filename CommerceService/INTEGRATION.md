# Интеграция через API

## Авторизация
Для авторизации запроса нужно каждый запрос снабжать заголовком 'token-access' с вашим токеном. Сейчас есть тестовый токен E1B4C4F4-69AA-4A83-BB79-E08728739A33. Пример запроса:
``` shell
PUT /api/orders/select HTTP/1.1
Host: api.your-site.ru
token-access: E1B4C4F4-69AA-4A83-BB79-E08728739A33
Content-Type: application/json
{
	"payload": {
		"statusesFilter": [],
		"afterDateUpdate": "2023-10-29T05:58:50.446Z",
		"addressForOrganizationFilter": 0,
		"organizationFilter": 0,
		"goodsFilter": 0,
		"offerFilter": 0,
		"includeExternalData": true
	}
}
```

## Контрагенты

Получение перечня контрагентов. В теле запроса можно указать актуальность изменения данных (поле: `afterDateUpdate`). Если указать эту дату, то в ответ будет сформирован перечень организаций, которые были изменены после этой даты.
``` shell
PUT /api/organizations/select HTTP/1.1
Host: api.your-site.ru
token-access: E1B4C4F4-69AA-4A83-BB79-E08728739A33
Content-Type: application/json
{
	"payload": {
		"forUserIdentityId": "",
		"afterDateUpdate": "2023-10-30T12:10:57.822Z",
		"includeExternalData": true
	},
	"pageSize": 0,
	"pageNum": 0,
	"sortingDirection": "Up",
	"sortBy": ""
}
```

В ответе нужно можно проверить наличие новых контрагентов или запросов на изменение реквизитов. Контрагент имеет следующую структуру:
``` json
{
	"newName": "string",
	"newLegalAddress": "string",
	"newINN": "string",
	"newKPP": "string",
	"newOGRN": "string",
	"newCurrentAccount": "string",
	"newCorrespondentAccount": "string",
	"newBankName": "string",
	"newBankBIC": "string",
	"hasRequestToChange": true,
	"users": [
		{
			"organization": "string",
			"organizationId": 0,
			"id": 0,
			"userPersonIdentityId": "string",
			"lastAtUpdatedUTC": "2024-10-30T12:11:35.495Z"
		}
	],
	"addresses": [
		{
			"organization": "string",
			"name": "string",
			"parentId": 0,
			"address": "string",
			"contacts": "string",
			"organizationId": 0,
			"id": 0
		}
	],
	"phone": "string",
	"email": "string",
	"legalAddress": "string",
	"inn": "string",
	"kpp": "string",
	"ogrn": "string",
	"currentAccount": "string",
	"correspondentAccount": "string",
	"bankName": "string",
	"bankBIC": "string",
	"lastAtUpdatedUTC": "2024-10-30T12:11:35.495Z",
	"createdAtUTC": "2024-10-30T12:11:35.495Z",
	"description": "string",
	"name": "string",
	"id": 0,
	"isDisabled": true
}
```
Для юридически значимых полей есть дополнительное/резервное поле с префиксом `new`. Например есть поле `inn` и `newINN`. При создании нового контрагента оба поля будут заполнены одним и тем же значением. Если клиент запросил изменения в реквизитах существующей компании, тогда в `inn` будет текущее/рабочее значение, а в поле `newINN` будет новое значение, которое клиент хочет установить. Загрузка заказов по таким контрагентам может привести к выписке не корректных документов в последствии. По каждому такому объекту нужно принять то или иное решение и сбросить состояние в стандартное (когда в полях с префиксом `new` будет пусто). Для сброса состояния юрлица нужно воспользоваться следующим методом:
```
curl -X 'POST' \
  'http://api.your-site.ru/api/organizations/legal-update' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json-patch+json' \
  -d '{
  "phone": "string",
  "email": "string",
  "legalAddress": "string",
  "inn": "string",
  "kpp": "string",
  "ogrn": "string",
  "currentAccount": "string",
  "correspondentAccount": "string",
  "bankName": "string",
  "bankBIC": "string",
  "description": "string",
  "name": "string",
  "id": 0,
  "isDisabled": true
}'
```
Этот метод установит новые данные для организации и удалит данные запроса на изменение/создание.

## Заказы
При получении заказов можно воспользоваться полем `afterDateUpdate` что бы получить только заказы изменённые после требуемой даты. Под изменением заказа имеется ввиду напримре смена статуса.
``` shell
PUT /api/orders/select HTTP/1.1
Host: api.your-site.ru
token-access: E1B4C4F4-69AA-4A83-BB79-E08728739A33
Content-Type: application/json
{
	"payload": {
		"statusesFilter": [],
		"afterDateUpdate": "2023-10-29T05:58:50.446Z",
		"addressForOrganizationFilter": 0,
		"organizationFilter": 0,
		"goodsFilter": 0,
		"offerFilter": 0,
		"includeExternalData": true
	}
}
```

### Смена статуса заказу
``` shell
curl -X 'POST' \
  'http://api.your-site.ru/api/order/5/stage-update/Created' \
  -H 'accept: text/plain' \
  -d ''
```

## Отправка файлов в заказ (вложение)
К заказу можно прикрепить файл (любой). Имена файлов должны быть уникальными и внятными, что бы потом в перечне файлов клиент мог легко ориентироваться.
``` shell
curl -X 'POST' \
  'http://api.your-site.ru/api/order/5/attachment-add' \
  -H 'accept: text/plain' \
  -H 'Content-Type: multipart/form-data' \
  -F 'uploadedFile=@2024-10-18_001.jpg;type=image/jpeg'
```

