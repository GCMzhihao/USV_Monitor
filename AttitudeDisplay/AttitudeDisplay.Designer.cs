namespace AttitudeDisplay
{
    partial class AttitudeDisplay
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AttitudeBox = new System.Windows.Forms.PictureBox();
            this.Timer_ATT = new System.Windows.Forms.Timer(this.components);
            this.Timer_CLR = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.AttitudeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // AttitudeBox
            // 
            this.AttitudeBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttitudeBox.ErrorImage = global::AttitudeDisplay.Properties.Resources.水平仪;
            this.AttitudeBox.Image = global::AttitudeDisplay.Properties.Resources.水平仪;
            this.AttitudeBox.InitialImage = global::AttitudeDisplay.Properties.Resources.磁力计;
            this.AttitudeBox.Location = new System.Drawing.Point(0, 0);
            this.AttitudeBox.Name = "AttitudeBox";
            this.AttitudeBox.Size = new System.Drawing.Size(502, 497);
            this.AttitudeBox.TabIndex = 0;
            this.AttitudeBox.TabStop = false;
            this.AttitudeBox.Resize += new System.EventHandler(this.AttitudeBox_Resize);
            // 
            // Timer_ATT
            // 
            this.Timer_ATT.Enabled = true;
            this.Timer_ATT.Interval = 25;
            this.Timer_ATT.Tick += new System.EventHandler(this.Timer_ATT_Tick);
            // 
            // Timer_CLR
            // 
            this.Timer_CLR.Enabled = true;
            this.Timer_CLR.Interval = 2000;
            this.Timer_CLR.Tick += new System.EventHandler(this.Timer_CLR_Tick);
            // 
            // AttitudeDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.AttitudeBox);
            this.Name = "AttitudeDisplay";
            this.Size = new System.Drawing.Size(502, 497);
            ((System.ComponentModel.ISupportInitialize)(this.AttitudeBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer Timer_ATT;
        protected System.Windows.Forms.PictureBox AttitudeBox;
        private System.Windows.Forms.Timer Timer_CLR;
    }
}
