# План обмена
- Контрагенты лица (компании/юрлица) создаются и редактируются на сайте, а внешняя ИС может [принимать или отклонять](#обновление-данных-по-контрагенту) новые компании или изменения в них.
- Заказы создаются на сайте, а внешняя ИС может редактировать их: менять статус, удалять/изменять строки документа, прикреплять файлы (счета, акты, накладные и т.д.)

## Авторизация
Для авторизации нужно каждый запрос снабжать заголовком 'token-access' с токеном доступа. Допустим есть токен E1B4C4F4-69AA-4A83-BB79-E08728739A33. Пример запроса может быть таким:
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

Для проверки статуса токена можно воспользоваться специальным GET запросом по адресу http://api.your-site.ru/api/info/my приложив к запросу свой токен в заголовке. Результат ожидается примерно следующий:
``` json
{
    "userName": "test",
    "roles": [
        "OrganizationsWriteCommerce",
        "PaymentsWriteCommerce",
        "OrdersWriteCommerce"
    ],
    "messages": []
}
```
Если токен действующий тогда по нему будет ясно какое имя назначено клиенту и какие роли ему назначены. Данный метод только и делает, что проверяет токен и выводит о нём информацию в случае если он валидный. Если токен не валидный ответом будет 404 ошибка.

## Контрагенты
Получение перечня контрагентов. В теле запроса можно указать актуальность изменения данных (поле: `afterDateUpdate`). Если указать эту дату, то в ответ будет сформирован перечень организаций, которые были изменены после этой даты. Если параметр `includeExternalData` равен `true`, тогда для каждой организации будет загружена информация по её адресам/филиалам и пользователей ИС связанных с организацией (те, кто может формировать заказы для неё).

Параметр `forUserIdentityId` позволяет получить компании, которые ассоциированы с указанным пользователем.

Пагинация: `pageNum` (отсчёт страниц начинается с нуля), `pageSize` (минимум: 10), ***sortingDirection*** (направление сортировки по полю даты последнего обновления юрлица) и ***sortBy*** (игнорируется, не используется).

``` shell
PUT /api/organizations/select HTTP/1.1
Host: api.your-site.ru
token-access: E1B4C4F4-69AA-4A83-BB79-E08728739A33
Content-Type: application/json
{
	"payload": {
		"forUserIdentityId": "",
		"afterDateUpdate": "2023-10-30T12:10:57.822Z",
		"includeExternalData": false
	},
	"pageNum": 0,
	"pageSize": 10,
	"sortingDirection": "Up",
	"sortBy": ""
}
```

В ответе нужно/можно проверить наличие новых контрагентов или запросов на изменение реквизитов. Контрагент имеет следующую структуру:
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

### Обновление данных по Контрагенту
Для юридически значимых полей (**Name**, **LegalAddress**, **INN**, **KPP**, **OGRN**, **CurrentAccount**, **CorrespondentAccount**, **BankName**, **BankBIC**) есть дополнительное/служебное поле с префиксом `new`. Например: для поля `inn` предназначено `newINN` (по остальным так же). При создании нового контрагента оба поля будут заполнены ***одним и тем же значением***. Если клиент попытается изменить реквизиты существующей компании, тогда, например, в `inn` будет текущее/рабочее значение, а в поле `newINN` будет новое значение, которое клиент хочет установить. Т.е. клиент не может просто изменить контролируемые реквизиты - он может только запросить это изменение (что сохраняется в служебном/вспомогательном поле), а бухгалтер должен их принять или отклонить в последствии. Загрузка заказов по таким контрагентам (у которых есть не закрытый запрос изменений) может привести к выписке не корректных документов в последствии. По каждому такому объекту нужно принять решение и сбросить состояние в стандартное (когда в полях с префиксом `new` станет пусто). Для сброса состояния юрлица нужно воспользоваться следующим методом:
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
Этот метод установит новые данные для организации и удалит данные запроса на изменение/создание. Этим же методом можно воспользоваться для изменения реквизитов по инициативе внешней системы без запроса от клиента.

## Заказы
При получении заказов можно воспользоваться полем `afterDateUpdate` что бы получить только заказы изменённые после требуемой даты.
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

Структура ответа такова:
``` json
{
	"statusDocument": 0,
	"authorIdentityUserId": "5ced9296-2e77-4f01-b32e-9e69bd18d186",
	"externalDocumentId": null,
	"information": null,
	"helpdeskId": 1,
	"organization": {
		"lastAtUpdatedUTC": "2024-10-02T20:03:33.657184Z",
		"createdAtUTC": "2024-09-30T04:57:51.6344Z",
		"newName": null,
		"newLegalAddress": "zxczxc",
		"newINN": null,
		"newKPP": null,
		"newOGRN": null,
		"newCurrentAccount": null,
		"newCorrespondentAccount": null,
		"newBankName": null,
		"newBankBIC": null,
		"hasRequestToChange": true,
		"users": null,
		"addresses": [
			{
				"organization": null,
				"name": "Станислав Успенский",
				"parentId": 48,
				"address": "zxczxc",
				"contacts": "zxczxc",
				"organizationId": 1,
				"id": 2
			}
		],
		"phone": "sadasds",
		"email": "dasdsad",
		"legalAddress": "zxczxc",
		"inn": "cxzczx",
		"kpp": "czxczxc",
		"ogrn": "zxczxc",
		"currentAccount": "zxczxc",
		"correspondentAccount": "xzczxc",
		"bankName": "zxczxczxc",
		"bankBIC": "xzczxc",
		"description": null,
		"name": "cxzcxz",
		"id": 1,
		"isDisabled": false
	},
	"organizationId": 1,
	"addressesTabs": [
		{
			"id": 1,
			"orderDocument": null,
			"orderDocumentId": 1,
			"addressOrganization": {
				"organization": {
					"lastAtUpdatedUTC": "2024-10-02T20:03:33.657184Z",
					"createdAtUTC": "2024-09-30T04:57:51.6344Z",
					"newName": null,
					"newLegalAddress": "zxczxc",
					"newINN": null,
					"newKPP": null,
					"newOGRN": null,
					"newCurrentAccount": null,
					"newCorrespondentAccount": null,
					"newBankName": null,
					"newBankBIC": null,
					"hasRequestToChange": true,
					"users": null,
					"addresses": [],
					"phone": "sadasds",
					"email": "dasdsad",
					"legalAddress": "zxczxc",
					"inn": "cxzczx",
					"kpp": "czxczxc",
					"ogrn": "zxczxc",
					"currentAccount": "zxczxc",
					"correspondentAccount": "xzczxc",
					"bankName": "zxczxczxc",
					"bankBIC": "xzczxc",
					"description": null,
					"name": "cxzcxz",
					"id": 1,
					"isDisabled": false
				},
				"name": "Станислав Успенский",
				"parentId": 48,
				"address": "zxczxc",
				"contacts": "zxczxc",
				"organizationId": 1,
				"id": 2
			},
			"addressOrganizationId": 2,
			"rows": [
				{
					"id": 1,
					"addressForOrderTab": null,
					"addressForOrderTabId": 1,
					"orderDocument": null,
					"orderDocumentId": 1,
					"offer": {
						"goods": {
							"baseUnit": "Thing",
							"offers": [],
							"lastAtUpdatedUTC": "2024-10-24T14:25:13.625921Z",
							"createdAtUTC": "2024-10-01T18:47:53.59351Z",
							"description": "<p><img src=\"../../cloud-fs/read/40/2024-10-18_001.jpg\" alt=\"\" width=\"2552\" height=\"3508\"></p>",
							"name": "cxzczxc",
							"id": 1,
							"isDisabled": false
						},
						"goodsId": 1,
						"offerUnit": "Stack",
						"multiplicity": 2,
						"price": 800,
						"pricesRules": null,
						"lastAtUpdatedUTC": "2024-10-01T18:48:05.419474Z",
						"createdAtUTC": "2024-10-01T18:48:05.419476Z",
						"description": null,
						"name": " cvxcvxc",
						"id": 1,
						"isDisabled": false
					},
					"offerId": 1,
					"goods": {
						"baseUnit": "Thing",
						"offers": [
							{
								"goods": null,
								"goodsId": 1,
								"offerUnit": "Stack",
								"multiplicity": 2,
								"price": 800,
								"pricesRules": null,
								"lastAtUpdatedUTC": "2024-10-01T18:48:05.419474Z",
								"createdAtUTC": "2024-10-01T18:48:05.419476Z",
								"description": null,
								"name": " cvxcvxc",
								"id": 1,
								"isDisabled": false
							}
						],
						"lastAtUpdatedUTC": "2024-10-24T14:25:13.625921Z",
						"createdAtUTC": "2024-10-01T18:47:53.59351Z",
						"description": "<p><img src=\"../../cloud-fs/read/40/2024-10-18_001.jpg\" alt=\"\" width=\"2552\" height=\"3508\"></p>",
						"name": "cxzczxc",
						"id": 1,
						"isDisabled": false
					},
					"goodsId": 1,
					"quantity": 1
				}
			]
		}
	],
	"lastAtUpdatedUTC": "0001-01-01T00:00:00",
	"createdAtUTC": "2024-10-01T04:02:21.036045Z",
	"description": null,
	"name": "Новый заказ",
	"id": 1,
	"isDisabled": false
}
```
У документа есть поле `addressesTabs`, которое представляет перечень адресов доставки по данному заказу. У каждого адреса доставки есть поле `rows` в котором хранятся строки/данные по номенклатуре, количеству и сумме заказа для конкретной строки документа. В каждой строке заказа есть оффер, который в свою очередь имеет ссылку на номенклатуру `goods`.

> Для возможности заказа одной и той же номенклатуры, но в разных фасовках (или по любому другому принципу) введено понятие оффера. У номенклатуры в ИС вовсе нет цены, а лишь базовая единица измерения, но у номенклатуры может быть сколь угодно офферов. У каждого оффера есть своя цена, своя единица измерения и кратность базовой единице номенклатуры. Таким образом для получения номенклатуры заказа нужно обратиться через оффер (к его номенклатуре в поле `goods`).

### Смена статуса заказу
``` shell
curl -X 'POST' \
  'http://api.your-site.ru/api/order/5/stage-update/Created' \
  -H 'accept: text/plain' \
  -d ''
```

### Отправка файлов в заказ (вложение)
К заказу можно прикрепить файл (любой). Имена файлов должны быть уникальными и внятными, что бы потом в перечне файлов клиент мог легко ориентироваться.
``` shell
curl -X 'POST' \
  'http://api.your-site.ru/api/order/5/attachment-add' \
  -H 'accept: text/plain' \
  -H 'Content-Type: multipart/form-data' \
  -F 'uploadedFile=@2024-10-18_001.jpg;type=image/jpeg'
```

### Добавить платёжный документ
``` shell
curl -X 'POST' \
  'http://api.your-site.ru/api/payments/update' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json-patch+json' \
  -d '{
  "name": "string",
  "amount": 5,
  "externalDocumentId": "СЧ-405",
  "orderDocumentId": 5,
  "id": 0
}'
```
Если `id` 0, тогда создание нового платёжного документа, в противном случае будет обновление. Для существующего документа-платёжки вместо внутреннего идентификатора можно указать внешний `externalDocumentId` (если он был указан при создании или предыдущем обновлении).