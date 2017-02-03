//---------------------------------------------------------------------
// <copyright file="EntityDesignPluralizationHandler.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       [....]
// @backupOwner [....]
//---------------------------------------------------------------------

namespace Std.Tools.Data.Metadata.Support
{
	internal class EntityDesignPluralizationHandler
	{
		/// <summary>
		/// Handler for pluralization service in Entity Design
		/// </summary>
		/// <param name="doPluralization">overall switch for the service, the service only start working when the value is true</param>
		/// <param name="userDictionaryPath"></param>
		/// <param name="errors"></param>
		internal EntityDesignPluralizationHandler(PluralizationService service)
		{
			Service = service;
		}

		/// <summary>
		/// user might set the service to null, so we have to check the null when using this property
		/// </summary>
		internal PluralizationService Service { get; set; }

		internal string GetEntityTypeName(string storeTableName)
		{
			return Service != null
				? Service.Singularize(storeTableName)
				: storeTableName;
		}

		internal string GetEntitySetName(string storeTableName)
		{
			return Service != null
				? Service.Pluralize(storeTableName)
				: storeTableName;
		}

		//    {
		//    if (this.Service != null)
		//{

		//internal string GetNavigationPropertyName(AssociationEndMember toEnd, string storeTableName)
		//        return toEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many ?
		//            this.Service.Pluralize(storeTableName) : this.Service.Singularize(storeTableName);
		//    }
		//    else
		//    {
		//        return storeTableName;
		//    }
		//}
	}
}