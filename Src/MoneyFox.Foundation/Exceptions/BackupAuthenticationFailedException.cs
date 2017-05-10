﻿using System;

namespace MoneyFox.Foundation.Exceptions
{
    /// <summary>
    ///     This Exception is thrown when the user couldn't authenticate on the backup service.
    /// </summary>
    public class BackupAuthenticationFailedException : Exception
    {
        /// <summary>
        ///     Creates an Backup Exception
        /// </summary>
        public BackupAuthenticationFailedException()
        {
        }

        /// <summary>
        ///     Creates an Backup Exception
        /// </summary>
        /// <param name="message">Exception message to show to the user.</param>
        public BackupAuthenticationFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates an Backup Exception
        /// </summary>
        /// <param name="message">Exception message to show to the user.</param>
        /// <param name="exception">Inner Exception of the backup exception.</param>
        public BackupAuthenticationFailedException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}