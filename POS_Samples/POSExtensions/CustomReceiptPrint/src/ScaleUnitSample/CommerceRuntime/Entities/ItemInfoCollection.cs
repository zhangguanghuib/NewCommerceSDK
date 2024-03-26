namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Collection of customer order item info.
    /// </summary>
    [Serializable]
    public class ItemInfoCollection : Collection<ItemInfo>
    {
    }
}
