using System;

namespace Indico.Types
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public enum ModelStatus
    {
        CREATING,

        TRAINING,

        FAILED,

        COMPLETE,

        NOT_ENOUGH_DATA
    }
}