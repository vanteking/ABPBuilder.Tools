using ABPGenerator.Common;
using Microsoft.VisualStudio.TextTemplating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPBuilder.Tools
{
    public partial class Default : Form
    {

        public List<string> list = new List<string>(); //获取所有的表
        public Default()
        {

            InitializeComponent();

            //默认保存在桌面-方便直接复制到项目里
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            txtCodePath.Text = dir;

            //配置默认的连接数据库字符串
            txtServerName.Text = @"";
            txtUsername.Text = "";
            txtPwd.Text = "";



        }

        private void btnConnection_Click(object sender, EventArgs e)
        {

            if (txtDatatName.Text == "")
            {
                MessageBox.Show("请输入数据库名称");
                txtDatatName.Focus();


            }
            else
            {
                try
                {
                    string conn = string.Format("Server={0}; Database={1}; User ID={2}; Password={3};", txtServerName.Text, txtDatatName.Text, txtUsername.Text, txtPwd.Text);
                    list = ABPGenerator.Common.DapperHelper.GetTable(conn);
                    cmbDb.DataSource = list;
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                txtCodePath.Text = foldPath;
            }
        }

        private void btnGenerator_Click(object sender, EventArgs e)
        {
            if (txtCodePath.Text == "")
            {
                MessageBox.Show("请选择文件夹路径");
            }
            //string conn = string.Format("Server={0}; Database={1}; User ID={2}; Password={3};", txtServerName.Text, txtDatatName.Text, txtUsername.Text, txtPwd.Text);
            //var list = ABPGenerator.Common.DapperHelper.GetField(conn, this.cmbDb.Text);

            try
            {
                if (list != null && list.Count() > 0)
                {
                    var _fileName = this.cmbDb.Text.Replace("Tb", "");


                    CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
                    host.Session = new TextTemplatingSession();
                    host.Session.Add("classname", _fileName);
                    host.Session.Add("list", list);


                    //文件保存地址
                    string _save_path = txtCodePath.Text + "/" + _fileName + "s";

                    string _dto_path = _save_path + "/Dto";
                    if (Directory.Exists(_dto_path) == false)
                    {
                        Directory.CreateDirectory(_dto_path);
                    }


                    //保存生成代码后的路径，可以根据实际项目需求修改
                    Dictionary<string, string> dicPath = new Dictionary<string, string>();

                    dicPath.Add("Interface.tt", "/I" + _fileName + "AppService.cs"); //create input
                    dicPath.Add("EntityAppService.tt", "/" + _fileName + "AppService.cs"); //create input


                    dicPath.Add("CreateInput.tt", "/Dto/Create" + _fileName + "Input.cs"); //create input
                    dicPath.Add("Dto.tt", "/Dto/" + _fileName + "Dto.cs"); //Dto                                                
                    dicPath.Add("GetInput.tt", "/Dto/Get" + _fileName + "Input.cs"); //GetInput
                    dicPath.Add("EditInput.tt", "/Dto/Edit" + _fileName + "Input.cs"); //EditInput

                    foreach (var item in dicPath)
                    {
                        #region 
                        var templatePath = System.Windows.Forms.Application.StartupPath + "/Template/" + item.Key; ;
                        host.TemplateFileValue = templatePath;
                        string input = System.IO.File.ReadAllText(templatePath);
                        string output = new Engine().ProcessTemplate(input, host);
                        System.IO.File.WriteAllText(_save_path + item.Value, output.TrimStart(), Encoding.UTF8);
                        #endregion
                    }

                    MessageBox.Show("代码生成成功");
                    //if (MessageBox.Show("打开代码所在文件吗？", "生成成功", MessageBoxButtons.OKCancel) == DialogResult.OK)

                    //{
                    //    System.Diagnostics.Process.Start(_save_path);
                    //}

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
