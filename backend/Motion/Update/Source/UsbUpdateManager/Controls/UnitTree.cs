// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTree.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitTree type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Motion.Update.UsbUpdateManager.Data;
    using Gorba.Motion.Update.UsbUpdateManager.Properties;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A tree containing Unit Groups and Units.
    /// </summary>
    public partial class UnitTree : UserControl
    {
        private const StringComparison DefaultStringComparison = StringComparison.CurrentCultureIgnoreCase;

        private readonly IProjectManager projectManager;

        private List<UnitGroup> groups;

        private bool readOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTree"/> class.
        /// </summary>
        public UnitTree()
        {
            this.InitializeComponent();

            this.imageList.Images.Add("UnitGroup", Resources.UnitGroup);
            this.imageList.Images.Add("Unit", Resources.Unit);

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Event that is risen when the selected item changes;
        /// either <see cref="SelectedUnit"/> or <see cref="SelectedUnitGroup"/>.
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Gets or sets the list of Unit Groups.
        /// From the outside, don't modify the list returned by this property but
        /// rather just set the property to a new list when you want to change it.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<UnitGroup> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                this.groups = value;
                this.UpdateTree();
            }
        }

        /// <summary>
        /// Gets the selected Unit or null if a Unit Group or nothing was selected.
        /// </summary>
        public Unit SelectedUnit
        {
            get
            {
                if (this.treeView.SelectedNode == null)
                {
                    return null;
                }

                return this.treeView.SelectedNode.Tag as Unit;
            }
        }

        /// <summary>
        /// Gets the selected Unit Group or null if a Unit or nothing was selected.
        /// </summary>
        public UnitGroup SelectedUnitGroup
        {
            get
            {
                if (this.treeView.SelectedNode == null)
                {
                    return null;
                }

                return this.treeView.SelectedNode.Tag as UnitGroup;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tree is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }

            set
            {
                if (this.readOnly == value)
                {
                    return;
                }

                this.readOnly = value;
                this.UpdateControlsEnabled();
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectedItemChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseSelectedItemChanged(EventArgs e)
        {
            this.UpdateControlsEnabled();

            var handler = this.SelectedItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void UpdateControlsEnabled()
        {
            this.addToolStripDropDownButton.Enabled = !this.readOnly;
            this.unitToolStripMenuItem.Enabled = this.treeView.SelectedNode != null && !this.ReadOnly;
            this.removeToolStripButton.Enabled = this.treeView.SelectedNode != null && !this.ReadOnly;
        }

        private void UpdateTree()
        {
            this.treeView.Nodes.Clear();

            if (this.Groups == null)
            {
                return;
            }

            var groupList = new List<UnitGroup>(this.Groups);
            groupList.Sort((a, b) => string.Compare(a.Name, b.Name, DefaultStringComparison));
            foreach (var unitGroup in groupList)
            {
                var groupNode = this.CreateGroupNode(unitGroup);
                this.treeView.Nodes.Add(groupNode);

                var units = new List<Unit>(unitGroup.Units);
                units.Sort((a, b) => string.Compare(a.Name, b.Name, DefaultStringComparison));

                foreach (var unit in units)
                {
                    var unitNode = this.CreateUnitNode(unit);
                    groupNode.Nodes.Add(unitNode);
                }
            }

            this.treeView.ExpandAll();
        }

        private void AddUnitGroup(string name)
        {
            var unitGroup = this.projectManager.CreateUnitGroup(name);

            if (this.Groups.Find(g => g.Name.Equals(unitGroup.Name, DefaultStringComparison)) != null)
            {
                MessageBox.Show(
                    this,
                    "A Unit Group with the same name already exists",
                    "New Unit Group",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.Groups.Add(unitGroup);
            this.projectManager.Save();
            var node = this.CreateGroupNode(unitGroup);
            this.InsertNodeSorted(this.treeView.Nodes, node);
            this.treeView.SelectedNode = node;
        }

        private void AddUnit(string name, TreeNode parent)
        {
            var parentGroup = parent.Tag as UnitGroup;
            if (parentGroup == null)
            {
                return;
            }

            var unit = new Unit { Name = name };

            foreach (var unitGroup in this.Groups)
            {
                if (unitGroup.Units.Find(u => u.Name.Equals(unit.Name, DefaultStringComparison)) != null)
                {
                    MessageBox.Show(
                        this,
                        "A Unit with the same name already exists",
                        "New Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            parentGroup.Units.Add(unit);
            this.projectManager.Save();
            var node = this.CreateUnitNode(unit);
            this.InsertNodeSorted(parent.Nodes, node);
            this.treeView.SelectedNode = node;
        }

        private void RemoveUnitGroup(TreeNode treeNode, UnitGroup unitGroup)
        {
            var text = string.Format(
                "Do you really want to remove the Unit Group \"{0}\" with all connected units?", unitGroup.Name);
            if (MessageBox.Show(
                this,
                text,
                "Remove Unit Group",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            this.Groups.Remove(unitGroup);
            this.projectManager.Save();
            treeNode.Remove();

            if (this.treeView.Nodes.Count == 0)
            {
                this.RaiseSelectedItemChanged(EventArgs.Empty);
            }
        }

        private void RemoveUnit(TreeNode treeNode, Unit unit)
        {
            var parent = treeNode.Parent;
            var parentGroup = parent.Tag as UnitGroup;
            if (parentGroup == null)
            {
                return;
            }

            if (MessageBox.Show(
                this,
                string.Format("Do you really want to remove the Unit \"{0}\"?", unit.Name),
                "Remove Unit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            parentGroup.Units.Remove(unit);
            this.projectManager.Save();
            parent.Nodes.Remove(treeNode);

            this.treeView.SelectedNode = parent;
        }

        private TreeNode CreateGroupNode(UnitGroup unitGroup)
        {
            var node = new TreeNode(unitGroup.Name);
            node.Tag = unitGroup;
            node.ImageKey = node.SelectedImageKey = "UnitGroup";
            return node;
        }

        private TreeNode CreateUnitNode(Unit unit)
        {
            var node = new TreeNode(unit.Name);
            node.Tag = unit;
            node.ImageKey = node.SelectedImageKey = "Unit";
            return node;
        }

        private void InsertNodeSorted(TreeNodeCollection nodes, TreeNode node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (string.Compare(node.Text, nodes[i].Text, DefaultStringComparison) < 0)
                {
                    nodes.Insert(i, node);
                    return;
                }
            }

            nodes.Add(node);
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.RaiseSelectedItemChanged(e);
        }

        private void UnitGroupToolStripMenuItemClick(object sender, EventArgs e)
        {
            var input = new TextInputDialog
            {
                Label = "Please provide a name for the unit group:",
                Text = "New Unit Group",
                InputRequired = true
            };

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.AddUnitGroup(input.InputText);
        }

        private void UnitToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode == null)
            {
                return;
            }

            var parent = this.treeView.SelectedNode.Parent ?? this.treeView.SelectedNode;

            var input = new TextInputDialog
            {
                Label = "Please provide a name for the unit:",
                Text = "New Unit",
                InputRequired = true
            };

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.AddUnit(input.InputText, parent);
        }

        private void RemoveToolStripButtonClick(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode == null)
            {
                return;
            }

            var unit = this.SelectedUnit;
            var unitGroup = this.SelectedUnitGroup;
            if (unit == null && unitGroup == null)
            {
                return;
            }

            if (unit != null)
            {
                this.RemoveUnit(this.treeView.SelectedNode, unit);
            }
            else
            {
                this.RemoveUnitGroup(this.treeView.SelectedNode, unitGroup);
            }
        }
    }
}
