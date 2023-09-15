using UnityEditor.IMGUI.Controls;

namespace Editor
{
    public class FileTreeView : TreeView
    {
        private FileTree fileTree;
        public FileTreeView(TreeViewState state) : base(state)
        {
        }
        
        public FileTreeView(TreeViewState state, FileTree fileTree) : base(state)
        {
            this.fileTree = fileTree;
        }

        public FileTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0,0,fileTree.root.name);
        }
    }
}