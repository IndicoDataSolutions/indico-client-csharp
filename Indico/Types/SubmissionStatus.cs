using System;

namespace Indico.Types
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public enum SubmissionStatus
    {
        COMPLETE,

        FAILED,

        PENDING_REVIEW,

        PROCESSING,

        PENDING_ADMIN_REVIEW,

        PENDING_AUTO_REVIEW

    }
}
