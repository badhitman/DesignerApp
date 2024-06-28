## Конструктор документов (*стадия активной разработки*).
> как развитие базового кейса '*Blazor NET.8 + TelegramBot*' из ветки [main](https://github.com/badhitman/DesignerApp/tree/main). Другими словами там уже есть встроеная поддержка TelegramBot

**Constructor** - гибкое web решение для конструирования документов, справочников и т.п.. А так же генерация C# .net кода для созданной структуры сущностей со всеми её связями (целиком). Решение позволит с нуля сконструировать новое приложение со своей структурой БД и CRUD+ набором сервисов (+ web/ui of `BlazorWebApp`). Такой сгенерированный код легко ляжет на [базовую заготовку](https://github.com/badhitman/DesignerApp/tree/main).

... добавлено [разного UI на Blazor](https://github.com/badhitman/DesignerApp/tree/constructor/BlazorServerLib/Components/Forms). А так же [немного](https://github.com/badhitman/DesignerApp/blob/constructor/DBContextLibs/DbLayerLib/ConstructorLayerContext.cs) [БД](https://github.com/badhitman/DesignerApp/tree/constructor/SharedLib/Models/db/forms) для [соответствующего сервиса](https://github.com/badhitman/DesignerApp/blob/constructor/SharedLib/IServices/main/IFormsService.cs).

### Проекты
![Проекты](./img/constructor/projects-list-page.png)

### Справочники (перечисления)
![справочники-перечисления](./img/constructor/directories-list-page.png)
в ограниченном режиме: *если владелец деактивировал проект*
![справочники-перечисления](./img/constructor/directories-off-list-page.png)

### Формы
![диалог работы с формой](./img/constructor/form-edit-dialog.png)

Доступные типы полей формы
![доступные типы полей](./img/constructor/fields-types-select.png)

Перечень полей формы
![Перечень полей формы](./img/constructor/fields-from-form-active.png)
в ограниченном режиме (если проект деактивирован владельцем)
![Перечень полей формы](./img/constructor/fields-from-form-off.png)

Редактирование поля формы
![Редактирование поля формы](./img/constructor/field-edit-dialog-active.png)
в случае выключено проекта форма доступна только для просмотра
![Редактирование поля формы ограничение](./img/constructor/field-edit-dialog-off.png)

### Ссылки/сессии
по ссылке пользователю доступна форма для заполнения
![ссылка для пользователя](./img/constructor/user-link-session.png)
в данном случае документ содержит одну вкладку, но их может быть сколько угодно и на каждой вкладке любое количество любых форм
