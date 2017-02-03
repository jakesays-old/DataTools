//------------------------------------------------------------------------------
// <copyright file="EDesignUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// @owner       [....]
// @backupOwner [....]
//------------------------------------------------------------------------------


using System;
using System.Data;

namespace Std.Tools.Data.Metadata.Support
{

    internal static class EDesignUtil {

        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        //
        // Helper Functions
        //
        internal static string GetMessagesFromEntireExceptionChain(Exception e)
        {
            // get the full error message list from the inner exceptions
            var message = e.Message;
            var count = 0;
            for (var inner = e.InnerException; inner != null; inner = inner.InnerException)
            {
                count++;
                var indent = string.Empty.PadLeft(count, '\t');
                message += Environment.NewLine + indent;
                message += inner.Message;
            }
            return message;
        }

        internal static T CheckArgumentNull<T>(T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw ArgumentNull(parameterName);
            }
            return value;
        }

        internal static void CheckStringArgument(string value, string parameterName)
        {
            // Throw ArgumentNullException when string is null
            CheckArgumentNull(value, parameterName);

            // Throw ArgumentException when string is empty
            if (value.Length == 0)
            {
                throw InvalidStringArgument(parameterName);
            }
        }

		internal static ArgumentException InvalidStringArgument(string parameterName) {
            var e = new ArgumentException($"Invalid argument '{parameterName}'");
            return e;
        }

        internal static ArgumentNullException ArgumentNull(string parameter) {
            var e = new ArgumentNullException(parameter);
            return e;
        }
    }
}
