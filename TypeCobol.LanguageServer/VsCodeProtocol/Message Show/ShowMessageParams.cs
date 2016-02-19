﻿/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

namespace TypeCobol.LanguageServer.VsCodeProtocol
{
    /// <summary>
    /// The parameters of a notification message.
    /// </summary>
    class ShowMessageParams
    {
        /// <summary>
        /// The message type. See {@link MessageType}
        /// </summary>
        public MessageType type { get; set; }

        /// <summary>
        /// The actual message
        /// </summary>
        public string message { get; set; }
    }
}
