using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfs.Compiler
{
	internal class IdNode
	{
		public IdNode(string id)
		{
			Id = id;
			Children = new HashSet<IdNode>(new IdNodeComparer());
		}

		public string Id { get; }
		public HashSet<IdNode> Children { get; }

		private class IdNodeComparer : IEqualityComparer<IdNode>
		{
			public bool Equals(IdNode x, IdNode y)
			{
				return x.Id == y.Id;
			}

			public int GetHashCode(IdNode obj)
			{
				return obj.Id.GetHashCode();
			}
		}

		public static IdNode FromIds(HashSet<string> ids)
		{
			var root = new IdNode(null);
			foreach (var id in ids)
			{
				FromIdParts(root, id.Split('.'));
			}
			return root;
		}

		private static void FromIdParts(IdNode parent, string[] idParts)
		{
			while (true)
			{
				if (idParts.Any())
				{
					var node = parent.Children.FirstOrDefault(n => n.Id == idParts[0]);
					if (node == null)
					{
						node = new IdNode(idParts[0]);
						parent.Children.Add(node);
					}
					parent = node;
					idParts = idParts.Skip(1).ToArray();
					continue;
				}
				break;
			}
		}
	}
}
