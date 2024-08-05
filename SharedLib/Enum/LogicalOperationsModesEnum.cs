////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Логические операции И/ИЛИ
/// </summary>
public enum LogicalOperationsModesEnum
{
    /// <summary>
    /// ИЛИ
    /// </summary>
    [Description("ИЛИ")]
    Or,

    /// <summary>
    /// И
    /// </summary>
    [Description("И")]
    And
}