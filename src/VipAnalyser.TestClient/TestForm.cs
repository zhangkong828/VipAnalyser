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
                var stopkey = Guid.NewGuid().ToString();
                txt_stopkey.Text = stopkey;
                rtxt_Result.Text = string.Empty;

                string result = string.Empty;
                if (radio_local.Checked)
                {
                    //socket
                    result = SocketAccess.Access<string, string>(
                        txtType.Text,
                        rtxt_Param.Text,
                        int.Parse(numeric_Timeout.Value.ToString()),
                        stopkey,
                        Convert.ToInt32(txtAddress.Text));
                }
                else
                {
                    //http
                    var url = txtAddress.Text + txtType.Text;
                    var postData = rtxt_Param.Text.Replace("\n","");
                    result = HttpHelper.Post(url, postData);
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

        private void radio_local_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_local.Checked)
            {
                txtAddress.Text = "6666";
                txtType.Text = "Decode";
            }
        }

        private void radio_remote_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_remote.Checked)
            {
                txtAddress.Text = "http://127.0.0.1:11234/";
                txtType.Text = "api/core/analyse";
            }
        }
    }
}
