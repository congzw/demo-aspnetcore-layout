using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class TreeNode<T>
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public T Data { get; set; }
    }

    public class TreeVo<T> : TreeNode<T>
    {
        public List<TreeVo<T>> Children { get; set; } = new List<TreeVo<T>>();
    }

    public class TreeVoHelper
    {
        public static TreeVoHelper Instance = new TreeVoHelper();

        internal IEnumerable<TreeNode<T>> ToTreeNodes<T>(IEnumerable<T> items, Func<T, string> getId, Func<T, string> getParentId)
        {
            return items.Select(x => new TreeNode<T>() { Id = getId(x), ParentId = getParentId(x), Data = x });
        }

        internal List<TreeVo<T>> CreateChildren<T>(IEnumerable<TreeNode<T>> items, string parentId)
        {
            var vos = new List<TreeVo<T>>();

            var theChildItems = FindChildren(items, parentId);
            if (theChildItems == null || theChildItems.Count() == 0)
            {
                return vos;
            }

            foreach (var theChildItem in theChildItems)
            {
                var childVo = new TreeVo<T>()
                {
                    Id = theChildItem.Id,
                    ParentId = theChildItem.ParentId,
                    Data = theChildItem.Data
                };
                vos.Add(childVo);

                var cc = CreateChildren(items, theChildItem.Id);
                if (cc.Count > 0)
                {
                    childVo.Children = cc;
                }
            }

            return vos;
        }

        internal IEnumerable<TreeNode<T>> FindChildren<T>(IEnumerable<TreeNode<T>> items, string parentId)
        {
            var query = items;
            if (items == null)
            {
                return null;
            }

            var theParentId = string.IsNullOrWhiteSpace(parentId) ? string.Empty : parentId;
            if (string.IsNullOrWhiteSpace(theParentId))
            {
                query = query.Where(x => string.IsNullOrWhiteSpace(x.ParentId));
            }
            else
            {
                query = query.Where(x => theParentId.Equals(x.ParentId, StringComparison.OrdinalIgnoreCase));
            }
            return query;
        }

        internal TreeNode<T> FindFirst<T>(IEnumerable<TreeNode<T>> items, string id)
        {
            var query = items;
            if (items == null)
            {
                return null;
            }
            var theId = string.IsNullOrWhiteSpace(id) ? string.Empty : id;
            if (string.IsNullOrWhiteSpace(theId))
            {
                return query.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Id));
            }
            return query.FirstOrDefault(x => theId.Equals(x.Id, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class TreeVoExtensions
    {
        public static IEnumerable<TreeNode<T>> ToTreeNodes<T>(this IEnumerable<T> items, Func<T, string> getId, Func<T, string> getParentId) 
            => TreeVoHelper.Instance.ToTreeNodes(items, getId, getParentId);

        public static List<TreeVo<T>> CreateTreeVos<T>(this IEnumerable<TreeNode<T>> nodes, string parentId) 
            => TreeVoHelper.Instance.CreateChildren(nodes, parentId);
    }
}