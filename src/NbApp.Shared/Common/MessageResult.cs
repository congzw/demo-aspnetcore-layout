using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class MessageResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public virtual object Data { get; set; }

        public static MessageResult Create(bool success, string message, object data = null)
        {
            return new MessageResult() { Success = success, Message = message, Data = data };
        }
        public static MessageResult CreateSuccess(string message, object data = null)
        {
            return new MessageResult() { Success = true, Message = message, Data = data };
        }
        public static MessageResult CreateFail(string message, object data = null)
        {
            return new MessageResult() { Success = false, Message = message, Data = data };
        }
        public static MessageResult ValidateResult(bool success = false, string successMessage = "验证通过", string failMessage = "验证失败")
        {
            var vr = new MessageResult
            {
                Message = success ? successMessage : failMessage,
                Success = success
            };
            return vr;
        }
    }

    public class MessageResult<T> : MessageResult
    {
        public new T Data { get => (T)base.Data; set => base.Data = value; }

        public static MessageResult<T> Create(bool success, string message, T data = default)
        {
            return new MessageResult<T>() { Success = success, Message = message, Data = data };
        }
    }

    public static class MessageResultExtension
    {
        public static MessageResult ToSingleResult(this IEnumerable<MessageResult> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            var messageResults = results.ToList();

            var messageResult = new MessageResult();
            if (messageResults.Count == 0)
            {
                messageResult.Message = "空结果";
                return messageResult;
            }

            if (messageResults.Count == 1)
            {
                return messageResults[0];
            }

            messageResult.Success = messageResults.All(x => x.Success);
            messageResult.Data = messageResults.LastOrDefault()?.Data;
            messageResult.Message = "聚合消息：" + string.Join(',', messageResults.Select(x => x.Message));
            return messageResult;
        }

        public static MessageResult WithDataBags(this MessageResult result, out IDictionary<string, object> bags)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            result.Data = bags;
            return result;
        }

        public static MessageResult WithDataList(this MessageResult result, out List<object> list)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            list = new List<object>();
            result.Data = list;
            return result;
        }
    }
}
