using System;

namespace Indico.Types
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public enum PurgeBlobStatus
    {
        /// <summary>
        /// Successfully removed blob
        /// </summary>
        SUCCESS,
        /// <summary>
        /// Purge failed
        /// </summary>
        FAILED
    }

}