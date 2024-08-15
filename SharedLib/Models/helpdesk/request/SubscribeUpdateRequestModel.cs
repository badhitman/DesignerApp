﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
///  Subscribe Update Request
/// </summary>
public class SubscribeUpdateRequestModel: UserUpdateRequestModel
{
    /// <summary>
    /// отключение отправки уведомлений
    /// </summary>
    public bool IsSilent { get; set; }

    /// <summary>
    /// Value for set
    /// </summary>
    public required bool SetValue { get; set; }
}