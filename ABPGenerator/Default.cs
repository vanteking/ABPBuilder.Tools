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

            ////默认保存在桌面-方便直接复制到项目里
            //string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //txtCodePath.Text = dir;


            //配置默认的连接数据库字符串
            txtServerName.Text = @"xxx.xxx.xxx.223\sqk2k8r2";
            txtUsername.Text = "xx";
            txtPwd.Text = "xxx";
            txtDatatName.Text = "xxDB";

            txtPermissionName.Text = "PermissionNames.Sys";


            List<string> listTheme = new List<string> { "Default", "Metronic" };
            cmbTheme.DataSource = listTheme;
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

        private void btnGenerator_Click(object sender, EventArgs e)
        {

            //string conn = string.Format("Server={0}; Database={1}; User ID={2}; Password={3};", txtServerName.Text, txtDatatName.Text, txtUsername.Text, txtPwd.Text);
            //var list = ABPGenerator.Common.DapperHelper.GetField(conn, this.cmbDb.Text);

            try
            {

                var _fileName = this.cmbDb.Text.Replace("Tb", "");


                CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
                host.Session = new TextTemplatingSession();
                host.Session.Add("classname", _fileName);
                host.Session.Add("lowername", _fileName.ToLower());
                host.Session.Add("permissionName", txtPermissionName.Text.Trim());

                //文件保存地址
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string _save_path = dir + "/" + _fileName + "s";

                string _dto_path = _save_path + "/Dto";
                if (Directory.Exists(_dto_path) == false)
                {
                    Directory.CreateDirectory(_dto_path);
                }

                string _controller_path = _save_path + "/Controller";


                string _view_path = _save_path + "/Controller/Views/" + _fileName;

                if (cebView.Checked)
                {
                    if (Directory.Exists(_controller_path) == false)
                    {
                        Directory.CreateDirectory(_controller_path);
                    }

                    if (Directory.Exists(_view_path) == false)
                    {
                        Directory.CreateDirectory(_view_path);
                    }
                }


                //保存生成代码后的路径，可以根据实际项目需求修改
                Dictionary<string, string> dicPath = new Dictionary<string, string>();

                dicPath.Add("/WP.Application/Interface.tt", "/I" + _fileName + "AppService.cs"); //create input
                dicPath.Add("/WP.Application/EntityAppService.tt", "/" + _fileName + "AppService.cs"); //create input


                dicPath.Add("/WP.Application/CreateInput.tt", "/Dto/Create" + _fileName + "Input.cs"); //create input
                dicPath.Add("/WP.Application/Dto.tt", "/Dto/" + _fileName + "Dto.cs"); //Dto                                                
                dicPath.Add("/WP.Application/GetInput.tt", "/Dto/Get" + _fileName + "Input.cs"); //GetInput
                dicPath.Add("/WP.Application/EditInput.tt", "/Dto/Edit" + _fileName + "Input.cs"); //EditInput

                /*Controller*/

                if (cebView.Checked)
                {
                    dicPath.Add("/WP.Web/Controller.tt", "/Controller/" + _fileName + "Controller.cs"); //Controller

                    dicPath.Add("/WP.Web/Views/"+cmbTheme.Text+"/Index.tt", "/Controller/Views/" + _fileName + "/Index.cshtml");
                    dicPath.Add("/WP.Web/Views/" + cmbTheme.Text + "/Create.tt", "/Controller/Views/" + _fileName + "/Create.cshtml");
                    dicPath.Add("/WP.Web/Views/" + cmbTheme.Text + "/Edit.tt", "/Controller/Views/" + _fileName + "/Edit.cshtml");
                }

                foreach (var item in dicPath)
                {
                    #region 
                    var templatePath = System.Windows.Forms.Application.StartupPath + "/Template/" + item.Key;
                    host.TemplateFileValue = templatePath;
                    string input = System.IO.File.ReadAllText(templatePath);
                    string output = new Microsoft.VisualStudio.TextTemplating.Engine().ProcessTemplate(input, host);
                    System.IO.File.WriteAllText(_save_path + item.Value, output.TrimStart(), Encoding.UTF8);
                    #endregion
                }

                MessageBox.Show("代码生成成功,已保存到桌面");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cmbDb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // txtPermissionName.Text = "namespace WP." + cmbDb.Text.Replace("Tb", "") + "s.Dto";
        }

    }
}
