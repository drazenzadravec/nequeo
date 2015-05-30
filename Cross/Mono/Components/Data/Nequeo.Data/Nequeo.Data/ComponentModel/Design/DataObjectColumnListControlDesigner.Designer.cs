namespace Nequeo.ComponentModel.Design
{
    partial class DataObjectColumnListControlDesigner
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataObjectColumnListControlDesigner));
            this.propertyGridType = new System.Windows.Forms.PropertyGrid();
            this.treeViewDataObjectProperties = new System.Windows.Forms.TreeView();
            this.imageListTree = new System.Windows.Forms.ImageList(this.components);
            this.treeViewSelectedDataObjectProperties = new System.Windows.Forms.TreeView();
            this.lblDataObjectProperties = new System.Windows.Forms.Label();
            this.lblSelectedDataObjectProperties = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.imageListContainer = new System.Windows.Forms.ImageList(this.components);
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGridType
            // 
            this.propertyGridType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridType.Location = new System.Drawing.Point(217, 3);
            this.propertyGridType.Name = "propertyGridType";
            this.propertyGridType.Size = new System.Drawing.Size(420, 455);
            this.propertyGridType.TabIndex = 0;
            // 
            // treeViewDataObjectProperties
            // 
            this.treeViewDataObjectProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewDataObjectProperties.ImageIndex = 0;
            this.treeViewDataObjectProperties.ImageList = this.imageListTree;
            this.treeViewDataObjectProperties.Location = new System.Drawing.Point(3, 29);
            this.treeViewDataObjectProperties.Name = "treeViewDataObjectProperties";
            this.treeViewDataObjectProperties.SelectedImageIndex = 0;
            this.treeViewDataObjectProperties.Size = new System.Drawing.Size(208, 126);
            this.treeViewDataObjectProperties.TabIndex = 1;
            // 
            // imageListTree
            // 
            this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
            this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTree.Images.SetKeyName(0, "Properties.png");
            // 
            // treeViewSelectedDataObjectProperties
            // 
            this.treeViewSelectedDataObjectProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewSelectedDataObjectProperties.ImageIndex = 0;
            this.treeViewSelectedDataObjectProperties.ImageList = this.imageListTree;
            this.treeViewSelectedDataObjectProperties.Location = new System.Drawing.Point(3, 217);
            this.treeViewSelectedDataObjectProperties.Name = "treeViewSelectedDataObjectProperties";
            this.treeViewSelectedDataObjectProperties.SelectedImageIndex = 0;
            this.treeViewSelectedDataObjectProperties.Size = new System.Drawing.Size(208, 212);
            this.treeViewSelectedDataObjectProperties.TabIndex = 2;
            // 
            // lblDataObjectProperties
            // 
            this.lblDataObjectProperties.AutoSize = true;
            this.lblDataObjectProperties.Location = new System.Drawing.Point(7, 8);
            this.lblDataObjectProperties.Name = "lblDataObjectProperties";
            this.lblDataObjectProperties.Size = new System.Drawing.Size(100, 13);
            this.lblDataObjectProperties.TabIndex = 3;
            this.lblDataObjectProperties.Text = "Available Properties";
            // 
            // lblSelectedDataObjectProperties
            // 
            this.lblSelectedDataObjectProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedDataObjectProperties.AutoSize = true;
            this.lblSelectedDataObjectProperties.Location = new System.Drawing.Point(3, 198);
            this.lblSelectedDataObjectProperties.Name = "lblSelectedDataObjectProperties";
            this.lblSelectedDataObjectProperties.Size = new System.Drawing.Size(99, 13);
            this.lblSelectedDataObjectProperties.TabIndex = 4;
            this.lblSelectedDataObjectProperties.Text = "Selected Properties";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(136, 161);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(136, 435);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // imageListContainer
            // 
            this.imageListContainer.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListContainer.ImageStream")));
            this.imageListContainer.TransparentColor = System.Drawing.Color.White;
            this.imageListContainer.Images.SetKeyName(0, "BuilderDialog_moveup.bmp");
            this.imageListContainer.Images.SetKeyName(1, "BuilderDialog_movedown.bmp");
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.BackColor = System.Drawing.SystemColors.Control;
            this.btnUp.ImageKey = "BuilderDialog_moveup.bmp";
            this.btnUp.ImageList = this.imageListContainer;
            this.btnUp.Location = new System.Drawing.Point(3, 435);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(25, 23);
            this.btnUp.TabIndex = 7;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.ImageKey = "BuilderDialog_movedown.bmp";
            this.btnDown.ImageList = this.imageListContainer;
            this.btnDown.Location = new System.Drawing.Point(34, 435);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(25, 23);
            this.btnDown.TabIndex = 8;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // DataObjectColumnListControlDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblSelectedDataObjectProperties);
            this.Controls.Add(this.lblDataObjectProperties);
            this.Controls.Add(this.treeViewSelectedDataObjectProperties);
            this.Controls.Add(this.treeViewDataObjectProperties);
            this.Controls.Add(this.propertyGridType);
            this.Name = "DataObjectColumnListControlDesigner";
            this.Size = new System.Drawing.Size(640, 461);
            this.Load += new System.EventHandler(this.DataObjectColumnListControlDesigner_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGridType;
        private System.Windows.Forms.TreeView treeViewDataObjectProperties;
        private System.Windows.Forms.TreeView treeViewSelectedDataObjectProperties;
        private System.Windows.Forms.Label lblDataObjectProperties;
        private System.Windows.Forms.Label lblSelectedDataObjectProperties;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ImageList imageListContainer;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ImageList imageListTree;
    }
}
