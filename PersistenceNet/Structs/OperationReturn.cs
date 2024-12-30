using Newtonsoft.Json;
using PersistenceNet.Enuns;

namespace PersistenceNet.Structs
{
    public class ReturnMessage
    {
        public string? Code { get; set; }
        public string? Text { get; set; }
        public bool IsSuccess { get { return (ReturnType == ReturnTypeEnum.Success); } }
        public ReturnTypeEnum ReturnType { get; set; } = ReturnTypeEnum.Success;
        public string ReturnTypeName { get { return ReturnType.ToString(); } }
        [JsonIgnore]
        public Exception? Exception { get; set; }
    }

    public class OperationReturn
    {
        public string? EntityName { get; set; }
        public string? Key { get; set; }
        public string? Field { get; set; }
        public bool IsSuccess { get { return (ReturnType == ReturnTypeEnum.Success); } }
        [JsonIgnore]
        public ReturnTypeEnum ReturnType { get; set; } = ReturnTypeEnum.Success;
        public string ReturnTypeName { get { return ReturnType.ToString(); } }
        public List<ReturnMessage> Messages { get; set; } = [];
        [JsonIgnore]
        public string FormatMessage
        {
            get
            {
                string _formatMessage = string.Empty;
                Messages.ForEach(m => _formatMessage += m.Text + ";");

                return !string.IsNullOrEmpty(_formatMessage) ? _formatMessage[..^2] : string.Empty;
            }
        }
    }
}