namespace cam_aforge1
{
    partial class GUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.viewFinder = new System.Windows.Forms.PictureBox();
            this.vidSrc = new System.Windows.Forms.ComboBox();
            this.vidSrcLabel = new System.Windows.Forms.Label();
            this.rfsh = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.fps = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ctrl = new System.Windows.Forms.Button();
            this.Circle = new System.Windows.Forms.Button();
            this.countLabel = new System.Windows.Forms.Label();
            this.countDisp = new System.Windows.Forms.Label();
            this.Pen = new System.Windows.Forms.Button();
            this.Line = new System.Windows.Forms.Button();
            this.Undo = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.viewFinder)).BeginInit();
            this.fps.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewFinder
            // 
            this.viewFinder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewFinder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewFinder.Location = new System.Drawing.Point(261, 11);
            this.viewFinder.Margin = new System.Windows.Forms.Padding(4);
            this.viewFinder.Name = "viewFinder";
            this.viewFinder.Size = new System.Drawing.Size(922, 585);
            this.viewFinder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.viewFinder.TabIndex = 0;
            this.viewFinder.TabStop = false;
            // 
            // vidSrc
            // 
            this.vidSrc.FormattingEnabled = true;
            this.vidSrc.Location = new System.Drawing.Point(16, 32);
            this.vidSrc.Margin = new System.Windows.Forms.Padding(4);
            this.vidSrc.Name = "vidSrc";
            this.vidSrc.Size = new System.Drawing.Size(208, 24);
            this.vidSrc.TabIndex = 1;
            // 
            // vidSrcLabel
            // 
            this.vidSrcLabel.AutoSize = true;
            this.vidSrcLabel.Location = new System.Drawing.Point(12, 11);
            this.vidSrcLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.vidSrcLabel.Name = "vidSrcLabel";
            this.vidSrcLabel.Size = new System.Drawing.Size(132, 17);
            this.vidSrcLabel.TabIndex = 2;
            this.vidSrcLabel.Text = "Select video source";
            // 
            // rfsh
            // 
            this.rfsh.Location = new System.Drawing.Point(17, 76);
            this.rfsh.Margin = new System.Windows.Forms.Padding(4);
            this.rfsh.Name = "rfsh";
            this.rfsh.Size = new System.Drawing.Size(91, 38);
            this.rfsh.TabIndex = 3;
            this.rfsh.Text = "&Refresh";
            this.rfsh.UseVisualStyleBackColor = true;
            this.rfsh.Click += new System.EventHandler(this.rfsh_Click);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(135, 76);
            this.start.Margin = new System.Windows.Forms.Padding(4);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(91, 38);
            this.start.TabIndex = 4;
            this.start.Text = "&Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // fps
            // 
            this.fps.Controls.Add(this.label2);
            this.fps.Location = new System.Drawing.Point(16, 177);
            this.fps.Margin = new System.Windows.Forms.Padding(4);
            this.fps.Name = "fps";
            this.fps.Padding = new System.Windows.Forms.Padding(4);
            this.fps.Size = new System.Drawing.Size(207, 34);
            this.fps.TabIndex = 6;
            this.fps.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Device ready..";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ctrl
            // 
            this.ctrl.Location = new System.Drawing.Point(17, 122);
            this.ctrl.Margin = new System.Windows.Forms.Padding(4);
            this.ctrl.Name = "ctrl";
            this.ctrl.Size = new System.Drawing.Size(91, 38);
            this.ctrl.TabIndex = 7;
            this.ctrl.Text = "&Control";
            this.ctrl.UseVisualStyleBackColor = true;
            this.ctrl.Click += new System.EventHandler(this.ctrl_Click);
            // 
            // Circle
            // 
            this.Circle.Location = new System.Drawing.Point(17, 323);
            this.Circle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Circle.Name = "Circle";
            this.Circle.Size = new System.Drawing.Size(91, 38);
            this.Circle.TabIndex = 10;
            this.Circle.Text = "Circle";
            this.Circle.UseVisualStyleBackColor = true;
            this.Circle.Click += new System.EventHandler(this.button3_Click);
            // 
            // countLabel
            // 
            this.countLabel.AutoSize = true;
            this.countLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countLabel.Location = new System.Drawing.Point(24, 462);
            this.countLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(95, 31);
            this.countLabel.TabIndex = 11;
            this.countLabel.Text = "Count:";
            // 
            // countDisp
            // 
            this.countDisp.AutoSize = true;
            this.countDisp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countDisp.Location = new System.Drawing.Point(129, 462);
            this.countDisp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.countDisp.Name = "countDisp";
            this.countDisp.Size = new System.Drawing.Size(29, 31);
            this.countDisp.TabIndex = 12;
            this.countDisp.Text = "0";
            // 
            // Pen
            // 
            this.Pen.Location = new System.Drawing.Point(17, 236);
            this.Pen.Name = "Pen";
            this.Pen.Size = new System.Drawing.Size(91, 38);
            this.Pen.TabIndex = 14;
            this.Pen.Text = "Pen";
            this.Pen.UseVisualStyleBackColor = true;
            this.Pen.Click += new System.EventHandler(this.button1_Click);
            // 
            // Line
            // 
            this.Line.Location = new System.Drawing.Point(17, 280);
            this.Line.Name = "Line";
            this.Line.Size = new System.Drawing.Size(91, 38);
            this.Line.TabIndex = 14;
            this.Line.Text = "Line";
            this.Line.UseVisualStyleBackColor = true;
            this.Line.Click += new System.EventHandler(this.button2_Click);
            // 
            // Undo
            // 
            this.Undo.Location = new System.Drawing.Point(17, 402);
            this.Undo.Name = "Undo";
            this.Undo.Size = new System.Drawing.Size(91, 38);
            this.Undo.TabIndex = 15;
            this.Undo.Text = "Undo";
            this.Undo.UseVisualStyleBackColor = true;
            this.Undo.Click += new System.EventHandler(this.Undo_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(179, 280);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Object";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 612);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Undo);
            this.Controls.Add(this.Line);
            this.Controls.Add(this.Pen);
            this.Controls.Add(this.countDisp);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.Circle);
            this.Controls.Add(this.ctrl);
            this.Controls.Add(this.fps);
            this.Controls.Add(this.start);
            this.Controls.Add(this.rfsh);
            this.Controls.Add(this.vidSrcLabel);
            this.Controls.Add(this.vidSrc);
            this.Controls.Add(this.viewFinder);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GUI";
            this.Text = "Orthoscope/Arthroscope Camera";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.GUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.viewFinder)).EndInit();
            this.fps.ResumeLayout(false);
            this.fps.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox viewFinder;
        private System.Windows.Forms.ComboBox vidSrc;
        private System.Windows.Forms.Label vidSrcLabel;
        private System.Windows.Forms.Button rfsh;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.GroupBox fps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button ctrl;
        private System.Windows.Forms.Button Circle;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label countDisp;
        private System.Windows.Forms.Button Pen;
        private System.Windows.Forms.Button Line;
        private System.Windows.Forms.Button Undo;
        private System.Windows.Forms.Button button1;
    }
}

