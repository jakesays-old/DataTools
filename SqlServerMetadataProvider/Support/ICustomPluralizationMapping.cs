//---------------------------------------------------------------------
// <copyright file="ICustomPluralizationMapping.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       leil
// @backupOwner jeffreed
//---------------------------------------------------------------------

namespace Std.Tools.Data.Metadata.Support
{
    public interface ICustomPluralizationMapping
    {
        void AddWord(string singular, string plural);
    }
}
