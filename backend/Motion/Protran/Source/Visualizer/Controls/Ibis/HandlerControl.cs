// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HandlerControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// Control that shows information about an <see cref="ITelegramHandler"/>.
    /// </summary>
    public partial class HandlerControl : UserControl, IIbisVisualizationControl
    {
        private readonly TreeNode treeNodeSynchronous;
        private readonly TreeNode treeNodeAsynchronous;

        private SideTab sideTab;

        private TreeNode currentNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerControl"/> class.
        /// </summary>
        public HandlerControl()
        {
            this.InitializeComponent();

            this.treeNodeSynchronous = this.treeView.Nodes[0];
            this.treeNodeAsynchronous = this.treeView.Nodes[1];
        }

        /// <summary>
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="controller">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        public void Configure(IIbisVisualizationService controller)
        {
            controller.TelegramParsing += this.OnTelegramParsing;
            controller.HandlingTelegram += this.OnHandlingTelegram;
            controller.HandledTelegram += this.OnHandledTelegram;
            controller.HandlerCreatedXimple += this.OnHandlerCreatedXimple;
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;
        }

        private static int CompareCells(XimpleCell left, XimpleCell right)
        {
            try
            {
                var diff = left.TableNumber - right.TableNumber;
                if (diff != 0)
                {
                    return diff;
                }

                diff = left.RowNumber - right.RowNumber;
                if (diff != 0)
                {
                    return diff;
                }

                return left.ColumnNumber - right.ColumnNumber;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void OnTelegramParsing(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.OnTelegramParsing), sender, e);
                return;
            }

            this.textBox.Clear();
            this.treeNodeSynchronous.Nodes.Clear();
            this.treeNodeAsynchronous.Nodes.Clear();
            if (this.sideTab != null)
            {
                this.sideTab.Description = string.Empty;
            }
        }

        private void OnHandlingTelegram(object sender, TelegramHandlerEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TelegramHandlerEventArgs>(this.OnHandlingTelegram), sender, e);
                return;
            }

            this.currentNode = new TreeNode(TypePropertyGrid.GetTypeName(e.Handler.GetType()));
            this.treeNodeSynchronous.Nodes.Add(this.currentNode);
            this.treeView.ExpandAll();
            if (this.sideTab != null)
            {
                this.sideTab.Description += this.currentNode.Text + "\r\n";
            }
        }

        private void OnHandledTelegram(object sender, TelegramHandlerEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TelegramHandlerEventArgs>(this.OnHandledTelegram), sender, e);
                return;
            }

            this.currentNode = null;
        }

        private void OnHandlerCreatedXimple(object sender, TelegramHandlerXimpleEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TelegramHandlerXimpleEventArgs>(this.OnHandlerCreatedXimple), sender, e);
                return;
            }

            var parent = this.GetNode(e.Handler);

            var node = parent.Nodes.Add(string.Format("Ximple [cells={0}]", e.Ximple.Cells.Count));
            node.Tag = e.Ximple;
            var cells = new List<XimpleCell>(e.Ximple.Cells);
            cells.Sort(CompareCells);
            foreach (var cell in cells)
            {
                var child =
                    node.Nodes.Add(
                        string.Format(
                            "[{0},{1},{2}]: {3}", cell.TableNumber, cell.RowNumber, cell.ColumnNumber, cell.Value));
                child.Tag = cell;
            }

            this.treeView.ExpandAll();
        }

        private TreeNode GetNode(object sender)
        {
            if (this.currentNode != null)
            {
                return this.currentNode;
            }

            var nodeText = TypePropertyGrid.GetTypeName(sender.GetType());
            if (this.treeNodeAsynchronous.Nodes.Count > 0)
            {
                var node = this.treeNodeAsynchronous.Nodes[this.treeNodeAsynchronous.Nodes.Count - 1];
                if (node.Text.Equals(nodeText))
                {
                    return node;
                }
            }

            if (this.sideTab != null)
            {
                this.sideTab.Description += nodeText + "\r\n";
            }

            return this.treeNodeAsynchronous.Nodes.Add(nodeText);
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
            {
                this.textBox.Text = string.Empty;
                return;
            }

            try
            {
                using (var writer = new StringWriter())
                {
                    var serializer = new XmlSerializer(e.Node.Tag.GetType());
                    var xmlWriter = XmlWriter.Create(
                        writer, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(xmlWriter, e.Node.Tag, namespaces);

                    this.textBox.Text = writer.ToString();
                }
            }
            catch (Exception)
            {
                this.textBox.Text = string.Empty;
            }
        }
    }
}
