﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Couchbase.Annotations;

namespace Nimator.Checks.Couchbase.Utlis
{
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the specified argument is not null.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argument">The argument.</param>
        [DebuggerStepThrough]
        [ContractAnnotation("halt <= argument:null")]
        public static void ArgumentNotNull(object argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
