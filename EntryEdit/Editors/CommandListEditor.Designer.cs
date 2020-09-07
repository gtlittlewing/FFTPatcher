﻿namespace EntryEdit.Editors
{
    partial class CommandListEditor
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
            this.btn_Add = new System.Windows.Forms.Button();
            this.tlp_Commands = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Page_Prev = new System.Windows.Forms.Button();
            this.btn_Page_Next = new System.Windows.Forms.Button();
            this.spinner_Page = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.spinner_Page)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(127, 10);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(24, 24);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "+";
            this.btn_Add.UseVisualStyleBackColor = true;
            // 
            // tlp_Commands
            // 
            this.tlp_Commands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlp_Commands.AutoScroll = true;
            this.tlp_Commands.ColumnCount = 2;
            this.tlp_Commands.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0F));
            this.tlp_Commands.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize, 545.0F));
            this.tlp_Commands.Location = new System.Drawing.Point(7, 40);
            this.tlp_Commands.Name = "tlp_Commands";
            this.tlp_Commands.Size = new System.Drawing.Size(585, 200);
            this.tlp_Commands.TabIndex = 1;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(157, 10);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(24, 24);
            this.btn_Delete.TabIndex = 2;
            this.btn_Delete.Text = "-";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Page_Prev
            // 
            this.btn_Page_Prev.Location = new System.Drawing.Point(7, 10);
            this.btn_Page_Prev.Name = "btn_Page_Prev";
            this.btn_Page_Prev.Size = new System.Drawing.Size(24, 24);
            this.btn_Page_Prev.TabIndex = 3;
            this.btn_Page_Prev.Text = "<";
            this.btn_Page_Prev.UseVisualStyleBackColor = true;
            this.btn_Page_Prev.Click += new System.EventHandler(this.btn_Page_Prev_Click);
            // 
            // btn_Page_Next
            // 
            this.btn_Page_Next.Location = new System.Drawing.Point(80, 10);
            this.btn_Page_Next.Name = "btn_Page_Next";
            this.btn_Page_Next.Size = new System.Drawing.Size(24, 24);
            this.btn_Page_Next.TabIndex = 4;
            this.btn_Page_Next.Text = ">";
            this.btn_Page_Next.UseVisualStyleBackColor = true;
            this.btn_Page_Next.Click += new System.EventHandler(this.btn_Page_Next_Click);
            // 
            // spinner_Page
            // 
            this.spinner_Page.Location = new System.Drawing.Point(37, 12);
            this.spinner_Page.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinner_Page.Name = "spinner_Page";
            this.spinner_Page.Size = new System.Drawing.Size(37, 20);
            this.spinner_Page.TabIndex = 5;
            this.spinner_Page.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinner_Page.ValueChanged += new System.EventHandler(this.spinner_Page_ValueChanged);
            // 
            // CommandListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spinner_Page);
            this.Controls.Add(this.btn_Page_Next);
            this.Controls.Add(this.btn_Page_Prev);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.tlp_Commands);
            this.Controls.Add(this.btn_Add);
            this.Name = "CommandListEditor";
            this.Size = new System.Drawing.Size(605, 250);
            ((System.ComponentModel.ISupportInitialize)(this.spinner_Page)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.TableLayoutPanel tlp_Commands;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Page_Prev;
        private System.Windows.Forms.Button btn_Page_Next;
        private System.Windows.Forms.NumericUpDown spinner_Page;
    }
}