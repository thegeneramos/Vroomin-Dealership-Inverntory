using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Linq;
using System.Runtime.Remoting.Contexts;

namespace Assignment10._3
{
    public partial class Vroomin_Dealership : Form
    {
        public Vroomin_Dealership()
        {
            InitializeComponent();
            LoadGrid();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void RefreshData()
        {
            txtVIN.Clear();
            txtYear.Clear();
            txtMake.Clear();
            txtModel.Clear();
            txtPrice.Clear();
        }
        private void LoadGrid()
        {
            using (var context = new CarInventoryDataContext())
            {
                Table<Car> currentInventory = InventoryConnection.cocheInventory();
                carGrid.DataSource = currentInventory;

                carGrid.RowHeadersVisible = false;
                carGrid.BackgroundColor = Color.FromArgb(30, 30, 30);
                carGrid.EnableHeadersVisualStyles = false;
                carGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
                carGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

                carGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                carGrid.ColumnHeadersDefaultCellStyle.Font = new Font("MS Reference Sans Serif", 10);
                carGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                carGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                carGrid.DefaultCellStyle.SelectionBackColor = Color.SlateGray;
                carGrid.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                carGrid.DefaultCellStyle.ForeColor = Color.White;
                carGrid.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

                carGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                carGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
       

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) // ValidateForm method should ensure all fields are filled
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                using (var context = new CarInventoryDataContext())
                {                   
                    Car newCar = new Car // Create a new Car object 
                    {
                        VIN = txtVIN.Text,
                        Year = int.Parse(txtYear.Text),
                        Make = txtMake.Text,
                        Model = txtModel.Text,
                        Price = int.Parse(txtPrice.Text)
                    };
                   
                    context.Cars.InsertOnSubmit(newCar); // Add the new car to the LINQ to SQL context                   
                    context.SubmitChanges();// Submit the changes to the database

                    MessageBox.Show("New car added to inventory");                    
                    LoadGrid();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding new car: {ex.Message}");
            }
        }
        private bool ValidateForm() // Method to check if all required fields are filled
        {            
            if (string.IsNullOrWhiteSpace(txtVIN.Text))
            {
                MessageBox.Show("VIN is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show("Year is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMake.Text))
            {
                MessageBox.Show("Make is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Model is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Price is required.");
                return false;
            }
            int year;
            if (!int.TryParse(txtYear.Text, out year))
            {
                MessageBox.Show("Invalid Year format. Please enter a valid number.");
                return false;
            }

            int price;
            if (!int.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Invalid Price format. Please enter a valid number.");
                return false;
            }

            return true;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string selectedVIN = carGrid.SelectedRows[0].Cells["VIN"].Value.ToString();
            MessageBox.Show(selectedVIN);
            if (carGrid.SelectedRows.Count > 0)
            {
                
                Car selectedCar = (Car)carGrid.SelectedRows[0].DataBoundItem;
                
                var result = MessageBox.Show("Are you sure you want to delete the record?", "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    try
                    {

                        using (var context = new CarInventoryDataContext())
                        {

                            string sql = $"DELETE FROM Cars WHERE VIN = '{selectedCar.VIN}'";
                            context.ExecuteCommand(sql);


                            MessageBox.Show("Car deleted successfully");

                            LoadGrid();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting car: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a car to delete");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string make = txtMake.Text.Trim();
            string model = txtModel.Text.Trim();
            string year = txtYear.Text.Trim();
            string price = txtPrice.Text.Trim();

            using (var context = new CarInventoryDataContext())
            {
                // Query the database using LINQ to filter cars based on the input criteria
                var query = from car in context.Cars
                            where (string.IsNullOrEmpty(make) || car.Make.Contains(make))
                               && (string.IsNullOrEmpty(model) || car.Model.Contains(model))
                               && (string.IsNullOrEmpty(year) || car.Year.ToString().Contains(year))
                               && (string.IsNullOrEmpty(price) || car.Price.ToString().Contains(price))
                            select car;

                var carList = query.ToList();
                if (carList.Any()) // If any cars match the criteria
                {                   
                    string message = "Cars found:\n"; 
                    foreach (var car in carList)
                    {
                        message += $"VIN: {car.VIN}, Make: {car.Make}, Model: {car.Model}, Year: {car.Year}, Price: {car.Price}\n";
                    }
                    MessageBox.Show(message, "Search Results"); 
                }
                else
                {
                    MessageBox.Show("No car exists in the database with the given criteria.", "Search Results"); 
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (carGrid.SelectedRows.Count > 0) 
            {
                Car selectedCar = (Car)carGrid.SelectedRows[0].DataBoundItem; // Get the selected car

                txtVIN.Text = selectedCar.VIN;
                txtYear.Text = selectedCar.Year.ToString();
                txtMake.Text = selectedCar.Make;
                txtModel.Text = selectedCar.Model;
                txtPrice.Text = selectedCar.Price.ToString();
            }
            else
            {
                MessageBox.Show("Please select a car to update");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (carGrid.SelectedRows.Count > 0) 
            {
                Car selectedCar = (Car)carGrid.SelectedRows[0].DataBoundItem; 

                try
                {
                    using (var context = new CarInventoryDataContext())
                    {
                        // Retrieve the car from the database using its ID or other unique identifier
                        Car carToUpdate = context.Cars.SingleOrDefault(car => car.VIN == selectedCar.VIN);

                        if (carToUpdate != null)
                        {
                            // Update the car's information with the values from the text boxes
                            carToUpdate.VIN = txtVIN.Text;                          
                            carToUpdate.Make = txtMake.Text;
                            carToUpdate.Model = txtModel.Text;
                            if (int.TryParse(txtYear.Text, out int year))
                            {
                                carToUpdate.Year = year;
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid year.");
                                return;
                            }

                            // Parse Price
                            if (decimal.TryParse(txtPrice.Text, out decimal price))
                            {
                                carToUpdate.Price = price;
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid price.");
                                return;
                            }
                            
                            context.SubmitChanges(); // Submit the changes to the database
                            MessageBox.Show("Car information updated successfully");
                            LoadGrid(); 
                        }
                        else
                        {
                            MessageBox.Show("Car not found in the database");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating car: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a car to update");
            }
        }
    }

}
