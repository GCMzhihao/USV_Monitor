
using MavLink;
using Steema.TeeChart.Styles;
using System;
using System.Reflection;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace 地面站
{
    class MavlinkProxy
    {
        private readonly Form1 form1;
        public MavlinkProxy(object sender)
        {
            form1 = (Form1)sender;
            int index = 0;
            foreach (var value in Enum.GetNames(typeof(CMD_TYPE)))
            {
                form1.dataGridView2.Rows.Add();
                form1.dataGridView2.Rows[index].Cells[0].Value = index + 1;
                form1.dataGridView2.Rows[index].Cells[1].Value = value;
                form1.dataGridView2.Rows[index].Cells[2].Value = "写入命令";
                index++;
                if (index + 1 == (int)CMD_TYPE.CMD_TYPE_ENUM_END)
                    break;
            }
        }
        public void TChart1Display(string text, double data)
        {
            if (!form1.CKLBox_DataSel.Items.Contains(text))
                form1.CKLBox_DataSel.Items.Add(text);
            if (form1.CKLBox_DataSel.CheckedItems.Contains(text))
            {
                int num = form1.CKLBox_DataSel.CheckedItems.IndexOf(text);
                form1.FastLines[num].Title = text;
                form1.FastLines[num].Legend.Text = text;
                form1.FastLines[num].Legend.Visible = true;
                form1.FastLines[num].Add(data);
            }
        }
        public void TChart6Display(string text, double x, double y)
        { 
        
        }

        public void Mavlink_PacketReceived(object sender, MavLink.MavlinkPacket e)
        {
            string msg = e.Message.ToString().Substring(12).ToLower();

            
            foreach (var info in (e.Message.GetType()).GetFields())//添加到波形显示
            {
                string WaveName = ((SYS_TYPE)e.SystemId).ToString().Substring(4).ToLower() + e.ComponentId.ToString() + "." + msg + '.' + info.Name;
                TChart1Display(WaveName, Convert.ToDouble(info.GetValue(e.Message).ToString()));
            }

            if (!form1.USV_ID_List.Contains((byte)e.ComponentId))//已有的ID不包含新的USV_ID
            {
                form1.USV_ID_List.Add((byte)e.ComponentId);
                form1.USVs[form1.USV_ID_List.Count - 1] = new USV(form1, (byte)e.ComponentId);
                form1.USVs[form1.USV_ID_List.Count - 1].Init(form1.HorizLines[form1.USV_ID_List.Count + 10]);

            }
            else 
            {
                if (e.Message.ToString().Contains("Msg_usv_state"))
                {
                    form1.USVs[form1.USV_ID_List.BinarySearch((byte)e.ComponentId)].state.latitude = ((Msg_usv_state)e.Message).latitude;
                    form1.USVs[form1.USV_ID_List.BinarySearch((byte)e.ComponentId)].state.longitude = ((Msg_usv_state)e.Message).longitude;
                    form1.USVs[form1.USV_ID_List.BinarySearch((byte)e.ComponentId)].state.speed = ((Msg_usv_state)e.Message).speed;
                    form1.USVs[form1.USV_ID_List.BinarySearch((byte)e.ComponentId)].state.heading = ((Msg_usv_state)e.Message).heading;
                    form1.USVs[form1.USV_ID_List.BinarySearch((byte)e.ComponentId)].state.Track = ((Msg_usv_state)e.Message).Track;

                }
            }

            if (e.Message.ToString().Contains("Msg_param_read_ack"))
            {
                int index = 0;
                while (index < form1.dataGridView1.RowCount)
                {
                    if (form1.dataGridView1.Rows[index].Cells[0].Value.ToString() == ((MavLink.Msg_param_read_ack)e.Message).param_id.ToString())//列表中有该参数数据
                    {
                        form1.dataGridView1.Rows[index].Cells[2].Value = ((MavLink.Msg_param_read_ack)e.Message).value;
                        return;
                    }
                    index++;
                }
                index = form1.dataGridView1.Rows.Add();
                form1.dataGridView1.Rows[index].Cells[0].Value = ((MavLink.Msg_param_read_ack)e.Message).param_id.ToString();
                form1.dataGridView1.Rows[index].Cells[1].Value = (PARAM_TYPE)(((MavLink.Msg_param_read_ack)e.Message).param_id);
                form1.dataGridView1.Rows[index].Cells[2].Value = ((MavLink.Msg_param_read_ack)e.Message).value;
            }
            else if (e.Message.ToString().Contains("Msg_param_write_ack"))
            {
                form1.MavlinkParamWriteAck = true;
                form1.SystemInfo(((PARAM_TYPE)(((Msg_param_write_ack)e.Message).param_id)).ToString() + "写入成功！");
            }
            else if (e.Message.ToString().Contains("Msg_cmd_ack"))
            {
                if (((Msg_cmd_ack)e.Message).cmd_ack_id == (byte)CMD_ACK_FLAG.CMD_ACK_WRITE_SUCESSED)
                {
                    form1.MavlinkCMDAck = true;
                    form1.SystemInfo(((CMD_TYPE)(((Msg_cmd_ack)e.Message).cmd_id)).ToString() + "写入成功！");
                }
                else if (((Msg_cmd_ack)e.Message).cmd_ack_id == (byte)CMD_ACK_FLAG.CMD_ACK_WRITE_FAILED)
                {
                    form1.MavlinkCMDAck = true;
                    form1.SystemInfo(((CMD_TYPE)(((Msg_cmd_ack)e.Message).cmd_id)).ToString() + "写入失败！请稍后再试！");
                }
            }
            else if (e.Message.ToString().Contains("Msg_rocker"))
            {
                if (form1.radioButton7.Checked)
                    form1.MavlinkSendMsg(((Msg_rocker)e.Message));
                form1.msg_Rocker.leftX = ((Msg_rocker)e.Message).leftX;
                form1.msg_Rocker.leftY = ((Msg_rocker)e.Message).leftY;
                form1.msg_Rocker.rightX = ((Msg_rocker)e.Message).rightX;
                form1.msg_Rocker.rightY = ((Msg_rocker)e.Message).rightY;
                form1.msg_Rocker.switchA = ((Msg_rocker)e.Message).switchA;
                form1.msg_Rocker.switchB = ((Msg_rocker)e.Message).switchB;
                form1.msg_Rocker.switchC = ((Msg_rocker)e.Message).switchC;
                form1.msg_Rocker.switchD = ((Msg_rocker)e.Message).switchD;
                form1.msg_Rocker.switchE = ((Msg_rocker)e.Message).switchE;
                form1.msg_Rocker.switchF = ((Msg_rocker)e.Message).switchF;
                form1.msg_Rocker.switchG = ((Msg_rocker)e.Message).switchG;
            }
            else if (e.Message.ToString().Contains("Msg_usv_state"))
            {
                form1.msg_usv_state.latitude = ((Msg_usv_state)e.Message).latitude;
                form1.msg_usv_state.longitude = ((Msg_usv_state)e.Message).longitude;
                form1.msg_usv_state.speed = ((Msg_usv_state)e.Message).speed;
                form1.msg_usv_state.heading = ((Msg_usv_state)e.Message).heading;
                form1.msg_usv_state.Track = ((Msg_usv_state)e.Message).Track;

                form1.MCT84Bl2xy(form1.msg_usv_state.longitude, form1.msg_usv_state.latitude, out form1.Usv_Position.X, out form1.Usv_Position.Y);
                //form1.Tchart6_Draw(form1.HorizLines[11], form1.Usv_Position.X-form1.X_Standard, form1.Usv_Position.Y-form1.Y_Standard);
            }

        }
    }
}

