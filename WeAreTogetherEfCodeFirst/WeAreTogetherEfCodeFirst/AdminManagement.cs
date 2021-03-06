﻿using LinqToSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeAreTogetherEfCodeFirst
{
    public partial class AdminManagement : Form
    {
        public AdminManagement()
        {
            InitializeComponent();
        }
        //We've created a instance to have DBContext connection
        WeAreTogetherDataContext _wrt = new WeAreTogetherDataContext();
        private void GetCities()
        {
            var query = from c in _wrt.Cities select c; //We get our cities from database
            cbxCity.DataSource = query.ToList();
            cbxCity.DisplayMember = "Name";
            cbxCity.ValueMember = "Id";
            GetDistricts();
            GetManagementTypes();
        }

        private void GetDistricts()
        {
            if (cbxCity.SelectedIndex != -1)
            {
                var CityDistrics = from d in _wrt.Districts
                                   where
                                            d.City == Convert.ToInt32(cbxCity.SelectedValue)
                                   select d;
                cbxDistrict.ValueMember = "Id";
                cbxDistrict.DisplayMember = "District1";
                cbxDistrict.DataSource = CityDistrics;
            }
        }

        private void GetManagements()
        {
            //We get our users from database
            
            var s1 = from x in _wrt.Managements select x;
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("ManagementType");
            dt.Columns.Add("City");
            dt.Columns.Add("District");
            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("Point");
            dt.Columns.Add("Username");
            dt.Columns.Add("Password");

            foreach (var item in s1)
            {
                var getManagementTypeId = from mt in _wrt.ManagementTypes where mt.Id == item.ManagementTypeId select mt;
                string managementTypeName = getManagementTypeId.ToList()[0].Name.ToString();
                var getCityId = from x in _wrt.Cities where x.Id == item.CityId select x;
                string Cityname = getCityId.ToList()[0].Name.ToString();
                var getDistrictId = from x in _wrt.Districts where x.Id == item.DistrictId select x;
                string DistrictName = getDistrictId.ToList()[0].District1.ToString();
                dt.Rows.Add(item.Id, managementTypeName, Cityname, DistrictName, item.Name, item.Address, item.Point, item.Username, item.Password);
            }
            dgwManagement.DataSource = dt;
        }

        //We've created that method to clear our tools after CRUD operations
        void Clear()
        {
            cbxCity.Text = "";
            cbxDistrict.Text = "";
            cbxManagementType.Text = "";
            tbxName.Text = "";
            tbxAddress.Text = "";
            tbxUsername.Text = "";
            tbxPassword.Text = "";
            GetCities();
            GetDistricts();
            GetManagements();
        }

        private void AdminManagement_Load(object sender, EventArgs e)
        {
            GetManagementTypes();
            GetCities();
            GetDistricts();
            GetManagements();
        }

        //Getting management types with a method
        private void GetManagementTypes()
        {
            var query = from m in _wrt.ManagementTypes select m;
            cbxManagementType.DataSource = query.ToList();
            cbxManagementType.DisplayMember = "Name";
            cbxManagementType.ValueMember = "Id";
        }

        private void cbxCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDistricts();
        }

        private void dgwManagement_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var getCityAndDistrictId = from m in _wrt.Managements where m.Id == Convert.ToInt32(dgwManagement.CurrentRow.Cells[0].Value) select m;
            cbxCity.Text = "";
            cbxDistrict.Text = "";
            cbxManagementType.Text = "";
            cbxManagementType.SelectedText= Convert.ToString(dgwManagement.CurrentRow.Cells[1].Value);
            cbxManagementType.SelectedValue = Convert.ToInt32(getCityAndDistrictId.ToList().FirstOrDefault().ManagementTypeId.ToString());
            cbxCity.SelectedText = Convert.ToString(dgwManagement.CurrentRow.Cells[2].Value);
            cbxCity.SelectedValue = Convert.ToInt32(getCityAndDistrictId.ToList().FirstOrDefault().CityId.ToString());
            cbxDistrict.SelectedText = Convert.ToString(dgwManagement.CurrentRow.Cells[3].Value);
            cbxDistrict.SelectedValue = Convert.ToInt32(getCityAndDistrictId.ToList().FirstOrDefault().DistrictId.ToString());
            tbxName.Text = Convert.ToString(dgwManagement.CurrentRow.Cells[4].Value);
            tbxAddress.Text= Convert.ToString(dgwManagement.CurrentRow.Cells[5].Value);
            tbxUsername.Text = Convert.ToString(dgwManagement.CurrentRow.Cells[7].Value);
            tbxPassword.Text= Convert.ToString(dgwManagement.CurrentRow.Cells[8].Value);
        }

        //Adding a new management
        private void btnAddManagement_Click(object sender, EventArgs e)
        {
            Management management = new Management();
            try
            {
                management.ManagementTypeId = Convert.ToInt32(cbxManagementType.SelectedValue);
                management.CityId = Convert.ToInt32(cbxCity.SelectedValue);
                management.DistrictId = Convert.ToInt32(cbxDistrict.SelectedValue);
                management.Name = tbxName.Text;
                management.Point = 0;
                management.Address = tbxAddress.Text;
                management.Username = tbxUsername.Text;
                management.Password = tbxPassword.Text;
                _wrt.Managements.InsertOnSubmit(management);
                _wrt.SubmitChanges();
                GetManagements();
                Clear();
                MessageBox.Show("Entity deleted successfully");
            }
            catch
            {
                MessageBox.Show("We got a problem on the system.Try it another time.");
            }
        }

        //Deleting a management with their id number
        private void btnDeleteManagement_Click(object sender, EventArgs e)
        {
            Management management = new Management();
            try
            {
                int id = Convert.ToInt32(dgwManagement.CurrentRow.Cells[0].Value);
                management = _wrt.Managements.FirstOrDefault(s => s.Id == id);
                _wrt.Managements.DeleteOnSubmit(management);
                _wrt.SubmitChanges();
                GetManagements();
                Clear();
                MessageBox.Show("Entity deleted successfully");
            }
            catch
            {
                MessageBox.Show("We got a problem on the system.Try it another time.");
            }
}

        //Updating a management with their id number
        private void btnUpdateManagement_Click(object sender, EventArgs e)
        {
            Management management = new Management();
            try
            {
                int id = Convert.ToInt32(dgwManagement.CurrentRow.Cells[0].Value);
                management = _wrt.Managements.FirstOrDefault(m => m.Id == id);
                management.ManagementTypeId = Convert.ToInt32(cbxManagementType.SelectedValue);
                management.CityId = Convert.ToInt32(cbxCity.SelectedValue);
                management.DistrictId = Convert.ToInt32(cbxDistrict.SelectedValue);
                management.Name = tbxName.Text;
                management.Address = tbxAddress.Text;
                management.Username = tbxUsername.Text;
                management.Password = tbxPassword.Text;
                _wrt.SubmitChanges();
                GetManagements();
                Clear();
                MessageBox.Show("Entity deleted successfully");
            }
            catch
            {
                MessageBox.Show("We got a problem on the system.Try it another time.");
            }
        }

        private void adminAnimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminAnimal an = new AdminAnimal();
            an.Show();
            this.Hide();
        }

        private void adminShelterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminShelter As = new AdminShelter();
            As.Show();
            this.Hide();
        }

        private void adminManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminManagement AdminM = new AdminManagement();
            AdminM.Show();
            this.Hide();
        }

        private void adminUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminUser au = new AdminUser();
            au.Show();
            this.Hide();
        }

        private void adminOtherThingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminOtherThings aot = new AdminOtherThings();
            aot.Show();
            this.Hide();
        }

        private void adminAddPart1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminAddPartOne one = new AdminAddPartOne();
            one.Show();
            this.Hide();
        }

        private void tbxName_Validating(object sender, CancelEventArgs e)
        {
            if (tbxName.Text.Length <= 0)
            {
                errorProvider.SetError(tbxName, "This field cannot be empty");
            }
            else
            {
                errorProvider.Clear();
            }
        }

        private void tbxAddress_Validating(object sender, CancelEventArgs e)
        {
            if (tbxAddress.Text.Length <= 0)
            {
                errorProvider.SetError(tbxAddress, "This field cannot be empty");
            }
            else
            {
                errorProvider.Clear();
            }
        }

        private void tbxUsername_Validating(object sender, CancelEventArgs e)
        {
            if (tbxUsername.Text.Length <= 0)
            {
                errorProvider.SetError(tbxUsername, "This field cannot be empty");
            }
            else
            {
                errorProvider.Clear();
            }
        }

        private void tbxPassword_Validating(object sender, CancelEventArgs e)
        {
            if (tbxPassword.Text.Length <= 0)
            {
                errorProvider.SetError(tbxPassword, "This field cannot be empty");
            }
            else
            {
                errorProvider.Clear();
            }
        }

        private void AdminManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseTheForm(e);
        }

        private void CloseTheForm(FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you wanna exit from application", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Login lgn = new Login();
                lgn.Show();
                this.Hide();
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        private void txtFindManagement_TextChanged(object sender, EventArgs e)
        {
            SearchManagement(txtFindManagement.Text);
        }
        public void SearchManagement(string valueToFind)
        {
            var searchManagement = from m in _wrt.Managements where m.Name.Contains(valueToFind) select m;
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("ManagementType");
            dt.Columns.Add("City");
            dt.Columns.Add("District");
            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("Point");
            dt.Columns.Add("Username");
            dt.Columns.Add("Password");

            foreach (var item in searchManagement)
            {
                var getManagementTypeId = from mt in _wrt.ManagementTypes where mt.Id == item.ManagementTypeId select mt;
                string managementTypeName = getManagementTypeId.ToList()[0].Name.ToString();
                var getCityId = from x in _wrt.Cities where x.Id == item.CityId select x;
                string Cityname = getCityId.ToList()[0].Name.ToString();
                var getDistrictId = from x in _wrt.Districts where x.Id == item.DistrictId select x;
                string DistrictName = getDistrictId.ToList()[0].District1.ToString();
                dt.Rows.Add(item.Id, managementTypeName, Cityname, DistrictName, item.Name, item.Address, item.Point, item.Username, item.Password);
            }
            dgwManagement.DataSource = dt;
        }
    }
}
