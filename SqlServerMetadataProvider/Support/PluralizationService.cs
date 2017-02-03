//---------------------------------------------------------------------
// <copyright file="PluralizationService.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       venkatja
// @backupOwner willa
//---------------------------------------------------------------------

using System;
using System.Globalization;

namespace Std.Tools.Data.Metadata.Support
{
    public abstract class PluralizationService
    {
        public CultureInfo Culture { get; protected set; }

        public abstract bool IsPlural(string word);
        public abstract bool IsSingular(string word);
        public abstract string Pluralize(string word);
        public abstract string Singularize(string word);

        /// <summary>
        /// Factory method for PluralizationService. Only support english pluralization.
        /// Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
        /// to extend the service to other locales.
        /// </summary>
        /// <param name="culture">CultureInfo</param>
        /// <returns>PluralizationService</returns>
        public static PluralizationService CreateService(CultureInfo culture)
        {
            EDesignUtil.CheckArgumentNull(culture, "culture");

            if (culture.TwoLetterISOLanguageName == "en")
            {
                return new EnglishPluralizationService();
            }

	        throw new NotImplementedException();
			//throw new NotImplementedException(Strings.UnsupportedLocaleForPluralizationServices(culture.DisplayName));
		}
	}
}
