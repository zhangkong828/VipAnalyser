using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.TestClient
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            cmb_Type.SelectedIndex = 0;
        }

        private void rtxt_Param_Leave(object sender, EventArgs e)
        {
            try
            {
                if (rtxt_Param.Text.Contains("{"))
                    rtxt_Param.Text = CommonCla.ConvertJsonString(rtxt_Param.Text);
            }
            catch
            { }
        }

        /// <summary>
        ///  调用
        /// </summary>
        private void btn_Excute_Click(object sender, EventArgs e)
        {
            btn_Excute.Enabled = false;
            var stopwatch = Stopwatch.StartNew();

            Task.Run(() =>
            {
                Excute(stopwatch);
                btn_Excute.Enabled = true;
            });

            Task.Run(() =>
            {
                while (stopwatch.IsRunning)
                {
                    lb_time.Text = string.Format("耗时(毫秒):{0}", stopwatch.ElapsedMilliseconds);
                    Thread.Sleep(10);
                }
            });
        }

        /// <summary>
        /// 连续调用
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //暂不需要
            return;
            Task.Run(() =>
            {
                while (true)
                {
                    btn_Excute.Enabled = false;
                    var stopwatch = Stopwatch.StartNew();

                    Task.WaitAll(
                        Task.Run(() =>
                        {
                            Excute(stopwatch);
                            btn_Excute.Enabled = true;
                        }),
                        Task.Run(() =>
                        {
                            while (stopwatch.IsRunning)
                            {
                                lb_time.Text = string.Format("耗时(毫秒):{0}", stopwatch.ElapsedMilliseconds);
                                Thread.Sleep(10);
                            }
                        })
                    );

                    Thread.Sleep(1000);
                }
            });
        }

        private void Excute(Stopwatch stopwatch = null)
        {
            try
            {
                var pipeName = cmb_Program.Text + ".0";

                var stopkey = Guid.NewGuid().ToString();
                txt_stopkey.Text = stopkey;
                rtxt_Result.Text = string.Empty;

                string result = string.Empty;
                if (radio_local.Checked)
                {
                    //本地调试
                    result = SocketAccess.Access<string, string>(
                        cmb_Type.Text,
                        rtxt_Param.Text,
                        int.Parse(numeric_Timeout.Value.ToString()),
                        stopkey,
                        6666);
                }
                else
                {
                    //远程模拟
                }

                try
                {
                    rtxt_Result.Text = CommonCla.ConvertJsonString(result);
                }
                catch
                {
                    rtxt_Result.Text = result;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    rtxt_Result.Text = CommonCla.ConvertJsonString(ex.Message);
                }
                catch
                {
                    rtxt_Result.Text = ex.Message;
                }
            }
            finally
            {
                if (stopwatch != null)
                    stopwatch.Stop();
            }
        }

       
    }
}
