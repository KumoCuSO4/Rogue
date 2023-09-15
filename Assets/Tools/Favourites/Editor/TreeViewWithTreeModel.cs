﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace FavouritesEd
{

	public class TreeViewItem<T> : TreeViewItem where T : TreeElement
	{
		public T Data { get; set; }

		public TreeViewItem(int id, int depth, string displayName, Texture2D icon, T data)
			: base(id, depth, displayName)
		{
			this.icon = icon;
			Data = data;
		}
	}

	public class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
	{
		private TreeModel<T> m_TreeModel;
		private readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);
		public event Action TreeChanged;

		public TreeModel<T> TreeModel { get { return m_TreeModel; } }
		public event Action<IList<TreeViewItem>> BeforeDroppingDraggedItems;

		public TreeViewWithTreeModel(TreeViewState state) 
			: base(state)
		{ }

		public TreeViewWithTreeModel(TreeViewState state, TreeModel<T> model) 
			: base(state)
		{
			Init(model);
		}

		public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
			: base(state, multiColumnHeader)
		{
			Init(model);
		}

		protected void Init(TreeModel<T> model)
		{
			m_TreeModel = model;
			m_TreeModel.ModelChanged += ModelChanged;
		}

		private void ModelChanged()
		{
			var handler = TreeChanged;
			if (handler != null) handler();
			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			int depthForHiddenRoot = -1;
			return new TreeViewItem<T>(m_TreeModel.Root.ID, depthForHiddenRoot, m_TreeModel.Root.Name, m_TreeModel.Root.Icon, m_TreeModel.Root);
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			if (m_TreeModel.Root == null)
			{
				Debug.LogError("tree model root is null. did you call SetData()?");
			}

			m_Rows.Clear();
			if (!string.IsNullOrEmpty(searchString))
			{
				Search(m_TreeModel.Root, searchString, m_Rows);
			}
			else
			{
				if (m_TreeModel.Root.HasChildren)
					AddChildrenRecursive(m_TreeModel.Root, 0, m_Rows);
			}

			// We still need to setup the child parent information for the rows since this 
			// information is used by the TreeView internal logic (navigation, dragging etc.)
			SetupParentsAndChildrenFromDepths(root, m_Rows);

			return m_Rows;
		}

		private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
		{
			foreach (T child in parent.Children)
			{
				var item = new TreeViewItem<T>(child.ID, depth, child.Name, child.Icon, child);
				newRows.Add(item);

				if (child.HasChildren)
				{
					if (IsExpanded(child.ID))
					{
						AddChildrenRecursive(child, depth + 1, newRows);
					}
					else
					{
						item.children = CreateChildListForCollapsedParent();
					}
				}
			}
		}

		private void Search(T searchFromThis, string search, List<TreeViewItem> result)
		{
			if (string.IsNullOrEmpty(search)) throw new ArgumentException("Invalid search: cannot be null or empty", "search");

			const int kItemDepth = 0; // tree is flattened when searching

			Stack<T> stack = new Stack<T>();
			foreach (var element in searchFromThis.Children)
			{
				stack.Push((T)element);
			}

			while (stack.Count > 0)
			{
				T current = stack.Pop();

				// Matches search?
				if (current.SearchHelper.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					result.Add(new TreeViewItem<T>(current.ID, kItemDepth, current.Name, current.Icon, current));
				}

				if (current.Children != null && current.Children.Count > 0)
				{
					foreach (var element in current.Children)
					{
						stack.Push((T)element);
					}
				}
			}

			SortSearchResult(result);
		}

		protected virtual void SortSearchResult(List<TreeViewItem> rows)
		{
			rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName)); // sort by displayName by default, can be overridden for multicolumn solutions
		}

		protected override IList<int> GetAncestors(int id)
		{
			return m_TreeModel.GetAncestors(id);
		}

		protected override IList<int> GetDescendantsThatHaveChildren(int id)
		{
			return m_TreeModel.GetDescendantsThatHaveChildren(id);
		}

		// Dragging -----------

		private const string k_GenericDragID = "GenericDragColumnDragging";

		protected override bool CanStartDrag(CanStartDragArgs args)
		{
			return true;
		}

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			if (hasSearch) return;
			DragAndDrop.PrepareStartDrag();
			var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
			DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
			DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
			string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
			DragAndDrop.StartDrag(title);
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
		{
			// Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
			var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
			if (draggedRows == null)
				return DragAndDropVisualMode.None;

			// Parent item is null when dragging outside any tree view items.
			switch (args.dragAndDropPosition)
			{
				case DragAndDropPosition.UponItem:
				case DragAndDropPosition.BetweenItems:
					{
						bool validDrag = ValidDrag(args.parentItem, draggedRows);
						if (args.performDrop && validDrag)
						{
							T parentData = ((TreeViewItem<T>)args.parentItem).Data;
							OnDropDraggedElementsAtIndex(draggedRows, parentData, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
						}
						return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
					}

				case DragAndDropPosition.OutsideItems:
					{
						if (args.performDrop)
							OnDropDraggedElementsAtIndex(draggedRows, m_TreeModel.Root, m_TreeModel.Root.Children.Count);

						return DragAndDropVisualMode.Move;
					}
				default:
					Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
					return DragAndDropVisualMode.None;
			}
		}

		public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex)
		{
			var handler = BeforeDroppingDraggedItems;
			if (handler != null) handler(draggedRows);

			var draggedElements = new List<TreeElement>();
			foreach (var x in draggedRows)
				draggedElements.Add(((TreeViewItem<T>)x).Data);

			var selectedIDs = draggedElements.Select(x => x.ID).ToArray();
			m_TreeModel.MoveElements(parent, insertIndex, draggedElements);
			SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
		}

		private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
		{
			TreeViewItem currentParent = parent;
			while (currentParent != null)
			{
				if (draggedItems.Contains(currentParent))
					return false;
				currentParent = currentParent.parent;
			}
			return true;
		}

		// ------------------------------------------------------------------------------------------------------------
	}
}
