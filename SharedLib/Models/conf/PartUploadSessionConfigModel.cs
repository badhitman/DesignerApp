﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PartUploadSessionConfigModel
/// </summary>
public class PartUploadSessionConfigModel
{
/// <summary>
    /// Срок жизни сессии (максимальный допустимый срок в секундах) для загрузки больших файлов (частями).
    /// </summary>
    /// <remarks>
    /// Внешняя система может инициировать загрузку файлов частями если файл слишком велик.
    /// Удалённый клиент должен инициировать сессию такой загрузки, а потом в границах данного таймаута успеть переслать все данные.
    /// Если удалённый клиент о каким-то причинам не успел отправить все части файла - тогда сессия уничтожается, а все данные в контексте этой сессии аннулируются.
    /// </remarks>
    public required uint PartUploadSessionTimeoutSeconds { get; set; }

    /// <summary>
    /// Размер порции данных (разрешённый) в байтах.
    /// </summary>
    /// <remarks>
    /// Размер порции данных (максимальный допустимый)
    /// </remarks>
    public required long PartUploadSize { get; set; }
}