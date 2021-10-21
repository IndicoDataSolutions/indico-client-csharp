using System;

namespace Indico.Types
{
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public enum JobStatus
    {
        /// <summary>
        /// Task state is unknown (assumed pending since you know the id).
        /// </summary>
        PENDING,
        /// <summary>
        /// Task was received by a worker (only used in events).
        /// </summary>
        RECEIVED,
        /// <summary>
        /// Task was started by a worker (:setting:task_track_started).
        /// </summary>
        STARTED,
        /// <summary>
        /// Task succeeded
        /// </summary>
        SUCCESS,
        /// <summary>
        /// Task failed
        /// </summary>
        FAILURE,
        /// <summary>
        /// Task was revoked.
        /// </summary>
        REVOKED,
        /// <summary>
        /// Task was rejected (only used in events).
        /// </summary>
        REJECTED,
        /// <summary>
        /// Task is waiting for retry.
        /// </summary>
        RETRY,
        /// <summary>
        /// Job Status IGNORED
        /// </summary>
        IGNORED
    }
}