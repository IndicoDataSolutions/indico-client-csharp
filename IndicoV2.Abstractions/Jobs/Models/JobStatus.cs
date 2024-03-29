﻿namespace IndicoV2.Jobs.Models
{
    public enum JobStatus
    {
        /// <summary>
        /// Task failed
        /// </summary>
        FAILURE,
        /// <summary>
        /// Job Status IGNORED
        /// </summary>
        IGNORED,
        /// <summary>
        /// Task state is unknown (assumed pending since you know the id).
        /// </summary>
        PENDING,
        /// <summary>
        /// Task was received by a worker (only used in events).
        /// </summary>
        RECEIVED,
        /// <summary>
        /// Task was rejected (only used in events).
        /// </summary>
        REJECTED,
        /// <summary>
        /// Task is waiting for retry.
        /// </summary>
        RETRY,
        /// <summary>
        /// Task was revoked.
        /// </summary>
        REVOKED,
        /// <summary>
        /// Task was started by a worker (:setting:task_track_started).
        /// </summary>
        STARTED,
        /// <summary>
        /// Task succeeded
        /// </summary>
        SUCCESS,
        /// <summary>
        /// Job Status TRAILED
        /// </summary>
        TRAILED
    }
}
