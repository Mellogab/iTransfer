using System.ComponentModel;

namespace iTransferencia.Core.Enums
{
    public enum TransferStatuses
    {
        [Description("PROCESSING")]
        PROCESSING = 1,

        [Description("FAILURE_TRANSFER")]
        FAILURE_TRANSFER = 2,

        [Description("FAILURE_UPDATE_SOURCE_ACCOUNT")]
        FAILURE_UPDATE_SOURCE_ACCOUNT = 3,

        [Description("FAILURE_UPDATE_DESTINATION_ACCOUNT")]
        FAILURE_UPDATE_DESTINATION_ACCOUNT = 4,

        [Description("FAILURE_NOTIFY_BANCEN")]
        FAILURE_NOTIFY_BANCEN = 5,

        [Description("SUCCESSFULLY_TRANSFER")]
        SUCCESSFULLY_TRANSFER = 6,

        [Description("SUCCESSFULLY_UPDATE_BALANCES")]
        SUCCESSFULLY_UPDATE_BALANCES = 7,

        [Description("FAILURE_UPDATE_BALANCES")]
        FAILURE_UPDATE_BALANCES = 8,

        [Description("UNKNOWN_FAILURE_ON_PROCESSING_FLOW")]
        UNKNOWN_FAILURE_ON_PROCESSING_FLOW = 9
    }
}