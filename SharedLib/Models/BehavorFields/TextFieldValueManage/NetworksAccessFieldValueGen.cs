using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// Сетевые доступы для опросника
/// </summary>
public partial class NetworksAccessFieldValueGen : FieldValueGeneratorAbstraction
{
    /// <inheritdoc/>
    public override string Name => "Сетевые доступы";

    /// <inheritdoc/>
    public override string? About => "Получить перечень сетевых доступов к ресурсам ВМ/OCP. В параметр передаётся объект JSON. Пример такого параметра-запроса предоставлен: замените в нём значения полей/свойств на требуемые. Потребуются имена вкладок OCP/ВМ и некоорые поля из них (тоже имена).";

    /// <summary>
    /// Сетевые доступы для опросника
    /// </summary>
    public NetworksAccessFieldValueGen()
    {
        RequestModel = new NetvorksAccessRequestModel()
        {
            PageNameOCP = "Имя страницы OCP",
            FieldRuntimeNameOCP = "Имя поля 'Среда'(ocp)",
            PageNameVM = "Имя страницы ВМ",
            FieldRuntimeNameVM = "Имя поля 'Среда'(vm)",
            FieldRoleNameVM = "Имя поля 'Роль'(vm)",
            FieldTechNameVM = "Имя поля 'Тех.имя хоста'(vm)"
        };
    }

    static Dictionary<string, NetvorksAccessRequestModel?> _cache_ser = new();

    /// <inheritdoc/>
    public override SimpleStringArrayResponseModel GetListElements(ConstructorFieldFormModelDB field, ConstructorFormSessionModelDB session_Questionnaire, ConstructorFormQuestionnairePageJoinFormModelDB? page_join_form = null, uint row_num = 0)
    {
        string? ds = field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();
        string? fp = field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)?.ToString();

        if (string.IsNullOrWhiteSpace(ds) || string.IsNullOrWhiteSpace(fp))
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Дескриптор или параметр не установлен: `{field.MetadataValueType}`. ошибка {{B4E1560C-7E72-4DE2-A476-D791E9316C66}}" }] };

        NetvorksAccessRequestModel? ar = null;

        lock (_cache_ser)
        {
            if (_cache_ser.TryGetValue(fp, out NetvorksAccessRequestModel? value))
                ar = value;
            else
            {
                ar = JsonConvert.DeserializeObject<NetvorksAccessRequestModel>(fp);
                _cache_ser[fp] = ar;
            }
        }

        if (ar is null || session_Questionnaire.Owner?.Pages is null || page_join_form is null || session_Questionnaire.SessionValues is null)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"Опции не удалось десериализовать: `{fp}`. ошибка {{A5376051-47D4-4902-B336-F80D0754BC5D}}" }] };

        Dictionary<string, Dictionary<uint, List<ConstructorFormSessionValueModelDB>>> pages_data = ConstructorFormQuestionnairePageModelDB.GetRowsData(session_Questionnaire);
        if (!pages_data.ContainsKey(ar.PageNameVM) || !pages_data.ContainsKey(ar.PageNameOCP))
            return new() { Elements = Enumerable.Empty<string>(), Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Info, Text = $"В данных не хватает необходимых страниц для формирования селектора-генератора" }] };

        List<string> elements_gen = [];
        KeyValuePair<uint, List<ConstructorFormSessionValueModelDB>>[] _page_data_ocp = [.. pages_data[ar.PageNameOCP]];
        ConstructorFormSessionValueModelDB? rv;
        foreach (KeyValuePair<uint, List<ConstructorFormSessionValueModelDB>> _kvp_row in _page_data_ocp)
        {
            rv = _kvp_row.Value.FirstOrDefault(x => x.Name.Equals(ar.FieldRuntimeNameOCP, StringComparison.OrdinalIgnoreCase));
            if (rv is null)
                return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"В строке данных [{_kvp_row.Key}] не хватает необходимой колонки `{ar.FieldRuntimeNameOCP}`. ошибка {{8F85CAA6-3457-4F2E-A4C3-B6F9FC2A8F56}}" }] };

            if (!string.IsNullOrWhiteSpace(rv.Value))
            {
                string ocp_src = $"OCP {rv.Value}";
                if (!elements_gen.Any(x => x.Equals(ocp_src)))
                    elements_gen.Add(ocp_src);
            }
        }

        Dictionary<uint, List<ConstructorFormSessionValueModelDB>> _page_data_vm = pages_data[ar.PageNameVM];
        foreach (KeyValuePair<uint, List<ConstructorFormSessionValueModelDB>> _kvp_row in _page_data_vm)
        {
            ConstructorFormSessionValueModelDB?
                rv_runtime_runtime = _kvp_row.Value.FirstOrDefault(x => x.Name.Equals(ar.FieldRuntimeNameVM)),
                rv_runtime_role = _kvp_row.Value.FirstOrDefault(x => x.Name.Equals(ar.FieldRoleNameVM)),
                rv_runtime_tech = _kvp_row.Value.FirstOrDefault(x => x.Name.Equals(ar.FieldTechNameVM));

            if (rv_runtime_runtime is null || rv_runtime_role is null || rv_runtime_tech is null)
                return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = $"В строке данных [{_kvp_row.Key}] не хватает необходимой колонки (одна из: `{ar.FieldRuntimeNameVM}`,`{ar.FieldRoleNameVM}`,`{ar.FieldTechNameVM}`). ошибка {{F13628DB-6D07-4871-AFA6-0D4806B47425}}" }] };

            if (!string.IsNullOrWhiteSpace(rv_runtime_runtime.Value) && !string.IsNullOrWhiteSpace(rv_runtime_role.Value) && !string.IsNullOrWhiteSpace(rv_runtime_tech.Value))
                elements_gen.Add($"{rv_runtime_role.Value} ({rv_runtime_runtime.Value}) {rv_runtime_tech.Value}");
        }

        return new() { Elements = elements_gen.Distinct().ToArray() };
    }
}